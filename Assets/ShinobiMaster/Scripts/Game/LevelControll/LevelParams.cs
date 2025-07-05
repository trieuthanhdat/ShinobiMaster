using UnityEngine;

namespace Game.LevelControll
{
	[CreateAssetMenu(menuName = "Level Params", fileName = "Level Params", order = 52)]
	public class LevelParams: ScriptableObject
	{
		public Sprite LevelMenuBackground;
		public Color LevelMenuColor;
		public Color FogColor;
		public StageColorScheme StageColorScheme;
		public Stage[] Stages;
	}
}