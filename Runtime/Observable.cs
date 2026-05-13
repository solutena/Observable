using System;
using System.Collections.Generic;

public class Observable<T>
{
	private T _value;
	private readonly bool _ignoreNull;

	public delegate void ChangedHandler(T previous, T current);
	public delegate void CurrentChangedHandler(T current);

	public event ChangedHandler OnChanged;
	public event CurrentChangedHandler OnCurrentChanged;

	public T Value => _value;

	public Observable(bool ignoreNull = true)
	{
		this._ignoreNull = ignoreNull;
		_value = default;
	}

	public Observable(T value, bool ignoreNull = true)
	{
		this._ignoreNull = ignoreNull;
		_value = value;
	}

	public void Set(T value, bool isNotify = true)
	{
		if (EqualityComparer<T>.Default.Equals(_value, value))
			return;

		if (_ignoreNull && value is null)
			return;

		var prev = _value;
		_value = value;

		if (!isNotify)
			return;

		OnChanged?.Invoke(prev, _value);
		OnCurrentChanged?.Invoke(_value);
	}

	public static implicit operator T(Observable<T> observable)
	{
		if (observable == null)
			return default;
		return observable.Value;
	}
}