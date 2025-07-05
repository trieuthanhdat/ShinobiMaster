using UnityEngine.Events;

namespace Game.EventGame
{
	public static class EventManager
	{
		public static UnityEvent<Stage> OnStageInited;
	
	
		static EventManager()
		{
			OnStageInited = new UnityEvent<Stage>();
		}
	}
}