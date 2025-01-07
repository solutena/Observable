using System.Collections.Generic;

public enum ObservableListChangedType
{
	Added,
	Removed,
	Updated
}

public interface IObservable<T>
{
	public delegate void OnItemChangedHandler(T item, ObservableListChangedType changedType);
	public delegate void OnCollectionChangedHandler(ICollection<T> collection);

	public event OnItemChangedHandler OnItemChanged;
	public event OnCollectionChangedHandler OnCollectionChanged;

	public void TriggerItemChanged(T item, ObservableListChangedType changedType);
	public void TriggerCollectionChanged();
}