using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static InteractableTweeningItem;

public class TweeningAnimationPunch : TweeningAnimation
{
    public enum PunchMethod
    {
        Transform,
        RectTransform
    }
    [SerializeField] protected GameObject ObjectPunch;
    [Header("TWEENING SETTINGS")]
    [SerializeField] protected PunchMethod punchMethod = PunchMethod.Transform;
    [SerializeField] protected InteractableTweeningType interactableTweeningType;
    [SerializeField] protected float eslaticity = 1;
    [SerializeField] protected int vibrato = 10;
    [SerializeField] protected int repeatPunchTime = 1;
    [Header("PUNCH_SCALE SETTINGS")]
    [SerializeField] protected Vector3 scaleToPunch = new Vector2(0.1f, 0.1f);
    [Header("PUNCH_ROTATION SETTINGS")]
    [SerializeField] protected Vector3 rotationToPunch = Vector3.one;
    [Header("PUNCH_POSITION SETTINGS")]
    [SerializeField] protected Vector2 positionToPucnh = Vector3.zero;

    private Transform m_Transform;
    private RectTransform m_RectTransform;

    protected Vector2    _initialScale;
    protected Vector2    _initialPosition;
    protected Quaternion _initialRotation;
    protected Vector2    _initialRectScale;
    protected Vector2    _initialRectPosition;
    protected Quaternion _initialRectRotation;
    private bool m_IsInited = false;
    public bool IsInited => m_IsInited;

    public TweeningAnimationPunch
    (
        float duration,
        Ease ease,
        TweeningAnimationType type,
        InteractableTweeningType interactableTweeningType,
        float eslaticity,
        int vibrato
    ) : base(duration, ease, type)
    {
        this.interactableTweeningType = interactableTweeningType;
        this.eslaticity = eslaticity;
        this.vibrato = vibrato;
        this.tweeningAnimationType = TweeningAnimationType.PUNCH;
    }
    public override void OnInit()
    {
        base.OnInit();
    }
    public override void SetupFirstState()
    {
        if(IsInited)
        {
            ResetToInitial();
        }
        base.SetupFirstState();
        SetCachedPropetires();
        m_IsInited = true;
    }
    public override void OnDisable()
    {
        if (!ObjectPunch) return;
        base.OnDisable();
        ResetToInitial();
    }

    private void ResetToInitial()
    {
        if (!ObjectPunch) return;

        switch (interactableTweeningType)
        {
            case InteractableTweeningType.TOUCH_PUNCH_SCALE:
                ResetScale();
                break;
            case InteractableTweeningType.TOUCH_PUNCH_ROTATION:
                ResetRotation();
                break;
            case InteractableTweeningType.TOUCH_PUNCH_POSITION:
                ResetPosition();
                break;
            case InteractableTweeningType.TOUCH_PUNCH_SCALE_ROTATION_POSITION:
                ResetScale();
                ResetRotation();
                ResetPosition();
                break;
        }
    }

    private void ResetScale()
    {
        switch (punchMethod)
        {
            case PunchMethod.Transform:
                if (!m_Transform) m_Transform = ObjectPunch.GetComponent<Transform>();
                if (m_Transform) m_Transform.localScale = _initialScale;
                break;
            case PunchMethod.RectTransform:
                if (!m_RectTransform) m_RectTransform = ObjectPunch.GetComponent<RectTransform>();
                if (m_RectTransform) m_RectTransform.localScale = _initialScale;
                break;
        }
    }

    private void ResetRotation()
    {
        switch (punchMethod)
        {
            case PunchMethod.Transform:
                if (!m_Transform) m_Transform = ObjectPunch.GetComponent<Transform>();
                if (m_Transform) m_Transform.rotation = _initialRotation;
                break;
            case PunchMethod.RectTransform:
                if (!m_RectTransform) m_RectTransform = ObjectPunch.GetComponent<RectTransform>();
                if (m_RectTransform) m_RectTransform.rotation = _initialRotation;
                break;
        }
    }

