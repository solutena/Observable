using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ObservableCollection;

public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IObservableCollection<KeyValuePair<TKey, TValue>>
{
	private readonly Dictionary<TKey, TValue> _dictionary;

	public event IObservableCollection<KeyValuePair<TKey, TValue>>.OnItemChangedHandler OnItemChanged;
	public event IObservableCollection<KeyValuePair<TKey, TValue>>.OnCollectionChangedHandler OnCollectionChanged;
	
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
				TriggerItemChanged(pair, ChangedType.Updated);
			}
			else
			{
				_dictionary[key] = value;
				TriggerItemChanged(pair, ChangedType.Added);
			}
			TriggerCollectionChanged();
		}
	}

	public void Add(TKey key, TValue value)
	{
		_dictionary.Add(key, value);
		var pair = new KeyValuePair<TKey, TValue>(key, value);
		TriggerItemChanged(pair, ChangedType.Added);
		TriggerCollectionChanged();
	}

	public bool Remove(TKey key)
	{
		if (_dictionary.Remove(key, out var value))
		{
			var pair = new KeyValuePair<TKey, TValue>(key, value);
			TriggerItemChanged(pair, ChangedType.Removed);
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
			TriggerItemChanged(item, ChangedType.Removed);
		TriggerCollectionChanged();
	}

	public void TriggerItemChanged(KeyValuePair<TKey, TValue> item, ChangedType changedType)
	{
		OnItemChanged?.Invoke(item, changedType);
	}

	public void TriggerCollectionChanged()
	{
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