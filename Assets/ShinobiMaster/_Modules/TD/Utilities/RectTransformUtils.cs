using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottonCenter,
    BottomRight,
    BottomStretch,

    VertStretchLeft,
    VertStretchRight,
    VertStretchCenter,

    HorStretchTop,
    HorStretchMiddle,
    HorStretchBottom,

    StretchAll
}

public enum PivotPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,
}

public static class RectTransformUtils
{
    public static AnchorPresets GetCurrentAnchorPreset(Vector2 anchorMin, Vector2 anchorMax)
    {
        if (anchorMin == new Vector2(0, 1) && anchorMax == new Vector2(0, 1))
            return AnchorPresets.TopLeft;
        if (anchorMin == new Vector2(0.5f, 1) && anchorMax == new Vector2(0.5f, 1))
            return AnchorPresets.TopCenter;
        if (anchorMin == new Vector2(1, 1) && anchorMax == new Vector2(1, 1))
            return AnchorPresets.TopRight;

        if (anchorMin == new Vector2(0, 0.5f) && anchorMax == new Vector2(0, 0.5f))
            return AnchorPresets.MiddleLeft;
        if (anchorMin == new Vector2(0.5f, 0.5f) && anchorMax == new Vector2(0.5f, 0.5f))
            return AnchorPresets.MiddleCenter;
        if (anchorMin == new Vector2(1, 0.5f) && anchorMax == new Vector2(1, 0.5f))
            return AnchorPresets.MiddleRight;

        if (anchorMin == new Vector2(0, 0) && anchorMax == new Vector2(0, 0))
            return AnchorPresets.BottomLeft;
        if (anchorMin == new Vector2(0.5f, 0) && anchorMax == new Vector2(0.5f, 0))
            return AnchorPresets.BottonCenter; // Note the typo in BottonCenter
        if (anchorMin == new Vector2(1, 0) && anchorMax == new Vector2(1, 0))
            return AnchorPresets.BottomRight;

        if (anchorMin == new Vector2(0, 1) && anchorMax == new Vector2(1, 1))
            return AnchorPresets.HorStretchTop;
        if (anchorMin == new Vector2(0, 0.5f) && anchorMax == new Vector2(1, 0.5f))
            return AnchorPresets.HorStretchMiddle;
        if (anchorMin == new Vector2(0, 0) && anchorMax == new Vector2(1, 0))
            return AnchorPresets.HorStretchBottom;

        if (anchorMin == new Vector2(0, 0) && anchorMax == new Vector2(0, 1))
            return AnchorPresets.VertStretchLeft;
        if (anchorMin == new Vector2(0.5f, 0) && anchorMax == new Vector2(0.5f, 1))
            return AnchorPresets.VertStretchCenter;
        if (anchorMin == new Vector2(1, 0) && anchorMax == new Vector2(1, 1))
            return AnchorPresets.VertStretchRight;

        if (anchorMin == new Vector2(0, 0) && anchorMax == new Vector2(1, 1))
            return AnchorPresets.StretchAll;

        // Default case
        return AnchorPresets.MiddleCenter;
    }

    public static void SetAnchor(RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
    {
        source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

        switch (allign)
        {
            case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottonCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.HorStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            case (AnchorPresets.HorStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
            case (AnchorPresets.HorStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.VertStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.VertStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.VertStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
        }
    }

    public static void SetPivot(RectTransform source, PivotPresets preset)
    {

        switch (preset)
        {
            case (PivotPresets.TopLeft):
                {
                    source.pivot = new Vector2(0, 1);
                    break;
                }
            case (PivotPresets.TopCenter):
                {
                    source.pivot = new Vector2(0.5f, 1);
                    break;
                }
            case (PivotPresets.TopRight):
                {
                    source.pivot = new Vector2(1, 1);
                    break;
                }

            case (PivotPresets.MiddleLeft):
                {
                    source.pivot = new Vector2(0, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleCenter):
                {
                    source.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleRight):
                {
                    source.pivot = new Vector2(1, 0.5f);
                    break;
                }

            case (PivotPresets.BottomLeft):
                {
                    source.pivot = new Vector2(0, 0);
                    break;
                }
            case (PivotPresets.BottomCenter):
                {
                    source.pivot = new Vector2(0.5f, 0);
                    break;
                }
            case (PivotPresets.BottomRight):
                {
                    source.pivot = new Vector2(1, 0);
                    break;
                }
        }
    }
    private static readonly Vector3[] s_Corners = new Vector3[4];
    

    public static float GetElementWidth(RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform is null.");
            return 0f;
        }

        return rectTransform.rect.width * rectTransform.lossyScale.x;
    }

    public static float GetElementHeight(RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform is null.");
            return 0f;
        }

        return rectTransform.rect.height * rectTransform.lossyScale.y;
    }
    public static bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector3 worldPoint)
    {
        worldPoint = Vector2.zero;
        Ray ray = ScreenPointToRay(cam, screenPoint);
        Plane plane = new Plane(rect.rotation * Vector3.back, rect.position);
        float enter = 0f;
        float num = Vector3.Dot(Vector3.Normalize(rect.position - ray.origin), plane.normal);
        if (num != 0f && !plane.Raycast(ray, out enter))
        {
            return false;
        }

        worldPoint = ray.GetPoint(enter);
        return true;
    }

    public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector2 localPoint)
    {
        localPoint = Vector2.zero;
        if (ScreenPointToWorldPointInRectangle(rect, screenPoint, cam, out var worldPoint))
        {
            localPoint = rect.InverseTransformPoint(worldPoint);
            return true;
        }

        return false;
    }

