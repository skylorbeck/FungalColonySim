using System;
using TMPro;
using UnityEngine;

public class ScoreMaster : MonoBehaviour
{
    public static ScoreMaster instance;

    [SerializeField] private TextMeshProUGUI brownMushroomText;
    [SerializeField] private TextMeshProUGUI redMushroomText;
    [SerializeField] private TextMeshProUGUI blueMushroomText;

    public void Reset()
    {
        SaveSystem.save.stats.mushrooms = new uint[3];
        UpdateMushroomText();
    }

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
        uint amount = 1 + SaveSystem.save.farmSave.upgrades.mushroomMultiplier;
        amount *= golden ? SaveSystem.save.farmSave.upgrades.goldenMultiplier : 1;
        // Debug.Log("Adding " + amount + " " + mushroomType + " mushrooms");
        // Debug.Log(golden);
        SaveSystem.save.stats.mushrooms[(int)mushroomType] += amount;
        SaveSystem.save.statsTotal.mushrooms[(int)mushroomType] += amount;

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
        blueMushroomText.text = SaveSystem.save.stats.mushrooms[(int)MushroomBlock.MushroomType.Blue].ToString("N0");
        GameMaster.instance.blueUpgradeMaster.UpdateButtons();
    }

    public void UpdateRedText()
    {
        redMushroomText.text = SaveSystem.save.stats.mushrooms[(int)MushroomBlock.MushroomType.Red].ToString("N0");
        GameMaster.instance.redUpgradeMaster.UpdateButtons();
    }

    public void UpdateBrownText()
    {
        brownMushroomText.text = SaveSystem.save.stats.mushrooms[(int)MushroomBlock.MushroomType.Brown].ToString("N0");
        GameMaster.instance.brownUpgradeMaster.UpdateButtons();
    }

    public bool SpendMushrooms(MushroomBlock.MushroomType type, uint amount)
    {
        if (SaveSystem.save.stats.mushrooms[(int)type] < amount) return false;
        SaveSystem.save.stats.mushrooms[(int)type] -= amount;
        UpdateMushroomText();
        return true;
    }
}