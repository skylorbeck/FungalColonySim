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
            float mushSpd = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomSpeed * 5);
            float brownFarmSpd = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.growthSpeedBonus[0]);
            float redFarmSpd = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.growthSpeedBonus[1]);
            float blueFarmSpd = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.growthSpeedBonus[2]);
            float brownAutoHarvest = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvestSpeed[0]);
            float redAutoHarvest = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvestSpeed[1]);
            float blueAutoHarvest = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvestSpeed[2]);
            float collectibleBonus = (SaveSystem.instance.GetSaveFile().GetCollectionMultiplier());

            globalGrowthBonus.text =
                (mushSpd + collectibleBonus).ToString("F1") + "% (" +
                mushSpd.ToString("F1") + "% + " +
                collectibleBonus.ToString("F1") + "%)";
        }
    }
}