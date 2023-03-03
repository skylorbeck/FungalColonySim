using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class MushroomBlock : Block
{
    [SerializeField] private MushroomType mushroomType;
    private MushroomType plantTypePrevious;
    [SerializeField] private bool isDead;
    [SerializeField] public bool isGrowing;
    [SerializeField] public bool isGrown;
    [SerializeField] private float growthTimer;
    [SerializeField] private float growthTime;
    [SerializeField] private float verticalOffset;
    [SerializeField] private float harvestTimer;
    [SerializeField] private float harvestTime = 1;
    [SerializeField] private ParticleSystem goldenParticles;
    public bool isGolden;

    private GameObject mushroomPop;

    void Start()
    {
        UpdateSprite();
        UpdateGrowthTime();

        if (mushroomPop == null)
        {
            mushroomPop = new GameObject();
            mushroomPop.transform.SetParent(transform);
            SpriteRenderer mushSprite = mushroomPop.AddComponent<SpriteRenderer>();
            mushSprite.sprite =  GetMushroomSprite(mushroomType);
            mushSprite.sortingOrder = 100;
            if (mushroomType is MushroomType.Blue or MushroomType.Red)
            {
                mushSprite.transform.rotation = Quaternion.Euler(0, 0, -20);
            }

            mushroomPop.transform.localScale = Vector3.zero;
            mushroomPop.transform.position = transform.position;

        }
    }

    protected override void Update()
    {
        if (isGrowing)
        {
            growthTimer += Time.deltaTime +
                           (SaveSystem.instance.GetSaveFile().growthSpeedBonus[(int)mushroomType] * Time.deltaTime *
                            0.1f) + (SaveSystem.instance.GetSaveFile().mushroomSpeed * Time.deltaTime * 0.05f);
            spriteRenderer.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, growthTimer / growthTime);

            spriteRenderer.transform.localPosition =
                Vector3.Lerp(Vector3.down * verticalOffset, Vector3.zero, growthTimer / growthTime);
            if (growthTimer >= growthTime)
            {
                growthTimer = 0;
                isGrowing = false;
                isGrown = true;
                UpdateSprite();
            }
        }
        else if (isGrown && SaveSystem.instance.GetSaveFile().autoHarvest[(int)mushroomType])
        {
            harvestTimer += Time.deltaTime +
                            (SaveSystem.instance.GetSaveFile().autoHarvestSpeed[(int)mushroomType] * Time.deltaTime *
                             0.1f);
            if (harvestTimer >= harvestTime)
            {
                harvestTimer = 0;
                Harvest();
            }
        }

        if (mushroomType != plantTypePrevious)
        {
            plantTypePrevious = mushroomType;
            UpdateSprite();
            UpdateGrowthTime();
        }

        base.Update();

    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = GetMushroomSprite(mushroomType);
        if (mushroomType == MushroomType.Blue || mushroomType == MushroomType.Red)
        {
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, -20);
        }
        this.name = "Mushroom " + mushroomType + " " + blockPos;

    }

    private void UpdateGrowthTime()
    {
        switch (mushroomType)
        {
            case MushroomType.Brown:
                growthTime = 5;
                break;
            case MushroomType.Red:
                growthTime = 7;
                break;
            case MushroomType.Blue:
                growthTime = 12;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isGrown)
        {
            Harvest();
        }
        else if (isDead)
        {
            isDead = false;
            UpdateSprite();
        }
        else
        {
            growthTimer += 1;
        }

        base.OnPointerClick(eventData);
    }

    private void Harvest()
    {
        if (Mathf.Abs(transform.position.x) < 100)
        {
            mushroomPop.transform.DOKill();
            mushroomPop.transform.position = transform.position;
            mushroomPop.transform.localScale = Vector3.zero;

            mushroomPop.transform.DOLocalJump(mushroomPop.transform.localPosition + Vector3.up * 2, 1, 1, 1)
                .onComplete += () =>
                mushroomPop.transform.DOScale(Vector3.zero, 0.25f);
            mushroomPop.transform.DOScale(Vector3.one * 0.5f, 0.5f).onComplete +=
                () => ScoreMaster.instance.AddMushroom(mushroomType);
            SFXMaster.instance.PlayMushPop();
        }
        else
        {
            ScoreMaster.instance.AddMushroom(mushroomType);
        }

        if (isGolden)
        {
            ScoreMaster.instance.AddMushroom(mushroomType ,SaveSystem.instance.GetSaveFile().goldenMultiplier);
        }

        isGolden = SaveSystem.instance.GetSaveFile().goldenSporeUnlocked && Random.value < 0.02f * SaveSystem.instance.GetSaveFile().goldenChanceMultiplier;//TODO make this an upgrade
        if (isGolden)
        {
            goldenParticles.Play();
        }
        else
        {
            goldenParticles.Stop();
        }

        isGrowing = true;
        isGrown = false;
    }
    

    public void PlaceBlock(int3 blockPos, MushroomType mushroomType)
    {
        base.PlaceBlock(blockPos);
        this.mushroomType = mushroomType;
        UpdateSprite();
    }


    public enum MushroomType
    {
        Brown,
        Red,
        Blue
    }

    public static Sprite GetMushroomSprite(MushroomType mushroomType)
    {
        return Resources.Load<Sprite>("Sprites/Blocks/Mushroom/" + mushroomType);
    }
}