using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipData : MonoBehaviour
{
    public string[] toolTipText;

    public string GetToolTipText()
    {
        string concat = "";
        foreach (var text in toolTipText)
        {
            concat += text + "\n";
        }

        return concat;
    }
}