using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreMaster : MonoBehaviour
{
    public static ScoreMaster instance;

    [SerializeField] private TextMeshProUGUI brownMushroomText;
    [SerializeField] private TextMeshProUGUI redMushroomText;
    [SerializeField] private TextMeshProUGUI blueMushroomText;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMushroom(MushroomBlock.MushroomType mushroomType, bool golden = false)
    {
        uint amount = 1 + SaveSystem.instance.GetSaveFile().mushroomMultiplier;
        amount *= (golden ? SaveSystem.instance.GetSaveFile().goldenMultiplier : 1);
        // Debug.Log("Adding " + amount + " " + mushroomType + " mushrooms");
        SaveSystem.instance.GetSaveFile().mushrooms[(int)mushroomType] +=amount;
        SaveSystem.instance.GetSaveFile().mushroomCount[(int)mushroomType] += amount;

        switch (mushroomType)
        {
            case MushroomBlock.MushroomType.Brown:
                UpdateBrownText();
                break;
            case MushroomBlock.MushroomType.Red:
                UpdateRedText();
                break;
            case MushroomBlock.MushroomType.Blue:
                UpdateBlueText();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mushroomType), mushroomType, null);
        }

    }

    public void UpdateMushroomText()
    {
        UpdateBrownText();
        UpdateRedText();
        UpdateBlueText();
    }

    public void UpdateBlueText()
    {
        blueMushroomText.text = SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Blue].ToString("N0");
        GameMaster.instance.blueUpgradeMaster.UpdateButtons();
    }

    public void UpdateRedText()
    {
        redMushroomText.text = SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Red].ToString("N0");
        GameMaster.instance.redUpgradeMaster.UpdateButtons();
    }

    public void UpdateBrownText()
    {
        brownMushroomText.text = SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Brown].ToString("N0");
        GameMaster.instance.brownUpgradeMaster.UpdateButtons();
    }

    public bool SpendMushrooms(MushroomBlock.MushroomType type, uint amount)
    {
        if (SaveSystem.instance.GetSaveFile().mushrooms[(int)type] < amount) return false;
        SaveSystem.instance.GetSaveFile().mushrooms[(int)type] -= amount;
        UpdateMushroomText();
        return true;
    }

    public void Reset()
    {
        SaveSystem.instance.GetSaveFile().mushrooms = new uint[3];
        UpdateMushroomText();
    }
}