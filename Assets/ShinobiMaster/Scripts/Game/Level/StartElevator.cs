using System.Collections;
using System.Collections.Generic;
using Game.LevelControll;
using Game.UI;
using Narrative.Data;
using UnityEngine;

public class StartElevator : StageEvent
{
    public event System.Action PlayerExitFromElevatorEvent;

    [SerializeField] private Transform elevator;

    [SerializeField] private Vector3 startElevatorPosLocal;

    [SerializeField] private Vector3 endElevatorPosLocal;

    [SerializeField] private Vector3 posPlayerOnElevator;

    [SerializeField] private float time;

    [SerializeField] private Vector3 moveToPos;

    [SerializeField] private float timeMove;


    private bool start;

    protected override void CallLoadStage()
    {
        stage.PlayerStage.StateController.BeginControll();
        elevator.transform.localPosition = startElevatorPosLocal;
        stage.PlayerStage.StateController.SetPosition(elevator.TransformPoint(posPlayerOnElevator));
    }

    protected override void CallLateUpdateStage()
    {
        if (!start && !Pause.IsPause) {
            TimeControll.Singleton.ChannelTimeTo(TimeControll.TimeNormal);
            StartCoroutine(StartGameAnim());
            start = true;
        }
    }

    IEnumerator StartGameAnim()
    {
        stage.PlayerStage.StateController.PlayerDirection(-1);

        int countIt = (int)(time / 0.02f);
        stage.PlayerStage.StateController.PlayerStay();
        
        stage.PlayerStage.StateController.SetPosition(elevator.TransformPoint(posPlayerOnElevator));

        if (GameHandler.Singleton.Level.CurrStageNumber != 0)
        {
            LevelInfoUI.LevelInfo.AnimShowStageProgressPanel(0f);
        }

        yield return new WaitForSecondsRealtime(0.02f);
        
        for (int i = 0; i < countIt; i++)
        {
            elevator.transform.localPosition = Vector3.Lerp(startElevatorPosLocal, endElevatorPosLocal, (float)i / (float)countIt);
            stage.PlayerStage.StateController.SetPosition(elevator.TransformPoint(posPlayerOnElevator));
            yield return new WaitForSecondsRealtime(0.02f);
        }      
        
        while (Pause.IsPause) {
            yield return null;
        }
        
        countIt = (int)(timeMove / 0.02f);
        stage.PlayerStage.StateController.PlayerRun();

        for (int i = 0; i < countIt; i++)
        {
            stage.PlayerStage.StateController.SetPosition(Vector3.Lerp(elevator.TransformPoint(posPlayerOnElevator), moveToPos, (float)i / (float)countIt));
            yield return new WaitForSecondsRealtime(0.02f);
        }
        stage.PlayerStage.StateController.PlayerStay();
        
        if (GameHandler.Singleton.Level.CurrStageNumber != 0)
        {
            LevelInfoUI.LevelInfo.AnimHideStageProgressPanel(1.5f);
        }

        if (GameHandler.Singleton.Level.CurrStageNumber != 0 && 
            GameHandler.Singleton.Level.CurrStageNumber != BaseLevel.StageCountOnLevel - 1)
        {
            stage.PlayerStage.StateController.EndControll();
            
            DragToAim.Singleton.Show();
            DragToAim.Singleton.Hide(3.0f);
        }

        if (PlayerExitFromElevatorEvent != null)
            PlayerExitFromElevatorEvent();
    }

    protected override void CallUpdateStage()
    {

    }

    protected override void CallUnLoadStage()
    {

    }
}
