using System;
using Game.Chests.Items;
using UnityEngine;

namespace Game.Chests
{
	public class IronChest: Chest
	{
		public override ChestItem Open()
		{
			OnOpened?.Invoke(this, this.item);
			IsOpened = true;
		
			return this.item;
		}
	}
}