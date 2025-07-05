using UnityEngine;

namespace Game
{
	public class Rotation: MonoBehaviour
	{
		public Vector3 RotationVec;
	
	
	
	
		private void Update()
		{
			this.transform.Rotate(RotationVec * Time.deltaTime);
		}
	}
}