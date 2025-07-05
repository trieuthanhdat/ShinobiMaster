using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControll
{
    public readonly string Jump = "Jump";
    public readonly string Fly = "Fly";
    public readonly string EndFly = "Landing";

    private Player player;
    public AnimationMaterial animationMaterial;
    private PlayerPhysics playerPhysics;
    private AttackEnemy attackEnemy;

    enum OldTypeAnimation
    {
        Move,
        Slide,
        Landing,
        StartJump,
        Other,
    }

    private OldTypeAnimation oldAnimation;

    public void SetParams(Player player, PlayerPhysics playerPhysics, AttackEnemy attackEnemy, AnimationMaterial material)
    {
        this.player = player;
        this.playerPhysics = playerPhysics;
        this.attackEnemy = attackEnemy;
        animationMaterial = material;
        ConnectToEvent();
    }

    public void ResetAnimator()
    {
        animationMaterial.AnimatorPlayer.enabled = false;
        animationMaterial.AnimatorPlayer.enabled = true;
        animationMaterial.AnimatorPlayer.SetBool("IsGrounded", true);
    }

    private void ConnectToEvent()
    {
        playerPhysics.PlayerStartFly += PlayerStartJump;
        playerPhysics.PlayerInFly += PlayerInFly;
        playerPhysics.PlayerEndFly += PlayerLanding;
        playerPhysics.PlayerStartMove += PlayerStartMove;
        playerPhysics.PlayerStopMove += PlayerStopMove;
        playerPhysics.PlayerRotate += RotatePlayer;
        playerPhysics.PlayerStartSlide += PlayerStartSlide;
        attackEnemy.PlayerAttack += AttackPlayer;
        playerPhysics.PlayerSaltoInFly += PlayerJumpInFly;
    }

    private void PlayerStartJump()
    {
        animationMaterial.AnimatorPlayer.SetTrigger("JumpT");
        oldAnimation = OldTypeAnimation.StartJump;
        animationMaterial.AnimatorPlayer.SetBool("Landing", false);
    }

    private void PlayerInFly()
    {
        if (oldAnimation != OldTypeAnimation.StartJump)
        {
            animationMaterial.AnimatorPlayer.SetTrigger("Fly");
            oldAnimation = OldTypeAnimation.Other;
            animationMaterial.AnimatorPlayer.SetBool("Landing", false);
        }
    }

    private void PlayerJumpInFly()
    {
        animationMaterial.AnimatorPlayer.SetTrigger("SaltoFly");
        oldAnimation = OldTypeAnimation.Other;
        animationMaterial.AnimatorPlayer.SetBool("Landing", false);
    }

    private void PlayerLanding()
    {
        if (oldAnimation != OldTypeAnimation.Landing)
        {
            ContactPlayer contact = playerPhysics.GetContacts();

            if (contact.Left || contact.Right)
            {
                animationMaterial.AnimatorPlayer.SetTrigger("LandingWall");
            }
            else
            {
                animationMaterial.AnimatorPlayer.SetBool("Landing", true);
            }
            oldAnimation = OldTypeAnimation.Landing;
        }
    }

    private void PlayerStartMove()
    {
        if (oldAnimation != OldTypeAnimation.Move)
        {
            animationMaterial.AnimatorPlayer.SetTrigger("Run");
            oldAnimation = OldTypeAnimation.Move;
            animationMaterial.AnimatorPlayer.SetBool("Landing", false);
        }
    }

    private void PlayerStopMove()
    {
        animationMaterial.AnimatorPlayer.SetTrigger("Stay");
        oldAnimation = OldTypeAnimation.Other;
        animationMaterial.AnimatorPlayer.SetBool("Landing", false);
    }

    private void PlayerStartSlide()
    {
        if (oldAnimation != OldTypeAnimation.Slide)
        {
            animationMaterial.AnimatorPlayer.SetTrigger("Slide");
            oldAnimation = OldTypeAnimation.Slide;
            animationMaterial.AnimatorPlayer.SetBool("Landing", false);
        }
    }


    private void RotatePlayer(float direct)
    {
        float sing = Mathf.Sign(direct);
        if (sing > 0)
        {
            animationMaterial.PlayerSkin.eulerAngles = new Vector3(0, -90, 0);
        }
        else
        {
            animationMaterial.PlayerSkin.eulerAngles = new Vector3(0, 90, 0);
        }
    }

    public void AttackPlayer()
    {
        animationMaterial.AnimatorPlayer.SetTrigger("Attack");
    }
}
