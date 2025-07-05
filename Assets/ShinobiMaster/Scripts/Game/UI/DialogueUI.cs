using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class DialogueUI: MonoBehaviour
	{
		public static DialogueUI Singleton { get; private set; }

		[SerializeField] 
		private GameObject panel;
		public Transform ReplicasContainer;
		

		private void Awake()
		{
			if (Singleton)
			{
				Destroy(gameObject);
				return;
			}
			else
			{
				Singleton = this;
			}
		}


		public void SetActive(bool active)
		{
			this.panel.SetActive(active);
		}

		public void SkipCurrentReplica()
		{
			if (GameHandler.Singleton.Level.CurrentDialogue != null)
			{
				GameHandler.Singleton.Level.CurrentDialogue.SkipCurrentReplica();
			}
		}
		
		public void SkipDialogue()
		{
			if (GameHandler.Singleton.Level.CurrentDialogue != null)
			{
				GameHandler.Singleton.Level.CurrentDialogue.SkipDialogue();
				
				SetActive(false);
			}
		}
	}
}