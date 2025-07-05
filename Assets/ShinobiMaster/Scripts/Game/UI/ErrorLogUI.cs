using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorLogUI : MonoBehaviour
{
    public static ErrorLogUI Singleton { get; private set; }

    [SerializeField] private Text text;

    private List<string> errors = new List<string>();

    private readonly string textName = "Error log: ";

    private void Awake()
    {
        if (Singleton)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }
    }

    public static string ConvertExceptionToString(Exception e)
    {
        return e.Data + " " + e.Message + '\n' + e.Source + '\n' + e.HelpLink + '\n' + e.StackTrace;
    }

    public void ReportError(string Log)
    {
        errors.Add(Log);
        text.text = textName + '\n';

        for (int i = 0; i < errors.Count; i++)
        {
            text.text += errors[i] + '\n';
        }
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }
}
