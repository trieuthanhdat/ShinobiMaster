using DG.Tweening;
using UnityEngine;

public enum OrbitMethod
{
    UI,
    World
}

public class TweeningAnimationOrbitAroundTarget : TweeningAnimation
{
    [Header("Orbit Settings")]
    public OrbitMethod orbitMethod = OrbitMethod.World;
    public Transform targetTransform;       // Target to orbit around (for World)
    public RectTransform targetUIElement;  // Target to orbit around (for UI)
    public Vector3 orbitAxis = Vector3.up; // Axis to orbit around
    public float orbitRadius = 100f;       // Distance from the target
    public bool clockwise = true;         // Orbit direction

    private Vector3 initialPosition;

    // Constructor
    public TweeningAnimationOrbitAroundTarget(float duration, Ease ease, TweeningAnimationType type, OrbitMethod orbitMethod, Transform targetTransform = null, RectTransform targetUIElement = null, Vector3? orbitAxis = null, float orbitRadius = 100f, bool clockwise = true)
        : base(duration, ease, type)
    {
        this.orbitMethod = orbitMethod;
        this.targetTransform = targetTransform;
        this.targetUIElement = targetUIElement;
        this.orbitAxis = orbitAxis ?? Vector3.up;
        this.orbitRadius = orbitRadius;
        this.clockwise = clockwise;
    }

    public override void OnInit()
    {
        base.OnInit();
        initialPosition = transform.position;
    }

    public override Tween GetTweenAnimation()
    {
        Tween tempTween;
        switch (orbitMethod)
        {
            case OrbitMethod.UI:
                tempTween = SetupUIOrbitTween();
                break;
            case OrbitMethod.World:
                tempTween = SetupWorldOrbitTween();
                break;
            default:
                return null;

        }
        RegisterOnStartAndOnCompleteCallbacks();
        m_TweenAnimation = tempTween;
        return m_TweenAnimation;
    }

    private Tween SetupUIOrbitTween()
    {
        if (targetUIElement == null)
        {
            Debug.LogWarning("Target UI Element is not assigned.");
            return null;
        }

        Vector3 direction = clockwise ? Vector3.forward : Vector3.back;
        return targetUIElement.DOLocalRotate(direction * 360f, tweenDuration, RotateMode.FastBeyond360)
                              .SetEase(easeType)
                              .SetRelative()
                              .SetLoops(-1, LoopType.Restart)
                              .SetDelay(tweenDelay)
                              .OnStart(() => OnStartAnimationEvent?.Invoke())
                              .OnComplete(() => OnCompleteAnimationEvent?.Invoke());
    }

    private Tween SetupWorldOrbitTween()
    {
        if (targetTransform == null)
        {
            Debug.LogWarning("Target Transform is not assigned.");
            return null;
        }

        Vector3 axis = orbitAxis.normalized;
        if (!clockwise) axis = -axis;

        return DOTween.To(
                () => 0f,
                angle => OrbitAroundTarget(angle, axis),
                360f,
                tweenDuration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Incremental)
            .SetDelay(tweenDelay)
            .OnStart(() => OnStartAnimationEvent?.Invoke())
            .OnComplete(() => OnCompleteAnimationEvent?.Invoke());
    }

    private void OrbitAroundTarget(float angle, Vector3 axis)
    {
        if (targetTransform == null) return;

        transform.position = targetTransform.position + Quaternion.AngleAxis(angle, axis) * (initialPosition - targetTransform.position);
    }
}
