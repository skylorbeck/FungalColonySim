using System;
using TMPro;
using UnityEngine;

public class OfflineProgressMenu : MonoBehaviour
{
    public TextMeshProUGUI totalTimeOfflineText;
    public TextMeshProUGUI[] totalMushroomsOfflineText;

    public void UpdateOfflineProgress()
    {
        totalMushroomsOfflineText[0].color = GameMaster.instance.mushroomColors[0];
        totalMushroomsOfflineText[1].color = GameMaster.instance.mushroomColors[1];
        totalMushroomsOfflineText[2].color = GameMaster.instance.mushroomColors[2];
        totalTimeOfflineText.text = TimeSpan.FromSeconds(SaveSystem.instance.offlineTime).Days + "d " +
                                    TimeSpan.FromSeconds(SaveSystem.instance.offlineTime).Hours + "h " +
                                    TimeSpan.FromSeconds(SaveSystem.instance.offlineTime).Minutes + "m " +
                                    TimeSpan.FromSeconds(SaveSystem.instance.offlineTime).Seconds + "s";
        totalMushroomsOfflineText[0].text = SaveSystem.instance.offlineMushrooms[0].ToString("N0") + " Brown";
        totalMushroomsOfflineText[1].text = SaveSystem.instance.offlineMushrooms[1].ToString("N0") + " Red";
        totalMushroomsOfflineText[2].text = SaveSystem.instance.offlineMushrooms[2].ToString("N0") + " Blue";
    }
}