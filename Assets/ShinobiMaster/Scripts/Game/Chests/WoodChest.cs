using System;
using Game.Chests.Items;
using UnityEngine;

namespace Game.Chests
{
	public class WoodChest: Chest, IBreakable, IHaveCenter
	{
		[SerializeField] private new ParticleSystem particleSystem;
		[SerializeField] private Collider[] colliders;
		[SerializeField] private Renderer[] renderers;


		public Action<IBreakable> OnBreak { get; set; }
		public bool IsBroken { get; set; }


		public override ChestItem Open()
		{
			if (IsOpened)
			{
				return null;
			}
			
			var withKey = this.item != null && this.item is KeyChestItem;
			
			ChestsRepository.Instance.AddOrUpdateChestData(GameHandler.Singleton.Level.GetNumLevel(),
				GameHandler.Singleton.Level.CurrStageNumber,
				new WoodChestData
				{
					Name = this.name,
					IsOpened = true,
					WithKey = withKey
				});
			
			var chestItem = base.Open();
		
			return chestItem;
		}
		
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