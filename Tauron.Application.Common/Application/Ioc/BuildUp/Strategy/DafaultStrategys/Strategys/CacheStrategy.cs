using ExpressionBuilder;
using ExpressionBuilder.Conditions;
using ExpressionBuilder.Enums;
using Tauron.Application.Ioc.Components;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class CacheStrategy : StrategyBase
    {
        private ICache _cache;

        public override void Initialize(ComponentRegistry components) => _cache = components.Get<ICache>();

        public override void OnPerpare(IBuildContext context)
        {
            context.ErrorTracer.Phase = "Reciving Build (" + context.Metadata + ") from Cache";
            
            var life = _cache.GetContext(context.Metadata);
            const string lieftimeValiable = CompilationUnit.DefaultVariableNames.LifeTimeContext;
            const string paramName = CompilationUnit.DefaultVariableNames.Input;

            var compareNullInput = new BinaryCondition(Operation.Variable(paramName), Operation.Null(), ComparaisonOperator.Different);

            context.CompilationUnit
                .WithParameter<object>(paramName)
                .AddCode(
                    CodeLine.CreateVariable<ILifetimeContext>(lieftimeValiable),
                    CodeLine.Assign(lieftimeValiable, Operation.Constant(life)),
                    CodeLine.CreateIf(compareNullInput)
                        .Then(CodeLine.Assign(CompilationUnit.TargetName, paramName))
                        .Else(CodeLine.Assign(CompilationUnit.TargetName, Operation.InvokeReturn(Operation.Variable(lieftimeValiable), "GetValue"))))
                .AddAndPush(CodeLine.CreateIf(Condition.And(
                        new BinaryCondition(Operation.Variable(paramName), Operation.Null(), ComparaisonOperator.Equal),
                        new BinaryCondition(Operation.Variable(CompilationUnit.TargetName), Operation.Null(), ComparaisonOperator.Different)))
                    .Then(CodeLine.Return()));
        }

        public override void OnPostBuild(IBuildContext context)
        {
            if (!context.CanCache()) return;

            context.ErrorTracer.Phase = "Saving Build (" + context.Metadata + ") to Cache";

            var policy = context.Policys.Get<LifetimeTimePolicy>();

            if (policy == null) return;

            const string lieftimeValiable = CompilationUnit.DefaultVariableNames.LifeTimeContext;

            context.CompilationUnit.AddCode(
                Operation.Invoke(Operation.Constant(_cache), nameof(_cache.Add),
                    Operation.Variable(lieftimeValiable), Operation.Constant(context.Metadata), Operation.Constant(policy.ShareLiftime)));
        }
    }
}