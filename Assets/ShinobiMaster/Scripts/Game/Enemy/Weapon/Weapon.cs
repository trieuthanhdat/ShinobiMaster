using Game.Enemy.Shell;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Enemy.Weapon
{
    public abstract class Weapon : MonoBehaviour
    {
        public UnityAction<Ray> OnFire { get; set; } 

        [SerializeField] protected Transform firePoint;

        [SerializeField] protected Transform baseWeapon;

        [SerializeField] protected global::Shell prefabShell;
        



        public virtual void Fire(Transform target)
        {
            AudioManager.Instance.PlayEnemyAttackSound();
        
            var shell = ShellGenerator.CreateShell(prefabShell, this);

            var dir = (target.position - firePoint.position).normalized;
                
            shell.StartFly(dir);
            CallFire();
            
            OnFire?.Invoke(new Ray(firePoint.position, dir));
        }

        protected virtual void CallFire()
        {

        }

        public global::Shell GetShell()
        {
            return prefabShell;
        }

        public Vector3 GetFireNormalize()
        {
            return firePoint.forward;
        }

        public Vector3 GetPosFire()
        {
            return new Vector3(firePoint.transform.position.x, firePoint.transform.position.y);
        }

        public void SetDirection(Vector3 normalize)
        {
            baseWeapon.rotation = Quaternion.LookRotation(normalize, Vector3.up);
        }

        public void SetRotation(Quaternion rotation)
        {
            baseWeapon.rotation = rotation;
        }
    }
}
