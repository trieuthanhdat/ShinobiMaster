using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TD.Utilities
{
    public static class InputUtils
    {
#if ENABLE_INPUT_SYSTEM
        public static System.Func<Vector3> GetVirtualMouseOverride;

        /// <summary>
        /// Returns the current mouse position, supporting both new and old Unity Input Systems.
        /// Supports optional override for custom virtual mouse.
        /// </summary>
        public static Vector3 GetMousePosition()
        {
            // Custom override if provided (e.g., by your InputManager)
            if (GetVirtualMouseOverride != null)
                return GetVirtualMouseOverride();

            return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector3.zero;
        }
#else
    public static Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }
#endif
    }

}
