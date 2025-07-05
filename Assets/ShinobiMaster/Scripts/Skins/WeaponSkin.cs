using UnityEngine;

namespace Skins
{
	[CreateAssetMenu(fileName = "New weapon skin", menuName = "WeaponSkin", order = 53)]
	public class WeaponSkin: Skin
	{
		[field: SerializeField] public GameObject VFXPrefab { get; set; }
	}
}