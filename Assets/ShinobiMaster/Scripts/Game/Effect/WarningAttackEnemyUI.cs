using System.Collections;
using System.Collections.Generic;
using Game.Enemy;
using UnityEngine;
using UnityEngine.UI;

public class WarningAttackEnemyUI : MonoBehaviour
{
    public static WarningAttackEnemyUI Instance { get; private set; }

    [SerializeField] private Player player;

    [SerializeField] private GameObject prefabIcon;

    List<GameObject> icons = new List<GameObject>();

    public bool Active { get; set; }


    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (!Active)
        {
            return;
        }
    
        Enemy[] enemies = EnemyControll.Singleton.GetArray();
        int numIcons = -1;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].attackTarget.IsAttack)
            {
                Vector3 pos = CameraControll.Singleton.GameCamera.WorldToScreenPoint(enemies[i].transform.position);
                //      Debug.Log(pos + "   " + Screen.width + ", "+Screen.height);
                if (Mathf.Abs(pos.x) > Screen.width || Mathf.Abs(pos.y) > Screen.height)
                {
                    numIcons++;
                    if (numIcons >= icons.Count)
                    {
                        GameObject icon = Instantiate(prefabIcon, prefabIcon.transform.parent);
                        icons.Add(icon);
                        icon.SetActive(true);
                    }
                    if (numIcons < icons.Count)
                        SetDirection(icons[numIcons].transform, enemies[i]);
                }
            }
        }

        if (numIcons < icons.Count - 1)
        {
            int count = icons.Count;
            for (int i = numIcons; i < count; i++)
            {
                if (icons.Count == 0)
                    break;
                Destroy(icons[0]);
                icons.RemoveAt(0);
            }
        }
    }

    private void SetDirection(Transform tr, Enemy enemy)
    {  
        int width = Screen.width / 2;
        int height = Screen.height / 2;

        Vector3 direct = enemy.transform.position - player.transform.position;

        Vector2 dir = new Vector2(Mathf.Abs(direct.x), Mathf.Abs(direct.y)).normalized;

        Vector2 centerUp = new Vector2(0, height);
        Vector2 rightUp = new Vector2(width, height);

        Vector2 rightCenter = new Vector2(width, 0);

        Vector2 startPointLine = Vector2.zero;

        Vector2 endPointLine = dir * rightUp.magnitude;

        float t1 = GetTContactLines(startPointLine, endPointLine, centerUp, rightUp) - 0.1f;
        float t2 = GetTContactLines(startPointLine, endPointLine, rightCenter, rightUp) - 0.1f;
       

        

        Transform parentTr = tr.GetChild(0);

        if (t1 <= 1)
        {
            parentTr.localPosition = new Vector3(0, endPointLine.magnitude * t1  , 0);
        }
         else if (t2 <=1) {
           parentTr.localPosition = new Vector3(0, endPointLine.magnitude * t2, 0);
        }

        tr.rotation = Quaternion.LookRotation(Vector3.forward, direct.normalized);

    }

    private float GetTContactLines(Vector2 p1, Vector2 p2, Vector2 m1, Vector2 m2) {
        float v = p2.x - p1.x;
        float w = p2.y - p1.y;

        float a = m2.y - m1.y;
        float b = m1.x - m2.x;
        float c = -m1.x * m2.y + m1.y * m2.x;

        float t = (-a * p1.x - b * p1.y - c) / (a * v + b * w);

        return t;
    }

    private IEnumerator ActiveWithDelayProcess(bool active, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (!active)
        {
            RemoveIcons();
        }
    
        Active = active;
    }
    
    public void ActiveWithDelay(bool active, float delay)
    {
        StartCoroutine(ActiveWithDelayProcess(active, delay));
    }

    public void RemoveIcons()
    {
        for (var i = 0; i < icons.Count; i++)
        {
            Destroy(icons[i]);
        }
            
        icons.Clear();
    }
}
