using Newtonsoft.Json;
using System;
using UnityEngine;

public class GradientConverter : JsonConverter<Gradient>
{
    public override void WriteJson(JsonWriter writer, Gradient value, JsonSerializer serializer)
    {
        var serializableGradient = new SerializableGradient(value);
        serializer.Serialize(writer, serializableGradient);
    }

    public override Gradient ReadJson(JsonReader reader, Type objectType, Gradient existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var serializableGradient = serializer.Deserialize<SerializableGradient>(reader);
        return serializableGradient.ToGradient();
    }
}
