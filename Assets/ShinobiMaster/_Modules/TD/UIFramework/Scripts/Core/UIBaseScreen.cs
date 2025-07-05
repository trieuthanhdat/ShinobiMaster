using UnityEngine;
using DG.Tweening;

namespace TD.UIFramework.Core
{
    public class UIBaseScreen : UIElement
    {
        public BaseUIType UIType { get; private set; } = BaseUIType.Screen;
    }
}

