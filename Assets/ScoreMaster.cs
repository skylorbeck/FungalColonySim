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
    [SerializeField] private Transform brownMushroomBanner;
    [SerializeField] private bool brownShowing = false;
    [SerializeField] private TextMeshProUGUI redMushroomText;
    [SerializeField] private Transform redMushroomBanner;
    [SerializeField] private bool redShowing = false;
    [SerializeField] private TextMeshProUGUI blueMushroomText;
    [SerializeField] private Transform blueMushroomBanner;
    [SerializeField] private bool blueShowing = false;

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
        if (!brownShowing && SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Brown] > 0)
        {
            brownShowing = true;
            brownMushroomBanner.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBounce);
        }

        if (!redShowing && SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Red] > 0)
        {
            redShowing = true;
            redMushroomBanner.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBounce);
        }

        if (!blueShowing && SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Blue] > 0)
        {
            blueShowing = true;
            blueMushroomBanner.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBounce);
        }
    }

    void FixedUpdate()
    {

    }

    public void AddMushroom(MushroomBlock.MushroomType mushroomType, uint amt = 1)
    {
        SaveSystem.instance.GetSaveFile().mushrooms[(int)mushroomType] +=
            amt + SaveSystem.instance.GetSaveFile().mushroomMultiplier;
        SaveSystem.instance.GetSaveFile().mushroomCount[(int)mushroomType] +=
            amt + SaveSystem.instance.GetSaveFile().mushroomMultiplier;

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
        blueMushroomText.text = "x" + SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Blue];
        GameMaster.instance.blueUpgradeMaster.UpdateButtons();
    }

    public void UpdateRedText()
    {
        redMushroomText.text = "x" + SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Red];
        GameMaster.instance.redUpgradeMaster.UpdateButtons();
    }

    public void UpdateBrownText()
    {
        brownMushroomText.text = "x" + SaveSystem.instance.GetSaveFile().mushrooms[(int)MushroomBlock.MushroomType.Brown];
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