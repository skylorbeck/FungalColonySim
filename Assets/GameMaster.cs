using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public BlockMaster brownBlockMaster;
    public BlockMaster redBlockMaster;
    public BlockMaster blueBlockMaster;
    public Camera camera;
    public Tooltip tooltip;

    public ModeMaster ModeMaster;
    public UpgradeMaster brownUpgradeMaster;
    public UpgradeMaster redUpgradeMaster;
    public UpgradeMaster blueUpgradeMaster;

    public ConvergenceMaster convergenceMaster;
    [SerializeField] private bool isConverging;

    public Hivemind Hivemind;

    public int saveTimer = 0;

    public Color[] mushroomColors = new Color[3];

    //TODO better scaling upgrade to increase gains from convergence
    //TODO harvest mushrooms by clicking on floor
    //TODO reset button
    //TODO red and blue upgrades have different scaling?
    //TODO Regenerate, a second layer of prestige that resets hivemind levels and gives a flat bonus to mushroom production

    IEnumerator Start()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        camera = Camera.main;
        yield return new WaitUntil(() => SaveSystem.instance != null);
        SaveSystem.instance.Load();
    }

    void Update()
    {
        //quit on escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


    }

    public IEnumerator Prestige()
    {
        isConverging = true;
        Coroutine brown = StartCoroutine(brownBlockMaster.DissolveWorld());
        Coroutine red = StartCoroutine(redBlockMaster.DissolveWorld());
        Coroutine blue = StartCoroutine(blueBlockMaster.DissolveWorld());
        yield return brown;
        yield return red;
        yield return blue;
        
        ScoreMaster.instance.Reset();
        brownUpgradeMaster.Reset();
        redUpgradeMaster.Reset();
        blueUpgradeMaster.Reset();
        Coroutine brown2 = StartCoroutine(brownBlockMaster.CreateWorld());
        Coroutine red2 = StartCoroutine(redBlockMaster.CreateWorld());
        Coroutine blue2 = StartCoroutine(blueBlockMaster.CreateWorld());
        yield return brown2;
        yield return red2;
        yield return blue2;
        
        SaveSystem.instance.GetSaveFile().totalConverges++;
        SaveSystem.instance.Save();
        ModeMaster.UpdateButton();
        isConverging = false;
    }

    void FixedUpdate()
    {
        TickSaver();
    }

    private void TickSaver()
    {
        saveTimer++;
        if (saveTimer > 500 && !convergenceMaster.inStore && !isConverging && (brownBlockMaster.isWorldCreated && redBlockMaster.isWorldCreated && blueBlockMaster.isWorldCreated))
        {
            saveTimer = 0;
            SaveSystem.instance.Save();
            
        }
    }

    public void OnDestroy()
    {
        SaveSystem.instance.Save();
    }
}
