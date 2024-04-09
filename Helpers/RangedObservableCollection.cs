using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace dev_flow.Helpers;

/// <summary>
/// Represents a collection that provides notifications when items get added, removed, or when the whole list is refreshed, 
/// and allows adding multiple items at once.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class RangedObservableCollection<T> : ObservableCollection<T>
{
    private bool _suppressNotification;

    /// <summary>
    /// Raises the <see cref="E:CollectionChanged"/> event with the provided arguments.
    /// </summary>
    /// <param name="e">Arguments of the event being raised.</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!_suppressNotification)
            base.OnCollectionChanged(e);
    }


    /// <summary>
    /// Adds the elements of the specified collection to the end of the ObservableCollection T.
    /// </summary>
    /// <param name="list">The collection whose elements should be added to the end of the ObservableCollection T.</param>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
    public void AddRange(IEnumerable<T> list)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        _suppressNotification = true;

        foreach (T item in list)
        {
            Add(item);
        }

        _suppressNotification = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}