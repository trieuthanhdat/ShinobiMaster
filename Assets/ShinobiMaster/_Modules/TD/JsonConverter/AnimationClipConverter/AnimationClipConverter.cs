using UnityEngine;
using Newtonsoft.Json;
using System;

public class AnimationClipConverter : JsonConverter<AnimationClip>
{
    public override void WriteJson(JsonWriter writer, AnimationClip value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteValue(value ? value.name : "None");

#if UNITY_EDITOR
        string assetPath = value ? UnityEditor.AssetDatabase.GetAssetPath(value) : "";
        writer.WritePropertyName("path");
        writer.WriteValue(assetPath);
#endif

        writer.WriteEndObject();
    }

    public override AnimationClip ReadJson(JsonReader reader, Type objectType, AnimationClip existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
#if UNITY_EDITOR
        string name = "";
        string path = "";

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) break;

            string propertyName = reader.Value.ToString();
            reader.Read();

            switch (propertyName)
            {
                case "name":
                    name = reader.Value.ToString();
                    break;
                case "path":
                    path = reader.Value.ToString();
                    break;
            }
        }

        if (!string.IsNullOrEmpty(path))
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        }
#endif

        return null;
    }
}
