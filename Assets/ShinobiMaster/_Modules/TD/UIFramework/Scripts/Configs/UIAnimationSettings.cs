using UnityEngine;
using DG.Tweening;

namespace TD.UIFramework.Configs
{
    [CreateAssetMenu(fileName = "UIAnimationSettings", menuName = "UI Framework/UI Animation Settings")]
    public class UIAnimationSettings : ScriptableObject
    {
        public float animationDuration = 0.5f;
        public Ease easeType = Ease.InOutQuad;
    }
    public enum UIIntroAnimationType
    {
        None,
        FadeIn,
        ZoomIn,
        MoveInFromBottom,
        MoveInFromTop,
        MoveInFromLeft,
        MoveInFromRight,
    }
    public enum UIOutroAnimationType
    {
        None,
        FadeOut,
        ZoomOut,
        MoveOutToBottom,
        MoveOutToTop,
        MoveOutToLeft,
        MoveOutToRight
    }
}

