using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Pastel;
using System.Threading.Tasks;

namespace TrashServer
{
    public class Handler
    {
        private const string _dateLogColor = "#28af03";
        private static readonly IReadOnlyDictionary<LogLevel, string> _colors = new Dictionary<LogLevel, string>
        {
            {LogLevel.Fatal, "#771010"},
            {LogLevel.Error, "#e70000"},
            {LogLevel.Warn, "#d1d338"},
            {LogLevel.Info, "#47d338"},
            {LogLevel.Debug, "#e0e0e0"},
            {LogLevel.Trace, "#646464"},
        };
        private readonly ConcurrentQueue<string> _commands;
        private readonly Thread _listenerThread;

        public static LogHandler Logging { get; private set; }
        public RequestManager RequestService { get; set; }
        public Config Config { get; private set; }
        public bool IsActive { get; private set; }

        public delegate void LogHandler(string message, LogLevel log = LogLevel.Info);

        public Handler(Config config)
        {
            this.Config = config;
            _commands = new();
            _listenerThread = new(new ThreadStart(ThreadHandler));
            Log("Server initialized!", LogLevel.Debug);
            Logging = Log;
        }

        public void Start()
        {
            IsActive = true;
            _listenerThread.Start();
            Log("Starting server", LogLevel.Debug);
        }

        public void Stop()
        {
            IsActive = false;
            Log("Stopping server", LogLevel.Debug);
        }

        public void Close()
        {
            Log("Force stopping server!", LogLevel.Debug);
            if (IsActive)
                _listenerThread.Interrupt();
            _commands.Clear();
            Config = null;
        }

        private void ThreadHandler()
        {
            using (HttpListener listener = new HttpListener())
            {
                listener.Prefixes.Add(Config.EndPoint);
                listener.Start();
                Log("Server started!");
                try
                {
                    Log("Waiting for incoming messages", LogLevel.Debug);
                    while (IsActive)
                    {
                        
                        listener.BeginGetContext(new AsyncCallback(ListenerCallback),listener).AsyncWaitHandle.WaitOne();
                    }
                }
                catch (Exception exception)
                {
                    Log("Thread of Handler died: " + exception.Message, LogLevel.Fatal);
                }
                finally
                {
                    IsActive = false;
                    listener.Stop();
                    listener.Close();
                    Log("Thread of Handler finished", LogLevel.Info);
                }
              
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);

            Log("Incoming request from: " + context.Request.RemoteEndPoint, LogLevel.Trace);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            
            string responseString = "<!DOCTYPE html>\n<html>\n<body>\n\tKurwa moje pole\n</body>\n</html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private void Log(string message, LogLevel log = LogLevel.Info)
        {
            ;
            if(log <= Config.LogLevel)
                Console.WriteLine("[{0}][{1}][{2}] {3}",
                    DateTime.Now.ToString("G").Pastel(_dateLogColor),
                    (Task.CurrentId?.ToString() ?? "------").PadLeft(6),
                    log.ToString().PadRight(5).Pastel(_colors[log]),
                    message);
        }
    }
}
