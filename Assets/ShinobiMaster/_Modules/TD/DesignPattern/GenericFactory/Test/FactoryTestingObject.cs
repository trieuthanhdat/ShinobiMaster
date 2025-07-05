using System.Collections;
using System.Collections.Generic;
using TD.DesignPattern.GenericBuilder.Validatable;
using TD.DesignPattern.GenericFactory;
using UnityEngine;

public class FactoryTestingObject : MonoBehaviour, IInitializable<TestingData>, IValidatable
{
    [SerializeField] private TestingData _data;
    public TestingData Data
    {
        get => _data;
        set => _data = value;
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        Initialize(_data);
    }
#endif

    public void Initialize(TestingData data)
    {
        _data = data as TestingData;
        if (_data == null)
        {
            Debug.LogError("Data is not of type TestingData!");
        }
    }

    public ValidationResult Validate()
    {
        var validator = new ValidationResult();
        if (_data == null)
            validator.AddError("Data should not be null!");
        else
        {
            if (string.IsNullOrEmpty(_data.id))
                validator.AddError("Data's id should not be empty!");
            if (string.IsNullOrEmpty(_data.name))
                validator.AddError("Data's name should not be empty!");
        }
        return validator;
    }
}
[System.Serializable]
public class TestingData
{
    public string name;
    public string id;
}