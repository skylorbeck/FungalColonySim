using System;
using System.Collections;
using System.Collections.Generic;
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
    public Gamemode currentMode;
    public Gamemode lastMode;

    public TextMeshProUGUI modeText;
    public Button nextButton;
    public Button previousButton;
    private void Start()
    {
        SetMode(Gamemode.BrownFarm);
        UpdateButton();
    }

    public void UpdateButton()
    {
        bool unlocked = (GameMaster.instance.SaveSystem.sporeCountTotal > 0);
        modeText.gameObject.SetActive(unlocked);
        nextButton.gameObject.SetActive(unlocked);
        previousButton.gameObject.SetActive(unlocked);
    }
    
    public void NextMode()
    {
        Gamemode mode = currentMode;
        mode++;
        
        if (!GameMaster.instance.SaveSystem.redUnlocked && mode == Gamemode.RedFarm)
        {
            mode++;
        }
        if (!GameMaster.instance.SaveSystem.blueUnlocked && mode == Gamemode.BlueFarm)
        {
            mode++;
        }

        if (!(GameMaster.instance.SaveSystem.sporeCountTotal>0) && mode == Gamemode.Hivemind)
        {
            mode++;
        }
        
        if (mode > Gamemode.Hivemind)
        {
            mode = Gamemode.BrownFarm;
        }
        SetMode(mode);
    }
    
    public void PreviousMode()
    {
        Gamemode mode = currentMode;
        mode--;
        
        if (!(GameMaster.instance.SaveSystem.sporeCountTotal>0) && mode == Gamemode.Hivemind)
        {
            mode--;
        }
        if (!GameMaster.instance.SaveSystem.blueUnlocked && mode == Gamemode.BlueFarm)
        {
            mode--;
        }
        if (!GameMaster.instance.SaveSystem.redUnlocked && mode == Gamemode.RedFarm)
        {
            mode--;
        }
       
        
        if (mode < Gamemode.BrownFarm)
        {
            mode = Gamemode.Hivemind;
        }
        SetMode(mode,true);
    }

    private void SetMode(Gamemode gamemode, bool left = false)
    {
        lastMode = currentMode;
        currentMode = gamemode;
        modeText.text = gamemode.ToString();
        BrownFarm.transform.DOComplete();
        RedFarm.transform.DOComplete();
        BlueFarm.transform.DOComplete();
        Hivemind.transform.DOComplete();
        
        BrownFarmUpgrades.SetActive(false);
        RedFarmUpgrades.SetActive(false);
        BlueFarmUpgrades.SetActive(false);

        int dist = left ? -distance : distance;
        
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
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        switch (currentMode)
        {
            case Gamemode.BrownFarm:
                /*if (!GameMaster.instance.brownBlockMaster.isWorldCreated)
                {
                    GameMaster.instance.brownBlockMaster.CreateWorld();
                }*/
                BrownFarm.transform.position = new Vector3(-dist, 0, 0);
                BrownFarm.transform.DOMoveX(0, duration).onComplete = () => BrownFarmUpgrades.SetActive(true);
                break;
            case Gamemode.RedFarm:
                /*if (!GameMaster.instance.redBlockMaster.isWorldCreated)
                {
                    GameMaster.instance.redBlockMaster.CreateWorld();
                }*/
                RedFarm.transform.position = new Vector3(-dist, 0, 0);
                RedFarm.transform.DOMoveX(0, duration).onComplete = () => RedFarmUpgrades.SetActive(true);
                break;
            case Gamemode.BlueFarm:
                /*if (!GameMaster.instance.blueBlockMaster.isWorldCreated)
                {
                    GameMaster.instance.blueBlockMaster.CreateWorld();
                }*/
                BlueFarm.transform.position = new Vector3(-dist, 0, 0);
                BlueFarm.transform.DOMoveX(0, duration).onComplete = () => BlueFarmUpgrades.SetActive(true);
                break;
            case Gamemode.Hivemind:
                Hivemind.transform.position = new Vector3(-dist, 0, 0);
                Hivemind.transform.DOMoveX(0, duration);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gamemode), gamemode, "No such gamemode");
        }
    }

    public enum Gamemode
    {
        BrownFarm,
        RedFarm,
        BlueFarm,
        Hivemind,
    }
}