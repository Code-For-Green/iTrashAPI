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

        public int StartTask(string key, string content, out string response)
        {
            Task task = _dictionary[key].Execute(JsonDocument.Parse(content).RootElement, out response);
            task.Start();
            task.Wait();
            return task.IsCompletedSuccessfully ? 200 : 500;
        }

        private void FindAttribute(Type[] classes)
        {
            foreach(Type classType in classes)
            {
                if (!classType.IsAssignableTo(_interfaceType))
                    return;
                if (Attribute.GetCustomAttribute(classType, _attributeType) is not RequestKeyAttribute attribute)
                {
                    Handler.Logging($"There is missing attribute in {classType.Name}", LogLevel.Error);
                    return;
                }
                if(_dictionary.ContainsKey(attribute.Key))
                {
                    Handler.Logging($"{classType.FullName} key is already in dictionary", LogLevel.Error);
                    return;
                }
                if(Activator.CreateInstance(classType) is not IRequest request)
                {
                    Handler.Logging($"Creating instance from class {classType.FullName} failed", LogLevel.Error);
                    return;
                }
                _dictionary.Add(attribute.Key, request);
                Handler.Logging($"Added {attribute.Key} to pool", LogLevel.Debug);
            }
        }
    }
}
