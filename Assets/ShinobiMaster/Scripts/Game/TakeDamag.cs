using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void Damag(int damag);
public class TakeDamag : MonoBehaviour
{
    public event Damag Take;

    public void AddDamag()
    {
        if (Take != null)
            Take(1);
    }
}
