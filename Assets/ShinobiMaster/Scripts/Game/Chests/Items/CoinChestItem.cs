using UnityEngine;

namespace Game.Chests.Items
{
	public class CoinChestItem: ChestItem
	{
		[field:SerializeField] 
		public int Coins { get; set; }
		[field:SerializeField] 
		public CoinType CoinType { get; set; }
	}
}