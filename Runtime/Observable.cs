using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Observable<T>
{
	[SerializeField] T _value;

	public delegate void OnChangedHandler(T prev, T current);
	public event OnChangedHandler OnChanged;

	public Observable() => _value = default;
	public Observable(T value) => _value = value;

	public T Value
	{
		get => _value;
		set
		{
			if (EqualityComparer<T>.Default.Equals(_value, value) == false)
			{
				var prev = _value;
				_value = value;
				OnChanged(prev, _value);
			}
		}
	}

	public static implicit operator T(Observable<T> observable)
	{
		return observable.Value;
	}
}