using UnityEngine;

namespace Skins
{
	[CreateAssetMenu(fileName = "New character skin", menuName = "CharacterSkin", order = 52)]
	public class CharacterSkin: Skin
	{
		[field: SerializeField] public Sprite IconSprite { get; set; }
	}
}