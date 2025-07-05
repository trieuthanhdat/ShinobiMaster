using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class TimeControll : MonoBehaviour
{

    public const float TimeNormal = 1f;

    public const float SlowTime = 0.105f;

    public static TimeControll Singleton { get; private set; }

    private Coroutine move;

    private float time;

    private bool pause;
    private Coroutine changeTimeScaleCoroutine;


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
        time = Time.timeScale;
    }

    public void ChannelTimeTo(float time)
    {
        if (this.pause)
        {
            return;
        }
    
        if (move != null)
        {
            StopCoroutine(move);
        }
        move = StartCoroutine(MoveTime(time));
    }
    
    public void ChangeTimeScaleSmoothly(float from, float to, float time)
    {
        if (this.changeTimeScaleCoroutine != null)
        {
            StopCoroutine(this.changeTimeScaleCoroutine);
        }
    
        this.changeTimeScaleCoroutine = StartCoroutine(ChangeTimeScaleSmoothlyProcess(from, to, time));
    }

    private IEnumerator ChangeTimeScaleSmoothlyProcess(float from, float to, float time)
    {
        Time.timeScale = from;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    
        var startTime = time;

        while (time > 0)
        {
            var lerp = 1 - time / startTime;

            Time.timeScale = Mathf.Lerp(from, to, lerp);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            time -= Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator MoveTime(float time)
    {
        float start = this.time;
        AudioManager.Instance.SetSlowDownAudioSourcePitch(time >= 1f ? 1f : AudioManager.Instance.SlowPitch);
        for (int i = 0; i < 15; i++)
        {
            if (!Pause.IsPause)
            {
                this.time = Mathf.Lerp(start, time, (float)i / 15f);
                Time.timeScale = this.time;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
            else
            {
                i--;
            }
            yield return new WaitForSeconds(0.003f);
        }
        move = null;
    }

    public void PauseTimeControl()
    {
        this.pause = true;
    
        if (this.move != null)
        {
            StopCoroutine(this.move);
        }
        
        if (this.changeTimeScaleCoroutine != null)
        {
            StopCoroutine(this.changeTimeScaleCoroutine);
        }
    }

    public void UnpauseTimeControl()
    {
        this.pause = false;
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }
}
