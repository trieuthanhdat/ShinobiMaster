using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class BossProgressIcon: MonoBehaviour
	{
		public Image BossImage;
		public Image DeadBossImage;
		public Image CirclePrevBossImage;
		public Image CircleCurrBossImage;
		public Image CircleNextBossImage;
		public Image KilledImage;
		public GameObject StageProgressPanel;
		public Text StageProgressText;
	}
}