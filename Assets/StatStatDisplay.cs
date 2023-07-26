using TMPro;
using UnityEngine;

public class StatStatDisplay : MonoBehaviour
{
    public TextMeshProUGUI globalGrowthTime;
    public TextMeshProUGUI autoHarvestTime;

    public void FixedUpdate()
    {
        if (GameMaster.instance.SettingsMenu.statsOpen)
        {
            globalGrowthTime.text = SaveSystem.GetBrownGrowthTimeActual().ToString("F2") + "s /" +
                                    SaveSystem.GetRedGrowthTimeActual().ToString("F2") + "s /" +
                                    SaveSystem.GetBlueGrowthTimeActual().ToString("F2") + "s";

            autoHarvestTime.text = SaveSystem.GetBrownAutoHarvestTimeActual().ToString("F2") + "s /" +
                                   SaveSystem.GetRedAutoHarvestTimeActual().ToString("F2") + "s /" +
                                   SaveSystem.GetBlueAutoHarvestTimeActual().ToString("F2") + "s";
        }
    }
}