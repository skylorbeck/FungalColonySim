using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class MushroomBlock : Block, IPointerEnterHandler
{
    public enum MushroomType
    {
        Brown,
        Red,
        Blue
    }

    [SerializeField] private MushroomType mushroomType;
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
    private MushroomType plantTypePrevious;

    void Start()
    {
        UpdateSprite();
        UpdateGrowthTime();

        if (mushroomPop == null)
        {
            mushroomPop = new GameObject();
            mushroomPop.transform.SetParent(transform);
            SpriteRenderer mushSprite = mushroomPop.AddComponent<SpriteRenderer>();
            mushSprite.sprite = GetMushroomSprite(mushroomType);
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
                           (SaveSystem.instance.GetSaveFile().farmSave.upgrades.growthSpeedBonus[(int)mushroomType] *
                            Time.deltaTime * 0.1f) +
                           (SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomSpeed * Time.deltaTime *
                            0.05f) +
                           (SaveSystem.instance.GetSaveFile().GetCollectionMultiplier() * Time.deltaTime * 0.01f);
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
        else if (isGrown && SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvest[(int)mushroomType])
        {
            harvestTimer += Time.deltaTime +
                            (SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvestSpeed[(int)mushroomType] *
                             Time.deltaTime *
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isGrown && Input.GetMouseButton(0))
        {
            Harvest();
        }
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
        if (spriteRenderer.isVisible)
        {
            mushroomPop.transform.DOKill();
            mushroomPop.transform.position = transform.position;
            mushroomPop.transform.localScale = Vector3.zero;

            mushroomPop.transform.DOLocalJump(mushroomPop.transform.localPosition + Vector3.up * 2, 1, 1, 1)
                .onComplete += () =>
                mushroomPop.transform.DOScale(Vector3.zero, 0.25f);
            mushroomPop.transform.DOScale(Vector3.one * 0.5f, 0.5f);
            SFXMaster.instance.PlayMushPop();
        }

        ScoreMaster.instance.AddMushroom(mushroomType, isGolden);

        isGolden = SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenSporeUnlocked && Random.value <
            0.02f * SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenChanceMultiplier;
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


    public void PlaceBlock(int3 blockPos, MushroomType mushroomType, bool isInstant = true)
    {
        base.PlaceBlock(blockPos, isInstant);
        this.mushroomType = mushroomType;
        UpdateSprite();
    }

    public static Sprite GetMushroomSprite(MushroomType mushroomType)
    {
        return Resources.Load<Sprite>("Sprites/Blocks/Mushroom/" + mushroomType);
    }
}