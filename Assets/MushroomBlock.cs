using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private float harvestTime =1;

    private GameObject mushroomPop;

    void Start()
    {
        UpdateSprite();
        if (mushroomPop==null)
        {
            mushroomPop = new GameObject();
            mushroomPop.transform.SetParent(transform);
            SpriteRenderer mushSprite = mushroomPop.AddComponent<SpriteRenderer>();
            mushSprite.sprite = Resources.Load<Sprite>("Sprites/Blocks/Mushroom/" + mushroomType);
            mushSprite.sortingOrder = 100;
            mushroomPop.transform.localScale = Vector3.zero;
            mushroomPop.transform.position = transform.position;

        }
    }

    protected override void Update()
    {
        if (mushroomType != MushroomType.None)
        {
            //TODO ask the block manager what block is below and kill the mushroom if it's the wrong biome?
            if (isGrowing)
            {
                growthTimer += Time.deltaTime + (GameMaster.instance.SaveSystem.growthSpeedBonus * Time.deltaTime * 0.1f) + (GameMaster.instance.SaveSystem.mushroomSpeed * Time.deltaTime * 0.05f);
                spriteRenderer.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, growthTimer / growthTime);
               
                spriteRenderer.transform.localPosition = Vector3.Lerp( Vector3.down * verticalOffset,Vector3.zero, growthTimer / growthTime);
                if (growthTimer >= growthTime)
                {
                    growthTimer = 0;
                    isGrowing = false;
                    isGrown = true;
                    UpdateSprite();
                }
            } else if (isGrown && GameMaster.instance.SaveSystem.autoHarvest)
            {
                harvestTimer += Time.deltaTime +
                                (GameMaster.instance.SaveSystem.autoHarvestSpeed * Time.deltaTime * 0.1f);
                if (harvestTimer >= harvestTime)
                {
                    harvestTimer = 0;
                    Harvest();
                }
            }
        }

        if (mushroomType != plantTypePrevious)
        {
            plantTypePrevious = mushroomType;
            UpdateSprite();
        }
        
        base.Update();

    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = mushroomType == MushroomType.None
            ? null
            : Resources.Load<Sprite>("Sprites/Blocks/Mushroom/" + mushroomType);
        this.name = "Mushroom " + mushroomType +" "+ blockPos;

    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isGrown)
        {
            Harvest();
        } else if (isDead)
        {
            mushroomType = MushroomType.None;
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
        if (Mathf.Abs(transform.position.x)<100)
        {
            mushroomPop.transform.DOLocalJump(mushroomPop.transform.localPosition + Vector3.up * 2, 1, 1, 1).onComplete += () =>
                mushroomPop.transform.DOScale(Vector3.zero, 0.25f).onComplete += () =>
                    mushroomPop.transform.position = transform.position;
            mushroomPop.transform.localScale = Vector3.zero;
            mushroomPop.transform.DOScale(Vector3.one * 0.5f, 0.5f).onComplete +=
                () => ScoreMaster.instance.AddMushroom(mushroomType);
        }
        else
        {
            ScoreMaster.instance.AddMushroom(mushroomType,true);
        }
        
        // Destroy(mushroomPop, 1.5f);

        isGrowing = true;
        isGrown = false;
        SFXMaster.instance.PlayMushPop();
    }

    void FixedUpdate()
    {

    }
    
    public void PlaceBlock(int3 blockPos, MushroomType mushroomType)
    {
        base.PlaceBlock(blockPos);
        this.mushroomType = mushroomType;
        UpdateSprite();
    }

   
    public enum MushroomType
    {
        None,
        Red,
        Brown,
        Blue
    }
}