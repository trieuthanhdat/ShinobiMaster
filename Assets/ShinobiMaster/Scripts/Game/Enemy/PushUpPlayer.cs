using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
	[RequireComponent(typeof(Enemy))]
	public class PushUpPlayer: MonoBehaviour
	{
		private Enemy enemy;

		public float Time;
		public float Radius;
		public float Force;
		private Coroutine pushupCoroutine;
		
		



		private void Awake()
		{
			this.enemy = GetComponent<Enemy>();
			
			
			this.enemy.OnDie += OnDie;
			this.enemy.OnTakeDamage += OnTakeDamage;
		}

		


		private void OnTakeDamage(int damage)
		{
			this.pushupCoroutine = StartCoroutine(PushUpPlayerProcess(Time, Radius, Force));
			Development.Vibration.Vibrate(200);
		}
		
		private void OnDie()
		{
			if (this.pushupCoroutine != null)
			{
				StopCoroutine(this.pushupCoroutine);
			}
		}
		
		
		private IEnumerator PushUpPlayerProcess(float time, float radius, float force)
		{
			yield return new WaitForSeconds(time);

			var player = GameHandler.Singleton.Player;

			var dist = (player.transform.position - this.transform.position).magnitude;

			if (dist < radius)
			{
				var rigid = player.GetComponent<Rigidbody>();
			
				rigid.velocity = Vector3.zero;
				rigid.angularVelocity = Vector3.zero;

				rigid.constraints = RigidbodyConstraints.None;
			
				rigid.AddForce((player.transform.position - this.transform.position).normalized * force,
					ForceMode.Impulse);
			}
		}
	}
}