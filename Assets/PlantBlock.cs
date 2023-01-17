using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBlock : Block
{
    [SerializeField] private PlantType plantType;
    private PlantType plantTypePrevious;
    [SerializeField] private bool isDead;
    [SerializeField] private bool isGrowing;
    [SerializeField] private float growthTimer;
    [SerializeField] private float growthTime;
    [SerializeField] private float verticalOffset;


    void Start()
    {
        UpdateSprite();
    }

    protected override void Update()
    {
        if (plantType != PlantType.None)
        {
            if (isGrowing)
            {
                growthTimer += Time.deltaTime;
                spriteRenderer.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, growthTimer / growthTime);
               
                spriteRenderer.transform.localPosition = Vector3.Lerp( Vector3.down * verticalOffset,Vector3.zero, growthTimer / growthTime);
                if (growthTimer >= growthTime)
                {
                    growthTimer = 0;
                    isGrowing = false;
                    UpdateSprite();
                }
            }
        }

        if (plantType != plantTypePrevious)
        {
            plantTypePrevious = plantType;
            UpdateSprite();
        }
        
        base.Update();

    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = plantType == PlantType.None
            ? null
            : Resources.Load<Sprite>("Sprites/Blocks/Plants/" + plantType);
        this.name = "PlantBlock " + plantType +" "+ blockPos;

    }

    void FixedUpdate()
    {

    }

    private enum PlantType
    {
        None,
        Tree,
        Bush,
        Treestump
    }
}