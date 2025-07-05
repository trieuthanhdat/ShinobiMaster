using UnityEngine;
using DG.Tweening;
using TD.UIFramework.Configs;
using System;
using System.Collections.Generic;

public enum BaseUIType
{
    Screen = 0,
    Popup = 1,
    Overlay = 2
}
public enum UIElementState
{
    Opening = 0,
    Opened  = 1,
    Closing = 2,
    Closed  = 3,
}

namespace TD.UIFramework.Core
{
    [System.Serializable]
    public abstract class UIElement : MonoBehaviour, IUIElement
    {
        public Transform content;
        public UIIntroAnimationSettings introAnimationSettings;
        public UIOutroAnimationSettings outroAnimationSettings;

        public event Action OnOpening;
        public event Action OnClosing;
        public event Action OnOpened;
        public event Action OnClosed;

        public bool CanClose()
        {
            return State == UIElementState.Opened;
        }
        public bool CanOpen()
        {
            return State == UIElementState.Closed;
        }
        public bool IsVisible()
        {
            return State == UIElementState.Opened;
        }
        #region ____PROPETIES____
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Transform _transform;

        private Vector2 _initialAnchoredPosition;  
        private Vector2 _initialScale;
        private float   _initialFade;
        private Queue<StateTransition> transitionQueue = new Queue<StateTransition>();
        private bool isTransitioning = false;

        private float lastActionTime = 0f;
        private float actionThreshold = 0.5f; // seconds
        [SerializeField]
        private UIElementState _state = UIElementState.Closed;
        public UIElementState State => _state;
        private Canvas _rootCanvas;
        public Canvas RootCanvas
        {
            get
            {
                if(_rootCanvas == null)
                {
                    _rootCanvas = GetComponentInParent<Canvas>();
                }
                return _rootCanvas;
            }
            set
            {
                _rootCanvas = value;
            }
        }
        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
            set
            {
                _canvasGroup = value;
            }
        }

