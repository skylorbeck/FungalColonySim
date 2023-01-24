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
    private bool inStore = false;
    [SerializeField] private TextMeshProUGUI totalSporeText;
    [SerializeField] private TextMeshProUGUI spendableSporeText;
    
    [SerializeField] private Button farmXButton;
    [SerializeField] private TextMeshProUGUI farmXText;
    [SerializeField] private int farmXCost = 100;
    [SerializeField] private TextMeshProUGUI farmXCostText;
    
    [SerializeField] private Button farmYButton;
    [SerializeField] private TextMeshProUGUI farmYText;
    [SerializeField] private int farmYCost = 100;
    [SerializeField] private TextMeshProUGUI farmYCostText;
    
    [SerializeField] private Button mushroomMultiButton;
    [SerializeField] private TextMeshProUGUI mushroomMultiText;
    [SerializeField] private int mushroomMultiCost = 100;
    [SerializeField] private TextMeshProUGUI mushroomMultiCostText;
    
    [SerializeField] private Button mushroomSpeedButton;
    [SerializeField] private TextMeshProUGUI mushroomSpeedText;
    [SerializeField] private int mushroomSpeedCost = 100;
    [SerializeField] private TextMeshProUGUI mushroomSpeedCostText;

    [Header("convergence menu")]
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button convergeButton;
    [SerializeField] private Button confirmConvergeButton;
    [SerializeField] private Button exitStoreButton;
    [SerializeField] private Toggle agreeToggle;
    public uint reward= 0;
    void Start()
    {
        convergenceStoreMenu.SetActive(false);
        convergenceMenu.SetActive(false);
        convergeButton.gameObject.SetActive(unlocked);
            confirmConvergeButton.interactable = false;
    }

    void Update()
    {
        
        
    }

    public void PurchaseFarmX()
    {
        SFXMaster.instance.PlayMenuClick();
        GameMaster.instance.SaveSystem.sporeCount -= (uint)farmXCost;
        GameMaster.instance.SaveSystem.farmSize.x += 1;
        UpdateUpgradeText();
    }
    
    public void PurchaseFarmY()
    {
        SFXMaster.instance.PlayMenuClick();
        GameMaster.instance.SaveSystem.sporeCount -= (uint)farmYCost;
        GameMaster.instance.SaveSystem.farmSize.y += 1;
        UpdateUpgradeText();
    }
    
    public void PurchaseMushroomMulti()
    {
        SFXMaster.instance.PlayMenuClick();
        GameMaster.instance.SaveSystem.sporeCount -= (uint)mushroomMultiCost;
        GameMaster.instance.SaveSystem.mushroomMultiplier += 1;
        UpdateUpgradeText();
    }

    public void PurchaseMushroomSpeed()
    {
        SFXMaster.instance.PlayMenuClick();
        GameMaster.instance.SaveSystem.sporeCount -= (uint)mushroomSpeedCost;
        GameMaster.instance.SaveSystem.mushroomSpeed += 1;
        UpdateUpgradeText();
    }
    
    public void UpdateUpgradeText()
    {
        farmXCost = Mathf.RoundToInt(10 * Mathf.Pow(1.1f, GameMaster.instance.SaveSystem.farmSize.x));
        farmYCost = Mathf.RoundToInt(10 * Mathf.Pow(1.1f, GameMaster.instance.SaveSystem.farmSize.y));
        farmXCostText.text = farmXCost.ToString();
        farmYCostText.text = farmYCost.ToString();
        
        farmXText.text = (3 + GameMaster.instance.SaveSystem.farmSize.x).ToString();
        farmYText.text = (3 + GameMaster.instance.SaveSystem.farmSize.y).ToString();
        farmXButton.interactable = GameMaster.instance.SaveSystem.sporeCount >= farmXCost;
        farmYButton.interactable = GameMaster.instance.SaveSystem.sporeCount >= farmYCost;
        
        mushroomMultiCost = Mathf.RoundToInt(5 * Mathf.Pow(1.2f, GameMaster.instance.SaveSystem.mushroomMultiplier));
        mushroomMultiCostText.text = mushroomMultiCost.ToString();
        mushroomMultiText.text = (GameMaster.instance.SaveSystem.mushroomMultiplier + 1).ToString();
        mushroomMultiButton.interactable = GameMaster.instance.SaveSystem.sporeCount >= mushroomMultiCost;
        
        mushroomSpeedCost = Mathf.RoundToInt(2 * Mathf.Pow(1.2f, GameMaster.instance.SaveSystem.mushroomSpeed));
        mushroomSpeedCostText.text = mushroomSpeedCost.ToString();
        mushroomSpeedText.text = (GameMaster.instance.SaveSystem.mushroomSpeed).ToString();
        mushroomSpeedButton.interactable = GameMaster.instance.SaveSystem.sporeCount >= mushroomSpeedCost;
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
        HideMenu();
    }
    
    public async Task HideMenu()
    {
        SFXMaster.instance.PlayMenuClick();
        float duration = 0.5f;
        convergenceMenu.transform.DOLocalMoveX(-2000, duration).onComplete += () =>
        {
            convergenceMenu.SetActive(false);
        };
        await Task.Delay((int)(duration * 1000));
    }
    
    public async void Converge()
    {
        // SFXMaster.instance.PlayMenuClick();
        GameMaster.instance.SaveSystem.sporeCountTotal += reward;
        GameMaster.instance.SaveSystem.sporeCount += reward;
        
        agreeToggle.isOn = false;
        confirmConvergeButton.interactable = false;
        await HideMenu();
        ShowStoreMenu();

    }

    public void ShowStoreMenu()
    {
        convergeButton.interactable = false;
        convergenceStoreMenu.SetActive(true);
        inStore = true;
        convergenceStoreMenu.transform.DOLocalMoveX(0, 0.5f);
        exitStoreButton.interactable = true;
    }

    public void CloseStoreButton()
    {
        CloseStoreMenu();
    }
    
    public async void CloseStoreMenu()
    {
        inStore = false;
        exitStoreButton.interactable = false;
        float duration = 0.5f;
        rayBlocker.DOFade(0, duration).onComplete += () => rayBlocker.gameObject.SetActive(false);
        convergenceStoreMenu.transform.DOLocalMoveX(-2000, duration).onComplete += () =>
        {
            convergenceStoreMenu.SetActive(false);
        };
        await Task.Delay((int)(duration * 1000));
        await GameMaster.instance.Prestige();
        convergeButton.interactable = true;

    }

    void FixedUpdate()
    {
        CalculateSporeReward();
        
        totalSporeText.text = "+" + reward;
        spendableSporeText.text = GameMaster.instance.SaveSystem.sporeCount + "Spores";
        rewardText.text = "+"+reward + " spores";
        
        if (!unlocked)
        {
            unlocked = GameMaster.instance.SaveSystem.mushroomBlockCount >=9 || GameMaster.instance.SaveSystem.totalConverges >= 1;
            convergeButton.gameObject.SetActive(unlocked);
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
        double sporeCost = Math.Pow(1.1, GameMaster.instance.SaveSystem.sporeCountTotal);
        uint mushValue = (uint)Mathf.Pow(GameMaster.instance.SaveSystem.BrownMushrooms, 1.5f);
        // uint rewardSqrt = (uint)(Mathf.Sqrt((float)(mushValue/sporeCost)));
        int rewardLogAsInt = Mathf.RoundToInt(Mathf.Log((float)(mushValue / sporeCost)));
        uint rewardLog = (uint)Mathf.Max(0, rewardLogAsInt);
        reward = rewardLog;
    }
}
