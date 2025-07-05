using System;

public interface ITimeProvider
{
    event System.Action OnNewDayCome;
    event System.Action OnNewWeekCome;
    public bool CanTimer { get; set; }
    public bool HasGotServerTime { get; set; }
    public bool IsInited { get; set; }
    public bool IsNewDay { get; set; }
    public bool IsNewWeek { get; set; }
    void GetTimeServer();
    int GetCurrentDayIndex();
    int GetCurrentWeekIndex();
    void SetupServerTime(double serverTimeStamp);
    void GetPlayfabServerTime(Action OnSuccess = null, Action<int> OnError = null);
    string GetTimeLeftUntilNextRefresh(DateTimeFormatType dateTimeFormatType = DateTimeFormatType.Hours_Minutes);
}
