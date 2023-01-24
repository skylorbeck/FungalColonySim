using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreMaster : MonoBehaviour
{
    public static ScoreMaster instance;

    [SerializeField] private TextMeshProUGUI brownMushroomText;
    [SerializeField] private TextMeshProUGUI enrichCostText;
    [SerializeField] private Button enrichButton;
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

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    public void AddMushroom(MushroomBlock.MushroomType mushroomType, bool silent = false)
    {
        
        switch (mushroomType)
        {
            case MushroomBlock.MushroomType.None:
                break;
            case MushroomBlock.MushroomType.Brown:
                GameMaster.instance.SaveSystem.BrownMushrooms+=1+GameMaster.instance.SaveSystem.mushroomMultiplier;
                GameMaster.instance.SaveSystem.mushroomCount[0]+=1+GameMaster.instance.SaveSystem.mushroomMultiplier;
                break;
            case MushroomBlock.MushroomType.Red:
                GameMaster.instance.SaveSystem.RedMushrooms+=1+GameMaster.instance.SaveSystem.mushroomMultiplier;
                GameMaster.instance.SaveSystem.mushroomCount[1]+=1+GameMaster.instance.SaveSystem.mushroomMultiplier;
                break;
            case MushroomBlock.MushroomType.Blue:
                GameMaster.instance.SaveSystem.BlueMushrooms+=1+GameMaster.instance.SaveSystem.mushroomMultiplier;
                GameMaster.instance.SaveSystem.mushroomCount[2]+=1+GameMaster.instance.SaveSystem.mushroomMultiplier;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mushroomType), mushroomType, null);
        }

        if (silent)
        {
            return;
        }
        UpdateMushroomText();
    }

    public void UpdateMushroomText()
    {
        brownMushroomText.text ="x"+ GameMaster.instance.SaveSystem.BrownMushrooms;
        uint cost = (uint) Mathf.Pow(GameMaster.instance.SaveSystem.mushroomBlockCount,2);
        enrichCostText.text = cost + "x";
        enrichButton.interactable = GameMaster.instance.SaveSystem.BrownMushrooms >= cost;
        UpgradeMaster.instance.UpdateButtons();

    }
    
    public bool SpendMushrooms(uint amount)
    {
        if (GameMaster.instance.SaveSystem.BrownMushrooms >= amount)
        {
            GameMaster.instance.SaveSystem.BrownMushrooms -= amount;
            UpdateMushroomText();
            return true;
        }
        return false;
    }

    public void Reset()
    {
        GameMaster.instance.SaveSystem.BrownMushrooms = 0;
        UpdateMushroomText();
    }
}
