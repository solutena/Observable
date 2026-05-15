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

	public delegate void ChangedHandler(T current);
	public delegate void ChangedWithPreviousHandler(T previous, T current);

	public event ChangedHandler OnChanged;
	public event ChangedWithPreviousHandler OnChangedWithPrevious;

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

			OnChangedWithPrevious?.Invoke(prev, _value);
			OnChanged?.Invoke(_value);
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