using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.UIFramework.Configs
{
    [CreateAssetMenu(fileName = "UIntroAnimationSettings", menuName = "UI Framework/UI Intro Animation Settings")]
    public class UIIntroAnimationSettings : UIAnimationSettings
    {
        public UIIntroAnimationType introAnimationType;
    }

}