    private void ResetPosition()
    {
        switch (punchMethod)
        {
            case PunchMethod.Transform:
                if (!m_Transform) m_Transform = ObjectPunch.GetComponent<Transform>();
                if (m_Transform) m_Transform.position = _initialPosition;
                break;
            case PunchMethod.RectTransform:
                if (!m_RectTransform) m_RectTransform = ObjectPunch.GetComponent<RectTransform>();
                if (m_RectTransform) m_RectTransform.position = _initialPosition;
                break;
        }
    }


    private void SetCachedPropetires()
    {
        if (!ObjectPunch) return;

        switch(punchMethod)
        {
            case PunchMethod.Transform:
                m_Transform      = ObjectPunch.GetComponent<Transform>();
                if (!m_Transform) return;
                _initialPosition = m_Transform.position;
                _initialScale    = m_Transform.localScale;
                _initialRotation = m_Transform.rotation;
                break;
            case PunchMethod.RectTransform:
                m_RectTransform  = ObjectPunch.GetComponent<RectTransform>();
                if (!m_RectTransform) return;
                _initialPosition = m_RectTransform.position;
                _initialScale    = m_RectTransform.localScale;
                _initialRotation = m_RectTransform.rotation;
                break;
        }
    }

    public override Tween GetTweenAnimation()
    {
        switch (interactableTweeningType)
        {
            case InteractableTweeningType.TOUCH_PUNCH_SCALE:
                m_TweenAnimation = GetTweenTOUCH_PUNCH_SCALE();
                break;
            case InteractableTweeningType.TOUCH_PUNCH_ROTATION:
                m_TweenAnimation = GetTweenTOUCH_PUNCH_ROTATION();
                break;
            case InteractableTweeningType.TOUCH_PUNCH_POSITION:
                m_TweenAnimation = GetTweenTOUCH_PUNCH_POSITION();
                break;
            case InteractableTweeningType.TOUCH_PUNCH_SCALE_ROTATION_POSITION:
                m_TweenAnimation = GetTweenTOUCH_PUNCH_SCALE_ROTATION_POSITION();
                break;
        }
        RegisterOnStartAndOnCompleteCallbacks();
        return m_TweenAnimation;
    }

    public virtual Tween GetTweenTOUCH_PUNCH_SCALE_ROTATION_POSITION()
    {
        Sequence sequence = DOTween.Sequence();
        switch (punchMethod)
        {
            case PunchMethod.RectTransform:
                if (m_RectTransform != null)
                {
                    m_RectTransform.DORewind();
                    sequence.Join(m_RectTransform.DOPunchScale(scaleToPunch, tweenDuration, vibrato, eslaticity).SetEase(easeType).SetLoops(repeatPunchTime));
                    sequence.Join(m_RectTransform.DOPunchRotation(rotationToPunch, tweenDuration, vibrato, eslaticity).SetEase(easeType).SetLoops(repeatPunchTime));
                    sequence.Join(m_RectTransform.DOPunchPosition(positionToPucnh, tweenDuration, vibrato, eslaticity).SetEase(easeType).SetLoops(repeatPunchTime));
                    sequence.OnComplete(() =>
                    {
                        DOReset(true, true, true);
                    });
                }
                break;
            case PunchMethod.Transform:
                m_Transform.DORewind();
                sequence.Join(m_Transform.DOPunchScale(scaleToPunch, tweenDuration, vibrato, eslaticity).SetEase(easeType).SetLoops(repeatPunchTime));
                sequence.Join(m_Transform.DOPunchRotation(rotationToPunch, tweenDuration, vibrato, eslaticity).SetEase(easeType).SetLoops(repeatPunchTime));
                sequence.Join(m_Transform.DOPunchPosition(positionToPucnh, tweenDuration, vibrato, eslaticity).SetEase(easeType).SetLoops(repeatPunchTime));
                sequence.OnComplete(() =>
                {
                    DOReset(true, true, true);
                });
                break;
        }
        return sequence;
    }

