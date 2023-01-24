using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

public class SaveSystem: MonoBehaviour
{
    public uint BrownMushrooms = 0;
    public uint RedMushrooms = 0;
    public uint BlueMushrooms = 0;
    public uint mushroomBlockCount = 1;
    public uint[] mushroomCount = new uint[3];
    public uint sporeCountTotal = 0;//TODO use sporeCountTotal to calculate hivemind points
    public uint sporeCount = 0;
    public int2 farmSize = new int2(0, 0);
    public uint mushroomMultiplier = 0;
    public uint mushroomSpeed = 0;
    public bool autoHarvest = false;
    public uint growthSpeedBonus = 0;
    public uint autoHarvestSpeed =0;
    public uint totalConverges = 0;
    public uint hivemindPointsSpent = 0;
    public bool redUnlocked = false;
    public bool blueUnlocked = false;

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
        BrownMushrooms = save.BrownMushrooms;
        RedMushrooms = save.RedMushrooms;
        BlueMushrooms = save.BlueMushrooms;
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
        hivemindPointsSpent = save.hivemindPointsSpent;
        redUnlocked = save.redUnlocked;
        blueUnlocked = save.blueUnlocked;
        return true;
    }


    public class SaveFile
    {
        public SaveFile(SaveSystem save)
        {
            BrownMushrooms = save.BrownMushrooms;
            RedMushrooms = save.RedMushrooms;
            BlueMushrooms = save.BlueMushrooms;
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
            hivemindPointsSpent = save.hivemindPointsSpent;
            redUnlocked = save.redUnlocked;
            blueUnlocked = save.blueUnlocked;
        }
        
        public uint BrownMushrooms = 0;
        public uint RedMushrooms = 0;
        public uint BlueMushrooms = 0;
        public uint mushroomBlockCount = 1;
        public uint[] mushroomCount = new uint[3];
        public uint sporeCountTotal = 0;
        public uint sporeCount = 0;
        public int2 farmSize = new int2(0, 0);
        public uint mushroomMultiplier = 0;
        public uint mushroomSpeed = 0;
        public bool autoHarvest = false;
        public uint growthSpeedBonus = 0;
        public uint autoHarvestSpeed =0;
        public uint totalConverges = 0;
        public uint hivemindPointsSpent = 0;
        public bool redUnlocked = false;
        public bool blueUnlocked = false;
        
    }
}