using System;
using System.Collections.Generic;

public class Observable<T>
{
	private T _value;
	private readonly bool _ignoreNullNotify;

	public Observable(bool ignoreNullNotify = true)
	{
		_value = default;
		_ignoreNullNotify = ignoreNullNotify;
	}

	public Observable(T value, bool ignoreNullNotify = true)
	{
		_value = value;
		_ignoreNullNotify = ignoreNullNotify;
	}

	public delegate void ChangedHandler(T previous, T current);
	public delegate void CurrentChangedHandler(T current);

	public event ChangedHandler OnChanged;
	public event CurrentChangedHandler OnCurrentChanged;

	public void NotifyChanged(T previous, T current) => OnChanged?.Invoke(previous, current);
	public void NotifyCurrentChanged(T current) => OnCurrentChanged?.Invoke(current);

	public T Value
	{
		get => _value;
		set
		{
			if (EqualityComparer<T>.Default.Equals(_value, value))
				return;

			if(!_ignoreNullNotify && value is null)
			{
				_value = value;
				return;
			}

			var prev = _value;
			_value = value;

			OnChanged?.Invoke(prev, _value);
			OnCurrentChanged?.Invoke(_value);
		}
	}

	public void SetWithoutNotify(T value)
	{
		if (EqualityComparer<T>.Default.Equals(_value, value))
			return;

		_value = value;
	}

	public static implicit operator T(Observable<T> observable)
	{
		if (observable == null)
			return default;
		return observable.Value;
	}
}