        protected RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null && content != null)
                    _rectTransform = content.GetComponent<RectTransform>();
                else
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        protected Transform CachedTransform
        {
            get
            {
                if (_transform == null && content != null)
                    _transform = content;
                else
                    _transform = GetComponent<Transform>();
                return _transform;
            }
        }
        #endregion

        #region ____UTIL METHODS____
        private void ProcessNextTransition()
        {
            if (transitionQueue.Count > 0 && !isTransitioning)
            {
                var transition = transitionQueue.Dequeue();
                isTransitioning = true;
                transition.TransitionAction.Invoke();
                _state = transition.TargetState;
            }
        }

        public void EnqueueTransition(Action transitionAction, UIElementState targetState)
        {
            transitionQueue.Enqueue(new StateTransition(transitionAction, targetState));
            ProcessNextTransition();
        }

        protected void OnTransitionCompleted()
        {
            isTransitioning = false;
            ProcessNextTransition();
        }
        protected virtual void OnUIOpening()
        {
            gameObject.SetActive(true);

            if(introAnimationSettings.introAnimationType != UIIntroAnimationType.FadeIn)
            {
                CanvasGroup.alpha = _initialFade;
            }
            if(introAnimationSettings.introAnimationType != UIIntroAnimationType.ZoomIn )
            {
                CachedTransform.localScale = _initialScale;
            }

            _state = UIElementState.Opening;
            OnOpening?.Invoke();

        }
        protected virtual void OnUIOpened()
        {
            _state = UIElementState.Opened;
            switch (introAnimationSettings.introAnimationType)
            {
                case UIIntroAnimationType.FadeIn:
                case UIIntroAnimationType.ZoomIn:
                    RectTransform.anchoredPosition = _initialAnchoredPosition;
                    break;
            }
            OnOpened?.Invoke();
        }
        protected virtual void OnUIClosing()
        {
            _state = UIElementState.Closing;
            OnClosing?.Invoke();
        }
        protected virtual void OnUIClosed()
        {
            _state = UIElementState.Closed;
            RectTransform.anchoredPosition = _initialAnchoredPosition;
            CachedTransform.localScale = _initialScale;
            CanvasGroup.alpha = _initialFade;

            OnClosed?.Invoke();
            gameObject.SetActive(false);
        }
        #endregion
        public virtual void Awake()
        {
            if (CanvasGroup == null)
            {
                CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            _initialAnchoredPosition = RectTransform.anchoredPosition;
            _initialScale = CachedTransform.localScale;
            _initialFade = CanvasGroup.alpha;
        }
        #region ____MAIN METHODS____
        public virtual void Show()
        {
            if (_state != UIElementState.Closed || isTransitioning)
                return;
            if (Time.time - lastActionTime < actionThreshold)
                return;
            lastActionTime = Time.time;
            OnUIOpening();

            EnqueueTransition(() => 
            {
                PlayIntroAnimation()?.OnComplete(() => 
                {
                    OnUIOpened();
                    OnTransitionCompleted();
                });
            }, UIElementState.Opening);

        }

        public virtual void Hide()
        {
            if (_state != UIElementState.Opened || isTransitioning)
                return;
            if (Time.time - lastActionTime < actionThreshold)
                return;
            lastActionTime = Time.time;
            OnUIClosing();

            EnqueueTransition(() => 
            {
                PlayOutroAnimation()?.OnComplete(() => 
                {
                    OnUIClosed();
                    OnTransitionCompleted();
                });
            }, UIElementState.Closing);

        }
        public void ApplyIntroAnimationSettings(UIIntroAnimationSettings settings)
        {
            this.introAnimationSettings = settings;
        }
        public virtual Tween PlayIntroAnimation()
        {
            switch (introAnimationSettings.introAnimationType)
            {
                case UIIntroAnimationType.FadeIn:
                    return PlayFadeInAnimation(introAnimationSettings);
                case UIIntroAnimationType.ZoomIn:
                    return PlayZoomInAnimation(introAnimationSettings);
                case UIIntroAnimationType.MoveInFromBottom:
                case UIIntroAnimationType.MoveInFromTop:
                case UIIntroAnimationType.MoveInFromLeft:
                case UIIntroAnimationType.MoveInFromRight:
                    return PlayMoveInAnimation(introAnimationSettings.introAnimationType, introAnimationSettings);
                default:
                    return null;
            }
        }

        public virtual Tween PlayOutroAnimation()
        {
            switch (outroAnimationSettings.outroAnimationType)
            {
                case UIOutroAnimationType.FadeOut:
                    return PlayFadeOutAnimation(outroAnimationSettings);
                case UIOutroAnimationType.ZoomOut:
                    return PlayZoomOutAnimation(outroAnimationSettings);
                case UIOutroAnimationType.MoveOutToBottom:
                case UIOutroAnimationType.MoveOutToTop:
                case UIOutroAnimationType.MoveOutToLeft:
                case UIOutroAnimationType.MoveOutToRight:
                    return PlayMoveOutAnimation(outroAnimationSettings.outroAnimationType, outroAnimationSettings);
                default:
                    OnUIClosed();
                    return null;
            }
        }

        private Tween PlayFadeInAnimation(UIAnimationSettings settings)
        {
            if(CanvasGroup == null)
            {
                OnUIOpened();
                return null;
            }
            CanvasGroup.alpha = 0f;
            return CanvasGroup.DOFade(1f, settings.animationDuration).SetEase(settings.easeType);
        }

        private Tween PlayFadeOutAnimation(UIAnimationSettings settings)
        {
            if (CanvasGroup == null)
            {
                OnUIClosed();
                return null;
            }
            return CanvasGroup.DOFade(0f, settings.animationDuration).SetEase(settings.easeType);
        }

        private Tween PlayZoomInAnimation(UIAnimationSettings settings)
        {
            if (CachedTransform == null)
            {
                OnUIOpened();
                return null;
            }
            CachedTransform.localScale = Vector3.zero;
            return CachedTransform.DOScale(Vector3.one, settings.animationDuration).SetEase(settings.easeType);
        }

        private Tween PlayZoomOutAnimation(UIAnimationSettings settings)
        {
            if (CachedTransform == null)
            {
                OnUIClosed();
                return null;
            }
            return CachedTransform.DOScale(Vector3.zero, settings.animationDuration).SetEase(settings.easeType);
        }

        private Tween PlayMoveInAnimation(UIIntroAnimationType type, UIAnimationSettings settings)
        {
            if (RectTransform == null)
            {
                OnUIOpened();
                return null;
            }
            Vector2 initialPosition = RectTransform.anchoredPosition;
            Vector2 offset = GetMoveOffset(type);
            RectTransform.anchoredPosition = initialPosition + offset;
            return RectTransform.DOAnchorPos(initialPosition, settings.animationDuration).SetEase(settings.easeType);
        }

        private Tween PlayMoveOutAnimation(UIOutroAnimationType type, UIAnimationSettings settings)
        {
            if (RectTransform == null)
            {
                OnUIClosed();
                return null;
            }
            Vector2 offset = GetMoveOffset(type);
            return RectTransform.DOAnchorPos(RectTransform.anchoredPosition + offset, settings.animationDuration).SetEase(settings.easeType);
        }

        private Vector2 GetMoveOffset(System.Enum type)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // Get the size of the canvas to calculate movement relative to full screen dimensions
            float canvasWidth = canvasRect.sizeDelta.x;
            float canvasHeight = canvasRect.sizeDelta.y;

            Vector2 offset = Vector2.zero;

            if (type is UIIntroAnimationType)
            {
                switch ((UIIntroAnimationType)type)
                {
                    case UIIntroAnimationType.MoveInFromBottom:
                        offset = new Vector2(0, -canvasHeight - RectTransformUtils.GetElementHeight(RectTransform));
                        break;
                    case UIIntroAnimationType.MoveInFromTop:
                        offset = new Vector2(0, canvasHeight + RectTransformUtils.GetElementHeight(RectTransform));
                        break;
                    case UIIntroAnimationType.MoveInFromLeft:
                        offset = new Vector2(-canvasWidth - RectTransformUtils.GetElementWidth(RectTransform), 0);
                        break;
                    case UIIntroAnimationType.MoveInFromRight:
                        offset = new Vector2(canvasWidth + RectTransformUtils.GetElementWidth(RectTransform), 0);
                        break;
                }
            }
            else if (type is UIOutroAnimationType)
            {
                switch ((UIOutroAnimationType)type)
                {
                    case UIOutroAnimationType.MoveOutToBottom:
                        offset = new Vector2(0, -canvasHeight - RectTransformUtils.GetElementHeight(RectTransform));
                        break;
                    case UIOutroAnimationType.MoveOutToTop:
                        offset = new Vector2(0, canvasHeight + RectTransformUtils.GetElementHeight(RectTransform));
                        break;
                    case UIOutroAnimationType.MoveOutToLeft:
                        offset = new Vector2(-canvasWidth - RectTransformUtils.GetElementWidth(RectTransform), 0);
                        break;
                    case UIOutroAnimationType.MoveOutToRight:
                        offset = new Vector2(canvasWidth + RectTransformUtils.GetElementWidth(RectTransform), 0);
                        break;
                }
            }

            return offset;
        }

        #endregion
    }

    public class StateTransition
    {
        public Action TransitionAction;
        public UIElementState TargetState;

        public StateTransition(Action transitionAction, UIElementState targetState)
        {
            TransitionAction = transitionAction;
            TargetState = targetState;
        }
    }
}
