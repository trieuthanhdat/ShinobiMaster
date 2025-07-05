using System;
using System.Collections;
using TD.DesignPattern.ObjectPooling;
using UnityEngine;

public class ReturnObjectToPool : MonoBehaviour
{
    public float lifetime = 2f;

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Deactivate), lifetime);
    }

    private void Deactivate()
    {
        PoolManager.Instance.ReturnObject(gameObject);
    }
}
