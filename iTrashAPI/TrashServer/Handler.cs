using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Pastel;

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
            Config = config;
            _commands = new();
            _listenerThread = new(new ThreadStart(ThreadHandler)) {
                Name = "Listener",
                Priority = ThreadPriority.AboveNormal
            };
            Logging = Log;
            Log("Server initialized!", LogLevel.Debug);
            Log("Loading database", LogLevel.Debug);
            API.Database.Init(Config.DatabasePath);
            try
            {
                API.Database.Load();
                Log("Database loaded!", LogLevel.Debug);
            }
            catch(Exception exception)
            {
                Log("Database loading FAILED!!!", LogLevel.Fatal);
                Log("Error log: " + exception.ToString(), LogLevel.Fatal);
            }
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
            try
            {
                API.Database.Save();
                Log("Database saved!", LogLevel.Debug);
            }
            catch (Exception exception)
            {
                Log("Database saving FAILED!!!", LogLevel.Fatal);
                Log("Error log: " + exception.ToString(), LogLevel.Fatal);
            }
            if (IsActive)
                _listenerThread.Interrupt();
            _commands.Clear();
            Config = null;
        }

        private void ThreadHandler()
        {
            using HttpListener listener = new();
            Semaphore semaphore = new(4,4);
            listener.Prefixes.Add(Config.EndPoint);
            listener.Start();
            Log("Server started!");
            try
            {
                Log("Waiting for incoming messages", LogLevel.Debug);
                while (IsActive)
                {
                    semaphore.WaitOne();
                    Task task = listener.GetContextAsync().ContinueWith(async (t) =>
                    {
                        try
                        {
                            semaphore.Release();
                            HttpListenerContext context = await t;
                            await ListenerCallback(context);
                        } 
                        catch(Exception exception)
                        {
                            Log("Task of listener died: " + exception.Message, LogLevel.Error);
                        }
                        finally
                        {
                            Log("Task finished", LogLevel.Trace);
                        }
                    });
                    Log("Preparing for next task", LogLevel.Trace);

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

        private async Task ListenerCallback(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            byte[] buffer = new byte[request.ContentLength64];
            string responseString = string.Empty;

            Log("Incoming request from: " + context.Request.RemoteEndPoint, LogLevel.Trace);
            response.StatusCode = (int)CheckRequest(request);
            if(response.StatusCode == (int)HttpStatusCode.OK)
            {
                string key = request.Url.Segments[1];
                Stream stream = request.InputStream;
                stream.Read(buffer, 0, buffer.Length);
                (response.StatusCode, responseString) = await RequestService.StartTask(key, request.ContentEncoding.GetString(buffer));
            }
            if (response.StatusCode != (int)HttpStatusCode.OK)
                responseString = Enum.GetName((HttpStatusCode)response.StatusCode);

            response.ContentEncoding = Encoding.UTF8;
            buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private HttpStatusCode CheckRequest(HttpListenerRequest request)
        {
            if (request.ContentType != "application/json")
            {
                Log("Bad request content type from: " + request.RemoteEndPoint, LogLevel.Trace);
                return HttpStatusCode.BadRequest;
            }
            if (request.Url.Segments.Length < 2)
            {
                Log("Bad request segment lenght type from: " + request.RemoteEndPoint, LogLevel.Trace);
                return HttpStatusCode.BadRequest;
            }
            string key = request.Url.Segments[1];
            if (!RequestService.ContainsKey(key))
            {
                Log("Bad request segment lenght type from: " + request.RemoteEndPoint, LogLevel.Trace);
                return HttpStatusCode.NotImplemented;
            }
            return HttpStatusCode.OK;
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
