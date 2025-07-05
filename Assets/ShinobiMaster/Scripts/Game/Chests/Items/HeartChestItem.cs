using Game.Pickups;
using UnityEngine;

namespace Game.Chests.Items
{
	public class HeartChestItem: ChestItem
	{
		[field: SerializeField] 
		public HeartPickup HeartPickup { get; set; }
	}
}