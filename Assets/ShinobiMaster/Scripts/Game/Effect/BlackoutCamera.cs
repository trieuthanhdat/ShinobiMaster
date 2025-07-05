using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutCamera : MonoBehaviour
{
    [SerializeField] private Image image;

    private Stopwatch timer;

    private bool toBlackout;

    private bool toClarification;


    public void Init()
    {
        timer = new Stopwatch();
        image.fillAmount = 1;
        GameHandler.Singleton.Level.Load += Clarification;
    }

    public void Blackout() {
        toBlackout = true;
        toClarification = false;
        image.fillAmount = 0;
        timer.Start();
    }

    public void Clarification()
    {
        toClarification = true;
        toBlackout = false;
        image.fillAmount = 1f;
        timer.Start();
    }

    private void Update()
    {
        if (toBlackout)
        {
            float time = timer.ElapsedMilliseconds / 500f;
            image.fillAmount = Mathf.Min(1, time);
            if (timer.ElapsedMilliseconds >= 500f)
            {
                image.fillAmount = 1;
                timer.Reset();
                toBlackout = false;
            }
        }
        else if (toClarification)
        {
            float time = 1f - timer.ElapsedMilliseconds / 500f;
            image.fillAmount = Mathf.Max(0, time);
            if (timer.ElapsedMilliseconds >= 500f)
            {
                image.fillAmount = 0;
                timer.Reset();
                toClarification = false;
            }
        }
    }
}
