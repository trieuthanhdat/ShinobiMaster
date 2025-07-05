using System.Collections;
using System.Collections.Generic;
using Game.Enemy;
using UnityEngine;

public class NinjaAnimationControll
{
    public readonly string Jump = "Jump";
    public readonly string Fly = "Fly";
    public readonly string EndFly = "Landing";

    private NinjaPhysics ninjaPhysics;
    private Animator animator;
    private Transform skin;
    private EnemyNinja ninja;
    

    enum OldTypeAnimation
    {
        Move,
        Slide,
        Landing,
        StartJump,
        Other,
    }

    private OldTypeAnimation oldAnimation;

    public void SetParams(NinjaPhysics ninjaPhysics, Animator animator, EnemyNinja ninja)
    {
        this.ninjaPhysics = ninjaPhysics;
        this.animator = animator;
        this.ninja = ninja;
        ConnectToEvent();
    }

    public void ResetAnimator()
    {
        animator.enabled = false;
        animator.enabled = true;
    }

    private void ConnectToEvent()
    {
        ninjaPhysics.PlayerStartFly += NinjaStartJump;
        ninjaPhysics.PlayerInFly += NinjaInFly;
        ninjaPhysics.PlayerEndFly += NinjaLanding;
        ninjaPhysics.PlayerStartMove += NinjaStartMove;
        ninjaPhysics.PlayerStopMove += NinjaStopMove;
        ninjaPhysics.PlayerRotate += RotateNinja;
        ninjaPhysics.PlayerStartSlide += NinjaStartSlide;
        ninjaPhysics.PlayerSaltoInFly += NinjaJumpInFly;
    }

    private void NinjaStartJump()
    {
        animator.SetTrigger("JumpT");
        oldAnimation = OldTypeAnimation.StartJump;
    }

    private void NinjaInFly()
    {
        if (oldAnimation != OldTypeAnimation.StartJump)
        {
            animator.SetTrigger("Fly");
            oldAnimation = OldTypeAnimation.Other;
        }
    }

    private void NinjaJumpInFly()
    {
        animator.SetTrigger("SaltoFly");
        oldAnimation = OldTypeAnimation.Other;
    }

    private void NinjaLanding()
    {
        if (oldAnimation != OldTypeAnimation.Landing)
        {
            ContactPlayer contact = ninjaPhysics.GetContacts();

            if (contact.Left || contact.Right)
            {
                animator.SetTrigger("LandingWall");
            }
            else
            {
                animator.SetTrigger("Landing");
            }
            oldAnimation = OldTypeAnimation.Landing;
        }
    }

    private void NinjaStartMove()
    {
        if (oldAnimation != OldTypeAnimation.Move)
        {
            animator.SetTrigger("Run");
            oldAnimation = OldTypeAnimation.Move;
        }
    }

    private void NinjaStopMove()
    {
        animator.SetTrigger("Stay");
        oldAnimation = OldTypeAnimation.Other;
    }

    private void NinjaStartSlide()
    {
        if (oldAnimation != OldTypeAnimation.Slide)
        {
            animator.SetTrigger("Slide");
            oldAnimation = OldTypeAnimation.Slide;
        }
    }


    private void RotateNinja(float direct)
    {
        float sing = Mathf.Sign(direct);
        if (sing > 0)
        {
            ninja.transform.eulerAngles = new Vector3(0, -90, 0);
        }
        else
        {
            ninja.transform.eulerAngles = new Vector3(0, 90, 0);
        }
    }

    private void AttackPlayer()
    {
        animator.SetTrigger("Attack");
    }
}
