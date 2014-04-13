using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public class ModelBase : ObservableObject, IModel, ICustomTypeDescriptor
    {
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

        public ModelBase()
        {
            _changedActions = new GroupDictionary<string, WeakAction>();
            _values = new Dictionary<string, object>();
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

            if (property.Metadata.PropertyChanged != null) property.Metadata.PropertyChanged(property, this, value);

            OnPropertyChangedExplicit(property.Name);
        }

        private void InvokePropertyChanged([NotNull] ObservableProperty prop, [NotNull] object value)
        {
            OnPropertyChangedExplicit(prop.Name);

            ICollection<WeakAction> changedActions;
            if (_changedActions.TryGetValue(prop.Name, out changedActions)) return;

            foreach (var changedAction in changedActions)
            {
                if (changedAction.ParameterCount == 0) changedAction.Invoke();
                else if (changedAction.ParameterCount == 2) changedAction.Invoke(this, value);
                else changedAction.Invoke(_values[prop.Name]);
            }
        }

        #region CustomTypeDescriptor

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

            foreach (var property in _properties.AllValues) coll.Add(new ObservablePropertyDescriptor(property, type));

            return coll;
        }

        [NotNull]
        object ICustomTypeDescriptor.GetPropertyOwner([NotNull] PropertyDescriptor pd)
        {
            return this;
        }

        #endregion CustomTypeDescriptor

    }
}
