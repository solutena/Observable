using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
			OnItemChanged?.Invoke(item, ObservableListChangedType.Added);
			OnCollectionChanged?.Invoke(_hashSet);
			return true;
		}
		return false;
	}

	public bool Remove(T item)
	{
		if (_hashSet.Remove(item))
		{
			OnItemChanged?.Invoke(item, ObservableListChangedType.Removed);
			OnCollectionChanged?.Invoke(_hashSet);
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
			OnItemChanged?.Invoke(item, ObservableListChangedType.Removed);
		OnCollectionChanged?.Invoke(_hashSet);
	}

	public int Count => _hashSet.Count;
	public bool IsReadOnly => false;
	public bool Contains(T item) => _hashSet.Contains(item);
	public void CopyTo(T[] array, int arrayIndex) => _hashSet.CopyTo(array, arrayIndex);
	public IEnumerator<T> GetEnumerator() => _hashSet.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _hashSet.GetEnumerator();
}