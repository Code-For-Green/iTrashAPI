using System;

namespace TrashServer
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequestKeyAttribute : Attribute
    {
        public readonly string Key;

        public RequestKeyAttribute(string key) => Key = key;
        private RequestKeyAttribute() { }
    }
}
