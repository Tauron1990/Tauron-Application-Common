using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private static Delegate CreateDelegate(IRuleFactory factory, string name)
        {
            return _delegates.GetOrAdd(name, Factory);

            Delegate Factory(string key)
            {
                var rule = factory.Create(key);

                if (!(rule is IRuleDescriptor descriptor))
                    throw new NotSupportedException("No Descriptor");

                Type ruleType = rule.GetType();
                Type parameterType = descriptor.ParameterType;
                Type returnType = descriptor.ParameterType;

                MethodInfo[] methods = ruleType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MethodInfo method;
                if (parameterType == null && returnType == null)
                {
                    method = methods.Where(NameFilter).Single(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0);

                    return Expression.Lambda<Func<Return>>(CreateErrorExpression(Expression.Call(Expression.Constant(rule, ruleType), method))).CompileFast();
                }

                if (parameterType == null)
                {
                    method = methods.Where(NameFilter).Single(m => m.ReturnType == returnType && m.GetParameters().Length == 0);

                    return Expression.Lambda<Func<Return>>(CreateErrorExpression(Expression.Call(Expression.Constant(rule, ruleType), method))).CompileFast();
                }

                if (returnType == null)
                {
                    method = methods.Where(NameFilter).Single(m =>
                    {
                        if (m.ReturnType != typeof(void)) return false;
                        var parms = m.GetParameters();

                        return parms.Length == 1 && parms[1].ParameterType == parameterType;

                    });

                    var parm = Expression.Parameter(parameterType);
                    var call = CreateErrorExpression(Expression.Call(Expression.Constant(rule, ruleType), method, parm));
                    var delegateType = typeof(Func<,>).MakeGenericType(parameterType, typeof(Return));

                    return Expression.Lambda(delegateType, call, parm).CompileFast();
                }
                
                method = methods.Where(NameFilter).Single(m => m.GetParameters().FirstOrDefault()?.ParameterType == parameterType && m.ReturnType == returnType);

                var parm2 = Expression.Parameter(parameterType ?? throw new InvalidOperationException("Parameter was Null"));
                var exp2 = CreateErrorExpression(Expression.Call(Expression.Constant(rule, ruleType), method, parm2));
                var delegateType2 = typeof(Func<,>).MakeGenericType(parameterType, typeof(Return));

                return Expression.Lambda(delegateType2, exp2, parm2).CompileFast();

                Expression CreateErrorExpression(Expression exp)
                {
                    Expression firstExp = exp;
                    Expression returnVariable = Expression.Variable(returnType ?? typeof(object));
                    if (returnType != null)
                        firstExp = Expression.Assign(returnVariable, exp);

                    Expression falseExpression;
                    if (returnType == null)
                    {
                        falseExpression = Expression.Return(Expression.Label(typeof(VoidReturn)),
                            Expression.New(typeof(VoidReturn)));
                    }
                    else
                    {
                        falseExpression = Expression.Return(Expression.Label(typeof(ObjectReturn)),
                            Expression.New(typeof(ObjectReturn).GetConstructor(new[] {typeof(object)}) ?? throw new InvalidOperationException(),
                                returnVariable));
                    }

                    return Expression.Block(firstExp,
                        Expression.IfThenElse(Expression.Property(Expression.Constant(rule), nameof(rule.Error)),
                            Expression.Return(Expression.Label(typeof(ErrorReturn)),
                                Expression.New(typeof(ErrorReturn).GetConstructor(new[] {typeof(IEnumerable<object>)}) ?? throw new InvalidOperationException(),
                                    Expression.Property(Expression.Constant(rule), nameof(rule.Errors)))), falseExpression));

                }
            }
        }

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

        private static void ThrowError(Return ret)
        {
            if(ret is ErrorReturn error)
                throw new CallErrorException(error.Errors);
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