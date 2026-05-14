using System;
using System.Collections;
using System.Collections.Generic;

public class ObservableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
	private readonly Dictionary<TKey, TValue> _dict;

	public ObservableDictionary() => _dict = new Dictionary<TKey, TValue>();
	public ObservableDictionary(IDictionary<TKey, TValue> dictionary) => _dict = new Dictionary<TKey, TValue>(dictionary);

	public delegate void AddedHandler(TKey key, TValue value);
	public delegate void RemovedHandler(TKey key, TValue value);
	public delegate void ReplacedHandler(TKey key, TValue previous, TValue current);

	public event AddedHandler OnAdded;
	public event RemovedHandler OnRemoved;
	public event ReplacedHandler OnReplaced;
	public event Action OnCollectionChanged;

	public TValue this[TKey key]
	{
		get => _dict[key];
		set => Set(key, value);
	}
	public int Count => _dict.Count;
	public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
	public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
	public IEnumerable<TKey> Keys => _dict.Keys;
	public IEnumerable<TValue> Values => _dict.Values;
	public TValue GetValueOrDefault(TKey key) => _dict.TryGetValue(key, out var value) ? value : default;
	public TValue GetValueOrDefault(TKey key, TValue defaultValue) => _dict.TryGetValue(key, out var value) ? value : defaultValue;
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

	public void Add(TKey key, TValue value)
	{
		if (!TryAdd(key, value))
			throw new ArgumentException($"An element with the key '{key}' already exists.", nameof(key));
	}

	public bool TryAdd(TKey key, TValue value, bool isNotify = true)
	{
		if (!_dict.TryAdd(key, value))
			return false;
		
		if (!isNotify)
			return true;
		OnAdded?.Invoke(key, value);
		OnCollectionChanged?.Invoke();
		return true;
	}

	public bool Set(TKey key, TValue value, bool isNotify = true)
	{
		if (_dict.TryGetValue(key, out TValue prev))
		{
			if (EqualityComparer<TValue>.Default.Equals(prev, value))
				return false;

			_dict[key] = value;

			if (!isNotify)
				return true;
			OnReplaced?.Invoke(key, prev, value);
			OnCollectionChanged?.Invoke();
			return true;
		}

		_dict.Add(key, value);

		if (!isNotify)
			return true;
		OnAdded?.Invoke(key, value);
		OnCollectionChanged?.Invoke();
		return true;
	}

	public bool Remove(TKey key, bool isNotify = true) => Remove(key, out _, isNotify);
	public bool Remove(TKey key, out TValue value, bool isNotify = true)
	{
		if (!_dict.Remove(key, out value))
			return false;

		if (!isNotify)
			return true;
		OnRemoved?.Invoke(key, value);
		OnCollectionChanged?.Invoke();
		return true;
	}

	public void Clear(bool isNotify = true)
	{
		if (_dict.Count == 0)
			return;

		if (!isNotify)
		{
			_dict.Clear();
			return;
		}

		var buffer = new List<KeyValuePair<TKey, TValue>>(_dict);
		_dict.Clear();
		for (int i = 0; i < buffer.Count; i++)
		{
			var pair = buffer[i];
			OnRemoved?.Invoke(pair.Key, pair.Value);
		}
		OnCollectionChanged?.Invoke();
	}
}