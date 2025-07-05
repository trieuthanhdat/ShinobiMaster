using TD.UIFramework.Configs;
using DG.Tweening;
using System;

namespace TD.UIFramework.Core
{
    public interface IUIElement
    {
        event Action OnOpening;
        event Action OnClosing;
        event Action OnOpened;
        event Action OnClosed;
        void Show();
        void Hide();
        void ApplyIntroAnimationSettings(UIIntroAnimationSettings settings);
        Tween PlayIntroAnimation();
        Tween PlayOutroAnimation();
    }

}

