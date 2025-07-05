using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public enum SequentialAnimationType
{
    Sequential,
    Simultanious,
}

public enum TweeningAnimationType
{
    ZOOM_OUT,
    ZOOM_IN,
    FADE_IN,
    FADE_OUT,
    MOVE_TO_TARGET,
    COUNTING_NUMBER,
    FILL_AMOUNT,
    MOVE_TO_TARGET_FADE_OUT,
    BLINKING,
    PUNCH,
    WOBBLE_TEXT,
    ROTATE_OVERTIME,
    SCALE_ON_HOVER,
    DAMAGE_TEXT_BEHAVIOUR,
    DISSOVLE_MATERIAL
}

public class TweeningAnimation : MonoBehaviour, ITweenAnimation
{
    public UnityEvent OnStartAnimationEvent;
    public UnityEvent OnCompleteAnimationEvent;

    public bool playOnEnable = false;
    public TweeningAnimationType tweeningAnimationType;
    public float m_TimeScale = 1f;
    public float tweenDuration = 0.5f;
    public float tweenDelay = 0.01f;
    public Ease easeType = Ease.Linear;

    protected bool _hasSpeedup = false;
    protected Tween m_TweenAnimation;

    public TweeningAnimationType TypeTweenAnimation
    {
        get => tweeningAnimationType;
        set => tweeningAnimationType = value;
    }
    public float TimeScale
    {
        get => m_TimeScale;
        set => m_TimeScale = value;
    }
    public float TweenDuration
    {
        get => tweenDuration;
        set => tweenDuration = value;
    }
    public float TweenDelay
    {
        get => tweenDelay;
        set => tweenDelay = value;
    }
    public Ease EaseType
    {
        get => easeType;
        set => easeType = value;
    }
    public bool HasSpeedUp
    {
        get => _hasSpeedup;
        set => _hasSpeedup = value;
    }
    public Tween TweenAnimation
    {
        get => m_TweenAnimation;
        set => m_TweenAnimation = value;
    }

    public virtual void Awake()
    {
        OnInit();
    }
#if UNITY_EDITOR
    public virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            GetTweenAnimation().Play();
        }
    }
#endif
    public virtual void OnEnable()
    {
        SetupFirstState();
        if(playOnEnable)
        {
            PlayAnimation();
        }
    }
    public virtual void OnDisable()
    {
        SetupSleepState();
    }

    public virtual void AddStartAnimationCallback(System.Action callback, bool removeAll = false)
    {
        if (removeAll) OnStartAnimationEvent.RemoveAllListeners();
        OnStartAnimationEvent.AddListener(() => { callback?.Invoke(); });
    }
    public virtual void AddCompleteAnimationCallback(System.Action callback, bool removeAll = false)
    {
        if (removeAll) OnCompleteAnimationEvent.RemoveAllListeners();
        OnCompleteAnimationEvent.AddListener(() => { callback?.Invoke(); });
    }
    
    public virtual void SetupSleepState()
    {
        _hasSpeedup = false;
    }
    public virtual void SetupFirstState()
    {
        _hasSpeedup = false;
    }

    public virtual void DailyMissionProgressUI_OnSpeedupAnimation()
    {
        if (_hasSpeedup) return;
        if (m_TweenAnimation == null) return;
        m_TimeScale = m_TweenAnimation.timeScale + 1; //Double the speed
        if (m_TweenAnimation.IsPlaying())
        {
            _hasSpeedup = true;
            m_TweenAnimation.timeScale = m_TimeScale;
            Debug.Log("TWEENING ANIMATION: speeding up!!");
        }

    }
    public virtual void OnInit()
    {
    }
    public virtual void RegisterOnStartAndOnCompleteCallbacks()
    {
        m_TimeScale = 1;
        m_TweenAnimation.OnStart(() => OnStartAnimationEvent?.Invoke())
                        .OnComplete(() => OnCompleteAnimationEvent?.Invoke());
    }
    public virtual void PlayAnimation()
    {
        GetTweenAnimation()?.Play();
    }
    public virtual void RestartAnimation()
    {
        if(TweenAnimation != null)
        {
            TweenAnimation.Rewind();
            TweenAnimation.Restart();
        }else
        {
            PlayAnimation();
        }
    }
    public virtual Tween GetTweenAnimation()
    {
        return null;
    }

    public TweeningAnimation(float duration, Ease ease, TweeningAnimationType type)
    {
        this.tweenDuration = duration;
        this.easeType = ease;
        this.tweeningAnimationType = type;
    }
}