    public void DOReset(bool resetScale = false, bool resetRotation = false, bool resetPosition = false)
    {
        switch(punchMethod)
        {
            case PunchMethod.RectTransform:
                if (resetScale) m_RectTransform.DOScale(_initialRectScale, tweenDuration).SetEase(easeType);
                if (resetRotation) m_RectTransform.DORotate(_initialRectRotation.eulerAngles, tweenDuration).SetEase(easeType);
                if (resetPosition) m_RectTransform.DOAnchorPos(_initialRectPosition, tweenDuration).SetEase(easeType);
                break;
            case PunchMethod.Transform:
                if (resetScale) m_Transform.DOScale(_initialScale, tweenDuration).SetEase(easeType);
                if (resetRotation) m_Transform.DORotate(_initialRotation.eulerAngles, tweenDuration).SetEase(easeType);
                if (resetPosition) m_RectTransform.DOMove(_initialPosition, tweenDuration).SetEase(easeType);
                break;
        }
    }

    public virtual Tween GetTweenTOUCH_PUNCH_POSITION()
    {
        Sequence sequence = DOTween.Sequence();
        switch (punchMethod)
        {
            case PunchMethod.RectTransform:
                if (m_RectTransform != null)
                {
                    m_RectTransform.DORewind();
                    sequence.Append(m_RectTransform.DOPunchPosition(positionToPucnh, tweenDuration, vibrato, eslaticity)
                                    .SetEase(easeType)
                                    .SetLoops(repeatPunchTime)
                                    .OnComplete(() =>
                                    {
                                        DOReset(false, false, true);
                                    }));
                }
                break;
            case PunchMethod.Transform:
                m_Transform.DORewind();
                sequence.Append(m_Transform.DOPunchPosition(positionToPucnh, tweenDuration, vibrato, eslaticity)
                                    .SetEase(easeType)
                                    .SetLoops(repeatPunchTime)
                                    .OnComplete(() =>
                                    {
                                        DOReset(false, false, true);
                                    }));
                break;
        }
        return sequence;
        
    }

    public virtual Tween GetTweenTOUCH_PUNCH_ROTATION()
    {
        Sequence sequence = DOTween.Sequence();
        switch (punchMethod)
        {
            case PunchMethod.RectTransform:
                if (m_RectTransform != null)
                {
                    m_RectTransform.DORewind();
                    sequence.Append(m_RectTransform.DOPunchRotation(rotationToPunch, tweenDuration, vibrato, eslaticity)
                                    .SetEase(easeType)
                                    .SetLoops(repeatPunchTime)
                                    .OnComplete(() =>
                                    {
                                        DOReset(false, true, false);
                                    }));
                }
                break;
            case PunchMethod.Transform:
                m_Transform.DORewind();
                sequence.Append(m_Transform.DOPunchRotation(rotationToPunch, tweenDuration, vibrato, eslaticity)
                            .SetEase(easeType)
                            .SetLoops(repeatPunchTime)
                            .OnComplete(() =>
                            {
                                DOReset(false, true, false);
                            }));
                break;
        }
        return sequence;
    }

    public virtual Tween GetTweenTOUCH_PUNCH_SCALE()
    {
        Sequence sequence = DOTween.Sequence();
        switch (punchMethod)
        {
            case PunchMethod.RectTransform:
                if (m_RectTransform != null)
                {
                    m_RectTransform.DORewind();
                    sequence.Append(m_RectTransform.DOPunchScale(scaleToPunch, tweenDuration, vibrato, eslaticity)
                                    .SetEase(easeType)
                                    .SetLoops(repeatPunchTime)
                                    .OnComplete(() =>
                                    {
                                        DOReset(true, false, false);
                                    }));
                }
                break;
            case PunchMethod.Transform:
                m_Transform.DORewind();
                sequence.Append(m_Transform.DOPunchScale(scaleToPunch, tweenDuration, vibrato, eslaticity)
                                .SetEase(easeType)
                                .SetLoops(repeatPunchTime)
                                .OnComplete(() =>
                                {
                                    DOReset(true, false, false);
                                }));
                break;
        }
        return sequence;
    }
    
}
