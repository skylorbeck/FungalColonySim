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
    public Canvas BrownFarmCanvas;
    public GameObject RedFarm;
    public GameObject BlueFarm;
    public GameObject Hivemind;
    public Gamemode currentMode;

    public TextMeshProUGUI modeText;
    private void Start()
    {
        SetMode(Gamemode.BrownFarm);
    }

    public void NextMode()
    {
        currentMode++;
        if (currentMode > Gamemode.Hivemind)
        {
            currentMode = Gamemode.BrownFarm;
        }
        SetMode(currentMode);
    }
    
    public void PreviousMode()
    {
        currentMode--;
        if (!GameMaster.instance.SaveSystem.redUnlocked && currentMode == Gamemode.RedFarm)
        {
            currentMode--;
        }
        if (!GameMaster.instance.SaveSystem.blueUnlocked && currentMode == Gamemode.BlueFarm)
        {
            currentMode--;
        }

        if (!(GameMaster.instance.SaveSystem.sporeCountTotal>0) && currentMode == Gamemode.Hivemind)
        {
            currentMode--;
        }
        if (currentMode < Gamemode.BrownFarm)
        {
            currentMode = Gamemode.Hivemind;
        }
        SetMode(currentMode,true);
    }

    private void SetMode(Gamemode gamemode, bool left = false)
    {
        
        modeText.text = gamemode.ToString();
        int dist = left ? -distance : distance;
        BrownFarm.transform.DOComplete();
        RedFarm.transform.DOComplete();
        BlueFarm.transform.DOComplete();
        Hivemind.transform.DOComplete();
        
        BrownFarmCanvas.enabled = false;

        
        switch (gamemode)
        {
            case Gamemode.BrownFarm:
                // BrownFarm.SetActive(true);
                BrownFarm.transform.position = new Vector3(-dist, 0, 0);
                BrownFarm.transform.DOMoveX(0, duration).onComplete = () => BrownFarmCanvas.enabled = true;
                RedFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => RedFarm.SetActive(false)*/;
                BlueFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => BlueFarm.SetActive(false)*/;
                Hivemind.transform.DOMoveX(dist, duration)/*.onComplete = () => Hivemind.SetActive(false)*/;
                break;
            case Gamemode.RedFarm:
                RedFarm.SetActive(true);
                RedFarm.transform.position = new Vector3(-dist, 0, 0);
                RedFarm.transform.DOMoveX(0, duration);
                BrownFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => BrownFarm.SetActive(false)*/;
                BlueFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => BlueFarm.SetActive(false)*/;
                Hivemind.transform.DOMoveX(dist, duration)/*.onComplete = () => Hivemind.SetActive(false)*/;
                break;
            case Gamemode.BlueFarm:
                BlueFarm.SetActive(true);
                BlueFarm.transform.position = new Vector3(-dist, 0, 0);
                BlueFarm.transform.DOMoveX(0, duration);
                BrownFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => BrownFarm.SetActive(false)*/;
                RedFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => RedFarm.SetActive(false)*/;
                Hivemind.transform.DOMoveX(dist, duration)/*.onComplete = () => Hivemind.SetActive(false)*/;
                break;
            case Gamemode.Hivemind:
                Hivemind.SetActive(true);
                Hivemind.transform.position = new Vector3(-dist, 0, 0);
                Hivemind.transform.DOMoveX(0, duration);
                BrownFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => BrownFarm.SetActive(false)*/;
                RedFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => RedFarm.SetActive(false)*/;
                BlueFarm.transform.DOMoveX(dist, duration)/*.onComplete = () => BlueFarm.SetActive(false)*/;
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