using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.Json;
using FastExpressionCompiler;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.CQRS.Common.Converter;

namespace CQRSTestApp
{
    public class TestService<T>
    {
        public TestObject Obj { get; }
        public string Param { get; }
        public T Test { get; set; }

        public TestService(TestObject obj, string param)
        {
            Obj = obj;
            Param = param;
        }
    }

    public class TestObject
    {
        public string Porp { get; set; }

        public TestObject()
        {
            Porp = "Hallo";
        }
    }

    class Program
    {


        static void Main(string[] args)
        {
            var coll = new ServiceCollection();
            coll.AddTransient<TestObject>();
            coll.AddTransient(typeof(TestService<>));

            var provider = coll.BuildServiceProvider();
            
            var fac = ActivatorUtilities.CreateFactory(typeof(TestService<>), new[] {typeof(string)});

            var result = fac(provider, new object[]{"Test"});
        }
    }
}
