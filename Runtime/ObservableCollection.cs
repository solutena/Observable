using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObservableCollection<T> : ICollection<T>
{
	readonly ICollection<T> _collection;

	public event Action<T> OnAdd;
	public event Action<T> OnRemove;
	public event Action<ObservableCollection<T>> OnChanged;

	public ObservableCollection(ICollection<T> collection) => _collection = collection;
	public ObservableCollection(IEnumerable<T> collection) => _collection = collection.ToList();

	public T this[int index]
	{
		get
		{
			if (_collection is IList<T> list)
				return list[index];
			return _collection.ElementAt(index);
		}
	}

	public void Add(T item)
	{
		_collection.Add(item);
		OnAdd?.Invoke(item);
		OnChanged?.Invoke(this);
	}

	public bool Remove(T item)
	{
		if (_collection.Contains(item) == false)
			return false;
		_collection.Remove(item);
		OnRemove?.Invoke(item);
		OnChanged?.Invoke(this);
		return true;
	}

	public void Clear()
	{
		var itemsToRemove = new List<T>(_collection);
		_collection.Clear();
		foreach (var item in itemsToRemove)
			OnRemove?.Invoke(item);
		OnChanged?.Invoke(this);
	}

	public void Refresh()
	{
		OnChanged?.Invoke(this);
	}

	public int Count => _collection.Count;
	public bool IsReadOnly => _collection.IsReadOnly;
	public bool Contains(T item) => _collection.Contains(item);
	public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

	public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
