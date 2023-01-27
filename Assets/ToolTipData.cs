using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipHeader;
    [TextArea]
    public string tooltipContent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameMaster.instance.tooltip.ShowToolTip(tooltipHeader, tooltipContent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameMaster.instance.tooltip.HideToolTip();
    }
}