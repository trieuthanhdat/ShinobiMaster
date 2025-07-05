using System.Collections;
using Game.Enemy.Weapon;
using UnityEngine;

namespace Game.Enemy
{
	public class Laser: MonoBehaviour
	{
		[SerializeField] private Weapon.Weapon weapon;
		[SerializeField] private float distance;
		[SerializeField] private float notShowAfterFireTime;
		[SerializeField] private LineRenderer lineRenderer;
		private bool show;




		private void Awake()
		{
			this.lineRenderer.positionCount = 2;

			this.show = true;
		
			this.weapon.OnFire += OnFire;
		}

		private void OnDestroy()
		{
			this.weapon.OnFire -= OnFire;
		}

		private void Update()
		{
			if (this.show)
			{
				var posFire = this.weapon.GetPosFire();
				var dirFire = this.weapon.GetFireNormalize();

				Vector3 endPos;

				if (Physics.Raycast(posFire, dirFire, out var hit, this.distance,
					(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Map") |
					 1 << LayerMask.NameToLayer("Border"))))
				{
					endPos = hit.point;
				}
				else
				{
					endPos = posFire + dirFire * this.distance;
				}

				this.lineRenderer.SetPosition(0, posFire);
				this.lineRenderer.SetPosition(1, endPos);
			}
		}




		private void OnFire(Ray ray)
		{
			StartCoroutine(NotShowProcess(this.notShowAfterFireTime));
		}


		private IEnumerator NotShowProcess(float duration)
		{
			this.lineRenderer.positionCount = 0;
			this.show = false;
			
			yield return new WaitForSeconds(duration);

			this.lineRenderer.positionCount = 2;
			this.show = true;
		}
	}
}