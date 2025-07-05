using System.Collections;
using System.Collections.Generic;
using Game.Enemy;
using UnityEngine;

public class HoverJump
{
    public bool Active { get; private set; }



    private Vector3 force;

    private Vector2 posMouse;

    private const float timeDist = 0.2f;

    private float maxForce = 500f;
    private float minForce = 300f;

    public HoverJump() {
        Input.multiTouchEnabled = false;
    }

    public void SetForce(float minForce, float maxForce)
    {
        this.maxForce = maxForce;
        this.minForce = minForce;
    }

    public bool CheckMouseClickHover(Player player)
    {
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }

    public void StartDetectTrajectory()
    {
        Active = true;
        posMouse = Input.mousePosition;
        LineTrajectory.Singleton.ShowTrajectory();
    }

    public void Update(Player player)
    {
        Vector2 direct = posMouse - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        direct += direct.normalized * minForce;

        Vector3 directFly = CameraControll.Singleton.TransformFromCameraToWorld(direct);
        if (directFly.magnitude < 4) {
            directFly = Vector3.up * minForce;
        }

        if (player.PlayerPhysics.contactPhysics.Collision != null)
        {
            var normal = player.PlayerPhysics.contactPhysics.Collision.contacts[0].normal;

            if (Mathf.Approximately(normal.y, 0f))
            {
                if (Mathf.Approximately(normal.x, -1f))
                {
                    if (directFly.x > 0)
                    {
                        directFly.x = 0;
                    }
                }

                if (Mathf.Approximately(normal.x, 1f))
                {
                    if (directFly.x < 0)
                    {
                        directFly.x = 0;
                    }
                }
            }

            if (Mathf.Approximately(normal.x, 0f))
            {
                if (Mathf.Approximately(normal.y, -1f))
                {
                    if (directFly.y > 0)
                    {
                        directFly.y = 0;
                    }
                }

                if (Mathf.Approximately(normal.y, 1f))
                {
                    if (directFly.y < 0)
                    {
                        directFly.y = 0;
                    }
                }
            }
        }

        force = directFly.normalized;
        force *= Mathf.Clamp(directFly.magnitude, minForce, maxForce);
        force *= 1f / 20f;
        UpdatePoints(player);
    }

    public bool CheckDisable()
    {
        if (!Input.GetMouseButton(0) && Active)
        {
            return true;
        }
        return false;
    }

    public void Disable()
    {
        Active = false;
        LineTrajectory.Singleton.HideTrajectory();
    }

    public Vector3 GetTrajectory()
    {
        return force;
    }

    private Vector3[] pointsP;

    private void UpdatePoints(Player player)
    {
        Vector3 offestStart = new Vector3(0, -0.25f, 0);
        this.pointsP = Trajectory.CreateTrajectory(10, 0.05f, force.magnitude, force.normalized);
        for (int i = 0; i < this.pointsP.Length; i++)
        {
            this.pointsP[i] += player.transform.position + offestStart;
        }
        LineTrajectory.Singleton.UpdateTrajectory(this.pointsP, player);
    }


    public Enemy GetEnemyInTrajectory()
    {
        if (this.pointsP == null)
        {
            return null;
        }
    
        if (Physics.Linecast(this.pointsP[0], this.pointsP[this.pointsP.Length - 1], out var hit,
            (1 << LayerMask.NameToLayer("Enemy") 
             | 1 << LayerMask.NameToLayer("Map") 
             | 1 << LayerMask.NameToLayer("Border"))))
        {
            var enemy = hit.transform.GetComponent<Enemy>();

            if (enemy != null)
            {
                return enemy;
            }
        }

        return null;
    }
}
