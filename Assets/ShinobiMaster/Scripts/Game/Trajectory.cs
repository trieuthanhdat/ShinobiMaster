using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Trajectory
{
    public static Vector3[] CreateTrajectory(int count, float timeDist, float force, Vector3 dir)
    {
        var points = new Vector3[count];
        points[0] = Vector3.zero;
        
        var velocity = dir * force;
        
        for (var i = 1; i < count; i++)
        {
            velocity += Physics.gravity * timeDist;

            points[i] = points[i - 1] + velocity * timeDist;
        }

        return points;
    }
}
