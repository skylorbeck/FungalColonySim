using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
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

    public void Reset()
    {
        saveFile = new SaveFile();
    }

    public SaveFile GetSaveFile()
    {
        return saveFile;
    }


    public bool SpendSpores(uint amount)
    {
        if (instance.saveFile.stats.spores < amount) return false;
        instance.saveFile.stats.spores -= amount;
        return true;
    }

    public bool SpendHivemindPoints(uint amount)
    {
        if (instance.saveFile.stats.skillPoints < amount) return false;
        instance.saveFile.stats.skillPoints -= amount;
        return true;
    }

    public bool SpendCoins(uint amount)
    {
        if (instance.saveFile.marketSave.coins < amount) return false;
        instance.saveFile.marketSave.coins -= amount;
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
        if (saveFile.saveVersion < 4)
        {
            Reset();
            saveFile.saveVersion = 4;
        }

        if (saveFile.farmSave.upgrades.goldenMultiplier < 2)
        {
            saveFile.farmSave.upgrades.goldenMultiplier = 2;
        }

        if (saveFile.saveVersion < 5)
        {
            saveFile.saveVersion = 5;
            int2 farmSize = saveFile.farmSave.farmSize;
            if (farmSize.x > 7)
            {
                farmSize.x = 7;
            }

            if (farmSize.y > 7)
            {
                farmSize.y = 7;
            }
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

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }
}

[Serializable]
public class SaveFile
{
    public uint saveVersion = 5;
    public FarmSave farmSave = new FarmSave();
    public StatTracking stats = new StatTracking();
    public StatTracking statsTotal = new StatTracking();
    public CauldronSave cauldronSave = new CauldronSave();
    public MarketSave marketSave = new MarketSave();
    public PlinkoSave plinkoSave = new PlinkoSave();
    public List<CollectionItemSaveData> collectionItems = new List<CollectionItemSaveData>();

    public float GetCollectionMultiplier()
    {
        return collectionItems.Count * 0.1f;
    }
}

[Serializable]
public class FarmSave
{
    public int2 farmSize = new int2(0, 0);
    public uint[] mushroomBlockCount = new uint[3];
    public FarmUpgrades upgrades = new FarmUpgrades();
}

[Serializable]
public class FarmUpgrades
{
    public uint mushroomMultiplier = 0;
    public uint mushroomSpeed = 0;
    public bool[] autoHarvest = new bool[3];
    public uint[] growthSpeedBonus = new uint[3];
    public uint[] autoHarvestSpeed = new uint[3];
    public bool redUnlocked = false;
    public bool blueUnlocked = false;
    public uint brownMultiplier;
    public uint redMultiplier;
    public uint blueMultiplier;
    public bool goldenSporeUnlocked;
    public uint goldenMultiplier = 2;
    public uint goldenChanceMultiplier;
    public uint sporeMultiplier = 1;
}

[Serializable]
public class StatTracking
{
    public uint[] mushrooms = new uint[3];
    public uint spores = 0;
    public uint converges = 0;

    public uint skillPoints = 0;
    //TODO more stats
    //cauldron clicks
    //sells
    //buys
}

[Serializable]
public class MarketSave
{
    public bool isUnlocked = false;
    public uint coins = 0;
    public uint[] potionsCount = new uint[3];
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