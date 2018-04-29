using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HelpTextData : ScriptableObject
{
    [System.Serializable]
    public class HelpText
    {
        public string Name;
        public string[] Lines;
    }

    public HelpText[] helpTexts;
    Dictionary<string, string[]> lines = new Dictionary<string, string[]>();

    public void Init()
    {
        lines.Clear();
        for (int i = 0; i < helpTexts.Length; i++)
            lines.Add(helpTexts[i].Name, helpTexts[i].Lines);
    }

    public string[] GetHelpText(string name)
    {
        return lines[name];
    }
}