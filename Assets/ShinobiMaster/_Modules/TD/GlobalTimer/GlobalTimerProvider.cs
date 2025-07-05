using System;
using TD.GlobalTimer;

public class GlobalTimeProvider : ITimeProvider
{
    public event Action OnNewDayCome;
    public event Action OnNewWeekCome;
    public bool CanTimer  { get => GlobalTimerCounter.Instance.CanTimer;  set => GlobalTimerCounter.Instance.CanTimer = value; }
    public bool IsInited  { get => GlobalTimerCounter.Instance.IsInited;  set => GlobalTimerCounter.Instance.IsInited = value; }
    public bool IsNewDay
    {
        get => GlobalTimerCounter.Instance.IsNewDay;
        set
        {
            GlobalTimerCounter.Instance.IsNewDay = value;
            if (value) OnNewDayCome?.Invoke();
        }
    }
    public bool IsNewWeek
    {
        get => GlobalTimerCounter.Instance.IsNewWeek;
        set
        {
            GlobalTimerCounter.Instance.IsNewDay = value;
            if (value) OnNewWeekCome?.Invoke();
        }
    }
    public bool HasGotServerTime { get => GlobalTimerCounter.Instance.HasGotServerTime; set => GlobalTimerCounter.Instance.HasGotServerTime = value; }

    public void GetTimeServer()
    {
        GlobalTimerCounter.Instance.GetTimeServer();
    }
    public int GetCurrentDayIndex()
    {
        return GlobalTimerCounter.Instance.GetCurrentDayIndex();
    }
    public int GetCurrentWeekIndex()
    {
        return GlobalTimerCounter.Instance.GetCurrentWeekIndex();
    }
    public void SetupServerTime(double serverTimeStamp)
    {
        GlobalTimerCounter.Instance.SetupServerTime(serverTimeStamp);
    }
    public string GetTimeLeftUntilNextRefresh(DateTimeFormatType dateTimeFormatType = DateTimeFormatType.Hour_Minutes_Seconds)
    {
        return GlobalTimerCounter.Instance.GetTimeLeftUntilNextRefresh(dateTimeFormatType);
    }

    public void GetPlayfabServerTime(Action OnSuccess = null, Action<int> OnError = null)
    {
        GlobalTimerCounter.Instance.GetPlayfabServerTime(OnSuccess, OnError);
    }
}

