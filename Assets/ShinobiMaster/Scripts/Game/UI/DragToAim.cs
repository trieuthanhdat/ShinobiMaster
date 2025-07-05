using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragToAim : MonoBehaviour
{
    public static DragToAim Singleton { get; private set; }

    [SerializeField] private GameObject uiObject;

    private void Awake()
    {
        if (Singleton)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }
    }

    public void Show()
    {
        uiObject.SetActive(true);
    }


    public void Hide()
    {
        uiObject.SetActive(false);
    }
    
    public void Hide(float delay)
    {
        StartCoroutine(HideWithDelayProcess(delay));
    }

    private IEnumerator HideWithDelayProcess(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        
        Hide();
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }
}
