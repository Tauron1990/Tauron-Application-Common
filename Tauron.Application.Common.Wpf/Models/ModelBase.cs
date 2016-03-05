using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;
using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;

namespace Tauron.Application.Models
{
    public class ModelBase : ObservableObject, IModel, ICustomTypeDescriptor, INotifyDataErrorInfo, INotifyBuildCompled
    {
        [NotNull]
        public static ModelBase ResolveModel([NotNull] string name)
        {
            return CommonApplication.Current.Container.Resolve<ModelBase>(name, false);
        }

        private static GroupDictionary<Type, ObservableProperty> _properties =
            new GroupDictionary<Type, ObservableProperty>();

        private GroupDictionary<string, WeakAction> _changedActions;
        private EditableDictionary<string, object> _values;

        public bool BlockIsNotEditing
        {
            get { return _values.BlockIsNotEditing; }
            set { _values.BlockIsNotEditing = value; OnPropertyChanged(); }
        }

        public bool IsEditing => _values.IsEditing;

        [NotNull]
        public static IEnumerable<ObservableProperty> GetProperties([NotNull] Type type)
        {
            ICollection<ObservableProperty> properties;
            return _properties.TryGetValue(type, out properties)
                       ? new ReadOnlyEnumerator<ObservableProperty>(properties)
                       : Enumerable.Empty<ObservableProperty>();
        }

        [NotNull]
        protected ValidatorContext ValidatorContext { get; private set; }

        public ModelBase()
        {
            _changedActions = new GroupDictionary<string, WeakAction>();
            _values = new EditableDictionary<string, object>();
            ValidatorContext = new ValidatorContext(this);
        }

        [NotNull]
        public static ObservableProperty RegisterProperty([NotNull] string name, [NotNull] Type ownerType,
                                                          [NotNull] Type type, [CanBeNull] ObservablePropertyMetadata metadata)
        {
            ICollection<ObservableProperty> properties = _properties[ownerType];

            ObservableProperty prop;
            if ((prop = properties.FirstOrDefault(p => p.Name == name)) != null) return prop;

            prop = new ObservableProperty(name, type, metadata);
            properties.Add(prop);
            return prop;
        }

        [NotNull]
        public static ObservableProperty RegisterProperty([NotNull] string name, [NotNull] Type ownerType,
                                                          [NotNull] Type type)
        {
            return RegisterProperty(name, ownerType, type, null);
        }

        public void RegisterPropertyChanged([NotNull] ObservableProperty prop,
                                            [NotNull] ObservablePropertyChanged changed)
        {
            if (prop == null) throw new ArgumentNullException(nameof(prop));
            if (changed == null) throw new ArgumentNullException(nameof(changed));

            _changedActions.Add(prop.Name, new WeakAction(changed.Target, changed.Method));
        }

        public void RegisterPropertyChanged([NotNull] ObservableProperty prop,
                                    [NotNull] Action<string> changed)
        {
            if (prop == null) throw new ArgumentNullException(nameof(prop));
            if (changed == null) throw new ArgumentNullException(nameof(changed));

            _changedActions.Add(prop.Name, new WeakAction(changed.Target, changed.Method));
        }

        public void SetValue([NotNull] ObservableProperty property, [CanBeNull] object value)
        {
            object prop;
            if (!_values.TryGetValue(property.Name, out prop))
            {
                SetValueReal(property, value);
                return;
            }

            if (Equals(value, prop)) return;

            SetValueReal(property, value);
        }

        [CanBeNull]
        protected object GetValue([NotNull] ObservableProperty property)
        {
            return GetValueCommon(property);
        }

        [CanBeNull]
        protected TType GetValue<TType>([NotNull] ObservableProperty property)
        {
            var value = GetValueCommon(property);
            return value != null ? (TType)value : default(TType);
        }

        [CanBeNull]
        private object GetValueCommon(ObservableProperty property)
        {
            var getter = property.Metadata.CustomGetter;
            if (getter != null) return getter(property);

            object value;

            return _values.TryGetValue(property.Name, out value) ? value : property.Metadata.DefaultValue;
        }

        protected void SetValue([NotNull] string property, [NotNull] object value)
        {
            ObservableProperty oprop = GetProperties(GetType()).FirstOrDefault(p => p.Name == property);

            if(oprop == null) return;

            object prop;
            if (!_values.TryGetValue(property, out prop))
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
            {
                throw new ArgumentException("Invalid Property Content: " + property.Name);
            }

            if (property.Metadata.CoerceValueCallback != null)
            {
                value = property.Metadata.CoerceValueCallback(this, value);
            }

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
            OnPropertyChangedExplicit(prop.Name);

            ICollection<WeakAction> changedActions;
            if (!_changedActions.TryGetValue(prop.Name, out changedActions)) return;

            if(changedActions == null) return;
            
            foreach (var changedAction in changedActions)
            {
                if (changedAction.ParameterCount == 0) changedAction.Invoke();
                else if (changedAction.ParameterCount == 2) changedAction.Invoke(this, value);
                else changedAction.Invoke(_values[prop.Name]);
            }
        }

