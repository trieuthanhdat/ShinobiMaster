using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class TapToPlayPanel: MonoBehaviour, IPointerDownHandler
	{
		public static TapToPlayPanel Instance { get; set; }
		
		public UnityAction OnClick { get; set; }
		
		
		
		
		private void Awake()
		{
			if (Instance)
			{
				Destroy(gameObject);
				return;
			}
			else
			{
				Instance = this;
			}
		}
	
	
	
		public void OnPointerDown(PointerEventData eventData)
		{
			OnClick?.Invoke();
		}
	}
}