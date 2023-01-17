using System;using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;


public class BiomeBlock : Block
{
    [SerializeField] public Biome biome;
    private Biome biomePrevious;

    void Start()
    {
        UpdateSprite();
    }

    protected override void Update()
    {
        if (biome != biomePrevious)
        {
            biomePrevious = biome;
            UpdateSprite();
        }
        base.Update();
    }

    private void UpdateSprite()
    {
        float sum = blockPos.x + blockPos.y + blockPos.z;
        string path = "Sprites/Blocks/" + biome + Mathf.FloorToInt(Mathf.Abs(sum % 3));
        this.name = "BiomeBlock "+ biome +" "+ blockPos;
        spriteRenderer.sprite = Resources.Load<Sprite>(path);
    }
    
    public void PlaceBlock(int3 blockPos, Biome biome)
    {
        PlaceBlock(blockPos);
        this.biome = biome;
        biomePrevious = biome;
        UpdateSprite();
    }
  
    void FixedUpdate()
    {
        
    }
}

public enum Biome
{
    Rock,
    Dirt,
    Grass,
    RockyGrass,
}
