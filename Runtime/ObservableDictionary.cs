using System;
using System.Collections;
using System.Collections.Generic;

public enum ObservableDictionaryChangeType
{
	Add,
	Update,
	Remove,
}

public readonly struct ObservableDictionaryChange<TKey, TValue>
{
	public readonly TKey Key;
	public readonly TValue PreviousValue;
	public readonly TValue CurrentValue;
	public readonly ObservableDictionaryChangeType Type;

	public ObservableDictionaryChange(TKey key, TValue previousValue, TValue currentValue, ObservableDictionaryChangeType type)
	{
		Key = key;
		PreviousValue = previousValue;
		CurrentValue = currentValue;
		Type = type;
	}
}

public class ObservableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
	private readonly Dictionary<TKey, TValue> _dict = new();

	public event Action<ObservableDictionaryChange<TKey, TValue>> OnChanged;

	public TValue this[TKey key] => _dict[key];
	public int Count => _dict.Count;
	public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
	public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
	public Dictionary<TKey, TValue>.KeyCollection Keys => _dict.Keys;
	public Dictionary<TKey, TValue>.ValueCollection Values => _dict.Values;
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

	public bool Add(TKey key, TValue value, bool isNotify = true)
	{
		if (_dict.ContainsKey(key))
			return false;

		_dict.Add(key, value);

		if (!isNotify)
			return true;

		OnChanged?.Invoke(new ObservableDictionaryChange<TKey, TValue>(key, default, value, ObservableDictionaryChangeType.Add));
		return true;
	}

	public bool Set(TKey key, TValue value, bool isNotify = true)
	{
		if (_dict.TryGetValue(key, out TValue prevValue))
		{
			if (EqualityComparer<TValue>.Default.Equals(prevValue, value))
				return false;

			_dict[key] = value;

			if (isNotify == false)
				return true;

			OnChanged?.Invoke(new ObservableDictionaryChange<TKey, TValue>(key, prevValue, value, ObservableDictionaryChangeType.Update));
			return true;
		}

		_dict.Add(key, value);

		if (isNotify == false)
			return true;

		OnChanged?.Invoke(new ObservableDictionaryChange<TKey, TValue>(key, default, value, ObservableDictionaryChangeType.Add));
		return true;
	}

	public bool Remove(TKey key, bool isNotify = true)
	{
		if (!_dict.TryGetValue(key, out TValue value))
			return false;

		_dict.Remove(key);

		if (isNotify == false)
			return true;

		OnChanged?.Invoke(new ObservableDictionaryChange<TKey, TValue>(key, value, default, ObservableDictionaryChangeType.Remove));
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
			OnChanged?.Invoke(new ObservableDictionaryChange<TKey, TValue>(pair.Key, pair.Value, default, ObservableDictionaryChangeType.Remove));
		}
	}
}