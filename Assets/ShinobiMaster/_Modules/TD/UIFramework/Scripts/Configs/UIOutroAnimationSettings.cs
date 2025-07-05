using System.Collections;
using System.Collections.Generic;
using TD.UIFramework.Configs;
using UnityEngine;

namespace TD.UIFramework.Configs
{
    [CreateAssetMenu(fileName = "UIOutroAnimationSettings", menuName = "UI Framework/UI Outro Animation Settings")]
    public class UIOutroAnimationSettings : UIAnimationSettings
    {
        public UIOutroAnimationType outroAnimationType;
    }

}
