using UnityEngine;

public static class ClassUtil
{
    public static void SetVisible(this GameObject obj, bool isVisible)
    {
        if (obj == null)
            return;
        obj.SetActive(isVisible);
    }
    public static void SetDisable(this UnityEngine.UI.Button button, bool isDisable)
    {
        if (button != null)
        {
            button.gameObject.SetActive(!isDisable);
        }
    }
}
