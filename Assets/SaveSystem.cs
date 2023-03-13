using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    public Image saveIcon;
    public TextMeshProUGUI saveText;
    public bool loaded = false;
    [SerializeField] private SaveFile saveFile;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public SaveFile GetSaveFile()
    {
        return saveFile;
    }


    public bool SpendSpores(uint amount)
    {
        if (instance.saveFile.sporeCount < amount) return false;
        instance.saveFile.sporeCount -= amount;
        return true;
    }

    public bool SpendHivemindPoints(uint amount)
    {
        if (instance.saveFile.hivemindPoints < amount) return false;
        instance.saveFile.hivemindPoints -= amount;
        return true;
    }
    
    public bool SpendCoins(uint amount)
    {
        if (instance.saveFile.coins < amount) return false;
        instance.saveFile.coins -= amount;
        return true;
    }

    private void Save()
    {
        Save("savefile", saveFile);
        saveIcon.DOKill();
        saveText.DOKill();
        saveIcon.DOFade(1, 0.1f).OnComplete(() => saveIcon.DOFade(0, 1f).SetEase(Ease.OutFlash));
        saveText.DOFade(1, 0.1f).OnComplete(() => saveText.DOFade(0, 1f).SetEase(Ease.OutFlash));
    }

    public static void SaveS()
    {
        instance.Save();
    }

    public void Load()
    {
        Load("savefile", out saveFile);
        if (saveFile.saveVersion<2)
        {
            Reset();
        }

        if (saveFile.saveVersion < 3)
        {
            saveFile.saveVersion = 3;
            saveFile.goldenMultiplier = 1;
            saveFile.goldenChanceMultiplier = 1;
        }

        if (saveFile.saveVersion < 4)
        {
            saveFile.saveVersion = 4;
            saveFile.cauldronSave = new CauldronSave();
            saveFile.potionsCount = new uint[3];
            saveFile.coins = 0;
            saveFile.marketSave = new MarketSave();
            if (saveFile.goldenMultiplier <= 1)
            {
                saveFile.goldenMultiplier = 2;
            }
            saveFile.sporeMultiplier = 1;
            saveFile.plinkoSave = new PlinkoSave();
            saveFile.collectionItems = new List<CollectionItemSaveData>();
        }
        
        loaded = true;
    }

    public void Load<T>(string path, out T data) where T : new()
    {
        path = Application.persistentDataPath + "/" + path + ".json";
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
            data = new T();
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);
        }
        else
        {
            string json = File.ReadAllText(path);
            if (json == "")
            {
                Debug.Log("File" + path + " is empty");
                data = new T();
                json = JsonUtility.ToJson(data);
                File.WriteAllText(path, json);
            }
            else
            {
                data = JsonUtility.FromJson<T>(json);
            }
        }
    }

    public void Save<T>(string path, T data)
    {
        path = Application.persistentDataPath + "/" + path + ".json";
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }

        string json = JsonUtility.ToJson(data,true);
        File.WriteAllText(path, json);
    }

    public void Reset()
    {
        saveFile = new SaveFile();
    }

   
}

[Serializable]
public class SaveFile
{
//TODO it would probably be smart to cluster some of these things together into their own sub-classes but that would destroy old saves
    public uint saveVersion = 0;
    public uint[] mushrooms = new uint[3];
    public uint[] mushroomBlockCount = new uint[3];
    public uint[] mushroomCount = new uint[3];
    public uint sporeCountTotal = 0;
    public uint sporeCount = 0;
    public int2 farmSize = new int2(0, 0);
    public uint mushroomMultiplier = 0;
    public uint mushroomSpeed = 0;
    public bool[] autoHarvest = new bool[3];
    public uint[] growthSpeedBonus = new uint[3];
    public uint[] autoHarvestSpeed = new uint[3];
    public uint totalConverges = 0;
    public uint hivemindPointsTotal = 0;
    public uint hivemindPoints = 0;
    public bool redUnlocked = false;
    public bool blueUnlocked = false;
    public uint brownMultiplier;
    public uint redMultiplier;
    public uint blueMultiplier;
    public float hivemindPointValue;
    public bool goldenSporeUnlocked;
    public uint goldenMultiplier = 2;
    public uint goldenChanceMultiplier;
    public uint sporeMultiplier = 1;
    public CauldronSave cauldronSave = new CauldronSave();
    public uint[] potionsCount = new uint[3];
    public uint coins = 0;
    public MarketSave marketSave = new MarketSave();
    public PlinkoSave plinkoSave = new PlinkoSave();
    public List<CollectionItemSaveData> collectionItems = new List<CollectionItemSaveData>();
}

[Serializable]
public class MarketSave
{
    public bool isUnlocked = false;
    public CurrencyVisualizer.Currency sellItem = CurrencyVisualizer.Currency.BrownMushroom;
    public uint sellPrice = 0;
    public bool sellSoldOut = false;
    public CurrencyVisualizer.Currency buyItem = CurrencyVisualizer.Currency.Spore;
    public uint buyPrice = 99999;
    public bool buySoldOut = false;
    public uint shopTicks = 2;
    public CollectionItemSaveData sellItemData = new CollectionItemSaveData();
}

[Serializable]
public class PlinkoSave
{
    public uint balls = 5;
    public uint ballSoftCap = 10;
    public uint score = 0;
    public float ballProgress = 0;
    public float ballRegenSpeed = 1;
    public uint ballRegenAmount = 1;
    public bool goldenBallsUnlocked = false;
    public bool goldenPegsUnlocked = false;
    public bool autofireUnlocked = false;
}