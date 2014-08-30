using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
