using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension 
{
    /// <summary>A useful Epsilon</summary>
    public const float Epsilon = 0.0001f;

    public static Vector2 ToXZ(this Vector3 v) => new Vector2(v.x, v.z);
    /// <summary>Round Decimal Places on a Vector</summary>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }


    public static Vector3 FlattenY(this Vector3 origin)
    {
        return new Vector3(origin.x, 0, origin.z);
    }

    /// <summary>Checks if a vector is close to Vector3.zero</summary>
    public static bool CloseToZero(this Vector3 v, float threshold = 0.0001f) => v.sqrMagnitude < threshold * threshold;

    /// <summary> Get the closest point on a line segment. </summary>
    /// <param name="p">A point in space</param>
    /// <param name="s0">Start of line segment</param>
    /// <param name="s1">End of line segment</param>
    /// <returns>The interpolation parameter representing the point on the segment, with 0==s0, and 1==s1</returns>
    public static Vector3 ClosestPointOnLine(this Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 aB = b - a;
        Vector3 aP = point - a;
        float sqrLenAB = aB.sqrMagnitude;

        if (sqrLenAB < Epsilon) return a;

        float t = Mathf.Clamp01(Vector3.Dot(aP, aB) / sqrLenAB);
        return a + (aB * t);
    }


    /// <summary>Get the closest point (0-1) on a line segment</summary>
    /// <param name="p">A point in space</param>
    /// <param name="s0">Start of line segment</param>
    /// <param name="s1">End of line segment</param>
    /// <returns>The interpolation parameter representing the point on the segment, with 0==s0, and 1==s1</returns>
    public static float ClosestTimeOnSegment(this Vector3 p, Vector3 s0, Vector3 s1)
    {
        Vector3 s = s1 - s0;
        float len2 = Vector3.SqrMagnitude(s);
        if (len2 < Epsilon)
            return 0; // degenrate segment
        return Mathf.Clamp01(Vector3.Dot(p - s0, s) / len2);
    }


    /// <summary> Calculate the Direction from an Origin to a Target or Destination  </summary>
    public static Vector3 DirectionTo(this Vector3 origin, Vector3 destination) => Vector3.Normalize(destination - origin);

    /// <summary>returns the delta position from a rotation.</summary>
    public static Vector3 DeltaPositionFromRotate(this Transform transform, Vector3 point, Vector3 axis, float deltaAngle)
    {
        var pos = transform.position;
        var direction = pos - point;
        var rotation = Quaternion.AngleAxis(deltaAngle, axis);
        direction = rotation * direction;

        pos = point + direction - pos;
        pos.y = 0;                                                      //the Y is handled by the Fix Position method

        return pos;
    }

    /// <summary>returns the delta position from a rotation.</summary>
    public static Vector3 DeltaPositionFromRotate(this Transform transform, Vector3 platform, Quaternion deltaRotation)
    {
        var pos = transform.position;

        var direction = pos - platform;
        var directionAfterRotation = deltaRotation * direction;

        var NewPoint = platform + directionAfterRotation;


        pos = NewPoint - transform.position;

        return pos;
    }

    /// <summary>  Returns if a point is inside a Sphere Radius </summary>
    /// <param name="point">Point you want to find inside a sphere</param>
    public static bool PointInsideSphere(this Vector3 point, Vector3 sphereCenter, float sphereRadius)
    {
        Vector3 direction = point - sphereCenter;
        float distanceSquared = direction.sqrMagnitude;
        return (distanceSquared <= sphereRadius * sphereRadius);
    }

    public static Vector3 CalculateDirectionToPoint(this Vector3 me, Vector3 target, bool ignoreY = true)
    {
        if(ignoreY)
        {
            target.y = 0f;
            me.y = 0;

        }
        return (target - me).normalized;
    }
    public static void LerpAngle(this Vector3 me, Vector3 target, float time)
    {
        me.x = Mathf.LerpAngle(me.x, target.x, time);
        me.y = Mathf.LerpAngle(me.y, target.y, time);
        me.z = Mathf.LerpAngle(me.z, target.z, time);
    }

    public static void LerpAngle(this Vector2 me, Vector2 target, float time)
    {
        me.x = Mathf.LerpAngle(me.x, target.x, time);
        me.y = Mathf.LerpAngle(me.y, target.y, time);
    }
}
