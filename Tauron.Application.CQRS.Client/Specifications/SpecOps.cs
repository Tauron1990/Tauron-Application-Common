using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Application.CQRS.Common.Dto;

namespace Tauron.Application.CQRS.Client.Specifications
{
    [PublicAPI]
    public static class SpecOps
    {
        private sealed class SimpleSpec<TType> : SpecificationBase<TType>
        {
            private readonly Func<TType, Task<OperationResult>> _eval;

            public SimpleSpec(Func<TType, Task<OperationResult>> eval) 
                => _eval = eval;

            protected override Task<OperationResult> IsSatisfiedBy(TType target) 
                => _eval(target);
        }

        private sealed class AndSpecification : ISpecification
        {
            private readonly ISpecification _left;
            private readonly ISpecification _right;

            public AndSpecification(ISpecification left, ISpecification right)
            {
                _left = left;
                _right = right;
            }

            public async Task<OperationResult> IsSatisfiedBy(object obj)
            {
                var res1 = await _left.IsSatisfiedBy(obj);
                var res2 = await _right.IsSatisfiedBy(obj);

                return res1.Merge(res2);
            }
        }

        private class OrSpecification : ISpecification
        {
            private readonly ISpecification _left;
            private readonly ISpecification _right;

            public OrSpecification(ISpecification left, ISpecification right)
            {
                _left = left;
                _right = right;
            }

            public async Task<OperationResult> IsSatisfiedBy(object obj)
            {
                var res1 = await _left.IsSatisfiedBy(obj);
                var res2 = await _right.IsSatisfiedBy(obj);

                if (res2.Error && res1.Error)
                    return res1.Merge(res2);

                return OperationResult.Success;
            }
        }

        public static ISpecification Simple<TType>(this Func<TType, Task<OperationResult>> eval) 
            => new SimpleSpec<TType>(eval);

        public static ISpecification And(this  ISpecification left, ISpecification right)
            => new AndSpecification(left, right);

        public static ISpecification Or(this ISpecification left, ISpecification right)
            => new OrSpecification(left, right);
    }
}