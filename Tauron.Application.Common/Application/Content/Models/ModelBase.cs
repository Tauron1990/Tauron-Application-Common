using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Models
{
    public class ModelBase : ObservableObject, IModel, ICustomTypeDescriptor, INotifyBuildCompled
    {
        private static GroupDictionary<Type, ObservableProperty> _properties = new GroupDictionary<Type, ObservableProperty>();

        private GroupDictionary<string, WeakAction> _changedActions;

        private GroupDictionary<string, PropertyIssue> _propertyIssues = new GroupDictionary<string, PropertyIssue>();
        private EditableDictionary<string, object> _values;

        public ModelBase()
        {
            _changedActions = new GroupDictionary<string, WeakAction>();
            _values = new EditableDictionary<string, object>();
            ValidatorContext = new ValidatorContext(this);
        }

        public bool BlockIsNotEditing
        {
            get => _values.BlockIsNotEditing;
            set
            {
                _values.BlockIsNotEditing = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing => _values.IsEditing;

        public bool BlockOnPropertyChanged { get; set; }

        [NotNull]
        protected ValidatorContext ValidatorContext { get; private set; }

        protected virtual bool HasErrorOverride => false;
        public bool HasNoErrors => !HasErrors;

        public bool ConvertPropertyIssuesToString { get; set; }

        [CanBeNull]
        public IEnumerable GetErrors([CanBeNull] string propertyName)
        {
            if (propertyName == null) return null;

            var enumerable = !_propertyIssues.ContainsKey(propertyName) ? GetErrorsOverride(propertyName) : _propertyIssues[propertyName];
            if (enumerable == null) return null;

            return ConvertPropertyIssuesToString ? enumerable.Cast<object>().Select(o => o.ToString()) : enumerable;
        }

        public bool HasErrors => _propertyIssues.Count != 0 || HasErrorOverride;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public virtual void BeginEdit()
        {
            _values.BeginEdit();
            OnPropertyChangedExplicit(nameof(IsEditing));
        }

        public virtual void EndEdit()
        {
            _values.EndEdit();
            OnPropertyChangedExplicit(nameof(IsEditing));
        }

        public virtual void CancelEdit()
        {
            _values.CancelEdit();
            OnPropertyChangedExplicit(nameof(IsEditing));
        }

        public virtual void BuildCompled()
        {
        }

        [NotNull]
        public static IModel ResolveModel([NotNull] string name) => CommonApplication.Current.Container.Resolve<IModel>(name, false);

        [NotNull]
        public static IEnumerable<ObservableProperty> GetProperties([NotNull] Type type) => 
            _properties.TryGetValue(type, out var properties)
            ? new ReadOnlyEnumerator<ObservableProperty>(properties)
            : Enumerable.Empty<ObservableProperty>();

        [NotNull]
        public static ObservableProperty RegisterProperty([NotNull] string name, [NotNull] Type ownerType,
            [NotNull] Type type, [CanBeNull] ObservablePropertyMetadata metadata)
        {
            var properties = _properties[ownerType];

            ObservableProperty prop;
            if ((prop = properties.FirstOrDefault(p => p.Name == name)) != null) return prop;

            prop = new ObservableProperty(name, type, metadata, ownerType);
            properties.Add(prop);
            return prop;
        }

        [NotNull]
        public static ObservableProperty RegisterProperty([NotNull] string name, [NotNull] Type ownerType,
            [NotNull] Type type) => RegisterProperty(name, ownerType, type, null);

        public void RegisterPropertyChanged([NotNull] ObservableProperty prop,
            [NotNull] ObservablePropertyChanged changed)
        {
            Argument.NotNull(prop, nameof(prop));
            Argument.NotNull(changed, nameof(changed));

            _changedActions.Add(prop.Name, new WeakAction(changed.Target, changed.Method));
        }

        public void RegisterPropertyChanged([NotNull] ObservableProperty prop,
            [NotNull] Action<string> changed)
        {
            Argument.NotNull(prop, nameof(prop));
            Argument.NotNull(changed, nameof(changed));

            _changedActions.Add(prop.Name, new WeakAction(changed.Target, changed.Method));
        }

        public void SetValue([NotNull] ObservableProperty property, [CanBeNull] object value)
        {
            if (!_values.TryGetValue(property.Name, out var prop))
            {
                SetValueReal(property, value);
                return;
            }

            if (Equals(value, prop)) return;

            SetValueReal(property, value);
        }

        private const string AnonymosPrefix = "Anonymos---";

        protected bool SetProperty(object value, [CallerMemberName] string name = null)
        {
            string realName = AnonymosPrefix + name;

            if (_values.TryGetValue(realName, out var tempValue) && Equals(tempValue, value)) return false;
            _values[realName] = value;

            OnPropertyChanged(new PropertyChangedEventArgs(name));
            return true;
        }

        protected bool SetProperty(object value, Action changed, [CallerMemberName] string name = null)
        {
            if (!SetProperty(value, name)) return false;

            changed();
            return true;

        }

        protected TType GetProperty<TType>([CallerMemberName] string name = null)
        {
            if (_values.TryGetValue(AnonymosPrefix + name, out var value))
                return value is TType propertyValue ? propertyValue : default;
            return default;
        }

        [CanBeNull]
        protected object GetValue([NotNull] ObservableProperty property) => GetValueCommon(property);

        [CanBeNull]
        public TType GetValue<TType>([NotNull] ObservableProperty property)
        {
            var value = GetValueCommon(property);
            return value is TType type ? type : default;
        }

        [CanBeNull]
        private object GetValueCommon(ObservableProperty property)
        {
            var getter = property.Metadata.CustomGetter;
            if (getter != null) return getter(property);

            return _values.TryGetValue(property.Name, out var value) ? value : property.Metadata.DefaultValue ?? GetDefaultValue(property.Type);
        }

        public static object GetDefaultValue(Type type) => type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;

        
        protected void SetValue([NotNull] string property, [NotNull] object value)
        {
            var oprop = GetProperties(GetType()).FirstOrDefault(p => p.Name == property);

            if (oprop == null) return;

            if (!_values.TryGetValue(property, out var prop))
            {
                SetValueReal(oprop, value);
                return;
            }

            if (Equals(value, prop)) return;

            SetValueReal(oprop, value);
        }

        private void SetValueReal([NotNull] ObservableProperty property, [CanBeNull] object value)
        {
            if (property.Metadata.IsReadOnly && _values.ContainsKey(property.Name))
                throw new InvalidOperationException($"Property Is ReadOnly: {property.Name}");

            if (!property.Metadata.ValidateValueCallback?.Invoke(this, property, value) ?? false)
                throw new ArgumentException("Invalid Property Content: " + property.Name);

            if (property.Metadata.CoerceValueCallback != null)
                value = property.Metadata.CoerceValueCallback(this, value);

            if (value != null)
            {
                if (value.GetType() != property.Type)
                    throw new ArgumentException("Invalid Content Type: " + property.Name);
            }

            _values[property.Name] = value;

            if (property.Metadata.ForceAllValidation)
                ValidateAll();
            else
                ValidateProperty(property, value);

            property.Metadata.PropertyChanged?.Invoke(property, this, value);

            InvokePropertyChanged(property, value);
        }

        private void InvokePropertyChanged([NotNull] ObservableProperty prop, [CanBeNull] object value)
        {
            if (BlockOnPropertyChanged) BlockOnPropertyChanged = false;
            else OnPropertyChangedExplicit(prop.Name);

            if (!_changedActions.TryGetValue(prop.Name, out var changedActions)) return;

            if (changedActions == null) return;

            foreach (var changedAction in changedActions)
                switch (changedAction.ParameterCount)
                {
                    case 0:
                        changedAction.Invoke();
                        break;
                    case 2:
                        changedAction.Invoke(this, value);
                        break;
                    default:
                        changedAction.Invoke(_values[prop.Name]);
                        break;
                }
        }

        public void ValidateAll()
        {
            var props = _properties[GetType()];
            if (props == null) return;

            foreach (var property in props) ValidateProperty(property, GetValue(property));
        }

        private void ValidateProperty([NotNull] ObservableProperty property, [CanBeNull] object value)
        {
            ValidatorContext.Property = property;
            SetIssues(property.Name, ValidatorConfiguration.Validator.Validate(ValidatorContext, value));

            // ReSharper disable once AssignNullToNotNullAttribute
            ValidatorContext.Property = null;

            OnErrorsChanged(property.Name);
            OnPropertyChanged(() => HasNoErrors);
            OnPropertyChanged(() => HasErrors);
        }

        public GroupDictionary<string, PropertyIssue> GetIssuesDictionary() => _propertyIssues;

        protected void SetIssues([NotNull] string propertyName, [NotNull] PropertyIssue[] issues)
        {
            if (_propertyIssues.ContainsKey(propertyName))
                _propertyIssues.Remove(propertyName);
            if (issues.Length == 0)
                return;

            _propertyIssues.AddRange(propertyName, issues);
        }

        protected virtual IEnumerable GetErrorsOverride(string property) => null;

        protected virtual void OnErrorsChanged([NotNull] string propertyName)
        {
            var handler = ErrorsChanged;
            handler?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #region CustomTypeDescriptor

        private sealed class MemberDescriptorEqualityComparer : IEqualityComparer<MemberDescriptor>
        {
            public static readonly MemberDescriptorEqualityComparer Comparer = new MemberDescriptorEqualityComparer();

            public bool Equals(MemberDescriptor x, MemberDescriptor y) => y != null && x != null && string.Equals(x.Name, y.Name);

            public int GetHashCode(MemberDescriptor obj) => obj.Name.GetHashCode();
        }

        public class ObservablePropertyDescriptor : PropertyDescriptor
        {
            private readonly ObservableProperty _prop;

            public ObservablePropertyDescriptor([NotNull] ObservableProperty prop, [NotNull] Type componentType, [NotNull] ModelBase owner)
                : base(prop.Name, null)
            {
                if (prop == null) throw new ArgumentNullException(nameof(prop));
                if (componentType == null) throw new ArgumentNullException(nameof(componentType));
                _prop = prop ?? throw new ArgumentNullException(nameof(prop));
                Owner = owner ?? throw new ArgumentNullException(nameof(owner));
                ComponentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
            }

            public ModelBase Owner { get; }

            [NotNull]
            public override Type ComponentType { get; }

            public override bool IsReadOnly => false;

            [NotNull]
            public override Type PropertyType => _prop.Type;

            public override bool CanResetValue(object component) => true;

            public override object GetValue(object component) => Owner.GetValue(_prop);

            public override void ResetValue(object component) => Owner.SetValue(_prop, _prop.Metadata.DefaultValue);

            public override void SetValue(object component, object value) => Owner.SetValue(_prop, value);

            public override bool ShouldSerializeValue(object component) => Owner.GetValue(_prop) != _prop.Metadata.DefaultValue;
        }

        [NotNull]
        AttributeCollection ICustomTypeDescriptor.GetAttributes() => TypeDescriptor.GetAttributes(this, true);

        string ICustomTypeDescriptor.GetClassName() => TypeDescriptor.GetClassName(this, true);

        string ICustomTypeDescriptor.GetComponentName() => TypeDescriptor.GetComponentName(this, true);

        TypeConverter ICustomTypeDescriptor.GetConverter() => TypeDescriptor.GetConverter(this, true);

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this, true);

        [NotNull]
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true) ?? throw new InvalidOperationException();

        [NotNull]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents([NotNull] Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);

        [NotNull]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => TypeDescriptor.GetEvents(this, true);

        [NotNull]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties([NotNull] Attribute[] attributes) => TypeDescriptor.GetProperties(this, attributes, true);

        [NotNull]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            var coll = TypeDescriptor.GetProperties(this, true);
            return
                new PropertyDescriptorCollection(
                    GetPropertyDescriptors()
                        .Union(CustomObservableProperties())
                        .Union(coll.Cast<MemberDescriptor>(), MemberDescriptorEqualityComparer.Comparer)
                        .Cast<PropertyDescriptor>()
                        .ToArray(), true);
        }

        [NotNull]
        object ICustomTypeDescriptor.GetPropertyOwner([NotNull] PropertyDescriptor pd)
        {
            if (pd is ObservablePropertyDescriptor descriptor) return descriptor.Owner;

            return this;
        }

        public IEnumerable<ObservablePropertyDescriptor> GetPropertyDescriptors()
        {
            var properties = new List<ObservablePropertyDescriptor>();

            var orgType = GetType();
            var type = orgType;

            while (type != null && type != typeof(ModelBase) && type != typeof(object))
            {
                if (_properties.TryGetValue(type, out var props))
                    properties.AddRange(props.Select(observableProperty => new ObservablePropertyDescriptor(observableProperty, orgType, this)));

                type = type.BaseType;
            }

            return properties;
        }

        protected virtual IEnumerable<ObservablePropertyDescriptor> CustomObservableProperties()
        {
            yield break;
        }

        #endregion CustomTypeDescriptor
    }
}