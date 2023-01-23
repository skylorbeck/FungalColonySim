using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

public class BlockMaster : MonoBehaviour
{
    public BiomeBlock biomeBlockPrefab;
    public MushroomBlock mushroomBlockPrefab;
    public SelectionBlock selectionBlock;
    public List<Block> allBlocks;
    public List<BiomeBlock> biomeBlocks;
    public List<BiomeBlock> dirtBlocks;
    public List<MushroomBlock> mushroomBlocks;
    public int xMax = 5;
    public int yMax = 5;
    
    public float floatDistance = 0.5f;
    public bool floatX = true;
    public bool floatY = true;
    
    public int3 lastMousePos;

    public GameObject enrichButton;
    
    public TextMeshProUGUI MushPerSecText;
    
    void Start()
    {
        biomeBlocks = new List<BiomeBlock>();
        mushroomBlocks = new List<MushroomBlock>();
        allBlocks = new List<Block>();
        dirtBlocks = new List<BiomeBlock>();
        
        CreateTestWorld();
    }

    void Update()
    {
        int3 mousePos = GetMousePos();

        foreach (Block block in allBlocks)
        {
            block.SetLightPos(mousePos);
                block.SetBlockOffset(new Vector3(0, Mathf.Sin(Time.time + (floatX?block.blockPos.x:0) + (floatY?block.blockPos.y:0))*floatDistance, 0));
        }
        
        selectionBlock.SetBlockPos(mousePos);
        
        MushPerSecText.text = "MPS: " + GetMushPerSec();
    }

    private float GetMushPerSec()
    {
        float mushPerSec = 0;
        if (GameMaster.instance.SaveSystem.autoHarvest)
        {
            mushPerSec = ((GameMaster.instance.SaveSystem.autoHarvestSpeed*0.1f)
                          +(GameMaster.instance.SaveSystem.growthSpeedBonus*5*0.1f)
                          +(GameMaster.instance.SaveSystem.mushroomSpeed*0.05f)
                          
                          +(1 + GameMaster.instance.SaveSystem.mushroomMultiplier)
                          
                          * GameMaster.instance.SaveSystem.mushroomBlockCount) / 6f;
        }
        //truncate to 2 decimal places
        return (float) Math.Round(mushPerSec, 2);
    }

    private int3 GetMousePos()
    {
        Ray ray = GameMaster.instance.camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Block"));
        if (hit)
        {
            lastMousePos = hit.transform.GetComponent<Block>().blockPos;
        }

        return lastMousePos;
    }

    private Block GetBlockAtPosition(int3 int3)
    {
        foreach (Block block in biomeBlocks)
        {
            if (block.blockPos.x == int3.x && block.blockPos.y == int3.y && block.blockPos.z == int3.z)
            {
                return block;
            }
        }
        return null;
    }

    public void EnrichBlock()
    {
        uint cost = (uint)Math.Pow(GameMaster.instance.SaveSystem.mushroomBlockCount, 2);
        // Debug.Log(cost);
        if (GameMaster.instance.SaveSystem.BrownMushrooms < cost)
        {
            Debug.Log("Not enough mushrooms, need " + cost);
            return;
        }
        bool success = false;

        if (dirtBlocks.Count > 0)
        {
            BiomeBlock biomeBlock = dirtBlocks[0];

            BiomeBlock newBlock = Instantiate(biomeBlockPrefab, biomeBlock.transform.position, Quaternion.identity);
            newBlock.PlaceBlock(biomeBlock.blockPos, Biome.Grass);
            biomeBlocks.Add(newBlock);
            biomeBlocks.Remove(biomeBlock);
            dirtBlocks.Remove(biomeBlock);
            allBlocks.Remove(biomeBlock);
            allBlocks.Add(newBlock);
            biomeBlock.RemoveBlock();

            MushroomBlock block =
                Instantiate(mushroomBlockPrefab, new Vector3(-10000, -10000, 0), Quaternion.identity, transform);
            block.PlaceBlock(biomeBlock.blockPos + new int3(0, 0, 1), MushroomBlock.MushroomType.Brown);
            block.isGrowing = true;
            mushroomBlocks.Add(block);
            allBlocks.Add(block);
            success = true;
        }

        if (!success)
        {
            Debug.Log("No dirt blocks to enrich");
            enrichButton.SetActive(false);

            return;
        }
        // SFXMaster.instance.PlayBlockReplace();
        ScoreMaster.instance.SpendMushrooms(cost);
        GameMaster.instance.SaveSystem.mushroomBlockCount++;
        ScoreMaster.instance.UpdateMushroomText();
        if (dirtBlocks.Count == 0)
        {
            enrichButton.SetActive(false);
        }
    }
    
    void FixedUpdate()
    {

    }

    public async Task CreateTestWorld()
    {
        floatX = false;
        floatY = false;
        while (!floatX && !floatY)
        {
            floatX = Random.value > 0.5f;
            floatY = Random.value > 0.5f;
        }
        int blockCount = 0;
        for (int x = 0; x < xMax+GameMaster.instance.SaveSystem.farmSize.x; x++)
        {
            for (int y = 0; y < yMax+GameMaster.instance.SaveSystem.farmSize.y; y++)
            {
                BiomeBlock block = Instantiate(biomeBlockPrefab, new Vector3(-10000,-10000, 0), Quaternion.identity, transform);
                Biome biome = Biome.Dirt;
                if (x == 0 || y == 0 || x == xMax+GameMaster.instance.SaveSystem.farmSize.x - 1 || y == yMax+GameMaster.instance.SaveSystem.farmSize.y - 1)
                {
                    biome = Biome.Rock;
                } else if (GameMaster.instance.SaveSystem.mushroomBlockCount>blockCount)
                {
                    biome = Biome.Grass;                    
                    blockCount++;
                    MushroomBlock mushroomBlock = Instantiate(mushroomBlockPrefab, new Vector3(-10000, -10000, 0), Quaternion.identity, transform);
                    mushroomBlock.PlaceBlock(new int3(x, y,1),MushroomBlock.MushroomType.Brown);
                    mushroomBlock.isGrowing = true;
                    mushroomBlocks.Add(mushroomBlock);
                    allBlocks.Add(mushroomBlock);
                }
                block.PlaceBlock(new int3(x, y, 0), biome);
                if (biome == Biome.Dirt)
                {
                    dirtBlocks.Add(block);
                }
                allBlocks.Add(block);
                biomeBlocks.Add(block);
                await Task.Delay(100);
            }
        }
        
        ScoreMaster.instance.UpdateMushroomText();
        if (dirtBlocks.Count ==0)
        {
            enrichButton.SetActive(false);
            return;
        }
        enrichButton.SetActive(true);

    }

    public async Task DissolveWorld()
    {
        foreach (var block in mushroomBlocks)
        {
            block.isGrowing = false;
            block.isGrown = false;
        }
        foreach (var block in allBlocks)
        {
            block.RemoveBlock();
            await Task.Delay(100);
        }
        biomeBlocks.Clear();
        mushroomBlocks.Clear();
        allBlocks.Clear();
        dirtBlocks.Clear();
        GameMaster.instance.SaveSystem.mushroomBlockCount = 1;
        
    }
}
