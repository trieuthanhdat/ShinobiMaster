using DG.Tweening;
using UnityEngine.Events;

public interface ITweenAnimation
{
    TweeningAnimationType TypeTweenAnimation { get; set; }
    float TimeScale { get; set; }
    float TweenDuration { get; set; }
    float TweenDelay { get; set; }

    Ease EaseType { get; set; }
    bool HasSpeedUp { get; set; }
    Tween TweenAnimation { get; set; }


    public void  OnInit();
    public Tween GetTweenAnimation();
    void PlayAnimation();
    void RestartAnimation();

    void AddStartAnimationCallback(System.Action callback, bool removeAll = false);
    void AddCompleteAnimationCallback(System.Action callback, bool removeAll = false);
}
