using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Pastel;

namespace TrashServer
{
    public class Handler : IDisposable
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

        public Config Config { get; private set; }
        public bool IsActive { get; private set; }

        public Handler(Config config)
        {
            this.Config = config;
            _commands = new();
            _listenerThread = new(new ThreadStart(ThreadHandler));
        }

        public void Start()
        {
            IsActive = true;
            _listenerThread.Start();
        }

        public void Stop()
        {
            IsActive = false;
            _listenerThread.Start();
        }

        public void Dispose()
        {
            if (IsActive)
                _listenerThread.Abort();
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
                    while (IsActive)
                    {
                        listener.GetContext();
                    }
                }
                catch (ThreadAbortException)
                {
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

        private void Log(string message, LogLevel log = LogLevel.Info)
        {
            if(log <= Config.LogLevel)
                Console.WriteLine("[{0}][{1}] {2}", DateTime.Now.ToString("G").Pastel(_dateLogColor), log.ToString().Pastel(_colors[log]), message);
        }
    }
}
