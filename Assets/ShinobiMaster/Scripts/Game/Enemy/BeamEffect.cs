using System.Collections;
using Game.Enemy.Weapon;
using UnityEngine;

namespace Game.Enemy
{
	public class BeamEffect: MonoBehaviour
	{
		[SerializeField] private float beamDuration;
		[SerializeField] private RaycastWeapon raycastWeapon;
		[SerializeField] private LineRenderer lineRenderer;



		private void Awake()
		{
			this.raycastWeapon.OnFire += OnFire;
		}

		private void OnDestroy()
		{
			this.raycastWeapon.OnFire -= OnFire;
		}




		private void OnFire(Ray ray)
		{
			StartCoroutine(EffectProcess(this.beamDuration));
		}


		private IEnumerator EffectProcess(float duration)
		{
			this.lineRenderer.positionCount = 2;

			var posFire = this.raycastWeapon.GetPosFire();
			var dirFire = this.raycastWeapon.GetFireNormalize();
		
			this.lineRenderer.SetPosition(0, posFire);
		
			if (Physics.Raycast(posFire, dirFire, out var hit, this.raycastWeapon.Distance, 
			(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Map") |
			                                                                    1 << LayerMask.NameToLayer("Border"))))
			{
				this.lineRenderer.SetPosition(1, hit.point);
			}
			else
			{
				this.lineRenderer.SetPosition(1, posFire + dirFire * this.raycastWeapon.Distance);
			}
			
			yield return new WaitForSeconds(duration);

			this.lineRenderer.positionCount = 0;
		}
	}
}