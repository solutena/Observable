using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IObservableCollection<KeyValuePair<TKey, TValue>>, ISerializationCallbackReceiver
{
	[SerializeField] private List<TKey> _keys;
	[SerializeField] private List<TValue> _values;
	private Dictionary<TKey, TValue> _dictionary;

	public ObservableDictionary() =>
		_dictionary = new Dictionary<TKey, TValue>();
	public ObservableDictionary(Dictionary<TKey, TValue> dictionary) =>
		_dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

	public event IObservableCollection<KeyValuePair<TKey, TValue>>.OnItemChangedHandler OnAddedChanged;
	public event IObservableCollection<KeyValuePair<TKey, TValue>>.OnItemChangedHandler OnRemovedChanged;
	public event IObservableCollection<KeyValuePair<TKey, TValue>>.OnItemChangedHandler OnUpdatedChanged;
	public event IObservableCollection<KeyValuePair<TKey, TValue>>.OnCollectionChangedHandler OnCollectionChanged;

	public void TriggerAddedChanged(KeyValuePair<TKey, TValue> item) => OnAddedChanged?.Invoke(item);
	public void TriggerRemovedChanged(KeyValuePair<TKey, TValue> item) => OnRemovedChanged?.Invoke(item);
	public void TriggerUpdatedChanged(KeyValuePair<TKey, TValue> item) => OnUpdatedChanged?.Invoke(item);
	public void TriggerCollectionChanged() => OnCollectionChanged?.Invoke(_dictionary);

	public TValue this[TKey key]
	{
		get => _dictionary[key];
		set
		{
			var pair = new KeyValuePair<TKey, TValue>(key, value);
			if (_dictionary.ContainsKey(key))
			{
				_dictionary[key] = value;
				TriggerUpdatedChanged(pair);
			}
			else
			{
				_dictionary[key] = value;
				TriggerAddedChanged(pair);
			}
			TriggerCollectionChanged();
		}
	}

	public void Add(TKey key, TValue value)
	{
		_dictionary.Add(key, value);
		var pair = new KeyValuePair<TKey, TValue>(key, value);
		TriggerAddedChanged(pair);
		TriggerCollectionChanged();
	}

	public bool Remove(TKey key)
	{
		if (_dictionary.Remove(key, out var value))
		{
			var pair = new KeyValuePair<TKey, TValue>(key, value);
			TriggerRemovedChanged(pair);
			TriggerCollectionChanged();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		if (_dictionary.Count == 0)
			return;
		var prevDic = new Dictionary<TKey, TValue>(_dictionary);
		_dictionary.Clear();
		foreach (var item in prevDic)
			TriggerRemovedChanged(item);
		TriggerCollectionChanged();
	}

	public int Count => _dictionary.Count;
	public bool IsReadOnly => false;
	public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
	public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
	public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
	public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
	public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);
	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);
	public ICollection<TKey> Keys => _dictionary.Keys;
	public ICollection<TValue> Values => _dictionary.Values;
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
	public void OnAfterDeserialize()
	{
		_dictionary = new Dictionary<TKey, TValue>();
		for (int i = 0; i < _keys.Count; i++)
			_dictionary.Add(_keys[i], _values[i]);
	}
	public void OnBeforeSerialize()
	{
		_keys = new List<TKey>(_dictionary.Keys);
		_values = new List<TValue>(_dictionary.Values);
	}
	public static implicit operator Dictionary<TKey, TValue>(ObservableDictionary<TKey, TValue> observable)
	{
		if (observable == null)
			throw new ArgumentNullException(nameof(observable));
		return observable._dictionary;
	}
}