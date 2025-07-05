using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController
{
    public bool PlayerControll { get; private set; }

    PlayerPhysics playerPhysics;
    Player player;

    public void InitElements(Player player, PlayerPhysics playerPhysics)
    {
        this.player = player;
        this.playerPhysics = playerPhysics;
    }

    public void BeginControll()
    {
        PlayerControll = true;
        playerPhysics.BlockPhysics();
    }


    public void PlayerStay()
    {
        if (PlayerControll)
        {
            playerPhysics.SetStay();
        }
    }

    public void SetPosition(Vector3 pos)
    {
        if (PlayerControll)
        {
            pos.z = player.transform.position.z;
            playerPhysics.SetPosition(pos);
        }
    }

    public void PlayerRun()
    {
        if (PlayerControll)
        {
            playerPhysics.SetMove();
        }
    }

    public void EndControll()
    {
        PlayerControll = false;
        playerPhysics.UnblockPhysics();
    }

    public void PlayerDirection(float direct) {
        playerPhysics.SetRotate(direct);
    }


    public bool CheckToGround()
    {
        return playerPhysics.CheckGroundContact();
    }

    public ContactPlayer GetContacts() {
        return playerPhysics.GetContacts();
    }
}
