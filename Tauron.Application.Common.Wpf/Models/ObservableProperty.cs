using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public delegate void ObservablePropertyChanged(ObservableProperty prop, ModelBase model, object value);

    public delegate object ObservableCoerceValueCallback(ModelBase model, object value);

    public delegate bool ObservableValidateValueCallback(ModelBase model, ObservableProperty property, object value);

    [Serializable, PublicAPI]
    public sealed class ObservablePropertyMetadata
    {
        [CanBeNull]
        public object DefaultValue { get; private set; }

        public bool IsReadOnly { get; private set; }
        [CanBeNull]
        public ObservableValidateValueCallback ValidateValueCallback { get; private set; }
        [CanBeNull]
        public ObservableCoerceValueCallback CoerceValueCallback { get; private set; }
        [CanBeNull]
        public ObservablePropertyChanged PropertyChanged { get; private set; }

        internal bool Prepare(Type targetType)
        {
            if (DefaultValue != null && targetType != DefaultValue.GetType()) return false;

            if (DefaultValue != null && targetType.BaseType == typeof (ValueType)) 
                DefaultValue = targetType.GetConstructor(Type.EmptyTypes).Invoke(null);

            return true;
        }

        public ObservablePropertyMetadata([CanBeNull] object defaultValue, bool isReadOnly,
                                          [CanBeNull] ObservableValidateValueCallback validateValueCallback,
                                          [CanBeNull] ObservableCoerceValueCallback coerceValueCallback,
                                          [CanBeNull] ObservablePropertyChanged propertyChanged)
        {
            DefaultValue = defaultValue;
            IsReadOnly = isReadOnly;
            ValidateValueCallback = validateValueCallback;
            CoerceValueCallback = coerceValueCallback;
            PropertyChanged = propertyChanged;
        }

        public ObservablePropertyMetadata([CanBeNull] object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public ObservablePropertyMetadata(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        public ObservablePropertyMetadata([CanBeNull] ObservablePropertyChanged propertyChanged)
        {
            PropertyChanged = propertyChanged;
        }

        public ObservablePropertyMetadata([CanBeNull] ObservableValidateValueCallback observableValidateValueCallback)
        {
            ValidateValueCallback = observableValidateValueCallback;
        }

        public ObservablePropertyMetadata([CanBeNull] ObservablePropertyChanged propertyChanged, [CanBeNull] ObservableValidateValueCallback observableValidateValueCallback)
        {
            ValidateValueCallback = observableValidateValueCallback;
            PropertyChanged = propertyChanged;
        }

        public ObservablePropertyMetadata()
        {
            
        }
    }

    [PublicAPI, Serializable]
    public sealed class ObservableProperty : IEquatable<ObservableProperty>
    {
        public bool Equals([CanBeNull] ObservableProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(ObservableProperty left, ObservableProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ObservableProperty left, ObservableProperty right)
        {
            return !Equals(left, right);
        }

        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public Type Type { get; private set; }

        [NotNull]
        public ObservablePropertyMetadata Metadata { get; private set; }

        public ObservableProperty([NotNull] string name, [NotNull] Type type,
                                  [CanBeNull] ObservablePropertyMetadata metadata)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            Name = name;
            Type = type;
            Metadata = metadata ?? new ObservablePropertyMetadata();
            Metadata.Prepare(Type);
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return ReferenceEquals(this, obj) || Equals(obj as ObservableProperty);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}