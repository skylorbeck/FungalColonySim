using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

public class SaveSystem: MonoBehaviour
{
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

    public void WipeSave()
    {
        string json = JsonUtility.ToJson(new SaveFile());
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Load();
    }
    
    public void Save()
    {
        string json = JsonUtility.ToJson(new SaveFile(this));
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public bool Load() //returns success
    {
        if (!File.Exists(Application.persistentDataPath + "/savefile.json"))
        {
            return false;
        }

        string json = File.ReadAllText(Application.persistentDataPath + "/savefile.json");
        SaveFile save = JsonUtility.FromJson<SaveFile>(json);
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
        return true;
    }


    public class SaveFile
    {
        public SaveFile()
        {
            
        }
        public SaveFile(SaveSystem save)
        {
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
        }
        
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
        
    }
}