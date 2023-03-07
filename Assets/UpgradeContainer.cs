using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeContainer : MonoBehaviour
{
    public Button upgradeButton;
    public Image upgradeIcon;
    public TextMeshProUGUI costText;

    public void ToggleButton(bool toggle)
    {
        upgradeButton.interactable = toggle;
    }

    public void SetCostText(string text)
    {
        costText.text = text;
    }

    public void SetIcon(Sprite sprite)
    {
        upgradeIcon.sprite = sprite;
    }
}
