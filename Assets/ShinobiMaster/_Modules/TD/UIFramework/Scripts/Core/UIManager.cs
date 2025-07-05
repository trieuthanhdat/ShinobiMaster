using DG.Tweening;
using System;
using System.Collections.Generic;
using TD.SerializableDictionary;
using TD.UIFramework.UIInput;
using UnityEngine;

namespace TD.UIFramework.Core
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private Transform screenParent;
        [SerializeField] private Transform popupParent;
        [SerializeField] private Transform overlayParent;
        [SerializeField] private UIInputHandler uiInputHandler;
        public bool CacheUIElementOnAwake = false;

        public UIInputHandler UIInputHandler
        {
            get
            {
                if (uiInputHandler == null)
                    uiInputHandler = GetComponent<UIInputHandler>();
                return uiInputHandler;
            }
            set
            {
                uiInputHandler = value;
            }
        }

        [Header("Debug Section")]
        [SerializeField] private SerializableDictionary<string, UIElement> cachedUIElements = new SerializableDictionary<string, UIElement>();
        [SerializeField] private UIBaseScreen  m_CurrentScreen;
        [SerializeField] private UIBasePopup   m_CurrentPopup;
        [SerializeField] private UIBaseOverlay m_CurrentOverlay;
        [SerializeField] private Stack<UIBasePopup> m_PopupStack = new Stack<UIBasePopup>();

        private void Awake()
        {
            if(UIInputHandler == null)
            {
                UIInputHandler = gameObject.AddComponent<UIInputHandler>();
            }
            if (CacheUIElementOnAwake)
            {
                CacheUIElements(screenParent, UIPrefabInfos.PatScreenPrefabs);
                CacheUIElements(popupParent, UIPrefabInfos.PathPopupPrefabs);
                CacheUIElements(overlayParent, UIPrefabInfos.PathOverlayPrefabs);
            }
        }
        private void OnEnable()
        {
            uiInputHandler.onKeyBackRaised += UIInputHandler_OnKeyBackRaised;
            uiInputHandler.onInputRaised   += UiInputHandler_onInputRaised;
        }

        
        private void OnDisable()
        {
            uiInputHandler.onKeyBackRaised -= UIInputHandler_OnKeyBackRaised;
            uiInputHandler.onInputRaised -= UiInputHandler_onInputRaised;
        }
       
        private void UiInputHandler_onInputRaised(KeyCode keycode)
        {
#if UNITY_EDITOR
            if(keycode == KeyCode.O)
            {
                ShowUIElement(UIPrefabInfos.Scenes.EquipmentPopup);
            }
            if(keycode == KeyCode.P)
            {
                HideUIElement(UIPrefabInfos.Scenes.EquipmentPopup);
            }
#endif
        }

        private void UIInputHandler_OnKeyBackRaised()
        {
            Debug.Log($"xx on keyback raised, count stack {m_PopupStack.Count}");

            ClosePopup();

            if (m_CurrentOverlay != null)
            {
                CloseOverlay();
            }
            else if(m_CurrentOverlay == null && m_CurrentScreen != null)
            {
                CloseScreen();
            }
        }

        private void CacheUIElements(Transform parent, string basePath)
        {
            foreach (Transform child in parent)
            {
                UIElement element = child.GetComponent<UIElement>();
                if (element != null && !cachedUIElements.ContainsKey(child.gameObject.name))
                {
                    cachedUIElements[child.gameObject.name] = element;
                    element.gameObject.SetActive(false);  // Start with all elements disabled
                }
            }
        }

        public void ShowUIElement(UIPrefabInfos.Scenes scene, Action onOpenPopupCallback = null, Action onClosePopupCallback = null)
        {
            string resourcePath = GetFullPath(scene);

            if (TryGetUIElement(resourcePath, out UIElement element))
            {
                if (!element.CanOpen()) return;
                RegisterElementCallbacks(onOpenPopupCallback, onClosePopupCallback, element);
                ManageUIElement(element);
            }
            else
            {
                LoadAndShowElement(resourcePath);
            }
        }

        private static void RegisterElementCallbacks(Action onOpenPopupCallback, Action onClosePopupCallback, UIElement element)
        {
            if (onOpenPopupCallback != null)
            {
                element.OnOpening -= onOpenPopupCallback;
                element.OnOpening += onOpenPopupCallback;
            }
            if (onClosePopupCallback != null)
            {
                element.OnClosed -= onClosePopupCallback;
                element.OnClosed += onClosePopupCallback;
            }
        }

        public void HideUIElement(UIPrefabInfos.Scenes scene)
        {
            string resourcePath = GetFullPath(scene);
            if (TryGetUIElement(resourcePath, out UIElement element))
            {
                if (!element.CanClose()) return;
                HandleClose(element);
            }
        }
        private UIElement TryGetUIElement(string resourcePath, out UIElement element)
        {
            return cachedUIElements.TryGetValue(resourcePath, out element) ? element : null;
        }
        private string GetFullPath(UIPrefabInfos.Scenes scene)
        {
            string basePath = UIPrefabInfos.ScenesMapper.ContainsKey(scene) ? GetBasePath(scene) : "";
            return basePath + UIPrefabInfos.ScenesMapper[scene];
        }

        private string GetBasePath(UIPrefabInfos.Scenes scene)
        {
            if (scene.ToString().Contains("Popup"))
                return UIPrefabInfos.PathPopupPrefabs;
            else if (scene.ToString().Contains("Screen"))
                return UIPrefabInfos.PatScreenPrefabs;
            else if (scene.ToString().Contains("Overlay"))
                return UIPrefabInfos.PathOverlayPrefabs;

            Debug.LogError($"UI MANAGER: Fail to get path!! check agian this path {scene}");
            return ""; // Default path, or throw an error
        }

        private void LoadAndShowElement(string resourcePath, Action onOpenPopupCallback = null, Action onClosePopupCallback = null)
        {
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab != null)
            {
                Transform parent    = DetermineParent(prefab.GetComponent<UIElement>());
                GameObject instance = Instantiate(prefab, parent);
                UIElement element   = instance.GetComponent<UIElement>();
                cachedUIElements[resourcePath] = element;
                RegisterElementCallbacks(onOpenPopupCallback, onClosePopupCallback, element);
                ManageUIElement(element);
            }
            else
            {
                Debug.LogError("Failed to load prefab from Resources: " + resourcePath);
            }
        }

        private Transform DetermineParent(UIElement element)
        {
            if (element is UIBaseScreen)
                return screenParent;
            else if (element is UIBasePopup)
                return popupParent;
            else if (element is UIBaseOverlay)
                return overlayParent;

            return transform; // Default parent
        }

        private void ManageUIElement(UIElement element)
        {
            element.OnOpening -= () => HandleOpen(element);
            element.OnClosed  -= () => HandleClose(element);

            element.OnOpening += () => HandleOpen(element);
            element.OnClosed  += () => HandleClose(element);

            element.Show();  // This ensures the show method is called with event subscriptions set up
        }

        private void HandleOpen(UIElement element)
        {
            if (element is UIBaseScreen screen)
            {
                if (m_CurrentScreen != null && m_CurrentScreen != screen)
                {
                    m_CurrentScreen.Hide();
                }
                m_CurrentScreen = screen;
            }
            else if (element is UIBasePopup popup)
            {
                if (m_PopupStack.Count > 0)
                {
                    var topPopup =  m_PopupStack.Peek();  // Optionally hide the current top popup
                    if(popup != topPopup)
                        topPopup.Hide();
                }
                m_CurrentPopup = popup;
                m_PopupStack.Push(popup);
            }
            else if (element is UIBaseOverlay overlay)
            {
                if (m_CurrentOverlay != null)
                {
                    m_CurrentOverlay.Hide();
                }
                m_CurrentOverlay = overlay;
            }
        }

        private void HandleClose(UIElement element)
        {
            if (element == null) return;
            if (element is UIBaseScreen)
            {
                CloseScreen();
            }
            else if (element is UIBasePopup)
            {
                ClosePopup();
            }
            else if (element is UIBaseOverlay)
            {
                CloseOverlay();
            }
        }
        public void ClosePopup()
        {
            if (m_CurrentPopup)
            {
                if (!m_CurrentPopup.CanClose()) return;
                m_CurrentPopup.Hide();
                m_CurrentPopup = null;
            }

            if (m_PopupStack.Count > 0)
            {
                UIBasePopup nextPopup = m_PopupStack.Pop();
                nextPopup.Show();
            }
           
        }

        public void CloseOverlay()
        {
            if (m_CurrentOverlay != null)
            {
                if (!m_CurrentOverlay.CanClose()) return;
                m_CurrentOverlay.Hide();
                m_CurrentOverlay = null;
            }
        }
        public void CloseScreen()
        {
            if (m_CurrentScreen != null)
            {
                if (!m_CurrentScreen.CanClose()) return;
                m_CurrentScreen.Hide();
                m_CurrentScreen = null;
            }
        }
    }
    

}
