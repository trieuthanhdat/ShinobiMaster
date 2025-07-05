using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweeningAnimationRotateOvertime : TweeningAnimation
{
    [SerializeField] private Vector3 rotationAngles = new Vector3(0, 360, 0);
    [SerializeField] private int loopCount = -1; // -1 for infinite looping
    [SerializeField] private LoopType loopType = LoopType.Restart;

    public TweeningAnimationRotateOvertime(float duration, Ease ease, TweeningAnimationType type, Vector3 rotationAngles, int loopCount, LoopType loopType)
       : base(duration, ease, type)
    {
        this.rotationAngles = rotationAngles;
        this.loopCount = loopCount;
        this.loopType = loopType;
    }
    public override Tween GetTweenAnimation()
    {
        // Create the rotation tween
        m_TweenAnimation = transform.DORotate(rotationAngles, tweenDuration, RotateMode.FastBeyond360)
                                    .SetEase(easeType)
                                    .SetLoops(loopCount, loopType);

        RegisterOnStartAndOnCompleteCallbacks();

        return m_TweenAnimation;
    }

    public override void PlayAnimation()
    {
        GetTweenAnimation()?.Play();
    }
}
