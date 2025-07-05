using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializableGradient
{
    public SerializableColorKey[] colorKeys;
    public SerializableAlphaKey[] alphaKeys;
    public GradientMode mode;

    public SerializableGradient(Gradient gradient)
    {
        colorKeys = gradient.colorKeys.Select(k => new SerializableColorKey(k)).ToArray();
        alphaKeys = gradient.alphaKeys.Select(k => new SerializableAlphaKey(k)).ToArray();
        mode = gradient.mode;
    }

    public Gradient ToGradient()
    {
        Gradient gradient = new Gradient();
        gradient.colorKeys = colorKeys.Select(k => k.ToColorKey()).ToArray();
        gradient.alphaKeys = alphaKeys.Select(k => k.ToAlphaKey()).ToArray();
        gradient.mode = mode;
        return gradient;
    }
}

[System.Serializable]
public struct SerializableColorKey
{
    public float time;
    public string color; // Use string to avoid serialization issues

    public SerializableColorKey(GradientColorKey key)
    {
        time = key.time;
        color = JsonConvert.SerializeObject(new SerializableColor(key.color));
    }

    public GradientColorKey ToColorKey()
    {
        return new GradientColorKey(JsonConvert.DeserializeObject<SerializableColor>(color).ToColor(), time);
    }
}

[System.Serializable]
public struct SerializableAlphaKey
{
    public float time;
    public float alpha;

    public SerializableAlphaKey(GradientAlphaKey key)
    {
        time = key.time;
        alpha = key.alpha;
    }

    public GradientAlphaKey ToAlphaKey()
    {
        return new GradientAlphaKey(alpha, time);
    }
}
