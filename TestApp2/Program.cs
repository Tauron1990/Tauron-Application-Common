using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tauron;
using Tauron.Application.Ioc;

namespace TestApp2
{
    [Export(typeof(TestClass1))]
    public sealed class TestClass1
    {
        public string Hello { get; } = "Hello";
    }

    [Export(typeof(TestClass2))]
    public sealed class TestClass2
    {
        [Inject]
        private TestClass1 _testClass1;

        [Inject]
        public Lazy<TestClass1> TestClass1 { get; set; }

        [Inject]
        public TestClass1 Class1 { get; private set; }
    }
    
    public interface ITestClass3
    {

    }

    [Export(typeof(ITestClass3))]
    public class TestClass3Instance : ITestClass3
    {
        [Inject]
        public TestClass1 TestClass1 { get; set; }
    }

    [Export(typeof(ITestClass3))]
    public class TestClass3Instance1 : ITestClass3
    {
        [Inject]
        public TestClass2 TestClass1;
    }

    [Export(typeof(ITestClass3))]
    public class TestClass3Instance2 : ITestClass3
    {

    }

    [Export(typeof(ITestClass3))]
    public class TestClass3Instance3 : ITestClass3
    {

    }

    [Export(typeof(TestClass4))]
    public sealed class TestClass4
    {
        [Inject]
        public ITestClass3[] Class3 { get; set; }

        [Inject]
        public List<ITestClass3> TestClass3s { get; set; }
    }

    [Export(typeof(TestClass5))]
    public sealed class TestClass5
    {
        [Inject]
        public TestClass4 Lazy { get; set; }      
    }

    class Program
    {
        static void Main(string[] args)
        {
            IContainer testContainer = new DefaultContainer();

            ExportResolver res = new ExportResolver();

            res.AddAssembly(Assembly.GetCallingAssembly());

            testContainer.Register(res);

            var watch = Stopwatch.StartNew();
            var inst = testContainer.Resolve<TestClass5>();
            var testLazy = inst.Lazy.TestClass3s.Find(t => t is TestClass3Instance1).SafeCast<TestClass3Instance1>().TestClass1.TestClass1;
            var obj = testLazy.Value;
            Console.WriteLine(watch.ElapsedMilliseconds);

            Console.ReadKey();
            Debugger.Break();
        }
    }
}
