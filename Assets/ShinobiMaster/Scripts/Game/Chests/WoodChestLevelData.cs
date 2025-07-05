using System;
using System.Collections.Generic;

namespace Game.Chests
{
	[Serializable]
	public class WoodChestLevelData
	{
		public int LevelNum;
		public int StageNum;
		public List<WoodChestData> ChestsData = new List<WoodChestData>();
	}
}