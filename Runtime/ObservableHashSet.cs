using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;

public class ObservableHashSet<T> : IEnumerable<T>
{
	private readonly HashSet<T> _hashSet;

	public ObservableHashSet() => _hashSet = new HashSet<T>();
	public ObservableHashSet(IEnumerable<T> collection) => _hashSet = new HashSet<T>(collection);

	public delegate void AddedHandler(T item);
	public delegate void RemovedHandler(T item);
	public delegate void CollectionChangedHandler(IReadOnlyCollection<T> collection);

	public event AddedHandler OnAdded;
	public event RemovedHandler OnRemoved;
	public event CollectionChangedHandler OnCollectionChanged;

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

	public void UnionWith(IEnumerable<T> other, bool isNotify = true)
	{
		if (!isNotify)
		{
			_hashSet.UnionWith(other);
			return;
		}

		bool changed = false;
		foreach (var item in other)
		{
			if (!_hashSet.Add(item))
				continue;

			changed = true;
			OnAdded?.Invoke(item);
		}
		if (changed)
			OnCollectionChanged?.Invoke(_hashSet);
	}

	public void ExceptWith(IEnumerable<T> other, bool isNotify = true)
	{
		if (!isNotify)
		{
			_hashSet.ExceptWith(other);
			return;
		}

		bool changed = false;
		foreach (var item in other)
		{
			if (!_hashSet.Remove(item))
				continue;

			changed = true;
			OnRemoved?.Invoke(item);
		}
		if (changed)
			OnCollectionChanged?.Invoke(_hashSet);
	}

	public void IntersectWith(IEnumerable<T> other, bool isNotify = true)
	{
		if (!isNotify)
		{
			_hashSet.IntersectWith(other);
			return;
		}

		var otherSet = other as HashSet<T> ?? new HashSet<T>(other);
		var removed = new List<T>();
		foreach (var item in _hashSet)
		{
			if (!otherSet.Contains(item))
				removed.Add(item);
		}

		if (removed.Count == 0)
			return;

		foreach (var item in removed)
		{
			_hashSet.Remove(item);
			OnRemoved?.Invoke(item);
		}
		OnCollectionChanged?.Invoke(_hashSet);
	}

	public void SymmetricExceptWith(IEnumerable<T> other, bool isNotify = true)
	{
		if (!isNotify)
		{
			_hashSet.SymmetricExceptWith(other);
			return;
		}

		var otherSet = other as HashSet<T> ?? new HashSet<T>(other);
		bool changed = false;
		foreach (var item in otherSet)
		{
			if (_hashSet.Remove(item))
			{
				changed = true;
				OnRemoved?.Invoke(item);
			}
			else if (_hashSet.Add(item))
			{
				changed = true;
				OnAdded?.Invoke(item);
			}
		}
		if (changed)
			OnCollectionChanged?.Invoke(_hashSet);
	}
}