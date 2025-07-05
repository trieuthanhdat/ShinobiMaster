using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapToPlayTextUI : MonoBehaviour
{
    public static TapToPlayTextUI TapToPlayText;

    [SerializeField] private Text text;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject panel;



    private void Awake()
    {
        if (TapToPlayText)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            TapToPlayText = this;
        }
    }
    
    
    

    public void ShowText()
    {
        text.gameObject.SetActive(true);
    }

    public void HideText()
    {
        text.gameObject.SetActive(false);
    }

    public void AnimHidePanel()
    {
        this.animator.Play("HideTapToPlay");
    }

    public void AnimShowPanel()
    {
        this.animator.Play("ShowTapToPlay");
    }
}
