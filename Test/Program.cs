using System;
using System.Diagnostics;
using Tauron;
using Tauron.Application;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.CastleProxy;
using Tauron.Application.Common.MVVM.Dynamic;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Con = System.Console;

namespace Test
{
    [ExportRule(Program.TestRule1)]
    public class TestRule1 : BusinessRuleBase
    {
        public override void ActionImpl()
        {
            Con.WriteLine("Hello from TestRule1");
        }
    }

    [ExportRule(Program.TestRule2)]
    public class TestRule2 : IBusinessRuleBase<string>
    {
        public override void ActionImpl(string input)
        {
            Con.WriteLine("Hello from TestRule2");
        }
    }

    [ExportRule(Program.TestRule3)]
    public class TestRule3 : OBusinessRuleBase<string>
    {
        public override string ActionImpl()
        {
            Con.WriteLine("Hello from TestRule3");
            return String.Empty;
        }
    }

    [ExportRule(Program.TestRule4)]
    public class TestRule4 : IOBusinessRuleBase<string, string>
    {
        public override string ActionImpl(string input)
        {
            Con.WriteLine("Hello from TestRule4");
            return string.Empty;
        }
    }

    [ExportRule(Program.ErrorTest)]
    public class ErrorTest : BusinessRuleBase
    {
        public override void ActionImpl()
        {
            SetError("Hello from Exception");
        }
    }

    [Export(typeof(TestClass))]
    [CreateRuleCall]
    public abstract class TestClass
    {
        [BindRule(Program.TestRule1)]
        protected abstract void Test1();

        [BindRule(Program.TestRule2)]
        protected abstract Return Test2(string input);

        [BindRule(Program.TestRule3)]
        protected abstract Return Test3();

        [BindRule(Program.TestRule4)]
        protected abstract string Test4(string input);

        [BindRule(Program.ErrorTest)]
        protected abstract void Error();

        public void RunTest()
        {
            Test1();
            var test = Test2(String.Empty);
            var test2 = Test3();
            var test3 = Test4(String.Empty);
            Error();
        }
    }

    class Program
    {
        public const string TestRule1 = "TestRule1";
        public const string TestRule2 = "TestRule2";
        public const string TestRule3 = "TestRule3";
        public const string TestRule4 = "TestRule4";
        public const string ErrorTest = "ErrorTest";

        static void Main(string[] args)
        {
            try
            {
                var container = new DefaultContainer();
                container.Register(new ProxyExtension());
                container.Register(new PropertyModelExtension());
                container.Register(new MvvmDynamicExtension());

                container.Register(new ExportResolver()
                    .AddAssembly(typeof(Program).Assembly)
                    .AddAssembly(typeof(RuleFactory).Assembly)
                    .AddAssembly(typeof(CommonApplication).Assembly)
                    .AddAssembly(typeof(ModelBase).Assembly));

                var watch = Stopwatch.StartNew();
                var testClass = container.Resolve<TestClass>();
                Con.WriteLine(watch.Elapsed);

                testClass.RunTest();
            }
            catch (CallErrorException e)
            {
                foreach (var o in e.Error)
                {
                    Con.WriteLine(o);
                }
            }


            Console.ReadKey();
        }
    }
}
