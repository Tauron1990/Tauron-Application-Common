using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [PublicAPI]
    public sealed class Bundle<TType>
    {
        public ModelBase Model { get; }
        public ObservableProperty Property { get; }

        public TType Value
        {
            get => Model.GetValue<TType>(Property);
            set => Model.SetValue(Property, value);
        }

        public static implicit operator TType(Bundle<TType> bundle) => bundle.Value;

        public Bundle(ModelBase model, ObservableProperty property)
        {
            Model = model;
            Property = property;
        }
    }
}