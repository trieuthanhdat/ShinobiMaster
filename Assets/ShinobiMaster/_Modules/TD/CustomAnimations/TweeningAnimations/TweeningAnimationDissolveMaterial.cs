using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TweeningAnimationDissolveMaterials : TweeningAnimation
{
    [Header("Dissolve Material Setups")]
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private string dissolvePropertyName = "_DissolveOffset"; // Property name used in the shader for the dissolve effect
    [SerializeField] private float startDissolveValue = 0f;
    [SerializeField] private float endDissolveValue = 1f;
    public bool EnableEditor_OnValidate = true;

    [Header("===DEBUG===")]
    [SerializeField] private List<Material> materials = new List<Material>();
    private bool _isValidated = false;

    // Constructor to initialize the dissolve animation with specific parameters
    public TweeningAnimationDissolveMaterials(float duration, Ease ease, TweeningAnimationType type, Renderer[] renderers, string dissolvePropertyName, float startValue, float endValue)
        : base(duration, ease, type)
    {
        this.renderers = renderers;
        this.dissolvePropertyName = dissolvePropertyName;
        this.startDissolveValue = startValue;
        this.endDissolveValue = endValue;

        OnInit();
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        if (!EnableEditor_OnValidate || Application.isPlaying) return;
        CacheMaterials();
        _isValidated = true;
    }
#endif

    public override void OnInit()
    {
        base.OnInit();
        if (renderers == null)
        {
            renderers = GetComponents<Renderer>();
        }
        if (_isValidated == false)
            CacheMaterials();
    }

    private void CacheMaterials()
    {
        materials.Clear();

        foreach (var renderer in renderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                newMaterials[i] = new Material(renderer.materials[i]);

                materials.Add(newMaterials[i]);
            }
            renderer.materials = newMaterials;
        }
    }


    public override void SetupFirstState()
    {
        SetDissolveValue(startDissolveValue);
        base.SetupFirstState();
    }

    public override void SetupSleepState()
    {
        SetDissolveValue(endDissolveValue);
        base.SetupSleepState();
    }

    public override Tween GetTweenAnimation()
    {
        Sequence dissolveSequence = DOTween.Sequence();

        foreach (var material in materials)
        {
            Vector4 matvector = material.GetVector(dissolvePropertyName);
            Tween dissolveTween = DOTween.To
            (
                () => material.GetVector(dissolvePropertyName).y,
                value => material.SetVector(dissolvePropertyName, new Vector4(matvector.x, value, matvector.z, matvector.w)), // Setter: update the dissolve value (y component)
                endDissolveValue,
                tweenDuration
            )
            .SetEase(easeType)
            .SetDelay(tweenDelay);

            dissolveSequence.Join(dissolveTween);
        }

        m_TweenAnimation = dissolveSequence;
        RegisterOnStartAndOnCompleteCallbacks();

        return m_TweenAnimation;
    }

    private void SetDissolveValue(float value)
    {
        foreach (var material in materials)
        {
            material.SetVector(dissolvePropertyName, new Vector4(0f, value, 0f, 0f));
        }
    }
}
