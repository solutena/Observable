using System;
using System.Collections;
using System.Collections.Generic;

public class ObservableHashSet<T> : ICollection<T>, IObservable<T>
{
	private readonly HashSet<T> _hashSet;

	public event IObservable<T>.OnItemChangedHandler OnItemChanged;
	public event IObservable<T>.OnCollectionChangedHandler OnCollectionChanged;

	public ObservableHashSet() => _hashSet = new HashSet<T>();
	public ObservableHashSet(HashSet<T> hashSet)
	{
		_hashSet = hashSet ?? throw new ArgumentNullException(nameof(hashSet));
	}
	public ObservableHashSet(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		_hashSet = new HashSet<T>(collection);
	}

	void ICollection<T>.Add(T item) => Add(item);
	public bool Add(T item)
	{
		if (_hashSet.Add(item))
		{
			TriggerItemChanged(item, ObservableListChangedType.Added);
			TriggerCollectionChanged();
			return true;
		}
		return false;
	}

	public bool Remove(T item)
	{
		if (_hashSet.Remove(item))
		{
			TriggerItemChanged(item, ObservableListChangedType.Removed);
			TriggerCollectionChanged();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		if (_hashSet.Count == 0)
			return;
		var prevList = new List<T>(_hashSet);
		_hashSet.Clear();
		foreach (var item in prevList)
			TriggerItemChanged(item, ObservableListChangedType.Removed);
		TriggerCollectionChanged();
	}

	public void TriggerItemChanged(T item, ObservableListChangedType changedType)
	{
		OnItemChanged?.Invoke(item, changedType);
	}

	public void TriggerCollectionChanged()
	{
		OnCollectionChanged?.Invoke(_hashSet);
	}

	public int Count => _hashSet.Count;
	public bool IsReadOnly => false;
	public bool Contains(T item) => _hashSet.Contains(item);
	public void CopyTo(T[] array, int arrayIndex) => _hashSet.CopyTo(array, arrayIndex);
	public IEnumerator<T> GetEnumerator() => _hashSet.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _hashSet.GetEnumerator();
}