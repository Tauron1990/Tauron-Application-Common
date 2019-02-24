using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    [PublicAPI]
    public static class RuleFactoryExtensions
    {
        private static ConcurrentDictionary<string, Delegate> _delegates = new ConcurrentDictionary<string, Delegate>();

        public static LambdaExpression  CreateExpression(this IRuleFactory factory, string name)
        {
            var rule = factory.Create(name);

            if (!(rule is IRuleDescriptor descriptor))
                throw new NotSupportedException("No Descriptor");
            
            Type ruleType = rule.GetType();
            Type parameterType = descriptor.ParameterType;
            Type returnType = descriptor.ReturnType;

            MethodInfo[] methods = ruleType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo method;
            if (parameterType == null && returnType == null)
            {
                method = methods.Where(NameFilter).Single(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0);
                var expr = CreateErrorExpression(Expression.Call(Expression.Constant(rule, ruleType), method));

                return Expression.Lambda(typeof(Func<Return>), expr);
            }

            if (parameterType == null)
            {
                method = methods.Where(NameFilter).Single(m => m.ReturnType == returnType && m.GetParameters().Length == 0);
                var tree = CreateErrorExpression(Expression.Call(Expression.Constant(rule, ruleType), method));

                return Expression.Lambda<Func<Return>>(tree);
            }

            if (returnType == null)
            {
                method = methods.Where(NameFilter).Single(m =>
                {
                    if (m.ReturnType != typeof(void)) return false;
                    var parms = m.GetParameters();

                    return parms.Length == 1 && parms[0].ParameterType == parameterType;

                });

                var parm = Expression.Parameter(parameterType);
                var call = CreateErrorExpression(Expression.Call(Expression.Constant(rule, ruleType), method, parm));
                var delegateType = typeof(Func<,>).MakeGenericType(parameterType, typeof(Return));

                return Expression.Lambda(delegateType, call, parm);
            }

            method = methods.Where(NameFilter).Single(m => m.GetParameters().FirstOrDefault()?.ParameterType == parameterType && m.ReturnType == returnType);

            var parm2 = Expression.Parameter(parameterType);
            var exp2 = CreateErrorExpression(Expression.Call(Expression.Constant(rule, ruleType), method, parm2));
            var delegateType2 = typeof(Func<,>).MakeGenericType(parameterType, typeof(Return));

            return Expression.Lambda(delegateType2, exp2, parm2);

            Expression CreateErrorExpression(Expression exp)
            {
                List<Expression> blockList = new List<Expression>();
                List<ParameterExpression> variables = new List<ParameterExpression>();

                ParameterExpression callVariable = Expression.Variable(typeof(object), "callResult");
                if (returnType != null)
                {
                    variables.Add(callVariable);
                    blockList.Add(Expression.Assign(callVariable, Expression.TypeAs(exp, typeof(object))));
                }
                else
                    blockList.Add(exp);

                Expression falseExpression;
                LabelTarget returnLabel = Expression.Label(typeof(Return), "returnLabel");
                //Expression returnVariable = Expression.Variable(typeof(Return), "returnVariable");
                if (returnType == null)
                {
                    falseExpression = Expression.Return(returnLabel,
                        Expression.New(typeof(VoidReturn)));
                }
                else
                {
                    falseExpression = Expression.Return(returnLabel,
                        Expression.New(typeof(ObjectReturn).GetConstructor(new[] { typeof(object) }) ?? throw new InvalidOperationException(),
                            callVariable));
                }

                blockList.Add(Expression.IfThenElse(Expression.Property(Expression.Constant(rule), nameof(rule.Error)),
                    Expression.Return(returnLabel,
                        Expression.New(typeof(ErrorReturn).GetConstructor(new[] { typeof(IRuleBase) }) ?? throw new InvalidOperationException(),
                            Expression.Constant(rule))),
                    falseExpression));
                blockList.Add(Expression.Label(returnLabel, Expression.Constant(null, typeof(Return))));

                return returnType == null ? Expression.Block(blockList.ToArray()) : Expression.Block(variables, blockList);
            }
        }

        public static Delegate CreateDelegate(this IRuleFactory factory, string name) => _delegates.GetOrAdd(name, key => CreateExpression(factory, key).CompileFast());


        [DebuggerStepThrough]
        private static bool NameFilter(MethodInfo info)
        {
            const string actionName = "Action";
            const string nameSpace = "Tauron.Application.Common.BaseLayer.BusinessLayer.";
            const string iRule = nameSpace + "IIBusinessRule";
            const string rule = nameSpace + "IBusinessRule";
            const string ioRule = nameSpace + "IOBussinesRule";

            var name = info.Name;
            if (name == actionName) return true;

            return name.StartsWith(iRule) || name.StartsWith(rule) || name.StartsWith(ioRule) && name.Contains(actionName);
        }
        
        public static Func<Return> Delegate(this IRuleFactory fac, string name) => (Func<Return>)CreateDelegate(fac, name);

        public static Func<TArg, Return> DelegateArg<TArg>(this IRuleFactory fac, string name) => (Func<TArg, Return>) CreateDelegate(fac, name);

        public static Return Call(this IRuleFactory factory, string name) => Delegate(factory, name)();

        public static Return Call<TArg>(this IRuleFactory factory, string name, TArg arg) => DelegateArg<TArg>(factory, name)(arg);

        public static void ThrowError(this Return ret)
        {
            if(ret is ErrorReturn error)
                throw new CallErrorException(error);
            throw new InvalidOperationException("No Compatiple Return");
        }

        public static void DirectCallVoid(this IRuleFactory factory, string name)
        {
            var res = Call(factory, name);
            if(res.Error) ThrowError(res);
        }

        public static void DirectCallVoid<TArg>(this IRuleFactory factory, string name, TArg arg)
        {
            var res = Call(factory, name, arg);
            if (res.Error) ThrowError(res);
        }

        public static TResult DirectCall<TResult>(this IRuleFactory factory, string name)
        {
            var res = Call(factory, name);
            if (res.Error) ThrowError(res);

            return (TResult) ((ObjectReturn) res).Result;
        }

        public static TResult DirectCall<TResult, TArg>(this IRuleFactory factory, string name, TArg arg)
        {
            var res = Call(factory, name, arg);
            if (res.Error) ThrowError(res);

            return (TResult)((ObjectReturn)res).Result;
        }
    }
}