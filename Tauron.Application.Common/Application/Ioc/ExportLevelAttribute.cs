using System;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExportLevelAttribute : Attribute
    {
        public int Level { get; private set; }

        public ExportLevelAttribute(int level)
        {
            Level = level;
        }
    }
}