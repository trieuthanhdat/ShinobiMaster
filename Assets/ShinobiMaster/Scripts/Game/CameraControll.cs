using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public static CameraControll Singleton { get; private set; }

    public Camera GameCamera;

    public BlackoutCamera Blackout;

    [SerializeField] private Vector3 BiasCamera;

    [Range(0f, -100f)]
    [SerializeField] private float distZ;




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
    
    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }




    public void InitGame() 
    {
        Blackout.Init();
    }

    public Vector3 TransformFromCameraToWorld(Vector2 direct)
    {
        Vector3 direct3 = GameCamera.transform.right * -direct.x + Vector3.up * -direct.y;
        direct3 = direct3.normalized * direct3.magnitude;
        return direct3;
    }

    public void SetPosition(Vector3 pos)
    {
        pos += BiasCamera;

        float dst = distZ - pos.z;
        pos.z = distZ;
        float catet = Mathf.Tan((GameCamera.fieldOfView / 2f) * Mathf.Deg2Rad) * Mathf.Abs(dst);

        if (GameHandler.Singleton.Level.CurrentStage == null)
        {
            return;
        }

        Vector2 heightMap = GameHandler.Singleton.Level.CurrentStage.GetHeight();
        pos.y = Mathf.Clamp(pos.y, heightMap.x + catet, heightMap.y - catet);
        GameCamera.transform.position = pos;
    }
}
