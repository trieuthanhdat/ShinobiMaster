using System;
using System.Collections;
using System.Collections.Generic;
using TD.UIFramework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TD.UIFramework.Misc
{
    [RequireComponent(typeof(Button))]
    public class UISymbolButton : MonoBehaviour
    {
        [SerializeField] UIPrefabInfos.Scenes sceneName;

        private Button m_Button;
        private void Awake()
        {
            m_Button = GetComponent<Button>();
            m_Button.onClick.AddListener(OnClick_OpenUI);
        }

        private void OnClick_OpenUI()
        {
            UIManager.Instance.ShowUIElement(sceneName);
        }
    }
}

