using System;
using Game.Chests.Items;
using UnityEngine;

namespace Game.Chests
{
	public abstract class Chest: MonoBehaviour
	{
		public Action<Chest, ChestItem> OnOpened { get; set; }
		
		public ChestItem Item => this.item;
		[field: SerializeField] protected ChestItem item;
		
		public bool IsOpened { get; set; }
		
		
		
		public virtual ChestItem Open()
		{
			if (IsOpened)
			{
				return null;
			}
			
			IsOpened = true;
		
			if (this.item == null)
			{
				OnOpened?.Invoke(this, null);
			
				return null;
			}

			if (this.item.gameObject.scene.name == null)
			{
				var item = Instantiate(this.item, this.transform.position + new Vector3(0, 0.4f, 0), this.item.transform.rotation);
			
				OnOpened?.Invoke(this, item);
			
				return item;
			}
			
			this.item.transform.SetParent(null);
			
			OnOpened?.Invoke(this, this.item);
		
			return this.item;
		}
		
		public virtual void Put(ChestItem chestItem)
		{
			if (chestItem == null)
			{
				return;
			}
			
			this.item = chestItem;
			IsOpened = false;
		}

		public bool HasItem()
		{
			return this.item != null;
		}
	}
}