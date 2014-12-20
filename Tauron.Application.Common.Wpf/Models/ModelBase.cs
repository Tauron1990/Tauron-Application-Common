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
        private Dictionary<string, object> _values;

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
            _values = new Dictionary<string, object>();
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
            if (prop == null) throw new ArgumentNullException("prop");
            if (changed == null) throw new ArgumentNullException("changed");

            _changedActions.Add(prop.Name, new WeakAction(changed.Target, changed.Method));
        }

        public void RegisterPropertyChanged([NotNull] ObservableProperty prop,
                                    [NotNull] Action<string> changed)
        {
            if (prop == null) throw new ArgumentNullException("prop");
            if (changed == null) throw new ArgumentNullException("changed");

            _changedActions.Add(prop.Name, new WeakAction(changed.Target, changed.Method));
        }

        [NotNull]
        public object GetValue([NotNull] ObservableProperty property)
        {
            object value;
            return _values.TryGetValue(property.Name, out value) ? value : value;
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

        [NotNull]
        protected object GetValue([NotNull] string property)
        {
            object value;
            return _values.TryGetValue(property, out value) ? value : value;
        }

        protected void SetValue([NotNull] string property, [NotNull] object value)
        {
            ObservableProperty oprop = GetProperties(GetType()).FirstOrDefault(p => p.Name == property);

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
            if (property.Metadata.ValidateValueCallback != null)
            {
                if (!property.Metadata.ValidateValueCallback(this, property, value))
                {
                    throw new ArgumentException("Invalid Property Content: " + property.Name);
                }
            }

            if (property.Metadata.CoerceValueCallback != null)
            {
                value = property.Metadata.CoerceValueCallback(this, value);
            }

            if (value != null)
            {
                if(value.GetType() != property.Type)
                    throw new ArgumentException("Invalid Content Type: " + property.Name);
            }

            _values[property.Name] = value;

            //ValidateProperty(property, value);
            ValidateAll();

            if (property.Metadata.PropertyChanged != null) property.Metadata.PropertyChanged(property, this, value);

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
            private readonly Type _componentType;

            public ObservablePropertyDescriptor([NotNull] ObservableProperty prop, [NotNull] Type componentType)
                : base(prop.Name, null)
            {
                Contract.Requires<ArgumentNullException>(prop != null, "prop");
                Contract.Requires<ArgumentNullException>(componentType != null, "componentType");

                _prop = prop;
                _componentType = componentType;
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
            public override Type ComponentType
            {
                get { return _componentType; }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            [NotNull]
            public override Type PropertyType
            {
                get { return _prop.Type; }
            }
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
                _properties.AllValues.Select(property => new ObservablePropertyDescriptor(property, type))
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
            ValidatorContext.Property = null;

            OnErrorsChanged(property.Name);
            OnPropertyChanged(() => HasNoErrors);
            OnPropertyChanged(() => HasErrors);
        }

        private GroupDictionary<string, PropertyIssue> _propertyIssues = new GroupDictionary<string, PropertyIssue>();

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
            return !_propertyIssues.ContainsKey(propertyName) ? null : _propertyIssues[propertyName];
        }

        public bool HasErrors { get { return _propertyIssues.Count != 0; } }
        public bool HasNoErrors { get { return !HasErrors; } }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private void OnErrorsChanged([NotNull] string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
            if (handler != null) handler(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public virtual void BuildCompled()
        {
        
        }
    }
}
