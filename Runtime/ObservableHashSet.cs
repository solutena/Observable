using System;
using System.Collections;
using System.Collections.Generic;

public class ObservableHashSet<T> : IEnumerable<T>
{
	private readonly HashSet<T> _hashSet;

	public ObservableHashSet() => _hashSet = new HashSet<T>();
	public ObservableHashSet(IEnumerable<T> collection) => _hashSet = new HashSet<T>(collection);

	public event Action<T> OnAdded;
	public event Action<T> OnRemoved;
	public event Action<IReadOnlyCollection<T>> OnCollectionChanged;

	public int Count => _hashSet.Count;
	public bool Contains(T item) => _hashSet.Contains(item);
	public IEnumerator<T> GetEnumerator() => _hashSet.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _hashSet.GetEnumerator();

	public bool Add(T item, bool isNotify = true)
	{
		if (!_hashSet.Add(item))
			return false;

		if (!isNotify)
			return true;

		OnAdded?.Invoke(item);
		OnCollectionChanged?.Invoke(_hashSet);
		return true;
	}

	public bool Remove(T item, bool isNotify = true)
	{
		if (!_hashSet.Remove(item))
			return false;

		if (!isNotify)
			return true;

		OnRemoved?.Invoke(item);
		OnCollectionChanged?.Invoke(_hashSet);
		return true;
	}

	public void Clear(bool isNotify = true)
	{
		if (_hashSet.Count == 0)
			return;

		if (!isNotify)
		{
			_hashSet.Clear();
			return;
		}

		var copy = new List<T>(_hashSet);
		_hashSet.Clear();

		foreach (var item in copy)
			OnRemoved?.Invoke(item);
		OnCollectionChanged?.Invoke(_hashSet);
	}

	public void UnionWith(IEnumerable<T> other)
	{
		foreach (var item in other)
		{
			if (_hashSet.Add(item))
				OnAdded?.Invoke(item);
		}
		OnCollectionChanged?.Invoke(_hashSet);
	}

	public void ExceptWith(IEnumerable<T> other)
	{
		foreach (var item in other)
		{
			if (_hashSet.Remove(item))
				OnRemoved?.Invoke(item);
		}
		OnCollectionChanged?.Invoke(_hashSet);
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		var toKeep = new HashSet<T>(other);
		var removed = new List<T>();
		foreach (var item in _hashSet)
		{
			if (!toKeep.Contains(item))
				removed.Add(item);
		}
		foreach (var item in removed)
		{
			_hashSet.Remove(item);
			OnRemoved?.Invoke(item);
		}
		OnCollectionChanged?.Invoke(_hashSet);
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		foreach (var item in other)
		{
			if (!_hashSet.Add(item))
			{
				_hashSet.Remove(item);
				OnRemoved?.Invoke(item);
			}
			else
				OnAdded?.Invoke(item);
		}
		OnCollectionChanged?.Invoke(_hashSet);
	}
}