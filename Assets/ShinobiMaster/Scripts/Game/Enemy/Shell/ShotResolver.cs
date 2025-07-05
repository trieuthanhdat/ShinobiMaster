using UnityEngine;

public static class ShotResolver
{
    public static void Resolver(RaycastHit hit, float pushForce)
    {
        var take = hit.collider.gameObject.GetComponent<TakeDamag>();
        if (take)
        {
            var rigidbody = hit.transform.GetComponent<Rigidbody>();
            
            if (rigidbody)
            {
                var player = hit.transform.GetComponent<Player>();
            
                if (player)
                {
                    if (!player.IsInvulnerable)
                    {
                        rigidbody.AddForce(-hit.normal * rigidbody.mass * pushForce, ForceMode.Impulse);
                    }
                }
                else
                {
                    rigidbody.AddForce(-hit.normal * rigidbody.mass * pushForce, ForceMode.Impulse);
                }
            }
            
            take.AddDamag();
        }
    }
}
