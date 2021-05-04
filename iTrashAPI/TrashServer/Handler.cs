using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Pastel;
using System.Drawing;

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
        private readonly ConcurrentQueue<string> _commands = new();
        private readonly Thread _listenerThread = new(new ThreadStart(()=>
        {
        }));

        public Config Config { get; private set; }

        public Handler(Config config) => this.Config = config;

        public void Command(params string[] args)
        {
            
        }

        private void ThreadHandler()
        {
            try
            {
                
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception exception)
            {
                Log("Thread of Handler died: " + exception.Message, LogLevel.Fatal);
            }
        }

        private void Log(string message, LogLevel log = LogLevel.Info)
        {
            if(log <= Config.LogLevel)
                Console.WriteLine("[{0}][{1}] {2}", DateTime.Now.ToString("G").Pastel(_dateLogColor), log.ToString().Pastel(_colors[log]), message);
        }
    }
}
