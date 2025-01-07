using System;
using System.Collections;
using System.Collections.Generic;
using ObservableCollection;

public class ObservableList<T> : IList<T>, IObservableCollection<T>
{
	private readonly IList<T> _list;

	public event IObservableCollection<T>.OnItemChangedHandler OnItemChanged;
	public event IObservableCollection<T>.OnCollectionChangedHandler OnCollectionChanged;

	public ObservableList() => _list = new List<T>();
	public ObservableList(IList<T> list)
	{
		_list = list ?? throw new ArgumentNullException(nameof(list));
	}
	public ObservableList(IEnumerable<T> collection)
	{
		_list = new ObservableList<T>(collection);
	}

	public T this[int index]
	{
		get => _list[index];
		set
		{
			if (_list[index].Equals(value))
				return;
			_list[index] = value;
			TriggerItemChanged(value, ChangedType.Updated);
			TriggerCollectionChanged();
		}
	}

	public void Add(T item)
	{
		_list.Add(item);
		TriggerItemChanged(item, ChangedType.Added);
		TriggerCollectionChanged();
	}

	public void AddRange(IEnumerable<T> items)
	{
		foreach (var item in items)
		{
			_list.Add(item);
			TriggerItemChanged(item, ChangedType.Added);
		}
		TriggerCollectionChanged();
	}

	public void Insert(int index, T item)
	{
		_list.Insert(index, item);
		TriggerItemChanged(item, ChangedType.Added);
		TriggerCollectionChanged();
	}

	public bool Remove(T item)
	{
		bool isRemoved = _list.Remove(item);
		if (isRemoved)
		{
			TriggerItemChanged(item, ChangedType.Removed);
			TriggerCollectionChanged();
		}
		return isRemoved;
	}

	public void RemoveAt(int index)
	{
		T item = _list[index];
		_list.RemoveAt(index);
		TriggerItemChanged(item, ChangedType.Removed);
		TriggerCollectionChanged();
	}

	public void Clear()
	{
		if (_list.Count == 0)
			return;
		var prevList = new List<T>(_list);
		_list.Clear();
		foreach (var item in prevList)
			TriggerItemChanged(item, ChangedType.Removed);
		TriggerCollectionChanged();
	}

	public void TriggerItemChanged(T item, ChangedType changedType)
	{
		OnItemChanged?.Invoke(item, changedType);
	}

	public void TriggerCollectionChanged()
	{
		OnCollectionChanged?.Invoke(_list);
	}

	public int Count => _list.Count;
	public bool IsReadOnly => _list.IsReadOnly;
	public bool Contains(T item) => _list.Contains(item);
	public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
	public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
	public int IndexOf(T item) => _list.IndexOf(item);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}