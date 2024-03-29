using UnityEngine;

public class UpgradeMaster : MonoBehaviour
{
    [Header("Auto Harvest")] [SerializeField]
    private UpgradeContainer autoHarvestButton;

    [SerializeField] private uint autoHarvestCost = 5;
    [SerializeField] private uint autoHarvestCostDefault = 50;
    [SerializeField] private bool autoHarvestVisible = false;

    [Header("Auto Harvest Speed")] [SerializeField]
    private UpgradeContainer autoHarvestSpeedButton;

    [SerializeField] private uint autoHarvestSpeedCost = 50;
    [SerializeField] private uint autoHarvestSpeedCostDefault = 50;
    [SerializeField] private uint autoHarvestSpeedRatio = 2;
    [SerializeField] private bool autoHarvestSpeedVisible = false;

    [Header("Growth Speed")] [SerializeField]
    private UpgradeContainer growthSpeedButton;

    [SerializeField] private uint growthSpeedCost = 5;
    [SerializeField] private uint growthSpeedCostDefault = 5;
    [SerializeField] private uint growthSpeedRatio = 3;
    [SerializeField] private bool growthSpeedVisible = false;

    [Header("Enrich")] [SerializeField] private UpgradeContainer enrichButton;

    public MushroomBlock.MushroomType mushroomType;

    public void Awake()
    {
    }

    public void Reset()
    {
        SaveSystem.save.farmSave.upgrades.autoHarvest = new bool[3];
        SaveSystem.save.farmSave.upgrades.growthSpeedBonus = new uint[3];
        SaveSystem.save.farmSave.upgrades.autoHarvestSpeed = new uint[3];
        autoHarvestVisible = false;
        growthSpeedVisible = false;
        autoHarvestSpeedVisible = false;
        autoHarvestCost = autoHarvestCostDefault;
        growthSpeedCost = growthSpeedCostDefault;
        autoHarvestSpeedCost = autoHarvestSpeedCostDefault;
        UpdateButtons();
    }

    void Start()
    {
        autoHarvestButton.SetIcon(MushroomBlock.GetMushroomSprite(mushroomType));
        autoHarvestSpeedButton.SetIcon(MushroomBlock.GetMushroomSprite(mushroomType));
        growthSpeedButton.SetIcon(MushroomBlock.GetMushroomSprite(mushroomType));
        enrichButton.SetIcon(MushroomBlock.GetMushroomSprite(mushroomType));
        UpdateButtons();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (!autoHarvestVisible)
        {
            autoHarvestVisible = SaveSystem.save.stats.mushrooms[(int)mushroomType] >= autoHarvestCost;
            autoHarvestButton.gameObject.SetActive(autoHarvestVisible &&
                                                   !SaveSystem.save.farmSave.upgrades.autoHarvest[(int)mushroomType]);
        }

        if (!growthSpeedVisible && SaveSystem.save.farmSave.upgrades.growthSpeedBonus[(int)mushroomType] < 20)
        {
            growthSpeedVisible = SaveSystem.save.stats.mushrooms[(int)mushroomType] >= growthSpeedCost ||
                                 SaveSystem.save.farmSave.upgrades.growthSpeedBonus[(int)mushroomType] is > 0 and < 20;
            growthSpeedButton.gameObject.SetActive(growthSpeedVisible);
        }

        if (!autoHarvestSpeedVisible && SaveSystem.save.farmSave.upgrades.autoHarvestSpeed[(int)mushroomType] < 20)
        {
            autoHarvestSpeedVisible = SaveSystem.save.stats.mushrooms[(int)mushroomType] >= autoHarvestSpeedCost ||
                                      SaveSystem.save.farmSave.upgrades.autoHarvestSpeed[(int)mushroomType] is > 0
                                          and < 20;
            autoHarvestSpeedButton.gameObject.SetActive(autoHarvestSpeedVisible);
        }
    }


