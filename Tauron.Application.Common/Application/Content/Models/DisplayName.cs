using System;

namespace Tauron.Application.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayNameAttribate : Attribute
    {
        public string PropertyResourceName { get; set; }
    }
}