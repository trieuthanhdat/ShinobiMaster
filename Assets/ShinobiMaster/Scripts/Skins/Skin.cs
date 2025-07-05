using UnityEngine;

namespace Skins
{
	public class Skin: ScriptableObject
	{
		[field: SerializeField] public int Id { get; set; }
		[field: SerializeField] public string Name { get; set; }
		[field: SerializeField] public GameObject SkinPrefab { get; set; }
		[field: SerializeField] public Sprite Sprite { get; set; }
		[field: SerializeField] public Sprite NotAvailableSprite { get; set; }
		[field: SerializeField] public bool Available { get; set; }
		[field: SerializeField] public bool Skipped { get; set; }
		[field: SerializeField] public bool ForAds { get; set; }
		[field: SerializeField] public int NeedViewCount { get; set; }
		[field: SerializeField] public int CurrentViewCount { get; set; }
		[field: SerializeField] public bool ForCoins { get; set; }
		[field: SerializeField] public int Price { get; set; }

        [field: SerializeField] public Material SkinMat { get; set; }


        public void UpdateData(SkinData skinData)
		{
			if (!skinData.Name.Equals(Name))
			{	
				return;
			}

			Available = skinData.Available;
			Skipped = skinData.Skipped;
			CurrentViewCount = skinData.AdsViewCount;
		}
	}
}