    public void PurchaseAutoHarvest()
    {
        if (ScoreMaster.instance.SpendMushrooms(mushroomType, autoHarvestCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.farmSave.upgrades.autoHarvest[(int)mushroomType] = true;
            autoHarvestButton.ToggleButton(false);
            autoHarvestButton.gameObject.SetActive(false);
        }
    }

    public void PurchaseAutoHarvestSpeed()
    {
        autoHarvestSpeedCost =
            (uint)Mathf.Max(
                50 + Mathf.Pow(autoHarvestSpeedRatio,
                    SaveSystem.save.farmSave.upgrades.autoHarvestSpeed[(int)mushroomType]), autoHarvestSpeedCost);
        if (ScoreMaster.instance.SpendMushrooms(mushroomType, autoHarvestSpeedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.farmSave.upgrades.autoHarvestSpeed[(int)mushroomType]++;
            UpdateAutoHarvestSpeedButton();
        }
    }

    public void PurchaseGrowthSpeed()
    {
        growthSpeedCost =
            (uint)Mathf.Max(
                Mathf.Pow(growthSpeedRatio, SaveSystem.save.farmSave.upgrades.growthSpeedBonus[(int)mushroomType]),
                growthSpeedCost);
        if (ScoreMaster.instance.SpendMushrooms(mushroomType, growthSpeedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.farmSave.upgrades.growthSpeedBonus[(int)mushroomType]++;
            UpdateGrowthSpeedButton();
        }
    }

    public void UpdateButtons()
    {
        UpdateAutoHarvestButton();
        UpdateGrowthSpeedButton();
        UpdateAutoHarvestSpeedButton();
        UpdateEnrichButton();
    }

    private void UpdateEnrichButton()
    {
        uint cost = (uint)Mathf.Pow(SaveSystem.save.farmSave.mushroomBlockCount[(int)mushroomType], 2);
        enrichButton.SetCostText(cost.ToString("N0"));
        enrichButton.ToggleButton(SaveSystem.save.stats.mushrooms[(int)mushroomType] >= cost);
    }

    private void UpdateAutoHarvestButton()
    {
        autoHarvestButton.ToggleButton(
            SaveSystem.save.stats.mushrooms[(int)mushroomType] >= autoHarvestCost);
        autoHarvestButton.SetCostText(autoHarvestCost.ToString("N0"));
    }

    private void UpdateGrowthSpeedButton()
    {
        growthSpeedCost =
            (uint)Mathf.Max(
                Mathf.Pow(growthSpeedRatio, SaveSystem.save.farmSave.upgrades.growthSpeedBonus[(int)mushroomType]),
                growthSpeedCost);
        growthSpeedButton.ToggleButton(SaveSystem.save.stats.mushrooms[(int)mushroomType] >= growthSpeedCost);
        growthSpeedButton.SetCostText(growthSpeedCost.ToString("N0"));
        if (SaveSystem.save.farmSave.upgrades.growthSpeedBonus[(int)mushroomType] >= 20)
        {
            growthSpeedButton.gameObject.SetActive(false);
        }
    }

    public void UpdateAutoHarvestSpeedButton()
    {
        autoHarvestSpeedCost =
            (uint)Mathf.Max(
                Mathf.Pow(autoHarvestSpeedRatio, SaveSystem.save.farmSave.upgrades.autoHarvestSpeed[(int)mushroomType]),
                autoHarvestSpeedCost);
        autoHarvestSpeedButton.ToggleButton(SaveSystem.save.stats.mushrooms[(int)mushroomType] >= autoHarvestSpeedCost);
        autoHarvestSpeedButton.SetCostText(autoHarvestSpeedCost.ToString("N0"));
        if (SaveSystem.save.farmSave.upgrades.autoHarvestSpeed[(int)mushroomType] >= 20)
        {
            autoHarvestSpeedButton.gameObject.SetActive(false);
        }
    }
}