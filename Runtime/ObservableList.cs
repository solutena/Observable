using System;
using System.Collections;
using System.Collections.Generic;

public class ObservableList<T> : IEnumerable<T>
{
	private readonly List<T> _list;

	public ObservableList() => _list = new List<T>();
	public ObservableList(IEnumerable<T> collection) => _list = new List<T>(collection);

	public delegate void AddedHandler(T item);
	public delegate void RemovedHandler(T item);
	public delegate void ReplacedHandler(int index, T previous, T current);

	public event AddedHandler OnAdded;
	public event RemovedHandler OnRemoved;
	public event ReplacedHandler OnReplaced;
	public event Action OnCollectionChanged;

	public T this[int index]
	{
		get => _list[index];
		set => ReplaceAt(index, value);
	}
	public int Count => _list.Count;
	public bool Contains(T item) => _list.Contains(item);
	public int IndexOf(T item) => _list.IndexOf(item);
	public void CopyTo(T[] array, int index) => _list.CopyTo(array, index);
	public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

	public void Add(T item, bool isNotify = true)
	{
		_list.Add(item);

		if (!isNotify)
			return;

		OnAdded?.Invoke(item);
		OnCollectionChanged?.Invoke();
	}

	public void Insert(int index, T item, bool isNotify = true)
	{
		_list.Insert(index, item);

		if (!isNotify)
			return;

		OnAdded?.Invoke(item);
		OnCollectionChanged?.Invoke();
	}

	public void AddRange(IEnumerable<T> items, bool isNotify = true)
	{
		_list.AddRange(items);

		if (!isNotify)
			return;

		foreach (var item in items)
			OnAdded?.Invoke(item);
		OnCollectionChanged?.Invoke();
	}

	public bool ReplaceAt(int index, T item, bool isNotify = true)
	{
		T prev = _list[index];

		if (EqualityComparer<T>.Default.Equals(prev, item))
			return false;

		_list[index] = item;

		if (!isNotify)
			return true;

		OnReplaced?.Invoke(index, prev, item);
		OnCollectionChanged?.Invoke();
		return true;
	}

	public bool Remove(T item, bool isNotify = true)
	{
		if (!_list.Remove(item))
			return false;

		if (!isNotify)
			return true;

		OnRemoved?.Invoke(item);
		OnCollectionChanged?.Invoke();
		return true;
	}

	public void RemoveAt(int index, bool isNotify = true)
	{
		T item = _list[index];
		_list.RemoveAt(index);

		if (!isNotify)
			return;

		OnRemoved?.Invoke(item);
		OnCollectionChanged?.Invoke();
	}

	public void RemoveRange(IEnumerable<T> items, bool isNotify = true)
	{
		var itemList = new List<T>(items);
		if (!isNotify)
		{
			foreach (var item in itemList)
				_list.Remove(item);
			return;
		}

		var removedItems = new List<T>();
		foreach (var item in itemList)
		{
			if (_list.Remove(item))
				removedItems.Add(item);
		}

		if (removedItems.Count == 0)
			return;

		foreach (var item in removedItems)
			OnRemoved?.Invoke(item);
		OnCollectionChanged?.Invoke();
	}

	public void Clear(bool isNotify = true)
	{
		if (_list.Count == 0)
			return;

		if (!isNotify)
		{
			_list.Clear();
			return;
		}

		var prevList = new List<T>(_list);
		_list.Clear();

		foreach (var item in prevList)
			OnRemoved?.Invoke(item);
		OnCollectionChanged?.Invoke();
	}
}