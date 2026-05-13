using System;
using System.Collections;
using System.Collections.Generic;

public class ObservableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
	private readonly Dictionary<TKey, TValue> _dict = new();

	public delegate void AddedHandler(TKey key, TValue value);
	public delegate void RemovedHandler(TKey key, TValue value);
	public delegate void ReplacedHandler(TKey key, TValue previous, TValue current);

	public event AddedHandler OnAdded;
	public event RemovedHandler OnRemoved;
	public event ReplacedHandler OnReplaced;

	public TValue this[TKey key]
	{
		get => _dict[key];
		set => Set(key, value);
	}
	public int Count => _dict.Count;
	public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
	public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
	public Dictionary<TKey, TValue>.KeyCollection Keys => _dict.Keys;
	public Dictionary<TKey, TValue>.ValueCollection Values => _dict.Values;
	public TValue GetValueOrDefault(TKey key) => _dict.TryGetValue(key, out var value) ? value : default;
	public TValue GetValueOrDefault(TKey key, TValue defaultValue) => _dict.TryGetValue(key, out var value) ? value : defaultValue;
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

	public bool Add(TKey key, TValue value, bool isNotify = true)
	{
		if (_dict.ContainsKey(key))
			return false;

		_dict.Add(key, value);

		if (isNotify)
			OnAdded?.Invoke(key, value);
		return true;
	}

	public bool TryAdd(TKey key, TValue value, bool isNotify = true)
	{
		if (_dict.ContainsKey(key))
			return false;

		_dict.Add(key, value);

		if (isNotify)
			OnAdded?.Invoke(key, value);
		return true;
	}

	public bool Set(TKey key, TValue value, bool isNotify = true)
	{
		if (_dict.TryGetValue(key, out TValue prev))
		{
			if (EqualityComparer<TValue>.Default.Equals(prev, value))
				return false;

			_dict[key] = value;

			if (isNotify)
				OnReplaced?.Invoke(key, prev, value);
			return true;
		}

		_dict.Add(key, value);

		if (isNotify)
			OnAdded?.Invoke(key, value);
		return true;
	}

	public bool Remove(TKey key, bool isNotify = true) => Remove(key, out _, isNotify);
	public bool Remove(TKey key, out TValue value, bool isNotify = true)
	{
		if (!_dict.TryGetValue(key, out value))
			return false;

		_dict.Remove(key);

		if (isNotify)
			OnRemoved?.Invoke(key, value);
		return true;
	}

	public void Clear(bool isNotify = true)
	{
		if (_dict.Count == 0)
			return;

		if (isNotify == false)
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
	}
}