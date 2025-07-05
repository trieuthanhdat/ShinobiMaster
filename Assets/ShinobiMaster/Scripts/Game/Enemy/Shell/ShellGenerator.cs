using UnityEngine;

namespace Game.Enemy.Shell
{
    public static class ShellGenerator
    {
        public static global::Shell CreateShell(global::Shell prefab, Weapon.Weapon weapon)
        {
            var shell = Object.Instantiate(prefab, weapon.GetPosFire(), Quaternion.LookRotation(weapon.GetFireNormalize(), Vector3.up));
            shell.SetParamsRigidbody();
            shell.StartFly(weapon.GetFireNormalize());

            if (shell.name.Contains("Shuriken"))
            {
                shell.transform.rotation = Quaternion.Euler(0, -90, 0);
            }

            return shell;
        }
    }
}
