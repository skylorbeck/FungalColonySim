using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConvergenceMaster : MonoBehaviour
{
    public bool unlocked = false;
    [SerializeField] private GameObject convergenceMenu;
    [SerializeField] private GameObject convergenceStoreMenu;

    [SerializeField] private Image rayBlocker;

    [Header("convergence store")]
    public bool inStore = false;
    [SerializeField] private TextMeshProUGUI totalSporeText;
    [SerializeField] private TextMeshProUGUI spendableSporeText;
    
    [SerializeField] private Button farmXButton;
    [SerializeField] private TextMeshProUGUI farmXText;
    [SerializeField] private uint farmXCost = 100;
    [SerializeField] private TextMeshProUGUI farmXCostText;
    
    [SerializeField] private Button farmYButton;
    [SerializeField] private TextMeshProUGUI farmYText;
    [SerializeField] private uint farmYCost = 100;
    [SerializeField] private TextMeshProUGUI farmYCostText;
    
    [SerializeField] private Button mushroomMultiButton;
    [SerializeField] private TextMeshProUGUI mushroomMultiText;
    [SerializeField] private uint mushroomMultiCost = 100;
    [SerializeField] private TextMeshProUGUI mushroomMultiCostText;
    
    [SerializeField] private Button mushroomSpeedButton;
    [SerializeField] private TextMeshProUGUI mushroomSpeedText;
    [SerializeField] private uint mushroomSpeedCost = 100;
    [SerializeField] private TextMeshProUGUI mushroomSpeedCostText;
    
    [SerializeField] private TextMeshProUGUI convergencePointCostText;
    [SerializeField] private TextMeshProUGUI convergenceSkillPointCostText;

    [Header("convergence menu")]
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI skillRewardText;
    [SerializeField] private Button convergeButton;
    [SerializeField] private CurrencyVisualizer sporeVisualizer;
    [SerializeField] private Button confirmConvergeButton;
    [SerializeField] private Button exitStoreButton;
    [SerializeField] private Toggle agreeToggle;
    
    public uint reward= 0;
    public uint skillReward= 0;
    
    public float convergeWarningTimer = 0;
    public uint convergeWarningTimerMax = 5;

    IEnumerator Start()
    {
        convergenceStoreMenu.SetActive(false);
        convergenceMenu.SetActive(false);
        confirmConvergeButton.interactable = false;
        yield return new WaitUntil(() => GameMaster.instance.brownBlockMaster.isWorldCreated);
        yield return new WaitUntil(() => GameMaster.instance.redBlockMaster.isWorldCreated);
        yield return new WaitUntil(() => GameMaster.instance.blueBlockMaster.isWorldCreated);
        yield return new WaitUntil(() => SaveSystem.instance != null);
        yield return new WaitUntil(() => SaveSystem.instance.loaded);
        convergeButton.gameObject.SetActive(unlocked=(SaveSystem.instance.GetSaveFile().farmSave.mushroomBlockCount[0] >=9 || SaveSystem.instance.GetSaveFile().statsTotal.converges >= 1));
        convergeButton.interactable = unlocked;
    }

    void Update()
    {
        
        
    }

    public void PurchaseFarmX()
    {
        SFXMaster.instance.PlayMenuClick();
        if (SaveSystem.instance.SpendSpores(farmXCost))
        {
            SaveSystem.instance.GetSaveFile().farmSave.farmSize.x += 1;
            UpdateUpgradeText();
        }
        
    }
    
    public void PurchaseFarmY()
    {
        SFXMaster.instance.PlayMenuClick();
        if (SaveSystem.instance.SpendSpores(farmYCost))
        {
            SaveSystem.instance.GetSaveFile().farmSave.farmSize.y += 1;
            UpdateUpgradeText();
        }
        
    }
    
    public void PurchaseMushroomMulti()
    {
        SFXMaster.instance.PlayMenuClick();
        if (SaveSystem.instance.SpendSpores(mushroomMultiCost))
        {
            SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomMultiplier += 1;
            UpdateUpgradeText();
        }
    }

    public void PurchaseMushroomSpeed()
    {
        SFXMaster.instance.PlayMenuClick();
        if (SaveSystem.instance.SpendSpores(mushroomSpeedCost))
        {
            SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomSpeed += 1;
            UpdateUpgradeText();
        }
        
    }
    
    public void UpdateUpgradeText()
    {
        farmXCost = (uint)Mathf.RoundToInt(5 * Mathf.Pow(1.1f, SaveSystem.instance.GetSaveFile().farmSave.farmSize.x));
        farmYCost = (uint)Mathf.RoundToInt(5 * Mathf.Pow(1.1f, SaveSystem.instance.GetSaveFile().farmSave.farmSize.y));
        farmXCostText.text = farmXCost.ToString();
        farmYCostText.text = farmYCost.ToString();
        
        farmXText.text = (3 + SaveSystem.instance.GetSaveFile().farmSave.farmSize.x).ToString();
        farmYText.text = (3 + SaveSystem.instance.GetSaveFile().farmSave.farmSize.y).ToString();
        farmXButton.interactable = SaveSystem.instance.GetSaveFile().stats.spores >= farmXCost;
        farmYButton.interactable = SaveSystem.instance.GetSaveFile().stats.spores >= farmYCost;
        
        mushroomMultiCost = (uint)Mathf.RoundToInt(10 * Mathf.Pow(1.2f, SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomMultiplier));
        mushroomMultiCostText.text = mushroomMultiCost.ToString();
        mushroomMultiText.text = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomMultiplier + 1).ToString();
        mushroomMultiButton.interactable = SaveSystem.instance.GetSaveFile().stats.spores >= mushroomMultiCost;
        
        mushroomSpeedCost = (uint)Mathf.RoundToInt(2 * Mathf.Pow(1.2f, SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomSpeed));
        mushroomSpeedCostText.text = mushroomSpeedCost.ToString();
        mushroomSpeedText.text = (SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomSpeed).ToString();
        mushroomSpeedButton.interactable = SaveSystem.instance.GetSaveFile().stats.spores >= mushroomSpeedCost;
    }

    public void Agree()
    {
        confirmConvergeButton.interactable = agreeToggle.isOn;
        SFXMaster.instance.PlayMenuClick();
    }
    
    public void ShowMenu()
    {
        SFXMaster.instance.PlayMenuClick();
        rayBlocker.gameObject.SetActive(true);
        rayBlocker.DOFade(0.5f, 0.5f);
        convergenceMenu.SetActive(true);
        convergenceMenu.transform.DOLocalMoveX(0, 0.5f);
    }

    public void HideMenuButton()
    {
        rayBlocker.DOFade(0, 0.5f).onComplete += () => rayBlocker.gameObject.SetActive(false);
        StartCoroutine(HideMenu());
    }
    
    public IEnumerator HideMenu()
    {
        SFXMaster.instance.PlayMenuClick();
        float duration = 0.5f;
        convergenceMenu.transform.DOLocalMoveX(-2000, duration).onComplete += () =>
        {
            convergenceMenu.SetActive(false);
        };
        yield return new WaitForSeconds(duration);
    }

    public void StartConverge()
    {
        StartCoroutine(Converge());
    }
    
    public IEnumerator Converge()
    {
        // SFXMaster.instance.PlayMenuClick();
        SaveSystem.instance.GetSaveFile().stats.spores += reward;
        SaveSystem.instance.GetSaveFile().statsTotal.spores += reward;
        SaveSystem.instance.GetSaveFile().stats.skillPoints += skillReward;
        SaveSystem.instance.GetSaveFile().statsTotal.skillPoints += skillReward;

        agreeToggle.isOn = false;
        confirmConvergeButton.interactable = false;
        yield return StartCoroutine(HideMenu());
        ShowStoreMenu();
        if (GameMaster.instance.ModeMaster.currentMode!=ModeMaster.Gamemode.BrownFarm)
        {
            GameMaster.instance.ModeMaster.SetMode(ModeMaster.Gamemode.BrownFarm);
        }

    }

    public void ShowStoreMenu()
    {
        ScoreMaster.instance.Reset();
        GameMaster.instance.brownUpgradeMaster.Reset();
        GameMaster.instance.redUpgradeMaster.Reset();
        GameMaster.instance.blueUpgradeMaster.Reset();
        convergeButton.interactable = false;
        convergenceStoreMenu.SetActive(true);
        inStore = true;
        convergenceStoreMenu.transform.DOLocalMoveX(0, 0.5f);
        exitStoreButton.interactable = true;
    }

    public void CloseStoreButton()
    {
        StartCoroutine(CloseStoreMenu());
    }
    
    public IEnumerator CloseStoreMenu()
    {
        inStore = false;
        exitStoreButton.interactable = false;
        float duration = 0.5f;
        rayBlocker.DOFade(0, duration).onComplete += () => rayBlocker.gameObject.SetActive(false);
        convergenceStoreMenu.transform.DOLocalMoveX(-2000, duration).onComplete += () =>
        {
            convergenceStoreMenu.SetActive(false);
        };
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(GameMaster.instance.Prestige());
        convergeButton.interactable = true;

    }

    void FixedUpdate()
    {
        CalculateSporeReward();
        CalculateHivemindPointsReward();

        totalSporeText.text = "+" + reward +" : +"+ skillReward;

        if (reward>5)
        {
            convergeWarningTimer += Time.fixedDeltaTime;
        }
        if (convergeWarningTimer > convergeWarningTimerMax)
        {
            convergeWarningTimer = 0;
            totalSporeText.transform.DOPunchScale(Vector3.one*0.5f, 1f, 1, 0.25f);
        }

        spendableSporeText.text = SaveSystem.instance.GetSaveFile().stats.spores + " Spores";
        rewardText.text = "+" + reward;
        skillRewardText.text = "+" + skillReward;
        if (!unlocked)
        {
            unlocked = (SaveSystem.instance.GetSaveFile().farmSave.mushroomBlockCount[0] >=9 || SaveSystem.instance.GetSaveFile().statsTotal.converges >= 1) && GameMaster.instance.brownBlockMaster.isWorldCreated && GameMaster.instance.redBlockMaster.isWorldCreated && GameMaster.instance.blueBlockMaster.isWorldCreated;
            convergeButton.gameObject.SetActive(unlocked);
            convergeButton.interactable = unlocked ;
            sporeVisualizer.gameObject.SetActive(unlocked);
            return;
        }
        if (inStore)
        {
            UpdateUpgradeText();
            return;
        }
    }
    
    

    private void CalculateSporeReward()
    {
        double sporeCost = Math.Pow(1.1, SaveSystem.instance.GetSaveFile().statsTotal.spores );
        float brownMultiplier = 1 + (SaveSystem.instance.GetSaveFile().farmSave.upgrades.brownMultiplier * GameMaster.instance.Marketplace.brownValueMultiplierGain);
        uint brownValue = (uint)Mathf.Pow(SaveSystem.instance.GetSaveFile().stats.mushrooms[(int)MushroomBlock.MushroomType.Brown]*brownMultiplier, 1.1f);

        float redMultiplier = 1 + (SaveSystem.instance.GetSaveFile().farmSave.upgrades.redMultiplier * GameMaster.instance.Marketplace.redValueMultiplierGain);//
        uint redValue = (uint)Mathf.Pow(SaveSystem.instance.GetSaveFile().stats.mushrooms[(int)MushroomBlock.MushroomType.Red]*redMultiplier, 1.3f);

        float blueMultiplier = 1 + (SaveSystem.instance.GetSaveFile().farmSave.upgrades.blueMultiplier * GameMaster.instance.Marketplace.blueValueMultiplierGain);
        uint blueValue = (uint)Mathf.Pow(SaveSystem.instance.GetSaveFile().stats.mushrooms[(int)MushroomBlock.MushroomType.Blue]*blueMultiplier, 1.5f);
        
        convergencePointCostText.text = "(" +brownValue + "+" + redValue + "+" + blueValue + ")\n" + sporeCost.ToString("0.00");

        // uint rewardSqrt = (uint)(Mathf.Sqrt((float)(mushValue/sporeCost)));
        int rewardLogAsInt = Mathf.RoundToInt(Mathf.Log((float)((brownValue+redValue+blueValue) / sporeCost)));
        uint rewardLog = (uint)Mathf.Max(0, rewardLogAsInt);
        reward = rewardLog;
    }

    private void CalculateHivemindPointsReward()
    {
        double pointCost = Math.Pow(1.1, SaveSystem.instance.GetSaveFile().statsTotal.skillPoints);
        float pointMultiplier = SaveSystem.instance.GetSaveFile().farmSave.upgrades.sporeMultiplier;
        uint pointValue = (uint)Mathf.Pow(reward*pointMultiplier, 1.5f);
        convergenceSkillPointCostText.text = pointValue.ToString("0.00")+"\n"+pointCost.ToString("0.00");
        // uint rewardSqrt = (uint)(Mathf.Sqrt((float)(pointValue/sporeCost)));
        int rewardLogAsInt = Mathf.RoundToInt(Mathf.Log((float)(pointValue / pointCost)));
        uint rewardLog = (uint)Mathf.Max(0, rewardLogAsInt);
        skillReward = rewardLog;
    }
}
