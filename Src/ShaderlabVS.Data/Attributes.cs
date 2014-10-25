using System;

namespace ShaderlabVS.Data
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class DefinationKeyAttribute : Attribute
    {
        public string Name { get; set; }

        public DefinationKeyAttribute(string name)
        {
            Name = name;
        }
    }
}
