using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveSystem: MonoBehaviour
{
    public uint saveVersion = 1;
    public uint[] mushrooms = new uint[3];//per run
    public uint[] mushroomBlockCount = new uint[3];
    public uint[] mushroomCount = new uint[3];//total 
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
    //hivemind skills
    public uint brownMultiplier;
    public uint redMultiplier;
    public uint blueMultiplier;
    //hivemind meta
    public float hivemindPointValue;
    public bool goldenSporeUnlocked;
    //version 1 end
    
  

    public void WipeSave()
    {
        SaveFile save = new SaveFile
        {
            saveVersion = this.saveVersion
        };
        string json = JsonUtility.ToJson(save);
        
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

        
        Load();
    }
    
    public void Save()
    {
        string json = JsonUtility.ToJson(new SaveFile(this));
        string path = Application.persistentDataPath + "/savefile.json";

        File.WriteAllText(path, json);
    }

    public bool Load() //returns success
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
            this.WipeSave();
            return false;
        }

        string json = File.ReadAllText(path);
        SaveFile save = JsonUtility.FromJson<SaveFile>(json);
        
        if (save.saveVersion <2)
        {
            this.WipeSave();
            return false;
        }
        
        mushrooms = save.mushrooms;
        mushroomBlockCount = save.mushroomBlockCount;
        for (var i = 0; i < mushroomBlockCount.Length; i++)
        {
            if (mushroomBlockCount[i]<1)
            {
                mushroomBlockCount[i] = 1;
            }
        }
        mushroomCount = save.mushroomCount;
        sporeCountTotal = save.sporeCountTotal;
        sporeCount = save.sporeCount;
        farmSize = save.farmSize;
        mushroomMultiplier = save.mushroomMultiplier;
        mushroomSpeed = save.mushroomSpeed;
        autoHarvest = save.autoHarvest;
        growthSpeedBonus = save.growthSpeedBonus;
        autoHarvestSpeed = save.autoHarvestSpeed;
        totalConverges = save.totalConverges;
        hivemindPointsTotal = save.hivemindPointsTotal;
        hivemindPoints = save.hivemindPoints;
        redUnlocked = save.redUnlocked;
        blueUnlocked = save.blueUnlocked;
        hivemindPointValue = save.hivemindPointValue;
        goldenSporeUnlocked = save.goldenSporeUnlocked;
        brownMultiplier = save.brownMultiplier;
        redMultiplier = save.redMultiplier;
        blueMultiplier = save.blueMultiplier;
        
        return true;
    }
    public bool SpendSpores(uint amount)
    {
        if (GameMaster.instance.SaveSystem.sporeCount < amount) return false;
        GameMaster.instance.SaveSystem.sporeCount-= amount;
        return true;
    }
    
    public bool SpendHivemindPoints(uint amount)
    {
        if (GameMaster.instance.SaveSystem.hivemindPoints < amount) return false;
        GameMaster.instance.SaveSystem.hivemindPoints-= amount;
        return true;
    }

    public class SaveFile
    {
        public SaveFile()
        {
            saveVersion = 0;
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
            blueMultiplier= 0;
             hivemindPointValue= 0;
             goldenSporeUnlocked = false;
        }

        public SaveFile(SaveSystem save)
        {
            saveVersion = save.saveVersion;
            mushrooms = save.mushrooms;
            mushroomBlockCount = save.mushroomBlockCount;
            mushroomCount = save.mushroomCount;
            sporeCountTotal = save.sporeCountTotal;
            sporeCount = save.sporeCount;
            farmSize = save.farmSize;
            mushroomMultiplier = save.mushroomMultiplier;
            mushroomSpeed = save.mushroomSpeed;
            autoHarvest = save.autoHarvest;
            growthSpeedBonus = save.growthSpeedBonus;
            autoHarvestSpeed = save.autoHarvestSpeed;
            totalConverges = save.totalConverges;
            redUnlocked = save.redUnlocked;
            blueUnlocked = save.blueUnlocked;
            hivemindPointsTotal = save.hivemindPointsTotal;
            hivemindPoints = save.hivemindPoints;
            hivemindPointValue = save.hivemindPointValue;
            goldenSporeUnlocked = save.goldenSporeUnlocked;
            brownMultiplier = save.brownMultiplier;
            redMultiplier = save.redMultiplier;
            blueMultiplier = save.blueMultiplier;
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

        
    }
}