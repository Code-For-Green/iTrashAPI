using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TrashServer
{
    public class RequestManager
    {
        private static readonly Type _attributeType = typeof(RequestKeyAttribute);
        private static readonly Type _interfaceType = typeof(IRequest);
        private readonly Dictionary<string, IRequest> _dictionary = new();

        public RequestManager() : this(Assembly.GetExecutingAssembly()) { }

        public RequestManager(params Assembly[] assemblies)
        {
            Handler.Logging($"Searching request keys in {assemblies.Length} assemblies", LogLevel.Debug);
            foreach (Assembly assembly in assemblies)
                FindAttribute(assembly.GetTypes());
            Handler.Logging($"Added {_dictionary.Count} diffrent requests to server", LogLevel.Debug);
        }

        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        public Task<int> StartTask(string key, string content, out string response)
        {
            try
            {
                Task task = _dictionary[key].Execute(JsonDocument.Parse(content).RootElement.GetRawText(), out response);
                return Task.FromResult(task.IsCompletedSuccessfully ? 200 : 500);
            }
            catch
            {
                Handler.Logging("Something went wrong in task " + key, LogLevel.Error);
                response = "Internal Server Error";
                return Task.FromResult(500);
            }
        }

        private void FindAttribute(Type[] classes)
        {
            foreach(Type classType in classes)
            {
                if (!_interfaceType.IsAssignableFrom(classType))
                    continue;
                if (Attribute.GetCustomAttribute(classType, _attributeType) is not RequestKeyAttribute attribute)
                {
                    if(classType != typeof(IRequest))
                        Handler.Logging($"There is missing attribute in {classType.Name}", LogLevel.Error);
                    continue;
                }
                if(_dictionary.ContainsKey(attribute.Key))
                {
                    Handler.Logging($"{classType.FullName} key is already in dictionary", LogLevel.Error);
                    continue;
                }
                if(Activator.CreateInstance(classType) is not IRequest request)
                {
                    Handler.Logging($"Creating instance from class {classType.FullName} failed", LogLevel.Error);
                    continue;
                }
                _dictionary.Add(attribute.Key, request);
                Handler.Logging($"Added {attribute.Key} to pool", LogLevel.Debug);
            }
        }
    }
}
