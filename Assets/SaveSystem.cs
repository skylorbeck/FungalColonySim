using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
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

    public void Save()
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
            saveFile.sellItem = CurrencyVisualizer.Currency.BrownMushroom;
            saveFile.buyItem = CurrencyVisualizer.Currency.Spore;
            saveFile.sellPrice = 0;
            saveFile.buyPrice = 99999;
            saveFile.shopTicks = 2;
            if (saveFile.goldenMultiplier <= 1)
            {
                saveFile.goldenMultiplier = 2;
            }
            saveFile.spoonEnchanted = false;
            saveFile.potentShrooms = false;
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

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    public void Reset()
    {
        saveFile = new SaveFile();
    }

    [Serializable]
    public class SaveFile
    {
        public SaveFile()
        {
            saveVersion = 3;
            mushrooms = new uint[3];
            mushroomBlockCount = new uint[3];
            mushroomCount = new uint[3];
            sporeCountTotal = 0;
            sporeCount = 0;
            farmSize = new int2(0, 0);
            mushroomMultiplier = 0;
            mushroomSpeed = 0;
            autoHarvest = new bool[3];
            growthSpeedBonus = new uint[3];
            autoHarvestSpeed = new uint[3];
            totalConverges = 0;
            hivemindPointsTotal = 0;
            hivemindPoints = 0;
            redUnlocked = false;
            blueUnlocked = false;
            brownMultiplier = 0;
            redMultiplier = 0;
            blueMultiplier = 0;
            hivemindPointValue = 0;
            goldenSporeUnlocked = false;
            goldenMultiplier = 2;
            goldenChanceMultiplier = 1;
            cauldronSave = new CauldronSave();
            potionsCount = new uint[3];
            coins = 0;
            sellItem = CurrencyVisualizer.Currency.BrownMushroom;
            buyItem = CurrencyVisualizer.Currency.Spore;
            sellPrice = 0;
            buyPrice = 99999;
            shopTicks = 2;
            spoonEnchanted = false;
            potentShrooms = false;
        }

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
        public CauldronSave cauldronSave = new CauldronSave();
        public uint[] potionsCount = new uint[3];
        public uint coins = 0;
        public CurrencyVisualizer.Currency sellItem = CurrencyVisualizer.Currency.BrownMushroom;
        public uint sellPrice = 0;
        public CurrencyVisualizer.Currency buyItem = CurrencyVisualizer.Currency.Spore;
        public uint buyPrice = 99999;
        public uint shopTicks = 2;
        public bool spoonEnchanted = false;
        public bool potentShrooms = false;
    }
}