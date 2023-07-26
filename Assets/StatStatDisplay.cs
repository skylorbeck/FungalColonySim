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
            float mushSpd = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomSpeed * .05f);
            float brownFarmSpd = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.growthSpeedBonus[0] * .1f);
            float redFarmSpd = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.growthSpeedBonus[1] * .1f);
            float blueFarmSpd = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.growthSpeedBonus[2] * .1f);
            float brownAutoHarvest = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvestSpeed[0] * .1f);
            float redAutoHarvest = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvestSpeed[1] * .1f);
            float blueAutoHarvest = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvestSpeed[2] * .1f);
            float collectibleBonus = (SaveSystem.instance.GetSaveFile().GetCollectionMultiplier() * 0.01f);

            //TODO move this calculation to the save file and use for offline progression
            globalGrowthTime.text = (5 / (mushSpd + brownFarmSpd + collectibleBonus + 1)).ToString("F2") + "s /" +
                                    (7 / (mushSpd + redFarmSpd + collectibleBonus + 1)).ToString("F2") + "s /" +
                                    (12 / (mushSpd + blueFarmSpd + collectibleBonus + 1)).ToString("F2") + "s";

            autoHarvestTime.text = (3 / (brownAutoHarvest + 1)).ToString("F1") + "s /" +
                                   (3 / (redAutoHarvest + 1)).ToString("F1") + "s /" +
                                   (3 / (blueAutoHarvest + 1)).ToString("F1") + "s";
        }
    }
}