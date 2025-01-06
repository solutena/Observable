using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
	private readonly Dictionary<TKey, TValue> _dictionary;

	public delegate void OnItemChangedHandler(TKey key, TValue value, ObservableListChangedType changedType);
	public delegate void OnCollectionChangedHandler(IDictionary<TKey, TValue> dictionary);

	public event OnItemChangedHandler OnItemChanged;
	public event OnCollectionChangedHandler OnCollectionChanged;

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
			if (_dictionary.ContainsKey(key))
			{
				_dictionary[key] = value;
				OnItemChanged?.Invoke(key, value, ObservableListChangedType.Updated);
			}
			else
			{
				_dictionary[key] = value;
				OnItemChanged?.Invoke(key, value, ObservableListChangedType.Added);
			}
			OnCollectionChanged?.Invoke(_dictionary);
		}
	}

	public void Add(TKey key, TValue value)
	{
		_dictionary.Add(key, value);
		OnItemChanged?.Invoke(key, value, ObservableListChangedType.Added);
		OnCollectionChanged?.Invoke(_dictionary);
	}

	public bool Remove(TKey key)
	{
		if (_dictionary.Remove(key, out var value))
		{
			OnItemChanged?.Invoke(key, value, ObservableListChangedType.Removed);
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
			OnItemChanged?.Invoke(item.Key, item.Value, ObservableListChangedType.Removed);
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