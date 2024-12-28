using System;
using System.Collections.Generic;
using System.ComponentModel;

public class Observable<T> : INotifyPropertyChanged
{
	private T _value;

	public event PropertyChangedEventHandler PropertyChanged;

	public T Value
	{
		get => _value;
		set
		{
			if (EqualityComparer<T>.Default.Equals(_value, value) == false)
			{
				_value = value;
				OnPropertyChanged(nameof(Value));
			}
		}
	}

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public static implicit operator T(Observable<T> observable)
	{
		return observable.Value;
	}
}