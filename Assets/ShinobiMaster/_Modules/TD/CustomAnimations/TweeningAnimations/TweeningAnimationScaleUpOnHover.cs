using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public enum HoverScaleMethod
{
    LocalScale,
    RectWidthAndHeight
}

public class TweeningAnimationScaleUpOnHover : TweeningAnimation, IPointerEnterHandler, IPointerExitHandler
{
    public HoverScaleMethod hoverScaleMethod = HoverScaleMethod.LocalScale;
    public Vector3 hoverScaleAddon = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 originalScale;
    private Vector2 originalSizeDelta;
    private Vector2 originalAnchoredPosition;
    private bool _isScalingUp = false;

    // Constructor
    public TweeningAnimationScaleUpOnHover(float duration, Ease ease, TweeningAnimationType type, Vector3 hoverScale) : base(duration, ease, type)
    {
        this.tweenDuration = duration;
        this.easeType = ease;
        this.hoverScaleAddon = hoverScale;
        this.TypeTweenAnimation = TweeningAnimationType.SCALE_ON_HOVER; // or any other type if needed
    }

    public override void OnInit()
    {
        if (hoverScaleMethod == HoverScaleMethod.LocalScale)
        {
            originalScale = transform.localScale;
        }
        else if (hoverScaleMethod == HoverScaleMethod.RectWidthAndHeight && GetComponent<RectTransform>() != null)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            originalSizeDelta = rectTransform.sizeDelta;
            originalAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (hoverScaleMethod == HoverScaleMethod.LocalScale)
        {
            originalScale = transform.localScale;
        }
        else if (hoverScaleMethod == HoverScaleMethod.RectWidthAndHeight && GetComponent<RectTransform>() != null)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            originalSizeDelta = rectTransform.sizeDelta;
            originalAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (hoverScaleMethod == HoverScaleMethod.LocalScale)
        {
            transform.localScale = originalScale;
        }
        else if (hoverScaleMethod == HoverScaleMethod.RectWidthAndHeight && GetComponent<RectTransform>() != null)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = originalSizeDelta;
            rectTransform.anchoredPosition = originalAnchoredPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isScalingUp = true;
        PlayAnimation();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isScalingUp = false;
        PlayAnimation();
    }

    public override Tween GetTweenAnimation()
    {
        
        if (hoverScaleMethod == HoverScaleMethod.LocalScale)
        {
            Vector3 nextScale = _isScalingUp ? originalScale + hoverScaleAddon : originalScale;
            m_TweenAnimation = transform.DOScale(nextScale, tweenDuration).SetEase(easeType);
        }
        else if (hoverScaleMethod == HoverScaleMethod.RectWidthAndHeight && GetComponent<RectTransform>() != null)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 nextSize = _isScalingUp ? originalSizeDelta + new Vector2(hoverScaleAddon.x, hoverScaleAddon.y) : originalSizeDelta;

            // Calculate the new anchored position to keep the object in place
            Vector2 deltaSize = nextSize - originalSizeDelta;
            Vector2 newAnchoredPosition = originalAnchoredPosition;

            AnchorPresets anchorPreset = RectTransformUtils.GetCurrentAnchorPreset(rectTransform.anchorMin, rectTransform.anchorMax);
            switch (anchorPreset)
            {
                case AnchorPresets.TopLeft:
                case AnchorPresets.MiddleLeft:
                case AnchorPresets.BottomLeft:
                case AnchorPresets.VertStretchLeft:
                    newAnchoredPosition.x += deltaSize.x / 2;
                    break;
                case AnchorPresets.TopRight:
                case AnchorPresets.MiddleRight:
                case AnchorPresets.BottomRight:
                case AnchorPresets.VertStretchRight:
                    newAnchoredPosition.x -= deltaSize.x / 2;
                    break;
                case AnchorPresets.TopCenter:
                case AnchorPresets.BottonCenter:
                case AnchorPresets.HorStretchTop:
                case AnchorPresets.HorStretchMiddle:
                case AnchorPresets.HorStretchBottom:
                    newAnchoredPosition.y += deltaSize.y / 2;
                    break;
                default:
                    newAnchoredPosition.x += deltaSize.x / 2;
                    newAnchoredPosition.y += deltaSize.y / 2;
                    break;
            }

            // Check for MiddleCenter, MiddleTop, and MiddleLeft to avoid changing position
            if (anchorPreset == AnchorPresets.MiddleCenter || anchorPreset == AnchorPresets.TopCenter || anchorPreset == AnchorPresets.MiddleLeft)
            {
                newAnchoredPosition = originalAnchoredPosition;
            }

            rectTransform.DOSizeDelta(nextSize, tweenDuration).SetEase(easeType);
            rectTransform.DOAnchorPos(newAnchoredPosition, tweenDuration).SetEase(easeType);
            m_TweenAnimation = rectTransform.DOSizeDelta(nextSize, tweenDuration).SetEase(easeType);
        }

        RegisterOnStartAndOnCompleteCallbacks();
        return m_TweenAnimation;
    }
}
