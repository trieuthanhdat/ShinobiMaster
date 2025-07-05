
using UnityEngine;

namespace TD.UIFramework.UIInput
{
    public interface IUIInputHandler
    {
        event System.Action<KeyCode> onInputRaised;
        event System.Action onKeyBackRaised;
        void Update();
        void OnInputRaised(KeyCode key);
        void OnKeyBackRaised();
    }
}


