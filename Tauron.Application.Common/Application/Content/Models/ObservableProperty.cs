﻿using System;
using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public delegate void ObservablePropertyChanged(ObservableProperty prop, ModelBase model, object value);

    public delegate object ObservableCoerceValueCallback(ModelBase model, object value);

    public delegate bool ObservableValidateValueCallback(ModelBase model, ObservableProperty property, object value);

    [Serializable]
    [PublicAPI]
    public sealed class ObservablePropertyMetadata
    {
        [CanBeNull] private string _displayName;

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

        public ObservablePropertyMetadata([CanBeNull] object defaultValue) => DefaultValue = defaultValue;

        public ObservablePropertyMetadata(bool isReadOnly) => IsReadOnly = isReadOnly;

        public ObservablePropertyMetadata([CanBeNull] ObservablePropertyChanged propertyChanged) => PropertyChanged = propertyChanged;

        public ObservablePropertyMetadata([CanBeNull] ObservableValidateValueCallback observableValidateValueCallback) => ValidateValueCallback = observableValidateValueCallback;

        public ObservablePropertyMetadata([CanBeNull] ObservablePropertyChanged propertyChanged, [CanBeNull] ObservableValidateValueCallback observableValidateValueCallback)
        {
            ValidateValueCallback = observableValidateValueCallback;
            PropertyChanged = propertyChanged;
        }

        public ObservablePropertyMetadata()
        {
        }

        [CanBeNull]
        public object DefaultValue { get; private set; }

        public bool IsReadOnly { get; private set; }

        [CanBeNull]
        public ObservableValidateValueCallback ValidateValueCallback { get; private set; }

        [CanBeNull]
        public ObservableCoerceValueCallback CoerceValueCallback { get; private set; }

        [CanBeNull]
        public ObservablePropertyChanged PropertyChanged { get; private set; }

        [CanBeNull]
        public Func<ObservableProperty, object> CustomGetter { get; set; }

        public bool ForceAllValidation { get; set; }

        public string PropertyName { get; private set; }
        public Type OwnerType { get; private set; }

        [CanBeNull]
        public string DisplayName
        {
            get => _displayName ?? PropertyName;
            set => _displayName = value;
        }

        internal bool Prepare([NotNull] Type targetType, [NotNull]Type ownerType, [NotNull]string propertyName)
        {
            Argument.NotNull(targetType, nameof(targetType));
            Argument.NotNull(ownerType, nameof(ownerType));
            Argument.NotNull(propertyName, nameof(propertyName));

            if(PropertyName != null || OwnerType != null)
                throw new InvalidOperationException("One Metadata on Multiple Propertys");

            PropertyName = propertyName;
            OwnerType = ownerType;

            if (string.IsNullOrWhiteSpace(DisplayName))
                DisplayName = DisplayNameHelper.GetDisplayName(ownerType.Name, propertyName, () => ownerType.GetProperty(propertyName));

            if (DefaultValue != null && targetType != DefaultValue.GetType()) return false;
            
            try
            {
                if (DefaultValue != null && targetType.BaseType == typeof(ValueType))
                {
                    var constructorInfo = targetType.GetConstructor(Type.EmptyTypes);
                    if (constructorInfo != null)
                        DefaultValue = constructorInfo.Invoke(null);
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }

            return true;
        }

    }

    [PublicAPI]
    [Serializable]
    public sealed class ObservableProperty : IEquatable<ObservableProperty>
    {
        public ObservableProperty([NotNull] string name, [NotNull] Type type,
            [CanBeNull] ObservablePropertyMetadata metadata, Type ownerType)
        {
            Name = Argument.NotNull(name, nameof(name));
            Type = Argument.NotNull(type, nameof(type));
            Metadata = metadata ?? new ObservablePropertyMetadata();
            Metadata.Prepare(Type, ownerType, name);
        }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public Type Type { get; private set; }

        [NotNull]
        public ObservablePropertyMetadata Metadata { get; private set; }

        public bool Equals(ObservableProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
        }

        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(ObservableProperty left, ObservableProperty right) => Equals(left, right);

        public static bool operator !=(ObservableProperty left, ObservableProperty right) => !Equals(left, right);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return ReferenceEquals(this, obj) || Equals(obj as ObservableProperty);
        }

        public override string ToString() => Name;
    }
}