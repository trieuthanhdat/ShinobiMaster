using UnityEngine;

namespace Game.UI
{
	public class HeartbreakUI: MonoBehaviour
	{
		[SerializeField] 
		private SpriteRenderer heartSprite;
		[SerializeField] 
		private ParticleSystem particleSystem;



		public void PlayHeartbreak()
		{
			SetHeartVisibility(false);
			this.particleSystem.Play();
		}

		public void SetHeartVisibility(bool visible)
		{
			this.heartSprite.enabled = visible;
		}

		public bool IsVisible()
		{
			return this.heartSprite.enabled;
		}

		public void StopAnim()
		{
			this.particleSystem.Stop();
		}
	}
}