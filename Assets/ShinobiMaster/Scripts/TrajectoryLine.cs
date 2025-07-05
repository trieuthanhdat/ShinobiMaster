using UnityEngine;

[ExecuteInEditMode]
public class TrajectoryLine : MonoBehaviour
{
	[SerializeField] private Transform[] points;




	private void OnDrawGizmos()
	{
		if (this.points != null)
		{
			for (var i = 0; i < this.points.Length - 1; i++)
			{
				Gizmos.DrawLine(this.points[i].transform.position, this.points[i + 1].transform.position);
			}
		}
	}
}
