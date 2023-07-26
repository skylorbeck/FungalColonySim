using TMPro;
using UnityEngine;

public class BonusStatDisplay : MonoBehaviour
{
    public TextMeshProUGUI globalGrowthBonus;
    public TextMeshProUGUI farmGrowthBonus;
    public TextMeshProUGUI autoHarvestBonus;

    public void FixedUpdate()
    {
        if (GameMaster.instance.SettingsMenu.statsOpen)
        {
            float mushSpd = (SaveSystem.GetMushroomSpeedAsPercent() * 100);
            float brownFarmSpd = (SaveSystem.GetBrownFarmSpeedAsPercent() * 100);
            float redFarmSpd = (SaveSystem.GetRedFarmSpeedAsPercent() * 100);
            float blueFarmSpd = (SaveSystem.GetBlueFarmSpeedAsPercent() * 100);
            float brownAutoHarvest = (SaveSystem.GetBrownAutoHarvestBonusAsPercent() * 100);
            float redAutoHarvest = (SaveSystem.GetRedAutoHarvestBonusAsPercent() * 100);
            float blueAutoHarvest = (SaveSystem.GetBlueAutoHarvestBonusAsPercent() * 100);
            float collectibleBonus = (SaveSystem.GetCollectionMultiplier());

            globalGrowthBonus.text =
                (mushSpd + collectibleBonus).ToString("F1") + "% (" +
                mushSpd.ToString("F1") + "% + " +
                collectibleBonus.ToString("F1") + "%)";

            farmGrowthBonus.text = brownFarmSpd.ToString("F1") + "% / " +
                                   redFarmSpd.ToString("F1") + "% / " +
                                   blueFarmSpd.ToString("F1") + "%";

            autoHarvestBonus.text = brownAutoHarvest.ToString("F1") + "% / " +
                                    redAutoHarvest.ToString("F1") + "% / " +
                                    blueAutoHarvest.ToString("F1") + "%";
        }
    }
}