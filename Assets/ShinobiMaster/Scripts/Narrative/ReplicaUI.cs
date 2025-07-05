using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Narrative
{
	[RequireComponent(typeof(RectTransform))]
	public class ReplicaUI: MonoBehaviour
	{
		public Replica Replica { get; private set; }
		[SerializeField] private Text dialogueText;
		public bool IsTextShowed { get; protected set; }
		private Coroutine showTextCoroutine;
		public RectTransform RectTransform { get; private set; }
		[field: SerializeField]
		public float DelayBetweenChars { get; set; }



		private void Awake()
		{
			RectTransform = GetComponent<RectTransform>();
		}

		public void SetReplica(Replica replica)
		{
			Replica = replica;
			
			this.dialogueText.text = replica.Text;
		}

		public void ShowText()
		{
			this.showTextCoroutine = StartCoroutine(ShowTextProcess());
		}

		public void ShowTextInstantly()
		{
			DelayBetweenChars = 0f;
		}

		public void SetActive(bool active)
		{
			if (!active)
			{
				StopShowTextProcess();
			}
		
			this.gameObject.SetActive(active);
		}

		private IEnumerator ShowTextProcess()
		{
			var charIdx = 0;

			var textStr = new StringBuilder(Replica.Text.Length);

			foreach (var ch in Replica.Text)
			{
				textStr.Append(ch);

				this.dialogueText.text = textStr.ToString();
				
				yield return new WaitForSeconds(this.DelayBetweenChars);
			}

			IsTextShowed = true;
		}

		private void StopShowTextProcess()
		{
			if (this.showTextCoroutine != null)
			{
				StopCoroutine(this.showTextCoroutine);
			}
		}
	}
}