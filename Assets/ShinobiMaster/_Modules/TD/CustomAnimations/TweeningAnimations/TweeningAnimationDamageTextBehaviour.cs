using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public enum DamageTextSimulationSpace
{
    UI,
    WorldPos
}

public class TweeningAnimationDamageTextBehaviour : TweeningAnimation
{
    [SerializeField] private TextMeshProUGUI damageTextPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform textPoolTransform;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, 0);
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private Ease animationEase = Ease.OutQuad;
    [SerializeField] private DamageTextSimulationSpace simulationSpace = DamageTextSimulationSpace.WorldPos;
    [SerializeField] private Canvas uiCanvas;

    private Queue<TextMeshProUGUI> textPool = new Queue<TextMeshProUGUI>();
    private TextMeshProUGUI currentTextInstance;

    public TweeningAnimationDamageTextBehaviour(float duration, Ease ease, TweeningAnimationType type)
    : base(duration, ease, type)
    {
        animationDuration = duration;
        animationEase = ease;
        tweeningAnimationType = type;
    }

    public override void Awake()
    {
        base.Awake();
        InitializePool();
    }

    private void InitializePool()
    {
        if (textPool == null) textPool = new Queue<TextMeshProUGUI>();
        for (int i = 0; i < poolSize; i++)
        {
            TextMeshProUGUI textInstance = Instantiate(damageTextPrefab, textPoolTransform);
            textInstance.gameObject.SetActive(false);
            textPool.Enqueue(textInstance);
        }
    }
    public void SetPlayerTransform(Transform playerTrans)
    {
        this.playerTransform = playerTrans;
    }
    public void ShowDamageText(int damageAmount)
    {
        if(textPool == null || textPool.Count == 0)
        {
            InitializePool();
        }
        if (textPool.Count > 0)
        {
            currentTextInstance = textPool.Dequeue();
            currentTextInstance.text = damageAmount.ToString();
            if (simulationSpace == DamageTextSimulationSpace.WorldPos)
            {
                currentTextInstance.transform.position = playerTransform.position + offset;
            }
            else if (simulationSpace == DamageTextSimulationSpace.UI)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, playerTransform.position + offset);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPoint, uiCanvas.worldCamera, out Vector2 localPoint);
                currentTextInstance.transform.localPosition = localPoint;
            }
            currentTextInstance.gameObject.SetActive(true);

            PlayAnimation();
        }
    }

    public override Tween GetTweenAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(currentTextInstance.transform.DOScale(Vector3.one * 1.5f, animationDuration * 0.5f).SetEase(animationEase));

        if (simulationSpace == DamageTextSimulationSpace.WorldPos)
        {
            // Move up and fade out in world space
            sequence.Join(currentTextInstance.transform.DOMoveY(playerTransform.position.y + offset.y + 2, animationDuration).SetEase(animationEase));
        }
        else if (simulationSpace == DamageTextSimulationSpace.UI)
        {
            // Move up and fade out in UI space
            sequence.Join(currentTextInstance.transform.DOLocalMoveY(currentTextInstance.transform.localPosition.y + 50, animationDuration).SetEase(animationEase));
        }

        sequence.Join(currentTextInstance.DOFade(0, animationDuration).SetEase(animationEase));

        sequence.OnComplete(() => 
        {
            currentTextInstance.gameObject.SetActive(false);
            currentTextInstance.color = new Color(currentTextInstance.color.r, currentTextInstance.color.g, currentTextInstance.color.b, 1);
            currentTextInstance.transform.localScale = Vector3.one;
            textPool.Enqueue(currentTextInstance);
        });

        return sequence;
    }

    public override void SetupFirstState()
    {
        base.SetupFirstState();
        if (currentTextInstance != null)
        {
            currentTextInstance.transform.localScale = Vector3.one;
            currentTextInstance.color = new Color(currentTextInstance.color.r, currentTextInstance.color.g, currentTextInstance.color.b, 1);
        }
    }

    public override void SetupSleepState()
    {
        base.SetupSleepState();
        if (currentTextInstance != null)
        {
            currentTextInstance.gameObject.SetActive(false);
        }
    }
}
