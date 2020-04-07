using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application

namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary> Interface for sequence root. </summary>
	public interface ISequenceRoot
	{
		void OnUpdate();
	}

	/// <summary> Interface for checkable item owner. </summary>
	public interface ICheckableItemOwner<T>
	{
		void ItemCheckedStateChanged(T value, bool state);
	}

	/// <summary> Checkable item. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.Support.ObservableObject"/>
// ReSharper disable once ClassNeverInstantiated.Global
	public class CheckableItem<T> : ObservableObject
	{
		private bool _isChecked;
		private ICheckableItemOwner<T> _owner;
		private string _name;
		private T _value;

		/// <summary> Gets or sets the name. </summary>
		/// <value> The name. </value>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				RaisePropertyChanged(() => Name);
			}
		}

		/// <summary> Gets or sets the value. </summary>
		/// <value> The value. </value>
		public T Value
		{
			get { return _value; }
			set
			{
				_value = value;
				RaisePropertyChanged(() => Value);
			}
		}

		/// <summary> Constructor. </summary>
		/// <param name="owner"> The owner. </param>
		/// <param name="value"> The value. </param>
		public CheckableItem(ICheckableItemOwner<T> owner, T value)
		{
			_owner = owner;
			Name = value.ToString();
			Value = value;
			IsChecked = false;
		}

		/// <summary> Updates the value described by value. </summary>
		/// <param name="value"> The value. </param>
		public void UpdateValue(T value)
		{
			Value = value;
			Name = value.ToString();
		}

		/// <summary> Gets or sets a value indicating whether this object is checked. </summary>
		/// <value> true if this object is checked, false if not. </value>
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				RaisePropertyChanged(() => IsChecked);
				if (_owner != null)
				{
					_owner.ItemCheckedStateChanged(Value, _isChecked);
				}
			}
		}

		/// <summary> Returns a string that represents the current object. </summary>
		/// <returns> A string that represents the current object. </returns>
		/// <seealso cref="M:System.Object.ToString()"/>
		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary> Checkable item state changed. </summary>
	/// <typeparam name="T"> Generic type parameter. </typeparam>
	/// <seealso cref="T:System.EventArgs"/>
	public class CheckableItemStateChanged<T> : EventArgs
	{
		/// <summary> Gets or sets the value. </summary>
		/// <value> The value. </value>
		public T Value { get; private set; }

		/// <summary> Gets or sets a value indicating whether the state. </summary>
		/// <value> true if state, false if not. </value>
		public bool State { get; private set; }

		/// <summary> Default constructor. </summary>
		public CheckableItemStateChanged(T value, bool state)
		{
			Value = value;
			State = state;
		}
	}

	/// <summary> Collection of checkables. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.ViewModel.ICheckableItemOwner{System.Char}"/>
	public class CheckableCollection<T1, T2> : ObservableObject, ICheckableItemOwner<T2> where T1 : CheckableItem<T2>
	{

		private readonly ObservableCollection<T1> _items;
		private List<T1> _checkedItems;
		private string _selectedAddressText;

		public event EventHandler<CheckableItemStateChanged<T2>> ItemUpdate; 

		public CheckableCollection(IEnumerable<T2> collection)
		{
			_items = new ObservableCollection<T1>(collection.Select(t => (T1) Activator.CreateInstance(typeof (T1), new object[] {this, t})));
			UpdateSelectedAddressText();
		}

		/// <summary> Item checked state changed. </summary>
		/// <param name="value"> The value. </param>
		/// <param name="state"> true to state. </param>
		public void ItemCheckedStateChanged(T2 value, bool state)
		{
			_checkedItems = _items == null ? new List<T1>() : _items.Where(i => i.IsChecked).ToList();
			UpdateSelectedAddressText();
			if (ItemUpdate != null)
			{
				ItemUpdate(this, new CheckableItemStateChanged<T2>(value, state));
			}
		}

		/// <summary> Updates the selected address text. </summary>
		protected void UpdateSelectedAddressText()
		{
			switch (_checkedItems.Count)
			{
				case 0:
					SelectedAddressText = "<none>";
					break;
				case 1:
					SelectedAddressText = _checkedItems.First().Name;
					break;
				default:
					string text = _checkedItems.AsEnumerable()
						.Select(i => i.Name)
						.Aggregate("", (s, i) => s + string.Format("{0},", i));
					if (text.Last() == ',')
					{
						text = text.Remove(text.Length - 1);
					}
					SelectedAddressText = text;
					break;
			}
		}

		/// <summary> Gets the items. </summary>
		/// <value> The items. </value>
		public ObservableCollection<T1> Items
		{
			get { return _items; }
		}

		/// <summary> Gets the checked items. </summary>
		/// <value> The checked items. </value>
		public List<T1> CheckedItems
		{
			get { return _checkedItems; }
		}

		/// <summary> Gets or sets the selected address text. </summary>
		/// <value> The selected address text. </value>
		public string SelectedAddressText
		{
			get { return _selectedAddressText; }
			private set
			{
				_selectedAddressText = value;
				RaisePropertyChanged(() => SelectedAddressText);
			}
		}
	}

}