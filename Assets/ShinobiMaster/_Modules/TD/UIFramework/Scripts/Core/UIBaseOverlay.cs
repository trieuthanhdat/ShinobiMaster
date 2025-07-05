using UnityEngine;
using DG.Tweening;

namespace TD.UIFramework.Core
{
    public class UIBaseOverlay : UIElement
    {
        public BaseUIType UIType { get; private set; } = BaseUIType.Overlay;
    }

}

