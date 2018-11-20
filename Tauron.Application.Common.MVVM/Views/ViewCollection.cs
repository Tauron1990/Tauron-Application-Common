using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using JetBrains.Annotations;
using Tauron.Application.Models;

namespace Tauron.Application.Views
{
    //[PublicAPI]
    //public class ViewCollection : ICollection<ViewModelBase>, INotifyCollectionChanged
    //{
    //    public IEnumerable<object> Views => _viewsInternal.Values;

    //    private ObservableDictionary<ViewModelBase, object> _viewsInternal;

    //    public ViewCollection() => _viewsInternal = new ObservableDictionary<ViewModelBase, object>();

    //    public IEnumerator<ViewModelBase> GetEnumerator() => _viewsInternal.Keys.GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //    public void Add(ViewModelBase item) => throw new NotSupportedException();
    //    public void Clear() => throw new NotSupportedException();
    //    public bool Contains(ViewModelBase item) => throw new NotSupportedException();
    //    public void CopyTo(ViewModelBase[] array, int arrayIndex) => throw new NotSupportedException();
    //    public bool Remove(ViewModelBase item) => throw new NotSupportedException();

    //    public int Count { get; }
    //    public bool IsReadOnly { get; }
    //    public event NotifyCollectionChangedEventHandler CollectionChanged;
    //}
}