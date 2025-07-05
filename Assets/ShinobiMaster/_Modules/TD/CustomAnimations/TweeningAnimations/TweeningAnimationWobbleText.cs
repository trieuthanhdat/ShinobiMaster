using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class TweeningAnimationWobbleText : TweeningAnimation
{
    public enum WobbleCalculationMethod
    {
        randomInsideUnitSphere,
        Sin,
        Cos,
        Tan,
        WaterWave
    }

    public enum WobbleTextType
    {
        ZoomInOut,
        Waving
    }

    [SerializeField] private bool playOnEnable = false;
    [SerializeField] private int loopTime = -1; // infinite
    [SerializeField] private float wobbleStrength = 10f;
    [SerializeField] private LoopType loopType = LoopType.Yoyo;
    [SerializeField] private WobbleTextType wobbleTextType = WobbleTextType.Waving;
    [SerializeField] private WobbleCalculationMethod wobbleMethod = WobbleCalculationMethod.randomInsideUnitSphere;

    private TMP_Text textMesh;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] originalVertices;

    public TweeningAnimationWobbleText(
        int loopTime,
        LoopType loopType,
        float strength,
        WobbleTextType wobbleTextType,
        WobbleCalculationMethod wobbleMethod,
        float duration,
        Ease ease,
        TweeningAnimationType type
    ) : base(duration, ease, type)
    {
        this.loopTime = loopTime;
        this.loopType = loopType;
        this.wobbleStrength = strength;
        this.wobbleMethod = wobbleMethod;
        this.wobbleTextType = wobbleTextType;
    }

    public bool IsValid()
    {
        if (textMesh == null) return false;
        return originalVertices != null && vertices != null;
    }

    public override void Awake()
    {
        base.Awake();
        textMesh = GetComponent<TMP_Text>();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (textMesh != null)
        {
            textMesh.ForceMeshUpdate();
            mesh = textMesh.mesh;
            vertices = mesh.vertices;
            originalVertices = mesh.vertices.Clone() as Vector3[];

            if (originalVertices == null)
            {
                Debug.LogError("Failed to clone mesh vertices.");
                return;
            }

            // Start the wobble effect if playOnEnable is true
            if (playOnEnable) PlayAnimation();
        }
    }

    public override Tween GetTweenAnimation()
    {
        if (!IsValid())
        {
            Debug.LogError("Mesh or vertices are not properly initialized.");
            return null;
        }

        Sequence sequence = DOTween.Sequence();

        switch (wobbleTextType)
        {
            case WobbleTextType.ZoomInOut:
                for (int i = 0; i < vertices.Length; i++)
                {
                    int index = i;
                    Vector3 originalPos = originalVertices[index];
                    float delay = UnityEngine.Random.Range(0f, tweenDelay);
                    float duration = tweenDuration;
                    Vector3 wobblePos = GetWooblePosition(originalPos);

                    sequence.Insert(delay, DOTween.To(() => vertices[index], x => vertices[index] = x, wobblePos * wobbleStrength, duration)
                                .OnStart(ForceUpdateMesh)
                                .SetEase(easeType)
                                .SetLoops(loopTime, loopType)
                                .OnUpdate(UpdateMesh));
                }
                break;

            case WobbleTextType.Waving:
                textMesh.ForceMeshUpdate();
                var textInfo = textMesh.textInfo;
                for (int i = 0; i < textInfo.characterCount; ++i)
                {
                    var charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                    for (int j = 0; j < 4; ++j)
                    {
                        int vertexIndex = charInfo.vertexIndex + j;
                        Vector3 originalPos = verts[vertexIndex];
                        Vector3 wobblePos = GetWooblePosition(originalPos);

                        sequence.Insert(0, DOTween.Sequence()
                            .Append(DOTween.To(() => verts[vertexIndex],
                                               x => verts[vertexIndex] = x,
                                               originalPos + wobblePos * wobbleStrength,
                                               tweenDuration / 2))
                            .AppendInterval(tweenDelay)
                            .Append(DOTween.To(() => verts[vertexIndex],
                                               x => verts[vertexIndex] = x,
                                               originalPos,
                                               tweenDuration / 2))
                            .SetEase(easeType)
                            .OnUpdate(() => UpdateTextMesh(verts, charInfo.materialReferenceIndex))); ;
                    }
                }
                sequence.OnComplete(UpdateGeometry);
                break;

        }

        m_TweenAnimation = sequence;
        RegisterOnStartAndOnCompleteCallbacks();

        return sequence;
    }

    public override void PlayAnimation()
    {
        GetTweenAnimation()?.Play();
    }

    private Vector3 GetWooblePosition(Vector3 originalPos)
    {
        switch (wobbleMethod)
        {
            case WobbleCalculationMethod.randomInsideUnitSphere:
                return originalPos + UnityEngine.Random.insideUnitSphere;
            case WobbleCalculationMethod.Sin:
                return Vector3.zero + new Vector3(0, Mathf.Sin(2f + originalPos.x * 0.01f), 0);
            case WobbleCalculationMethod.Cos:
                return Vector3.zero + new Vector3(Mathf.Cos(2f + originalPos.y * 0.01f), 0, 0);
            case WobbleCalculationMethod.Tan:
                return Vector3.zero + originalPos + new Vector3(0, 0, Mathf.Sin(2f + originalPos.y * 0.01f));
            case WobbleCalculationMethod.WaterWave: // New case for water wave effect
                float waveFrequency = 2f;
                float waveAmplitude = 0.5f;
                return originalPos + new Vector3(
                    Mathf.Sin(waveFrequency + originalPos.y * 0.1f) * waveAmplitude,
                    Mathf.Cos(waveFrequency + originalPos.x * 0.1f) * waveAmplitude,
                    0
                );
        }
        return Vector3.zero;
    }

    private void UpdateTextMesh(Vector3[] verts, int materialReferenceIndex)
    {
        var textInfo = textMesh.textInfo;
        textInfo.meshInfo[materialReferenceIndex].mesh.vertices = verts;
        textMesh.UpdateGeometry(textInfo.meshInfo[materialReferenceIndex].mesh, materialReferenceIndex);
    }

    private void UpdateGeometry()
    {
        var textInfo = textMesh.textInfo;
        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    private void ForceUpdateMesh()
    {
        if (textMesh)
        {
            textMesh.ForceMeshUpdate();
        }
    }

    private void UpdateMesh()
    {
        if (!IsValid())
        {
            Debug.LogError("Mesh or vertices are not properly initialized.");
            return;
        }
        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }
}
