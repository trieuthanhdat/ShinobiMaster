using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaPhysics
{
    public event System.Action<Collision> PlayerConnect;

    public event System.Action PlayerDisconnect;


    public event System.Action PlayerStartFly;

    public event System.Action PlayerInFly;

    public event System.Action PlayerEndFly;

    public event System.Action PlayerStartMove;

    public event System.Action PlayerStopMove;

    public event System.Action PlayerStartSlide;

    public event System.Action PlayerSaltoInFly;

    public event VectorVelocity PlayerRotate;

    Rigidbody rigidbody;

    public ContactPhysics contactPhysics;

    GroundDetect groundDetect;

    CapsuleCollider capsuleCollider;

    Vector3 connectNormal;

    public bool isStop;

    private float run;

    private bool move;

    int sideMove;

    private bool isBlock;

    public void SetRigidbody(Rigidbody rigidbody, CapsuleCollider capsuleCollider, ContactPhysics contactPhysics, GroundDetect groundDetect)
    {
        this.rigidbody = rigidbody;
        this.contactPhysics = contactPhysics;
        this.groundDetect = groundDetect;
        this.capsuleCollider = capsuleCollider;

        if (this.contactPhysics)
        {
            this.contactPhysics.Enter -= Connect;
            this.contactPhysics.Exit -= Disconnect;
        }
        contactPhysics.Enter += Connect;
        contactPhysics.Exit += Disconnect;
    }

    public Vector3 GetVelocity()
    {
        return this.rigidbody.velocity;
    }

    public void ResetParams()
    {
        if (PlayerStopMove != null)
        {
            PlayerStopMove();
        }
        move = false;
        rigidbody.velocity = Vector3.zero;
        CorrectRotate(-1);
    }

    public void MoveRunUpdate()
    {
        if (move)
        {
            if (!groundDetect.CheckDetect())
            {
                move = false;
                
                SetFly();
                return;
            }

            if (rigidbody.velocity.magnitude > 18f)
            {
                SetMove();
                ContactPlayer contact = GetContacts();
                if (contact.Right || contact.Left)
                {
                    MirrorContactRotate(contact);
                }
                return;
            }
            else
            {
                SetMove();
                ContactPlayer contact = GetContacts();
                if (contact.Right || contact.Left)
                {
                    MirrorContactRotate(contact);
                }
                return;
            }
        }
    }

    public void StartMove()
    {
        move = true;
    }

    public void StopMove()
    {
        move = false;
    }


    public bool CheckMoveOnNormal(Vector3 normal)
    {
        if (normal.y > 0f && Mathf.Abs(Vector3.Dot(normal, rigidbody.velocity.normalized)) < 0.99f && rigidbody.velocity.magnitude > 3f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckJumpInDirection(Vector3 normalDirect)
    {
        ContactPlayer contact = GetContacts();
        if (contact.Down && Vector3.Dot(normalDirect, Vector3.up) < -1f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void CorrectVelocityOnNormal(Vector3 normal)
    {
        Vector3 vel = rigidbody.velocity;
        vel.y = -3;
        rigidbody.velocity = vel;
    }

    public void SetVelocityFly(Vector3 velocity)
    {
        if (CheckContact())
        {       
            SetStartJump();
        }
        else
        {
            SetSalto();
        }
        OffStop();
        CorrectRotate(-velocity.x);
        rigidbody.velocity = velocity;
    }

    private void MirrorContactRotate(ContactPlayer contact)
    {
        if (contact.Left)
        {
            CorrectRotate(-1);
        }
        else if (contact.Right)
        {
            CorrectRotate(1);
        }
    }

    private void CorrectRotate(float direct)
    {
        if (isBlock)
        {
            return;
        }
    
        if (sideMove != Mathf.Sign(direct))
        {
            sideMove = (int)Mathf.Sign(direct);
            SetRotate(direct);
        }
    }

    public Collider GetCollider()
    {
        return this.capsuleCollider;
    }

    public Vector3 GetNormalContact()
    {
        return connectNormal;
    }

    public bool CheckContact()
    {
        return isStop || groundDetect.CheckDetect();
    }

    public bool CheckGroundContact()
    {
        return groundDetect.CheckDetect();
    }

    public ContactPlayer GetContacts()
    {
        ContactPlayer contact = new ContactPlayer();
        contact.Down = groundDetect.CheckDetect();

        Vector3 capsulePos = capsuleCollider.transform.TransformVector(capsuleCollider.center) + capsuleCollider.transform.position;

        Vector3 startBase = capsulePos + capsuleCollider.transform.up * ((capsuleCollider.height - 1) / 2f);
        Vector3 endBase = capsulePos - capsuleCollider.transform.up * ((capsuleCollider.height - 1) / 2f);


        contact.Up = Physics.CheckCapsule(startBase + Vector3.up * (capsuleCollider.radius / 2f), endBase + Vector3.up * (capsuleCollider.radius / 2f), capsuleCollider.radius * 0.6f, groundDetect.GetMask());

        Vector3 start = (startBase - Vector3.right * (capsuleCollider.radius / 1.05f)) - new Vector3(0, capsuleCollider.height * 0.2f, 0);
        Vector3 end = (endBase - Vector3.right * (capsuleCollider.radius / 1.05f)) + new Vector3(0, capsuleCollider.height * 0.2f, 0);

        contact.Left = Physics.CheckCapsule(start, end, capsuleCollider.radius, groundDetect.GetMask());

        start = (startBase + Vector3.right * (capsuleCollider.radius / 1.05f)) - new Vector3(0, capsuleCollider.height * 0.2f, 0);
        end = (endBase + Vector3.right * (capsuleCollider.radius / 1.05f)) + new Vector3(0, capsuleCollider.height * 0.2f, 0);

        contact.Right = Physics.CheckCapsule(start, end, capsuleCollider.radius, groundDetect.GetMask());


        return contact;
    }

    public float GetRun()
    {
        return run;
    }

    public bool GetMove()
    {
        return move;
    }

    private void Connect(Collision collision)
    {
        if (!isBlock)
        {
            if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Border") && collision.collider.gameObject.layer != LayerMask.NameToLayer("EnemyRagdoll"))
            {
                Vector3 normal = Vector3.zero;
                for (int i = 0; i < collision.contactCount; i++)
                {
                    normal += collision.contacts[i].normal;
                    if (Vector3.Dot(-Vector3.up, collision.contacts[i].normal) > 0.4f)
                        rigidbody.transform.position += collision.contacts[i].separation * -collision.contacts[i].normal;
                }
                normal /= collision.contactCount;
                connectNormal = normal;

                PlayerConnect?.Invoke(collision);
            }
        }
    }


    private void Disconnect(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Border"))
        {
            if (PlayerDisconnect != null)
                PlayerDisconnect();
        }
    }

    public void SetStop()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        isStop = true;
    }

    public void OffStop()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        isStop = false;
    }

    public void PlayerStopFly()
    {
        ContactPlayer contact = GetContacts();
        if (contact.Right)
        {
            CorrectRotate(-1);
        }
        else if (contact.Left)
        {
            CorrectRotate(1);
        }
        
        SetStop();
        SetLanding();
    }

    #region Методы для контроля физики из вне и блокировки

    public void BlockPhysics()
    {
        isBlock = true;
        SetStop();
    }



    public void UnblockPhysics()
    {
        isBlock = false;
        OffStop();
    }
    #endregion

    #region Методы для запуска стостояний 
    public void SetStay()
    {
        if (PlayerStopMove != null)
        {
            PlayerStopMove();
        }
    }

    public void SetSlide()
    {
        if (PlayerStartSlide != null)
            PlayerStartSlide();
    }

    public void SetMove()
    {
        if (PlayerStartMove != null)
            PlayerStartMove();
    }

    public void SetStartJump()
    {
        if (PlayerStartFly != null)
            PlayerStartFly();
    }

    public void SetFly()
    {
        if (PlayerInFly != null)
            PlayerInFly();
    }

    public void SetSalto() {
        if (PlayerSaltoInFly != null)
            PlayerSaltoInFly();
    }

    public void SetLanding()
    {
        if (PlayerEndFly != null)
            PlayerEndFly();
    }

    public void SetRotate(float direct)
    {
        if (PlayerRotate != null)
            PlayerRotate(direct);
    }

    public void SetPosition(Vector3 pos)
    {
        rigidbody.transform.position = pos;
    }


    #endregion
}