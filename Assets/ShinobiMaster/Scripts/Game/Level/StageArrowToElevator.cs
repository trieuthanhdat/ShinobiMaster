using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageArrowToElevator : StageEvent
{

    [SerializeField] private EndElevator endElevator;

    [SerializeField] private Vector3 localPositionElevator;

    bool show;

    bool gamePlay;
    private float startDist;
    

    protected override void CallLoadStage()
    {
        ArrowToElevator.Singleton.Hide();
        
        if (GameHandler.Singleton.Level.CurrStageNumber != 0)
        {
            gamePlay = true;
        }

        endElevator.PlayerExitFromStage += StopGame;
        this.startDist = Vector3.Distance(stage.PlayerStage.transform.position, endElevator.elevator.position);
    }

    protected override void CallLateUpdateStage()
    {
        if (stage.IsBossDead() && gamePlay)
        {
            show = true;

            var distToElev = Vector3.Distance(stage.PlayerStage.transform.position, endElevator.elevator.position);

            var ratio = 1 - distToElev / startDist;

            var startSize = ArrowToElevator.Singleton.StartSize;

            var size = Mathf.Clamp(startSize.x * ratio, 80, startSize.x);
            var alpha = Mathf.Clamp(ratio, 0.1f, 1f);
            
            ArrowToElevator.Singleton.SetSize(Vector2.one * size);
            ArrowToElevator.Singleton.SetAlpha(alpha);
            
            ArrowToElevator.Singleton.SetPositionAndDirect(localPositionElevator - stage.PlayerStage.transform.position);
            ArrowToElevator.Singleton.Show();
        }
    }

    private void StopGame() {
        gamePlay = false;
        ArrowToElevator.Singleton.Hide();
    }

    protected override void CallUnLoadStage()
    {
        if (show)
        {
            ArrowToElevator.Singleton.Hide();
        }
    }
}
