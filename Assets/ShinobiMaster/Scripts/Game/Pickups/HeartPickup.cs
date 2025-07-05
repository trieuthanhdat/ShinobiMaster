using UnityEngine;

namespace Game.Pickups
{
	public class HeartPickup: Pickup
	{
		[field:SerializeField]
		public int HeartAmount { get; set; }
		[SerializeField] 
		private new ParticleSystem particleSystem;
	
	
	
		protected override void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				var player = other.GetComponent<Player>();

				if (player.Health == player.MaxHealth)
				{
					return;
				}

				player.Health += HeartAmount;

				player.PickHP++;
				
				PlayEffect();
			
				SetRenderersEnabled(false);
				SetColliderEnabled(false);
			}
		}

		private void PlayEffect()
		{
			this.particleSystem.Play();
		}
	}
}