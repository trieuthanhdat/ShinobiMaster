using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Pickups
{
	public abstract class Pickup: MonoBehaviour
	{
		[SerializeField]
		private List<Renderer> renderers;
		private new Collider collider;




		protected virtual void Awake()
		{
			this.collider = GetComponent<Collider>();
		}



		protected abstract void OnTriggerEnter(Collider other);


		public void SetRenderersEnabled(bool enabled)
		{
			foreach (var renderer in this.renderers)
			{
				renderer.enabled = enabled;
			}
		}

		public void SetColliderEnabled(bool enabled)
		{
			this.collider.enabled = enabled;
		}
	}
}