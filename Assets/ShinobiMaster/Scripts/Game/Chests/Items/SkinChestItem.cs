using Skins;
using UnityEngine;

namespace Game.Chests.Items
{
	public class SkinChestItem: ChestItem
	{
		[field:SerializeField] 
		public Skin Skin { get; set; }
	}
}