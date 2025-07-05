using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Rigidbody[] rigidbodies;

    public Transform Spine2;
    public Transform Spine2IK;
    public Transform Hips;

    [SerializeField] private GameObject[] objects;

    [ContextMenu("Cache Components")]
    private void CacheComponents()
    {
        // Auto-find all child rigidbodies
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);

        // Auto-find Spine2, Spine2IK, and Hips by name if not assigned
        if (Spine2 == null)
            Spine2 = FindDeepChild(transform, "Spine2");
        if (Spine2IK == null)
            Spine2IK = FindDeepChild(transform, "Spine2IK");
        if (Hips == null)
            Hips = FindDeepChild(transform, "Hips");
    }

    [ContextMenu("Get All Children Objects")]
    protected void GetAllChildObjects()
    {
        var allChildren = new List<GameObject>();
        GetChildrenRecursive(transform, allChildren);
        objects = allChildren.ToArray();
    }

    private void GetChildrenRecursive(Transform parent, List<GameObject> result)
    {
        foreach (Transform child in parent)
        {
            if (result.Contains(child.gameObject))
            {
                continue;
            }
            result.Add(child.gameObject);
            GetChildrenRecursive(child, result);
        }
    }
    private static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private void Awake()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        foreach (var rigidbody in rigidbodies)
        {
            var vector = rigidbody.transform.position;

            if (GameHandler.Singleton.Level.CurrentStage == null)
            {
                break;
            }

            var border = GameHandler.Singleton.Level.CurrentStage.GetWidth();

            vector.z = Mathf.Clamp(vector.z, border.x, border.y);
            rigidbody.position = vector;
        }
    }

    public void SetForceRigidbody(Rigidbody rb)
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.velocity = rb.velocity;
            rigidbody.angularVelocity = rb.angularVelocity;
        }
    }

    public void AddForce(Vector3 force)
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
    public void SetMask(LayerMask mask)
    {
        foreach (var obj in objects)
        {
            if (obj == null)
            {
                continue;
            }

            obj.layer = mask;
        }
    }

    public void ZeroVelocity()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