    public static Ray ScreenPointToRay(Camera cam, Vector2 screenPos)
    {
        if (cam != null)
        {
            return cam.ScreenPointToRay(screenPos);
        }

        Vector3 origin = screenPos;
        origin.z -= 100f;
        return new Ray(origin, Vector3.forward);
    }

    public static Vector2 WorldToScreenPoint(Camera cam, Vector3 worldPoint)
    {
        if (cam == null)
        {
            return new Vector2(worldPoint.x, worldPoint.y);
        }

        return cam.WorldToScreenPoint(worldPoint);
    }

    public static Bounds CalculateRelativeRectTransformBounds(Transform root, Transform child)
    {
        RectTransform[] componentsInChildren = child.GetComponentsInChildren<RectTransform>(includeInactive: false);
        if (componentsInChildren.Length != 0)
        {
            Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
            int i = 0;
            for (int num = componentsInChildren.Length; i < num; i++)
            {
                componentsInChildren[i].GetWorldCorners(s_Corners);
                for (int j = 0; j < 4; j++)
                {
                    Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(s_Corners[j]);
                    vector = Vector3.Min(lhs, vector);
                    vector2 = Vector3.Max(lhs, vector2);
                }
            }

            Bounds result = new Bounds(vector, Vector3.zero);
            result.Encapsulate(vector2);
            return result;
        }

        return new Bounds(Vector3.zero, Vector3.zero);
    }

    public static Bounds CalculateRelativeRectTransformBounds(Transform trans)
    {
        return CalculateRelativeRectTransformBounds(trans, trans);
    }

    //
    // Summary:
    //     Flips the alignment of the RectTransform along the horizontal or vertical axis,
    //     and optionally its children as well.
    //
    // Parameters:
    //   rect:
    //     The RectTransform to flip.
    //
    //   keepPositioning:
    //     Flips around the pivot if true. Flips within the parent rect if false.
    //
    //   recursive:
    //     Flip the children as well?
    //
    //   axis:
    //     The axis to flip along. 0 is horizontal and 1 is vertical.
    public static void FlipLayoutOnAxis(RectTransform rect, int axis, bool keepPositioning, bool recursive)
    {
        if (rect == null)
        {
            return;
        }

        if (recursive)
        {
            for (int i = 0; i < rect.childCount; i++)
            {
                RectTransform rectTransform = rect.GetChild(i) as RectTransform;
                if (rectTransform != null)
                {
                    FlipLayoutOnAxis(rectTransform, axis, keepPositioning: false, recursive: true);
                }
            }
        }

        Vector2 pivot = rect.pivot;
        pivot[axis] = 1f - pivot[axis];
        rect.pivot = pivot;
        if (!keepPositioning)
        {
            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition[axis] = 0f - anchoredPosition[axis];
            rect.anchoredPosition = anchoredPosition;
            Vector2 anchorMin = rect.anchorMin;
            Vector2 anchorMax = rect.anchorMax;
            float num = anchorMin[axis];
            anchorMin[axis] = 1f - anchorMax[axis];
            anchorMax[axis] = 1f - num;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }
    }

    //
    // Summary:
    //     Flips the horizontal and vertical axes of the RectTransform size and alignment,
    //     and optionally its children as well.
    //
    // Parameters:
    //   rect:
    //     The RectTransform to flip.
    //
    //   keepPositioning:
    //     Flips around the pivot if true. Flips within the parent rect if false.
    //
    //   recursive:
    //     Flip the children as well?
    public static void FlipLayoutAxes(RectTransform rect, bool keepPositioning, bool recursive)
    {
        if (rect == null)
        {
            return;
        }

        if (recursive)
        {
            for (int i = 0; i < rect.childCount; i++)
            {
                RectTransform rectTransform = rect.GetChild(i) as RectTransform;
                if (rectTransform != null)
                {
                    FlipLayoutAxes(rectTransform, keepPositioning: false, recursive: true);
                }
            }
        }

        rect.pivot = GetTransposed(rect.pivot);
        rect.sizeDelta = GetTransposed(rect.sizeDelta);
        if (!keepPositioning)
        {
            rect.anchoredPosition = GetTransposed(rect.anchoredPosition);
            rect.anchorMin = GetTransposed(rect.anchorMin);
            rect.anchorMax = GetTransposed(rect.anchorMax);
        }
    }

    private static Vector2 GetTransposed(Vector2 input)
    {
        return new Vector2(input.y, input.x);
    }
    public static bool IsIpadOrTablet()
    {
        var identifier = SystemInfo.deviceModel;
        Debug.Log($"RECT TRANS EXTENTION SETTINGS: identifier {identifier}");
#if UNITY_IOS && !UNITY_EDITOR//IOS 
        if (identifier.StartsWith("iPhone"))
        {
            // iPhone logic
            return false;
        }
        else if (identifier.StartsWith("iPad") || identifier.StartsWith("Apple iPad"))
        {
            // iPad logic
            Debug.Log("RECT TRANS EXTENSION SETTINGS: IN IPAD SCREEN !!");
            return true;
        }
        else
        {
            // Mac logic?
            return false;
        }
#else   //ANDROID OR OTHERS
        var scaleScreenType = DetectScreenSize.Instance.GetScreenType();
        Debug.Log("RECT TRANS EXTENSION SETTINGS: Screen type !!" + scaleScreenType);
        return scaleScreenType == ScaleScreenType.Ipad;
#endif
    }
}