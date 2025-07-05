using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransforms : MonoBehaviour
{
    [SerializeField] private Transform root;
    public void SyncFrom(Transform transform)
    {
        Transform baseThis = root;
        Transform baseSync = transform;
        Sync(baseThis, baseSync);

    }

    private void Sync(Transform b, Transform s)
    {
        b.position = s.position;
        b.rotation = s.rotation;
        if (b.childCount != s.childCount || b.childCount == 0)
        {
            return;
        }
        else
        {
            for (int i = 0; i < b.childCount; i++)
            {
                Sync(b.GetChild(i), s.GetChild(i));
            }
        }
    }
}
