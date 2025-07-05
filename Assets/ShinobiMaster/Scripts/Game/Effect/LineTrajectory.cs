using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineTrajectory : MonoBehaviour
{
    public static LineTrajectory Singleton { get; private set; }

    [SerializeField] private LineRenderer baseLine;

    [SerializeField] private Transform point;

    private void Awake()
    {
        if (Singleton)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }
    }

    public void UpdateTrajectory(Vector3[] worldPoint, Player player)
    {
        var lastPoint = worldPoint.Last();
        var firstPoint = worldPoint.First();

        var playerCol = ((CapsuleCollider) player.PlayerPhysics.GetCollider());

        var hit = new RaycastHit();

        for (var i = 0; i < worldPoint.Length - 1; i++)
        {
            if (
                Physics.CapsuleCast(worldPoint[i] + Vector3.up * 0.9f,
                    worldPoint[i] - Vector3.up * 0.3f,
                    playerCol.radius * 0.9f,
                    (worldPoint[i+1] - worldPoint[i]).normalized, out hit, (worldPoint[i+1] - worldPoint[i]).magnitude,
                    1 << LayerMask.NameToLayer("Map")
                    | 1 << LayerMask.NameToLayer("Border")))
            {
            
                break;
            }
        }


        if (hit.transform != null)
        {
            var points = new[] {firstPoint, hit.point};

            worldPoint = points;
        }
        
        var maxCount = worldPoint.Length;
        
        baseLine.positionCount = maxCount;
        baseLine.SetPositions(worldPoint);
        
        for (var i = 0; i < maxCount; i++)
        {
            Vector3 newPos = baseLine.GetPosition(i);
            Transform camera = CameraControll.Singleton.GameCamera.transform;
            Vector3 direct = newPos - camera.position;
            direct = direct.normalized;

            newPos = direct * 14f + camera.position;
            baseLine.SetPosition(i, newPos);
        }

        if (hit.transform != null)
        {
            baseLine.positionCount = maxCount + 1;
            baseLine.SetPosition(maxCount, baseLine.GetPosition(maxCount - 1));
        }

        point.transform.position = baseLine.GetPosition(maxCount - 1);
    }

    public void HideTrajectory()
    {
        baseLine.gameObject.SetActive(false);
        point.gameObject.SetActive(false);
    }

    public void ShowTrajectory()
    {
        baseLine.gameObject.SetActive(true);
        point.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }
}