        #region CustomTypeDescriptor

        private sealed class MemberDescriptorEqualityComparer : IEqualityComparer<MemberDescriptor>
        {
            public static readonly MemberDescriptorEqualityComparer Comparer = new MemberDescriptorEqualityComparer();
            public bool Equals([NotNull] MemberDescriptor x, [NotNull] MemberDescriptor y)
            {
                return string.Equals(x.Name, y.Name);
            }

            public int GetHashCode([NotNull] MemberDescriptor obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        private class ObservablePropertyDescriptor : PropertyDescriptor
        {
            private readonly ObservableProperty _prop;

            public ObservablePropertyDescriptor([NotNull] ObservableProperty prop, [NotNull] Type componentType)
                : base(prop.Name, null)
            {
                Contract.Requires<ArgumentNullException>(prop != null, "prop");
                Contract.Requires<ArgumentNullException>(componentType != null, "componentType");

                _prop = prop;
                ComponentType = componentType;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override object GetValue(object component)
            {
                return ((ModelBase) component).GetValue(_prop);
            }

            public override void ResetValue(object component)
            {
                ((ModelBase)component).SetValue(_prop, _prop.Metadata.DefaultValue);
            }

            public override void SetValue(object component, object value)
            {
                ((ModelBase)component).SetValue(_prop, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return ((ModelBase) component).GetValue(_prop) != _prop.Metadata.DefaultValue;
            }

            [NotNull]
            public override Type ComponentType { get; }

            public override bool IsReadOnly => false;

            [NotNull]
            public override Type PropertyType => _prop.Type;
        }

        [NotNull]
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        [NotNull]
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        [NotNull]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents([NotNull] Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        [NotNull]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        [NotNull]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties([NotNull] Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes, true);
        }

        [NotNull]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            var coll = TypeDescriptor.GetProperties(this, true);
            var type = GetType();
            var props =
                _properties.AllValues.Concat(CustomObservableProperties())
                    .Select(property => new ObservablePropertyDescriptor(property, type))
                    .Cast<PropertyDescriptor>()
                    .ToList();

            return
                new PropertyDescriptorCollection(
                    props.Union(coll.Cast<MemberDescriptor>(), MemberDescriptorEqualityComparer.Comparer)
                        .Cast<PropertyDescriptor>()
                        .ToArray(), true);
        }

        [NotNull]
        object ICustomTypeDescriptor.GetPropertyOwner([NotNull] PropertyDescriptor pd)
        {
            return this;
        }

        protected virtual IEnumerable<ObservableProperty> CustomObservableProperties()
        {
            yield break;
        }
        #endregion CustomTypeDescriptor

        public void ValidateAll()
        {
            var props = _properties[GetType()];
            if(props == null) return;

            foreach (var property in props)
            {
                ValidateProperty(property, GetValue(property));
            }
        }

        private void ValidateProperty([NotNull] ObservableProperty property, [CanBeNull] object value)
        {
            var rules = property.Metadata.ModelRules;
            if (rules == null) return;
            
            ValidatorContext.Property = property;
            SetIssues(property.Name,
                rules.Where(r => !r.IsValidValue(value, ValidatorContext))
                    .Select(r => new PropertyIssue(property.Name, value, string.Format(r.Message(), value)))
                    .ToArray());
            // ReSharper disable once AssignNullToNotNullAttribute
            ValidatorContext.Property = null;

            OnErrorsChanged(property.Name);
            OnPropertyChanged(() => HasNoErrors);
            OnPropertyChanged(() => HasErrors);
        }

        private GroupDictionary<string, PropertyIssue> _propertyIssues = new GroupDictionary<string, PropertyIssue>();

        internal GroupDictionary<string, PropertyIssue> GetIssuesDictionary()
        {
           return _propertyIssues;
        }

        private void SetIssues([NotNull] string propertyName, [NotNull] PropertyIssue[] issues)
        {
                if (_propertyIssues.ContainsKey(propertyName))
                    _propertyIssues.Remove(propertyName);
                if (issues.Length == 0)
                    return;

                _propertyIssues.AddRange(propertyName, issues);
        }

        [CanBeNull]
        public IEnumerable GetErrors([NotNull] string propertyName)
        {
            return !_propertyIssues.ContainsKey(propertyName) ? GetErrorsOverride(propertyName) : _propertyIssues[propertyName];
        }

        protected virtual bool HasErrorOverride => false;
        protected virtual IEnumerable GetErrorsOverride(string property)
        {
            return null;
        }

        public bool HasErrors => _propertyIssues.Count != 0 &&  HasErrorOverride;
        public bool HasNoErrors => !HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private void OnErrorsChanged([NotNull] string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
            handler?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public virtual void BuildCompled()
        {
        
        }

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
    }
}
