using UnityEngine;
using DG.Tweening;

namespace TD.UIFramework.Core
{
    [System.Serializable]
    public class UIBasePopup : UIElement
    {
        public BaseUIType UIType { get; private set; } = BaseUIType.Popup;
    }

}
