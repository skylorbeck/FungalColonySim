using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModeMaster : MonoBehaviour
{
    //TODO check for current mode and enable/disable processing intensive things
    public int distance = 500;
    public float duration = 0.5f;

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
    public UnityAction OnModeChange;
    [SerializeField] private Gamemode[] modesToDisableCamera;

    private void Start()
    {
        BrownFarmUpgrades.SetActive(false);
        RedFarmUpgrades.SetActive(false);
        BlueFarmUpgrades.SetActive(false);
        HivemindUpgrades.SetActive(false);
        PotionsUpgrades.SetActive(false);
        MarketplaceUpgrades.SetActive(false);
        
        currentMode = Gamemode.Hivemind;
        SetMode(Gamemode.BrownFarm);
        UpdateButton();
    }

    public void UpdateButton()
    {
        bool unlocked = (SaveSystem.instance.GetSaveFile().sporeCountTotal > 0);
        modeText.gameObject.SetActive(unlocked);
        nextButton.gameObject.SetActive(unlocked);
        previousButton.gameObject.SetActive(unlocked);
    }

    public void NextMode()
    {
        SFXMaster.instance.PlayMenuClick();
        Gamemode mode = currentMode;
        mode++;

        if (!SaveSystem.instance.GetSaveFile().redUnlocked && mode == Gamemode.RedFarm)
        {
            mode++;
        }

        if (!SaveSystem.instance.GetSaveFile().blueUnlocked && mode == Gamemode.BlueFarm)
        {
            mode++;
        }

        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0) && mode == Gamemode.Hivemind)
        {
            mode++;
        }

        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0) && mode == Gamemode.Potions)
        {
            mode++;
        }
        
        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0) && mode == Gamemode.Marketplace)
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

        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0) && mode == Gamemode.Marketplace)
        {
            mode--;
        }

        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0) && mode == Gamemode.Potions)
        {
            mode--;
        }

        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0) && mode == Gamemode.Hivemind)
        {
            mode--;
        }

        if (!SaveSystem.instance.GetSaveFile().blueUnlocked && mode == Gamemode.BlueFarm)
        {
            mode--;
        }

        if (!SaveSystem.instance.GetSaveFile().redUnlocked && mode == Gamemode.RedFarm)
        {
            mode--;
        }


        if (mode < Gamemode.BrownFarm)
        {
            mode = Gamemode.Marketplace;
        }

        SetMode(mode, true);
    }

    private void SetMode(Gamemode gamemode, bool left = false)
    {
        lastMode = currentMode;
        currentMode = gamemode;
        
        BrownFarm.transform.DOComplete();
        RedFarm.transform.DOComplete();
        BlueFarm.transform.DOComplete();
        Hivemind.transform.DOComplete();
        Potions.transform.DOComplete();
        Marketplace.transform.DOComplete();

        BrownFarmUpgrades.transform.DOComplete();
        RedFarmUpgrades.transform.DOComplete();
        BlueFarmUpgrades.transform.DOComplete();
        HivemindUpgrades.transform.DOComplete();
        PotionsUpgrades.transform.DOComplete();
        MarketplaceUpgrades.transform.DOComplete();

        UpdateDots();

        int dist = left ? distance : -distance;

        switch (lastMode)
        {
            case Gamemode.BrownFarm:
                BrownFarm.transform.DOMoveX(dist, duration);
                BrownFarmUpgrades.transform.DOScale(5, duration).onComplete = () =>
                    {
                        BrownFarmUpgrades.transform.localScale = Vector3.one;
                        BrownFarmUpgrades.SetActive(false);
                    };
                break;
            case Gamemode.RedFarm:
                RedFarm.transform.DOMoveX(dist, duration);
                RedFarmUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    RedFarmUpgrades.transform.localScale = Vector3.one;
                    RedFarmUpgrades.SetActive(false);
                };
                break;
            case Gamemode.BlueFarm:
                BlueFarm.transform.DOMoveX(dist, duration);
                BlueFarmUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    BlueFarmUpgrades.transform.localScale = Vector3.one;
                    BlueFarmUpgrades.SetActive(false);
                };
                break;
            case Gamemode.Hivemind:
                Hivemind.transform.DOMoveX(dist, duration);
                HivemindUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    HivemindUpgrades.transform.localScale = Vector3.one;
                    HivemindUpgrades.SetActive(false);
                };
                break;
            case Gamemode.Potions:
                Potions.transform.DOMoveX(dist, duration);
                PotionsUpgrades.transform.DOScale(5, duration).onComplete = () =>
                {
                    PotionsUpgrades.transform.localScale = Vector3.one;
                    PotionsUpgrades.SetActive(false);
                };
                break;
            case Gamemode.Marketplace:
                Marketplace.transform.DOMoveX(dist, duration);
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
        
        switch (currentMode)
        {
            case Gamemode.BrownFarm:
                modeText.text = "Brown Mushroom Farm";
                BrownFarm.transform.position = new Vector3(-dist, 0, 0);
                BrownFarm.transform.DOMoveX(0, duration);
                BrownFarmUpgrades.transform.localScale = Vector3.one * 5;
                BrownFarmUpgrades.SetActive(true);
                BrownFarmUpgrades.transform.DOScale(1, duration).onComplete = () => BrownFarmUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.RedFarm:
                modeText.text = "Red Mushroom Farm";
                RedFarm.transform.position = new Vector3(-dist, 0, 0);
                RedFarm.transform.DOMoveX(0, duration).onComplete = () => RedFarmUpgrades.SetActive(true);
                RedFarmUpgrades.transform.localScale = Vector3.one * 5;
                RedFarmUpgrades.SetActive(true);
                RedFarmUpgrades.transform.DOScale(1, duration).onComplete = () => RedFarmUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.BlueFarm:
                modeText.text = "Blue Mushroom Farm";
                BlueFarm.transform.position = new Vector3(-dist, 0, 0);
                BlueFarm.transform.DOMoveX(0, duration).onComplete = () => BlueFarmUpgrades.SetActive(true);
                BlueFarmUpgrades.transform.localScale = Vector3.one * 5;
                BlueFarmUpgrades.SetActive(true);
                BlueFarmUpgrades.transform.DOScale(1, duration).onComplete = () => BlueFarmUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.Hivemind:
                modeText.text = "Hivemind Core";
                Hivemind.transform.position = new Vector3(-dist, 0, 0);
                Hivemind.transform.DOMoveX(0, duration).onComplete = () => HivemindUpgrades.SetActive(true);
                HivemindUpgrades.transform.localScale = Vector3.one * 5;
                HivemindUpgrades.SetActive(true);
                HivemindUpgrades.transform.DOScale(1, duration).onComplete = () => HivemindUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.Potions:
                modeText.text = "Potions";
                Potions.transform.position = new Vector3(-dist, 0, 0);
                Potions.transform.DOMoveX(0, duration).onComplete = () => PotionsUpgrades.SetActive(true);
                PotionsUpgrades.transform.localScale = Vector3.one * 5;
                PotionsUpgrades.SetActive(true);
                PotionsUpgrades.transform.DOScale(1, duration).onComplete = () => PotionsUpgrades.transform.localScale = Vector3.one;
                break;
            case Gamemode.Marketplace:
                modeText.text = "Marketplace";
                Marketplace.transform.position = new Vector3(-dist, 0, 0);
                Marketplace.transform.DOMoveX(0, duration).onComplete = () => MarketplaceUpgrades.SetActive(true);
                MarketplaceUpgrades.transform.localScale = Vector3.one * 5;
                MarketplaceUpgrades.SetActive(true);
                MarketplaceUpgrades.transform.DOScale(1, duration).onComplete = () => MarketplaceUpgrades.transform.localScale = Vector3.one;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gamemode), gamemode, "No such gamemode");
        }

        modeText.DOFade(0, 0.5f).SetDelay(1.5f);
        CameraCheck();
        OnModeChange?.Invoke();
    }

    public enum Gamemode
    {
        BrownFarm,
        RedFarm,
        BlueFarm,
        Hivemind,
        Potions,
        Marketplace,
    }

    public void UpdateDots()
    {
        dots[1].gameObject.SetActive(SaveSystem.instance.GetSaveFile().redUnlocked);
        dots[2].gameObject.SetActive(SaveSystem.instance.GetSaveFile().blueUnlocked);
        dots[3].gameObject.SetActive(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0);
        dots[0].gameObject.SetActive(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0);
        dots[4].gameObject.SetActive(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0);
        dots[5].gameObject.SetActive(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0);

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
}