using System.Collections.Generic;

public interface IObservableCollection<T>
{
	public delegate void OnItemChangedHandler(T item);
	public delegate void OnCollectionChangedHandler(ICollection<T> collection);

	public event OnItemChangedHandler OnAddedChanged;
	public event OnItemChangedHandler OnRemovedChanged;
	public event OnCollectionChangedHandler OnCollectionChanged;

	public void TriggerAddedChanged(T item);
	public void TriggerRemovedChanged(T item);
	public void TriggerCollectionChanged();
}
