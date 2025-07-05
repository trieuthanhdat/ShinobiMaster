using UnityEngine;

namespace Game.Chests.Items
{
	public class KeyChestItem: ChestItem
	{
		[field:SerializeField] 
		public Key Key { get; set; }
	}
}