using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Contact(Collision collision);

public class ContactPhysics : MonoBehaviour
{
    public event Contact Stay;

    public event Contact Exit;

    public event Contact Enter;

    public Collision Collision;

    private void OnCollisionEnter(Collision collision)
    {
        Collision = collision;
    
        if (Enter != null)
            Enter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        Collision = null;
    
        if (Exit != null)
            Exit(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
		if (Stay != null)
            Stay(collision);
    }
}
