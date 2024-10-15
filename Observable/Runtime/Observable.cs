using System;

public class Observable<T> where T : struct
{
	T _value;

	public Observable(T value)
	{
		_value = value;
	}

	public T Value
	{
		get { return _value; }
		set
		{
			if (_value.Equals(value) == false)
			{
				_value = value;
				OnChanged?.Invoke(this);
			}
		}
	}
	
	public void Refresh()
	{
		OnChanged?.Invoke(this);
	}

	public static implicit operator T(Observable<T> observable)
	{
		return observable.Value;
	}

	public event Action<T> OnChanged;
}