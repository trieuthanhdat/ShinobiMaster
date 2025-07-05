using System;
using UnityEngine;

namespace Game
{
	public class Glass: MonoBehaviour, IBreakable, IHaveCenter
	{
		public Action<IBreakable> OnBreak { get; set; }
		public bool IsBroken { get; set; }
	
		[SerializeField] private new ParticleSystem particleSystem;
		[SerializeField] private Collider[] colliders;
		[SerializeField] private Renderer[] renderers;


		

		public void Break()
		{
			if(IsBroken) return;
			
			OnBreak?.Invoke(this);
			
			foreach (var renderer in this.renderers)
			{
				renderer.enabled = false;
			}
			
			foreach (var collider in this.colliders)
			{
				collider.enabled = false;
			}

			IsBroken = true;
		
			this.particleSystem.Play();
			Destroy(this.gameObject, 2.0f);
		}
		
		public Vector3 GetCenter()
		{
			var center = Vector3.zero;
		
			foreach (var collider in this.colliders)
			{
				center += collider.bounds.center;
			}

			if (this.colliders.Length > 0)
			{
				center /= this.colliders.Length;
			}

			return center;
		}
	}
}