using UnityEngine;

namespace Game.Chests
{
	public class Key: MonoBehaviour
	{
		[SerializeField] private ParticleSystem particleSystem;
		[SerializeField] private Renderer renderer;
		[SerializeField] private Collider collider;
		
	

		public void Disappearance()
		{
			this.renderer.enabled = false;
			this.particleSystem.Play();
			this.collider.enabled = false;
			
			Destroy(this.gameObject, 2.0f);
		}
	}
}