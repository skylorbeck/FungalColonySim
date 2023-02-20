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
        if (GameMaster.instance.SaveSystem.SpendSpores(farmXCost))
        {
            GameMaster.instance.SaveSystem.farmSize.x += 1;
            UpdateUpgradeText();
        }
        
    }
    
    public void PurchaseFarmY()
    {
        SFXMaster.instance.PlayMenuClick();
        if (GameMaster.instance.SaveSystem.SpendSpores(farmYCost))
        {
            GameMaster.instance.SaveSystem.farmSize.y += 1;
            UpdateUpgradeText();
        }
        
    }
    
    public void PurchaseMushroomMulti()
    {
        SFXMaster.instance.PlayMenuClick();
        if (GameMaster.instance.SaveSystem.SpendSpores(mushroomMultiCost))
        {
            GameMaster.instance.SaveSystem.mushroomMultiplier += 1;
            UpdateUpgradeText();
        }
    }

    public void PurchaseMushroomSpeed()
    {
        SFXMaster.instance.PlayMenuClick();
        if (GameMaster.instance.SaveSystem.SpendSpores(mushroomSpeedCost))
        {
            GameMaster.instance.SaveSystem.mushroomSpeed += 1;
            UpdateUpgradeText();
        }
        
    }
    
    public void UpdateUpgradeText()
    {
        farmXCost = (uint)Mathf.RoundToInt(10 * Mathf.Pow(1.1f, GameMaster.instance.SaveSystem.farmSize.x));
        farmYCost = (uint)Mathf.RoundToInt(10 * Mathf.Pow(1.1f, GameMaster.instance.SaveSystem.farmSize.y));
        farmXCostText.text = farmXCost.ToString();
        farmYCostText.text = farmYCost.ToString();
        
        farmXText.text = (3 + GameMaster.instance.SaveSystem.farmSize.x).ToString();
        farmYText.text = (3 + GameMaster.instance.SaveSystem.farmSize.y).ToString();
        farmXButton.interactable = GameMaster.instance.SaveSystem.sporeCount >= farmXCost;
        farmYButton.interactable = GameMaster.instance.SaveSystem.sporeCount >= farmYCost;
        
        mushroomMultiCost = (uint)Mathf.RoundToInt(5 * Mathf.Pow(1.2f, GameMaster.instance.SaveSystem.mushroomMultiplier));
        mushroomMultiCostText.text = mushroomMultiCost.ToString();
        mushroomMultiText.text = (GameMaster.instance.SaveSystem.mushroomMultiplier + 1).ToString();
        mushroomMultiButton.interactable = GameMaster.instance.SaveSystem.sporeCount >= mushroomMultiCost;
        
        mushroomSpeedCost = (uint)Mathf.RoundToInt(2 * Mathf.Pow(1.2f, GameMaster.instance.SaveSystem.mushroomSpeed));
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
        GameMaster.instance.SaveSystem.sporeCountTotal += reward;
        GameMaster.instance.SaveSystem.sporeCount += reward;
        GameMaster.instance.SaveSystem.hivemindPointsTotal += skillReward;
        GameMaster.instance.SaveSystem.hivemindPoints += skillReward;

        agreeToggle.isOn = false;
        confirmConvergeButton.interactable = false;
        yield return StartCoroutine(HideMenu());
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

        spendableSporeText.text = GameMaster.instance.SaveSystem.sporeCount + " Spores";
        rewardText.text = "+"+reward + " spores";
        skillRewardText.text = "+"+skillReward + " skill points";
        if (!unlocked)
        {
            unlocked = GameMaster.instance.SaveSystem.mushroomBlockCount[0] >=9 || GameMaster.instance.SaveSystem.totalConverges >= 1;
            convergeButton.gameObject.SetActive(unlocked);
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
        double sporeCost = Math.Pow(1.1, GameMaster.instance.SaveSystem.sporeCountTotal);
        float brownMultiplier = 1 + (GameMaster.instance.SaveSystem.brownMultiplier * GameMaster.instance.Hivemind.brownValueMultiplierGain);
        uint brownValue = (uint)Mathf.Pow(GameMaster.instance.SaveSystem.mushrooms[(int)MushroomBlock.MushroomType.Brown]*brownMultiplier, 1.1f);

        float redMultiplier = 1 + (GameMaster.instance.SaveSystem.redMultiplier * GameMaster.instance.Hivemind.redValueMultiplierGain);//
        uint redValue = (uint)Mathf.Pow(GameMaster.instance.SaveSystem.mushrooms[(int)MushroomBlock.MushroomType.Red]*redMultiplier, 1.3f);

        float blueMultiplier = 1 + (GameMaster.instance.SaveSystem.blueMultiplier * GameMaster.instance.Hivemind.blueValueMultiplierGain);
        uint blueValue = (uint)Mathf.Pow(GameMaster.instance.SaveSystem.mushrooms[(int)MushroomBlock.MushroomType.Blue]*blueMultiplier, 1.5f);
        
        convergencePointCostText.text = "(" +brownValue + "+" + redValue + "+" + blueValue + ")\n" + sporeCost.ToString("0.00");

        // uint rewardSqrt = (uint)(Mathf.Sqrt((float)(mushValue/sporeCost)));
        int rewardLogAsInt = Mathf.RoundToInt(Mathf.Log((float)((brownValue+redValue+blueValue) / sporeCost)));
        uint rewardLog = (uint)Mathf.Max(0, rewardLogAsInt);
        reward = rewardLog;
    }

    private void CalculateHivemindPointsReward()
    {
        double pointCost = Math.Pow(1.1, GameMaster.instance.SaveSystem.hivemindPointsTotal);
        float pointMultiplier = 1 + (GameMaster.instance.SaveSystem.hivemindPointValue * 0.1f);//TODO make this a meta meta upgrade
        uint pointValue = (uint)Mathf.Pow(reward*pointMultiplier, 1.5f);
        convergenceSkillPointCostText.text = pointValue.ToString("0.00")+"\n"+pointCost.ToString("0.00");
        // uint rewardSqrt = (uint)(Mathf.Sqrt((float)(pointValue/sporeCost)));
        int rewardLogAsInt = Mathf.RoundToInt(Mathf.Log((float)(pointValue / pointCost)));
        uint rewardLog = (uint)Mathf.Max(0, rewardLogAsInt);
        skillReward = rewardLog;
    }
}
