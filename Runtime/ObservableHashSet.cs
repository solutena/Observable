using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservableHashSet<T> : ICollection<T>, IObservableCollection<T>, ISerializationCallbackReceiver
{
	[SerializeField] private List<T> _serialized;
	private HashSet<T> _hashSet;

	public ObservableHashSet() => _hashSet = new HashSet<T>();
	public ObservableHashSet(HashSet<T> hashSet) => Initialize(hashSet);
	public ObservableHashSet(IEnumerable<T> collection) => Initialize(collection);

	public event IObservableCollection<T>.OnItemChangedHandler OnAddedChanged;
	public event IObservableCollection<T>.OnItemChangedHandler OnRemovedChanged;
	public event IObservableCollection<T>.OnCollectionChangedHandler OnCollectionChanged;

	public void Initialize(HashSet<T> hashSet) => _hashSet = hashSet ?? throw new ArgumentNullException(nameof(hashSet));
	public void Initialize(IEnumerable<T> collection) => _hashSet = new HashSet<T>(collection ?? throw new ArgumentNullException(nameof(collection)));
	public void TriggerAddedChanged(T item) => OnAddedChanged?.Invoke(item);
	public void TriggerRemovedChanged(T item) => OnRemovedChanged?.Invoke(item);
	public void TriggerCollectionChanged() => OnCollectionChanged?.Invoke(_hashSet);

	void ICollection<T>.Add(T item) => Add(item);
	public bool Add(T item)
	{
		if (_hashSet.Add(item))
		{
			TriggerAddedChanged(item);
			TriggerCollectionChanged();
			return true;
		}
		return false;
	}

	public bool Remove(T item)
	{
		if (_hashSet.Remove(item))
		{
			TriggerRemovedChanged(item);
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
			TriggerRemovedChanged(item);
		TriggerCollectionChanged();
	}

	public int Count => _hashSet.Count;
	public bool IsReadOnly => false;
	public bool Contains(T item) => _hashSet.Contains(item);
	public void CopyTo(T[] array, int arrayIndex) => _hashSet.CopyTo(array, arrayIndex);
	public IEnumerator<T> GetEnumerator() => _hashSet.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _hashSet.GetEnumerator();
	public void OnAfterDeserialize() => _hashSet = new(_serialized);
	public void OnBeforeSerialize() => _serialized = new(_hashSet);
	public static implicit operator HashSet<T>(ObservableHashSet<T> observable)
	{
		if (observable == null)
			throw new ArgumentNullException(nameof(observable));
		return observable._hashSet;
	}
}