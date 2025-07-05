using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Narrative
{
	[Serializable]
	public class Replica
	{
		public string Author;
		public string Text;

		public Replica()
		{
			
		}
		
		public Replica(string author, string text)
		{
			Author = author;
			Text = text;
		}
	}

	[Serializable]
	public class AuthorReplicaUI
	{
		[field: SerializeField]
		public string Author { get; set; }
		[field: SerializeField]
		public ReplicaUI ReplicaUi { get; set; }
		[field: SerializeField]
		public Transform AuthorTransform { get; set; }
		[field: SerializeField]
		public Vector2 Offset { get; set; }
	}

	public class Dialogue: MonoBehaviour
	{
		[field: SerializeField]
		public string Name { get; set; }
		[field: SerializeField]
		public List<Replica> Replicas { get; set; }
		[field: SerializeField]
		public List<AuthorReplicaUI> AuthorsReplicasUI { get; set; }
		[field: SerializeField]
		public float DelayBetweenReplicas { get; set; }
		[field: SerializeField] 
		public Canvas Canvas { get; set; }
		private Camera camera;
		public Transform ReplicasContainer { get; set; }
		private ReplicaUI currentReplicaUi;
		public bool IsShowed { get; private set; }
		private Coroutine dialogueProcess;


		private void Awake()
		{
			this.camera = Camera.main;
		}



		public void ResetDialogueState()
		{
			IsShowed = false;
		}

		public void ShowDialog()
		{
			this.dialogueProcess = StartCoroutine(ShowDialogProcess());
		}

		public void SkipCurrentReplica()
		{
			this.currentReplicaUi.ShowTextInstantly();
		}

		public void SkipDialogue()
		{
			if (this.dialogueProcess != null)
			{
				StopCoroutine(this.dialogueProcess);

				this.dialogueProcess = null;
				
				if (this.currentReplicaUi != null)
				{
					this.currentReplicaUi.SetActive(false);
				}

				IsShowed = true;
			}
		}
		
		private IEnumerator ShowDialogProcess()
		{
			foreach (var replica in Replicas)
			{
				var authorDialogItemUI = AuthorsReplicasUI.Single(i => i.Author.Equals(replica.Author));

				this.currentReplicaUi = Instantiate(authorDialogItemUI.ReplicaUi, ReplicasContainer);

				yield return null;

				this.currentReplicaUi.SetReplica(replica);

				if (authorDialogItemUI.AuthorTransform != null)
				{
					SetScreenSpacePos(this.currentReplicaUi.RectTransform, authorDialogItemUI.AuthorTransform.position, 
						authorDialogItemUI.Offset);
				}
				
				this.currentReplicaUi.ShowText();

				while (!this.currentReplicaUi.IsTextShowed)
				{
					yield return null;
				}
				
				yield return new WaitForSeconds(DelayBetweenReplicas);
				
				this.currentReplicaUi.SetActive(false);
			}

			IsShowed = true;
		}
 
		private void SetScreenSpacePos(RectTransform rectTransform, Vector3 objectTransformPosition, Vector2 offset)
		{
			var uiOffset = new Vector2((float)Canvas.GetComponent<RectTransform>().sizeDelta.x / 2f, 
				(float)Canvas.GetComponent<RectTransform>().sizeDelta.y / 2f);
		
			var ViewportPosition = this.camera.WorldToViewportPoint(objectTransformPosition);
			var proportionalPosition = new Vector2(ViewportPosition.x * Canvas.GetComponent<RectTransform>().sizeDelta.x, 
				ViewportPosition.y * Canvas.GetComponent<RectTransform>().sizeDelta.y);
         
			rectTransform.localPosition = proportionalPosition - uiOffset + offset;
		}
	}
}