using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public enum Dir
	{
		Left, Right
	}

	[RequireComponent(typeof(Image))]
	public class PointerHandler<T>: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public Action<Dir> OnSwipe;
		public Action<T> OnTap;

		[SerializeField] private GraphicRaycaster graphicRaycaster;
	
		private Vector2 pointerStartPos;
		private Button currButton;




		public void OnPointerDown(PointerEventData eventData)
		{
			this.pointerStartPos = eventData.position;
			
			var results = new List<RaycastResult>();
			
			this.graphicRaycaster.Raycast(eventData, results);
			
			foreach (var result in results)
			{
				var button = result.gameObject.GetComponent<Button>();

				if (button != null)
				{
					this.currButton = button;
					
					break;
				}
			}
			
			foreach (var result in results)
			{
				var obj = result.gameObject.GetComponentInParent<MultiImageButton>();

				if (obj != null)
				{
					obj.OnPointerDown(eventData);
				}
			}

			if (this.currButton != null)
			{
				this.currButton.OnPointerDown(eventData);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			var dist = (pointerStartPos - eventData.position).magnitude;

			if (dist > Screen.width * 0.04)
			{
				OnSwipe?.Invoke(Mathf.Sign(pointerStartPos.x - eventData.position.x) > 0 ? Dir.Right : Dir.Left);
				
				if (this.currButton != null)
				{
					this.currButton.OnPointerUp(eventData);
				}
			}
			else
			{
				var results = new List<RaycastResult>();

				this.graphicRaycaster.Raycast(eventData, results);

				foreach (var result in results)
				{
					var obj = result.gameObject.GetComponent<T>();

					if (obj != null)
					{
						OnTap?.Invoke(obj);
						
						break;
					}
				}

				foreach (var result in results)
				{
					var obj = result.gameObject.GetComponentInParent<MultiImageButton>();

					if (result.gameObject.name.Equals("[Image] RaycastBox"))
					{
						obj.OnPointerUp(eventData);
						obj.onClick?.Invoke();
						
						break;
					}
				}

				if (this.currButton != null)
				{
					this.currButton.OnPointerClick(eventData);
					this.currButton.OnPointerUp(eventData);
				}
			}

			this.currButton = null;

			this.pointerStartPos = Vector2.zero;
		}
	}
}