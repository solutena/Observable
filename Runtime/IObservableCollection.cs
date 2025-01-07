using System.Collections.Generic;

namespace ObservableCollection
{
	public enum ChangedType
	{
		Added,
		Removed,
		Updated
	}

	public interface IObservableCollection<T>
	{
		public delegate void OnItemChangedHandler(T item, ChangedType changedType);
		public delegate void OnCollectionChangedHandler(ICollection<T> collection);

		public event OnItemChangedHandler OnItemChanged;
		public event OnCollectionChangedHandler OnCollectionChanged;

		public void TriggerItemChanged(T item, ChangedType changedType);
		public void TriggerCollectionChanged();
	}
}
