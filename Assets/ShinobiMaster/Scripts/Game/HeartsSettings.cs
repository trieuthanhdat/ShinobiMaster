using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "HeartsSettings", menuName = "HeartsSettings")]
	public class HeartsSettings: ScriptableObject
	{
		public int HeartsAmountOnLevel;
		public List<HeartsOnStageParams> HeartsOnStageList;
	}
	
	[Serializable]
	public class HeartsOnStageParams
	{
		public int Stage;
		public int HeartsAmount;
		public float Chance;
	}
}