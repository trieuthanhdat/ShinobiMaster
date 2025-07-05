using Game.Enemy;
using UnityEngine;
using UnityEngine.UI;

public class AttackIcon : MonoBehaviour
{
    [SerializeField] private Image circleRed;

    [SerializeField] private GameObject baseIcon;

    private AttackTarget attackTarget;


    private void Awake()
    {
        SetActiveIcon(false);
    }

    private void LateUpdate()
    {
        if (this.attackTarget != null)
        {
            SetActiveIcon(this.attackTarget.IsAttack && !this.attackTarget.Shooter.IsFiring && this.attackTarget.IsReadyToAttack);
            
            if (this.attackTarget.IsReadyToAttack)
            {
                Transform camera = CameraControll.Singleton.transform;
                Vector3 pos = (transform.position - camera.position).normalized * 13f + camera.position;
                baseIcon.transform.position = pos;
                baseIcon.transform.rotation =
                    Quaternion.LookRotation((transform.position - camera.position).normalized, Vector3.up);
            }

            circleRed.fillAmount = this.attackTarget.CurrentAttackDelay / this.attackTarget.speedAttack;
        }
    }

    public void SetAttackTarget(AttackTarget attackTarget)
    {
        this.attackTarget = attackTarget;
    }

    public void SetActiveIcon(bool active)
    {
        baseIcon.SetActive(active);
    }
}
