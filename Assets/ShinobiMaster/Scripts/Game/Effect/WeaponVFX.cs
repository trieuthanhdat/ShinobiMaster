using System.Collections;
using Skins;
using UnityEngine;

public class WeaponVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem trailPS;

    [SerializeField] private Player player;
    [SerializeField] private Transform weaponVFXParent;
    private Coroutine setWeaponCoroutine;



    private void Start()
    {
        player.AttackEnemys.PlayerAttack += StartAttack;
    }



    private void StartAttack() {
        ShowAnim();
    }

    public void ShowAnim() {
        if (trailPS == null)
        {
            return;
        }
        
        trailPS.Play();
    }

    public void SetWeapon(WeaponSkin weaponSkin)
    {
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if (this.setWeaponCoroutine != null)
        {
            StopCoroutine(this.setWeaponCoroutine);
        }

        this.setWeaponCoroutine = StartCoroutine(SetWeaponProcess(weaponSkin));
    }

    public IEnumerator SetWeaponProcess(WeaponSkin weaponSkin)
    {    
        if (this.weaponVFXParent.childCount > 0)
        {
            foreach (Transform vfxTr in this.weaponVFXParent.transform)
            {
                Destroy(vfxTr.gameObject);

                yield return null;
            }
        }

        if (weaponSkin.VFXPrefab != null)
        {
            var vfx = Instantiate(weaponSkin.VFXPrefab, this.weaponVFXParent);

            yield return null;

            vfx.transform.localScale = Vector3.one;
            vfx.transform.localPosition = Vector3.zero;
            vfx.transform.localEulerAngles = new Vector3(0, 0, 0);

            this.trailPS = vfx.GetComponent<ParticleSystem>();
        }
    }
}
