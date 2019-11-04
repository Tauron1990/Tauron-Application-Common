using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FastExpressionCompiler;

namespace TestApp
{
    class Program
    {
        
        static void Main(string[] args)
        {
            ParameterExpression variable = Expression.Variable(typeof(int), "TestValue");
            var returnLabel = Expression.Label(typeof(int));
            var returnExpression = Expression.Label(returnLabel, Expression.Constant(-1));

            var block = Expression.Block(
                new[] {variable}, 
                Expression.Assign(variable, Expression.Constant(42)),
                Expression.Call(typeof(Console).GetMethod(nameof(Console.WriteLine), new []{typeof(int)}), variable),
                Expression.Block(new[] { variable },
                    Expression.AddAssign(variable, Expression.Constant(8))),
                Expression.Return(returnLabel, Expression.Constant(0)),
                returnExpression);

            Console.WriteLine(Expression.Lambda<Func<int>>(block).CompileFast()());


            Console.ReadKey();
        }
    }
}
