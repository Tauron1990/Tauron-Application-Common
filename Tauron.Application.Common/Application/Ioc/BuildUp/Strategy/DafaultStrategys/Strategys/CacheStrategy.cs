using ExpressionBuilder;
using ExpressionBuilder.Enums;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class CacheStrategy : StrategyBase
    {
        private ICache _cache;
        private RebuildManager _rebuildManager;

        public override void Initialize(ComponentRegistry components)
        {
            _cache = components.Get<ICache>();
            _rebuildManager = components.Get<RebuildManager>();
        }

        public override void OnPerpare(IBuildContext context)
        {
            context.ErrorTracer.Phase = "Reciving Build (" + context.Metadata + ") from Cache";

            var unit = context.CompilationUnit;

            string lieftimeValiable = unit.LifeTimeContext;
            string paramName = unit.Input;

            var targetSetCon = Condition.Compare(unit.TargetName, Operation.Null(), ComparaisonOperator.Different);
            var compareNullInput = Condition.Compare(paramName, Operation.Null());

            unit.AddCode(
                CodeLine.CreateVariable<ILifetimeContext>(lieftimeValiable),
                CodeLine.Assign(lieftimeValiable, Operation.InvokeReturn(Operation.Constant(_cache), nameof(_cache.GetContext), Operation.Constant(context.Metadata))));

            if (!context.CompilationUnit.RealFunction.NoInput)
            {
                targetSetCon = Condition.And(targetSetCon, Condition.Compare(paramName, Operation.Null()));
                unit.AddCode(CodeLine.CreateIf(compareNullInput)
                    .Then(CodeLine.Assign(unit.TargetName, paramName))
                    .Else(CodeLine.Assign(unit.TargetName, Operation.InvokeReturn(Operation.Variable(lieftimeValiable), "GetValue"))));
            }
            else
                unit.AddCode(CodeLine.Assign(unit.TargetName, Operation.InvokeReturn(Operation.Variable(lieftimeValiable), "GetValue")));

            unit
                .WithParameter<object>(paramName)
                .AddAndPush(CodeLine.CreateIf(targetSetCon)
                    .Then(CodeLine.Return()));
        }

        public override void OnPostBuild(IBuildContext context)
        {
            if (!context.CanCache()) return;

            context.ErrorTracer.Phase = "Saving Build (" + context.Metadata + ") to Cache";

            var policy = context.Policys.Get<LifetimeTimePolicy>();

            if (policy == null) return;

            var unit = context.CompilationUnit;

            string lieftimeValiable = unit.LifeTimeContext;
            string buildObjectVar = unit.VariableNamer.GetValiableName("buildObject");

            var finishOp = new[]
            {
                Operation.Invoke(Operation.Constant(_cache), nameof(_cache.Add),
                    Operation.Variable(lieftimeValiable), Operation.Constant(context.Metadata), Operation.Constant(policy.ShareLiftime)),
                CodeLine.CreateVariable(typeof(BuildObject), buildObjectVar),
                CodeLine.Assign(buildObjectVar, Operation.CreateInstance(typeof(BuildObject),
                    Operation.Get(Operation.Get(Operation.Constant(context.Metadata), nameof(ExportMetadata.Export)), nameof(IExport.ImportMetadata)),
                    Operation.Constant(context.Metadata),
                    Operation.Constant<BuildParameter[]>(context.Parameters))),
                Operation.Set(buildObjectVar, nameof(BuildObject.Instance), Operation.Variable(unit.TargetName)),
                Operation.Invoke(Operation.Constant(_rebuildManager), nameof(RebuildManager.AddBuild), Operation.Variable(buildObjectVar))
            };

            if (unit.RealFunction.NoInput)
                unit.AddCode(finishOp);
            else
            {
                unit.AddCode(CodeLine.CreateIf(Condition.Compare(unit.Input, Operation.Null()))
                    .Then(finishOp));
            }

        }
    }
}