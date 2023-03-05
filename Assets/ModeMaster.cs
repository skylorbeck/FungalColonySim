using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeMaster : MonoBehaviour
{
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
    public Gamemode currentMode;
    public Gamemode lastMode;

    public TextMeshProUGUI modeText;
    public Button nextButton;
    public Button previousButton;

    public Image[] dots;
    
    [SerializeField] private Gamemode[] modesToDisableCamera;
    private void Start()
    {
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

        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal>0) && mode == Gamemode.Hivemind)
        {
            mode++;
        } 
        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal>0) && mode == Gamemode.Potions)
        {
            mode++;
        }

        if (mode > Gamemode.Potions)
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
        
        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal>0) && mode == Gamemode.Potions)
        {
            mode--;
        }
        if (!(SaveSystem.instance.GetSaveFile().sporeCountTotal>0) && mode == Gamemode.Hivemind)
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
            mode = Gamemode.Potions;
        }
        SetMode(mode,true);
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
        
        BrownFarmUpgrades.SetActive(false);
        RedFarmUpgrades.SetActive(false);
        BlueFarmUpgrades.SetActive(false);
        HivemindUpgrades.SetActive(false);
        PotionsUpgrades.SetActive(false);
        
        UpdateDots();

        int dist = left ? distance : -distance;
        
        switch (lastMode)
        {
            case Gamemode.BrownFarm:
                BrownFarm.transform.DOMoveX(dist, duration);
                break;
            case Gamemode.RedFarm:
                RedFarm.transform.DOMoveX(dist, duration);
                break;
            case Gamemode.BlueFarm:
                BlueFarm.transform.DOMoveX(dist, duration);
                break;
            case Gamemode.Hivemind:
                Hivemind.transform.DOMoveX(dist, duration);
                break;
            case Gamemode.Potions:
                Potions.transform.DOMoveX(dist, duration);
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
                BrownFarm.transform.DOMoveX(0, duration).onComplete = () => BrownFarmUpgrades.SetActive(true);
                break;
            case Gamemode.RedFarm:
                modeText.text = "Red Mushroom Farm";
                RedFarm.transform.position = new Vector3(-dist, 0, 0);
                RedFarm.transform.DOMoveX(0, duration).onComplete = () => RedFarmUpgrades.SetActive(true);
                break;
            case Gamemode.BlueFarm:
                modeText.text = "Blue Mushroom Farm";
                BlueFarm.transform.position = new Vector3(-dist, 0, 0);
                BlueFarm.transform.DOMoveX(0, duration).onComplete = () => BlueFarmUpgrades.SetActive(true);
                break;
            case Gamemode.Hivemind:
                modeText.text = "Hivemind Core";
                Hivemind.transform.position = new Vector3(-dist, 0, 0);
                Hivemind.transform.DOMoveX(0, duration).onComplete = () => HivemindUpgrades.SetActive(true);
                break;
            case Gamemode.Potions:
                modeText.text = "Potions";
                Potions.transform.position = new Vector3(-dist, 0, 0);
                Potions.transform.DOMoveX(0, duration).onComplete = () => PotionsUpgrades.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gamemode), gamemode, "No such gamemode");
        }
        modeText.DOFade(0, 0.5f).SetDelay(1.5f);
        CameraCheck();
    }

    public enum Gamemode
    {
        BrownFarm,
        RedFarm,
        BlueFarm,
        Hivemind,
        Potions,
    }

    public void UpdateDots()
    {
        dots[1].gameObject.SetActive(SaveSystem.instance.GetSaveFile().redUnlocked);
        dots[2].gameObject.SetActive(SaveSystem.instance.GetSaveFile().blueUnlocked);
        dots[3].gameObject.SetActive(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0);
        dots[0].gameObject.SetActive(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0);
        dots[4].gameObject.SetActive(SaveSystem.instance.GetSaveFile().sporeCountTotal > 0);
        
        dots[(int)lastMode].transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        dots[(int)currentMode].transform.DOScale(2, 0.5f).SetEase(Ease.OutBack);    }

    public void CameraCheck()
    {
        if (modesToDisableCamera.Contains(currentMode))
        {
            GameMaster.instance.camera.GetComponent<CameraController>().Disable();
        } else
        {
            GameMaster.instance.camera.GetComponent<CameraController>().Enable();
        }
    }
}