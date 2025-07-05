using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoUI : MonoBehaviour
{
    public static LevelInfoUI LevelInfo;

    [SerializeField] private Text levelInfo;

    [SerializeField] private Transform iconStage;
    [SerializeField] private StageIconUI prefabIcon;

    [SerializeField] private Text finalLevel;
    [SerializeField] private Text stageComplite;
    [SerializeField] private GameObject stagesCountPanel;
    [SerializeField] private Animator stageProgressPanelAnimator;
    [SerializeField] private RectTransform stageProgressParent;
    [SerializeField] private Text currLvlAndStageText;
    [SerializeField] private Text currConfigText;
    [SerializeField] private float finalStageDelay;
    [SerializeField] private GameObject keysPanel;
    [SerializeField] private float showKeysPanelDuration;
    private Coroutine finalStageCoroutine;
    private Coroutine finalLevelCoroutine;
    private Coroutine showKeysPanelCoroutine;
    
    
    private void Awake()
    {
        if (LevelInfo)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            LevelInfo = this;
        }
    }

    private void Start()
    {
        GameHandler.Singleton.PlayerProfile.OnKeysCountChanged += OnKeysCountChanged;
    }

    private void OnKeysCountChanged(int keyCount, int prevKeyCount)
    {
        if (keyCount > prevKeyCount)
        {
            ShowKeysPanel(this.showKeysPanelDuration);
        }
    }

    public void SetNameLevel(int numer)
    {
        levelInfo.text = "Level " + numer.ToString();
    }

    public void SetCurrLvlAndStageText(int lvl, int stage)
    {
        this.currLvlAndStageText.text = lvl + "." + stage;
    }
    
    public void SetCurrConfigText(int config)
    {
        this.currConfigText.text = config.ToString();
    }

    public void SetActiveKeysPanel(bool active)
    {
        if (this.showKeysPanelCoroutine != null)
        {
            StopCoroutine(this.showKeysPanelCoroutine);
        }
    
        this.keysPanel.SetActive(active);
    }

    public void ShowKeysPanel(float duration)
    {
        if (this.showKeysPanelCoroutine != null)
        {
            StopCoroutine(this.showKeysPanelCoroutine);
        }
    
        this.showKeysPanelCoroutine = StartCoroutine(ShowKeysPanelProcess(duration));
    }

    private IEnumerator ShowKeysPanelProcess(float duration)
    {
        SetActiveKeysPanel(true);
        
        yield return new WaitForSecondsRealtime(duration);
        
        SetActiveKeysPanel(false);
    }

    public void SetParamsLevelIcon(StageIconParams[] stageIconParams)
    {
        foreach (Transform tr in this.iconStage)
        {
            Destroy(tr.gameObject);
        }
    
        for (var i = 0; i < stageIconParams.Length; i++)
        {
            var stageIcon = Instantiate(this.prefabIcon, this.iconStage);
            
            stageIcon.SetParams(stageIconParams[i]);
        }
    }

    public void HideFinalLevel()
    {
        if (this.finalLevelCoroutine != null)
        {
            StopCoroutine(this.finalLevelCoroutine);
        }
    
        finalLevel.gameObject.SetActive(false);
    }

    public void HideFinalStage()
    {
        if (this.finalStageCoroutine != null)
        {
            StopCoroutine(this.finalStageCoroutine);
        }
    
        stageComplite.gameObject.SetActive(false);
    }

    public void FinalLevel()
    {
        this.finalLevelCoroutine = StartCoroutine(FinalLevelWithDelay(this.finalStageDelay));
    }

    public void FinalStage()
    {
        this.finalStageCoroutine = StartCoroutine(FinalStageWithDelay(this.finalStageDelay));
    }

    private IEnumerator FinalStageWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        
        stageComplite.gameObject.SetActive(true);
        finalLevel.gameObject.SetActive(false);
    }
    
    private IEnumerator FinalLevelWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        
        finalLevel.gameObject.SetActive(true);
        stageComplite.gameObject.SetActive(false);
    }

    public void SetActiveStagesCountPanel(bool active)
    {
        this.stagesCountPanel.SetActive(active);
    }

    public void SetActiveLevelText(bool active)
    {
        this.levelInfo.gameObject.SetActive(active);
    }

    public void AnimShowStageProgressPanel(float delay)
    {
        StartCoroutine(PlayWithDelay(this.stageProgressPanelAnimator, "StageProgressShow", delay));
    }
    
    public void AnimHideStageProgressPanel(float delay)
    {
        StartCoroutine(PlayWithDelay(this.stageProgressPanelAnimator, "StageProgressHide", delay));
    }

    private IEnumerator PlayWithDelay(Animator animator, string anim, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
    
        animator.Play(anim);
    }
}
