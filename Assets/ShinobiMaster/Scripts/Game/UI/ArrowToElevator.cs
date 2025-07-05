using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowToElevator : MonoBehaviour
{
    public static ArrowToElevator Singleton { get; private set; }


    [SerializeField] private Transform arrow;

    [SerializeField] private Image arrowSprite;
    
    public Vector3 StartSize { get; private set; }
    
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

        StartSize = this.arrowSprite.rectTransform.sizeDelta;
    }

    public void Show()
    {
        arrowSprite.enabled = true;
    }

    public void SetPositionAndDirect(Vector3 direct)
    {
        transform.right = direct;
    }



    public void Hide()
    {
        arrowSprite.enabled = false;
    }

    public void SetAlpha(float alpha)
    {
        var color = this.arrowSprite.color;

        color.a = alpha;

        this.arrowSprite.color = color;
    }

    public void SetSize(Vector2 size)
    {
        this.arrowSprite.rectTransform.sizeDelta = size;
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }
}
