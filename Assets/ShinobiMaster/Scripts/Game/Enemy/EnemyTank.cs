using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
	public class EnemyTank : Enemy
	{
		public Transform hat;

		protected override void OnTakeDamageHandler(int damage)
		{
			base.OnTakeDamageHandler(damage);

			this.attackTarget.StopAttack();

			if (this.hat != null)
			{
				this.hat.SetParent(null);
				var hatRigid = this.hat.GetComponent<Rigidbody>();
			
				hatRigid.isKinematic = false;
				hatRigid.AddForce(Vector3.up * 40f, ForceMode.Impulse);
				this.hat.gameObject.layer = LayerMask.NameToLayer("EnemyDeadParts");

				Destroy(this.hat.gameObject, 3.0f);

				this.hat = null;
			}

			StartCoroutine(LayProcess(this.layDuration));
		}

		private IEnumerator LayProcess(float time)
		{
			this.lay = true;
		
			Invincible = true;
	
			yield return new WaitForSeconds(time);
		
			if (Health > 0)
			{
				GetUp();
			}
		}

		public override void Kill()
		{
			this.hat.GetComponent<Renderer>().material = this.deadMaterial;
		
			base.Kill();
		}
	}
}
