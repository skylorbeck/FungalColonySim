using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModeMaster : MonoBehaviour
{
    public enum Gamemode
    {
        BrownFarm,
        RedFarm,
        BlueFarm,
        Hivemind,
        Potions,
        Marketplace,
    }

    //TODO check for current mode and enable/disable processing intensive things
    public int distance = 200;
    public float duration = 1;

    public GameObject BrownFarm;
    public GameObject BrownFarmUpgrades;
    public GameObject RedFarm;
    public GameObject RedFarmUpgrades;
    public GameObject BlueFarm;
    public GameObject BlueFarmUpgrades;
    public GameObject Hivemind;
    public GameObject HivemindUpgrades;
    public GameObject Potions;
    public GameObject PotionsUpgrades;
    public GameObject Marketplace;
    public GameObject MarketplaceUpgrades;
    public Gamemode currentMode;
    public Gamemode lastMode;

    public TextMeshProUGUI modeText;
    public Button nextButton;
    public Button previousButton;

    public Image[] dots;
    [SerializeField] private Gamemode[] modesToDisableCamera;
    public UnityAction OnModeChange;

    private IEnumerator Start()
    {
        BrownFarmUpgrades.SetActive(false);
        RedFarmUpgrades.SetActive(false);
        BlueFarmUpgrades.SetActive(false);
        HivemindUpgrades.SetActive(false);
        PotionsUpgrades.SetActive(false);
        MarketplaceUpgrades.SetActive(false);

        currentMode = Gamemode.Hivemind;
        yield return new WaitUntil(() => SaveSystem.instance.loaded);
        SetMode(Gamemode.BrownFarm);
        UpdateButton();
        BrownFarm.transform.position = new Vector3(0, 0, 0);
        RedFarm.transform.position = new Vector3(distance, 0, 0);
        BlueFarm.transform.position = new Vector3(distance * 2, 0, 0);
        Hivemind.transform.position = new Vector3(distance * 3, 0, 0);
        Potions.transform.position = new Vector3(distance * 4, 0, 0);
        Marketplace.transform.position = new Vector3(distance * 5, 0, 0);
    }

    public void UpdateButton()
    {
        bool unlocked = (SaveSystem.save.statsTotal.converges > 0);
        modeText.gameObject.SetActive(unlocked);
        nextButton.gameObject.SetActive(unlocked);
        previousButton.gameObject.SetActive(unlocked);
    }

    public void NextMode()
    {
        SFXMaster.instance.PlayMenuClick();
        Gamemode mode = currentMode;
        mode++;

        if (!SaveSystem.save.farmSave.upgrades.redUnlocked && mode == Gamemode.RedFarm)
        {
            mode++;
        }

        if (!SaveSystem.save.farmSave.upgrades.blueUnlocked && mode == Gamemode.BlueFarm)
        {
            mode++;
        }

        if (!(SaveSystem.save.statsTotal.converges > 0) && mode == Gamemode.Hivemind)
        {
            mode++;
        }

        if (!(SaveSystem.save.cauldronSave.isUnlocked) && mode == Gamemode.Potions)
        {
            mode++;
        }

        if (!(SaveSystem.save.marketSave.isUnlocked) && mode == Gamemode.Marketplace)
        {
            mode++;
        }

        if (mode > Gamemode.Marketplace)
        {
            mode = Gamemode.BrownFarm;
        }

        SetMode(mode);
    }

    public void PreviousMode()
    {
        SFXMaster.instance.PlayMenuClick();
        Gamemode mode = currentMode;
        mode--;

        if (mode < Gamemode.BrownFarm)
        {
            mode = Gamemode.Marketplace;
        }

        if (!(SaveSystem.save.marketSave.isUnlocked) && mode == Gamemode.Marketplace)
        {
            mode--;
        }

        if (!(SaveSystem.save.cauldronSave.isUnlocked) && mode == Gamemode.Potions)
        {
            mode--;
        }

        if (!(SaveSystem.save.statsTotal.converges > 0) && mode == Gamemode.Hivemind)
        {
            mode--;
        }

        if (!SaveSystem.save.farmSave.upgrades.blueUnlocked && mode == Gamemode.BlueFarm)
        {
            mode--;
        }

        if (!SaveSystem.save.farmSave.upgrades.redUnlocked && mode == Gamemode.RedFarm)
        {
            mode--;
        }


        SetMode(mode, true);
    }

    public void SetMode(Gamemode gamemode, bool left = false)
    {
        lastMode = currentMode;
        currentMode = gamemode;

        BrownFarmUpgrades.transform.DOComplete();
        RedFarmUpgrades.transform.DOComplete();
        BlueFarmUpgrades.transform.DOComplete();
        HivemindUpgrades.transform.DOComplete();
        PotionsUpgrades.transform.DOComplete();
        MarketplaceUpgrades.transform.DOComplete();

        UpdateDots();

        //zoom out the ui
        switch (lastMode)
        {
            case Gamemode.BrownFarm:
                BrownFarmUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    BrownFarmUpgrades.transform.localScale = Vector3.one;
                    BrownFarmUpgrades.SetActive(false);
                };
                break;
            case Gamemode.RedFarm:
                RedFarmUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    RedFarmUpgrades.transform.localScale = Vector3.one;
                    RedFarmUpgrades.SetActive(false);
                };
                break;
            case Gamemode.BlueFarm:
                BlueFarmUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    BlueFarmUpgrades.transform.localScale = Vector3.one;
                    BlueFarmUpgrades.SetActive(false);
                };
                break;
            case Gamemode.Hivemind:
                HivemindUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    HivemindUpgrades.transform.localScale = Vector3.one;
                    HivemindUpgrades.SetActive(false);
                };
                break;
            case Gamemode.Potions:
                PotionsUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    PotionsUpgrades.transform.localScale = Vector3.one;
                    PotionsUpgrades.SetActive(false);
                };
                break;
            case Gamemode.Marketplace:
                MarketplaceUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    MarketplaceUpgrades.transform.localScale = Vector3.one;
                    MarketplaceUpgrades.SetActive(false);
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        modeText.DOComplete();
        modeText.alpha = 1;
        //zoom in the ui, move the camera
        GameMaster.instance.camera.transform.DOKill();
        GameMaster.instance.camera.transform.DOLocalMoveX(distance * (int)currentMode, duration);
        switch (currentMode)
        {
            case Gamemode.BrownFarm:
                modeText.text = "Brown Mushroom Farm";
                BrownFarmUpgrades.transform.localScale = Vector3.one * 5;
                BrownFarmUpgrades.SetActive(true);
                BrownFarmUpgrades.transform.DOScale(1, duration).onComplete =
                    () => BrownFarmUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.RedFarm:
                modeText.text = "Red Mushroom Farm";
                RedFarmUpgrades.transform.localScale = Vector3.one * 5;
                RedFarmUpgrades.SetActive(true);
                RedFarmUpgrades.transform.DOScale(1, duration).onComplete =
                    () => RedFarmUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.BlueFarm:
                modeText.text = "Blue Mushroom Farm";
                BlueFarmUpgrades.transform.localScale = Vector3.one * 5;
                BlueFarmUpgrades.SetActive(true);
                BlueFarmUpgrades.transform.DOScale(1, duration).onComplete =
                    () => BlueFarmUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.Hivemind:
                modeText.text = "Plinko Board";
                HivemindUpgrades.transform.localScale = Vector3.one * 5;
                HivemindUpgrades.SetActive(true);
                HivemindUpgrades.transform.DOScale(1, duration).onComplete =
                    () => HivemindUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.Potions:
                modeText.text = "Potions";
                PotionsUpgrades.transform.localScale = Vector3.one * 5;
                PotionsUpgrades.SetActive(true);
                PotionsUpgrades.transform.DOScale(1, duration).onComplete =
                    () => PotionsUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.Marketplace:
                modeText.text = "Marketplace";
                MarketplaceUpgrades.transform.localScale = Vector3.one * 5;
                MarketplaceUpgrades.SetActive(true);
                MarketplaceUpgrades.transform.DOScale(1, duration).onComplete =
                    () => MarketplaceUpgrades.transform.localScale = Vector3.one;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gamemode), gamemode, "No such gamemode");
        }

        modeText.DOFade(0, 0.5f).SetDelay(1.5f);
        CameraCheck();
        OnModeChange?.Invoke();
    }

    public void UpdateDots()
    {
        dots[1].gameObject.SetActive(SaveSystem.save.farmSave.upgrades.redUnlocked);
        dots[2].gameObject.SetActive(SaveSystem.save.farmSave.upgrades.blueUnlocked);
        dots[3].gameObject.SetActive(SaveSystem.save.statsTotal.converges > 0);
        dots[0].gameObject.SetActive(SaveSystem.save.statsTotal.converges > 0);
        dots[4].gameObject.SetActive(SaveSystem.save.cauldronSave.isUnlocked);
        dots[5].gameObject.SetActive(SaveSystem.save.marketSave.isUnlocked);

        dots[(int)lastMode].transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        dots[(int)currentMode].transform.DOScale(2, 0.5f).SetEase(Ease.OutBack);
    }

    public void CameraCheck()
    {
        if (modesToDisableCamera.Contains(currentMode))
        {
            GameMaster.instance.camera.GetComponent<CameraController>().Disable();
        }
        else
        {
            GameMaster.instance.camera.GetComponent<CameraController>().Enable();
        }
    }

    public float PreCalculateCameraDistance()
    {
        return (int)currentMode * distance;
    }
}