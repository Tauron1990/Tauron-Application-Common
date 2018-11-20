using System;
using System.Collections.Generic;
using System.Reflection;
using Tauron.Application.Ioc.BuildUp.Strategy;

namespace Tauron.Application.Common.CastleProxy.Impl
{
    public class ObjectContextPolicy : IPolicy
    {
        public ObjectContextPolicy() => ContextPropertys = new List<Tuple<ObjectContextPropertyAttribute, MemberInfo>>();
        public string ContextName { get; set; }
        
        public List<Tuple<ObjectContextPropertyAttribute, MemberInfo>> ContextPropertys { get; }
    }
}