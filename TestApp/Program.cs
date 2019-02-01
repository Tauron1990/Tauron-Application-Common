using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace TestApp
{
    class Program
    {
        private class RuleFactoryMock : IEnumerable<KeyValuePair<string, IRuleBase>>, IRuleFactory
        {
            private Dictionary<string, IRuleBase> _ruleBases = new Dictionary<string, IRuleBase>();

            public void Add(string name, IRuleBase rule) => _ruleBases.Add(name, rule);

            public IEnumerator<KeyValuePair<string, IRuleBase>> GetEnumerator() => _ruleBases.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IRuleBase Create(string name) => _ruleBases[name];

            public IBusinessRule CreateBusinessRule(string name) => (IBusinessRule)Create(name);

            public IIBusinessRule<TType> CreateIiBusinessRule<TType>(string name) => (IIBusinessRule<TType>) Create(name);

            public IIOBusinessRule<TInput, TOutput> CreateIioBusinessRule<TInput, TOutput>(string name) => (IIOBusinessRule<TInput, TOutput>) Create(name);

            public IOBussinesRule<TOutput> CreateOBussinesRule<TOutput>(string name) => (IOBussinesRule<TOutput>) Create(name);

            public CompositeRule<TInput, TOutput> CreateComposite<TInput, TOutput>(params string[] names) => throw new NotSupportedException();
        }

        private class RuleTest1 : BusinessRuleBase
        {
            public override void ActionImpl()
            {
                Console.WriteLine("Hello from Business Rule!!");
                Console.WriteLine();
            }
        }

        private class RuleTest2 : IBusinessRuleBase<string>
        {
            public override void ActionImpl(string input)
            {
                Console.WriteLine("Hello from Input Business Rule!!" + input);
                Console.WriteLine();
            }
        }

        private class RuleTest3 : OBusinessRuleBase<int>
        {
            public override int ActionImpl()
            {
                Console.WriteLine("Hello from Output Business Rule!!");
                Console.WriteLine();

                return 42;
            }
        }

        private class RuleTest4 : IOBusinessRuleBase<string, int>
        {
            public override int ActionImpl(string input)
            {

                Console.WriteLine("Hello from input output Business Rule!!" + input);
                Console.WriteLine();

                return 42;
            }
        }

        private class ErrorTest : BusinessRuleBase
        {
            public override void ActionImpl() => throw new NotSupportedException();
        }

        static void Main(string[] args)
        {
            var mock = new RuleFactoryMock
            {
                {nameof(RuleTest1), new RuleTest1() },
                {nameof(RuleTest2), new RuleTest2() },
                {nameof(RuleTest3), new RuleTest3() },
                {nameof(RuleTest4), new RuleTest4() },
                {nameof(ErrorTest), new ErrorTest() }
            };

            foreach (var mockName in mock.Select(m => m.Key))
            {
                switch (mockName)
                {
                    case nameof(RuleTest1):
                        mock.DirectCallVoid(nameof(RuleTest1));
                        Console.WriteLine("Test Passsed");
                        Console.WriteLine();
                        break;
                    case nameof(RuleTest2):
                        mock.DirectCallVoid(nameof(RuleTest2), "Hello There");
                        Console.WriteLine("Test Passsed");
                        Console.WriteLine();
                        break;
                    case nameof(RuleTest3):
                        Console.WriteLine(mock.DirectCall<int>(nameof(RuleTest3)));
                        Console.WriteLine("Test Passsed");
                        Console.WriteLine();
                        break;
                    case nameof(RuleTest4):
                        Console.WriteLine(mock.DirectCall<int, string>(nameof(RuleTest4), "Hello There 2"));
                        Console.WriteLine("Test Passsed");
                        Console.WriteLine();
                        break;
                    case nameof(ErrorTest):
                        try
                        {
                            mock.DirectCallVoid(nameof(ErrorTest));
                        }
                        catch (CallErrorException e)
                        {
                            Console.WriteLine(e);
                            Console.WriteLine(e.Error.First());
                        }
                        Console.WriteLine("Test Passsed");
                        break;
                }
            }

            Console.ReadKey();
        }
    }
}
