using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObservableList<T> : IList<T>, IObservableCollection<T>
{
	[SerializeField] private List<T> _list;

	public ObservableList() => _list = new List<T>();
	public ObservableList(IEnumerable<T> collection) => Initialize(collection);

	public event IObservableCollection<T>.OnItemChangedHandler OnAddedChanged;
	public event IObservableCollection<T>.OnItemChangedHandler OnRemovedChanged;
	public event IObservableCollection<T>.OnCollectionChangedHandler OnCollectionChanged;
	
	public delegate void OnItemUpdatedHandler(int index);
	public event OnItemUpdatedHandler OnUpdatedChanged;

	public void Initialize(IEnumerable<T> collection) => _list = new List<T>(collection ?? throw new ArgumentNullException(nameof(collection)));
	public void TriggerAddedChanged(T item) => OnAddedChanged?.Invoke(item);
	public void TriggerRemovedChanged(T item) => OnRemovedChanged?.Invoke(item);
	public void TriggerUpdatedChanged(int index) => OnUpdatedChanged?.Invoke(index);
	public void TriggerCollectionChanged() => OnCollectionChanged?.Invoke(_list);

	public T this[int index]
	{
		get => _list[index];
		set
		{
			if (_list[index].Equals(value))
				return;
			_list[index] = value;
			OnUpdatedChanged?.Invoke(index);
			OnCollectionChanged?.Invoke(_list);
		}
	}

	public void Add(T item)
	{
		_list.Add(item);
		OnAddedChanged?.Invoke(item);
		OnCollectionChanged?.Invoke(_list);
	}

	public void AddRange(IEnumerable<T> items)
	{
		foreach (var item in items)
		{
			_list.Add(item);
			OnAddedChanged?.Invoke(item);
		}
		OnCollectionChanged?.Invoke(_list);
	}

	public void Insert(int index, T item)
	{
		_list.Insert(index, item);
		OnAddedChanged?.Invoke(item);
		OnCollectionChanged?.Invoke(_list);
	}

	public bool Remove(T item)
	{
		bool isRemoved = _list.Remove(item);
		if (isRemoved)
		{
			OnRemovedChanged?.Invoke(item);
			OnCollectionChanged?.Invoke(_list);
		}
		return isRemoved;
	}

	public void RemoveAt(int index)
	{
		T item = _list[index];
		_list.RemoveAt(index);
		OnRemovedChanged?.Invoke(item);
		OnCollectionChanged?.Invoke(_list);
	}

	public void Clear()
	{
		if (_list.Count == 0)
			return;
		var prevList = new List<T>(_list);
		_list.Clear();
		foreach (var item in prevList)
			OnRemovedChanged?.Invoke(item);
		OnCollectionChanged?.Invoke(_list);
	}

	public int Count => _list.Count;
	public bool IsReadOnly => false;
	public bool Contains(T item) => _list.Contains(item);
	public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
	public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
	public int IndexOf(T item) => _list.IndexOf(item);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public static implicit operator List<T>(ObservableList<T> observable)
	{
		if (observable == null)
			throw new ArgumentNullException(nameof(observable));
		return observable._list;
	}
}