using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtension
{
    public static Vector3 WorldToScreenPoint2(this Camera camera, Vector3 targetPosition)
    {
        return camera.WorldToScreenPoint(targetPosition);
    }
    //public static ClipPlanePoints NearClipPlanePoints(this Camera camera, Vector3 pos, float clipPlaneMargin)
    //{
    //    var clipPlanePoints = new ClipPlanePoints();

    //    var transform = camera.transform;
    //    var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
    //    var aspect = camera.aspect;
    //    var distance = camera.nearClipPlane;
    //    var height = distance * Mathf.Tan(halfFOV);
    //    var width = height * aspect;
    //    height *= 1 + clipPlaneMargin;
    //    width *= 1 + clipPlaneMargin;
    //    clipPlanePoints.LowerRight = pos + transform.right * width;
    //    clipPlanePoints.LowerRight -= transform.up * height;
    //    clipPlanePoints.LowerRight += transform.forward * distance;

    //    clipPlanePoints.LowerLeft = pos - transform.right * width;
    //    clipPlanePoints.LowerLeft -= transform.up * height;
    //    clipPlanePoints.LowerLeft += transform.forward * distance;

    //    clipPlanePoints.UpperRight = pos + transform.right * width;
    //    clipPlanePoints.UpperRight += transform.up * height;
    //    clipPlanePoints.UpperRight += transform.forward * distance;

    //    clipPlanePoints.UpperLeft = pos - transform.right * width;
    //    clipPlanePoints.UpperLeft += transform.up * height;
    //    clipPlanePoints.UpperLeft += transform.forward * distance;

    //    return clipPlanePoints;
    //}
    /// <summary>
    /// Normalized the angle. between -180 and 180 degrees
    /// </summary>
    /// <param Name="eulerAngle">Euler angle.</param>
    public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
    {
        var delta = eulerAngle;

        if (delta.x > 180)
        {
            delta.x -= 360;
        }
        else if (delta.x < -180)
        {
            delta.x += 360;
        }

        if (delta.y > 180)
        {
            delta.y -= 360;
        }
        else if (delta.y < -180)
        {
            delta.y += 360;
        }

        if (delta.z > 180)
        {
            delta.z -= 360;
        }
        else if (delta.z < -180)
        {
            delta.z += 360;
        }

        return new Vector3(delta.x, delta.y, delta.z);//round values to angle;
    }
    public static void GetViewMinMax(this Camera cam, out Vector3 min, out Vector3 max)
    {
        Transform trans = cam.transform;
        min = Vector3.zero;
        max = Vector3.zero;

        float camWidth = cam.aspect * cam.orthographicSize;

        min.x = trans.position.x - camWidth;
        max.x = trans.position.x + camWidth;

        min.y = trans.position.y - cam.orthographicSize;
        max.y = trans.position.y + cam.orthographicSize;
    }
}
