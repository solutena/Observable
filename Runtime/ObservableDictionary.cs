using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IObservable<KeyValuePair<TKey, TValue>>
{
	private readonly Dictionary<TKey, TValue> _dictionary;

	public event IObservable<KeyValuePair<TKey, TValue>>.OnItemChangedHandler OnItemChanged;
	public event IObservable<KeyValuePair<TKey, TValue>>.OnCollectionChangedHandler OnCollectionChanged;

	public ObservableDictionary() => _dictionary = new Dictionary<TKey, TValue>();
	public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
	{
		if (dictionary == null)
			throw new ArgumentNullException(nameof(dictionary));
		_dictionary = new Dictionary<TKey, TValue>(dictionary);
	}

	public TValue this[TKey key]
	{
		get => _dictionary[key];
		set
		{
			var pair = new KeyValuePair<TKey, TValue>(key, value);
			if (_dictionary.ContainsKey(key))
			{
				_dictionary[key] = value;
				OnItemChanged?.Invoke(pair, ObservableListChangedType.Updated);
			}
			else
			{
				_dictionary[key] = value;
				OnItemChanged?.Invoke(pair, ObservableListChangedType.Added);
			}
			OnCollectionChanged?.Invoke(_dictionary);
		}
	}

	public void Add(TKey key, TValue value)
	{
		_dictionary.Add(key, value);
		var pair = new KeyValuePair<TKey, TValue>(key, value);
		OnItemChanged?.Invoke(pair, ObservableListChangedType.Added);
		OnCollectionChanged?.Invoke(_dictionary);
	}

	public bool Remove(TKey key)
	{
		if (_dictionary.Remove(key, out var value))
		{
			var pair = new KeyValuePair<TKey, TValue>(key, value);
			OnItemChanged?.Invoke(pair, ObservableListChangedType.Removed);
			OnCollectionChanged?.Invoke(_dictionary);
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
			OnItemChanged?.Invoke(item, ObservableListChangedType.Removed);
		OnCollectionChanged?.Invoke(_dictionary);
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
}