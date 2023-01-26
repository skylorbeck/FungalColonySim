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
    
    public bool isWorldCreated = false;
    
    public float floatDistance = 0.5f;
    public bool floatX = true;
    public bool floatY = true;
    
    public int3 lastMousePos;

    public GameObject enrichButton;
    
    public TextMeshProUGUI MushPerSecText;
    
    
    
    public MushroomBlock.MushroomType currentMushroomType = MushroomBlock.MushroomType.Brown;

    async void Start()
    {
        biomeBlocks = new List<BiomeBlock>();
        mushroomBlocks = new List<MushroomBlock>();
        allBlocks = new List<Block>();
        dirtBlocks = new List<BiomeBlock>();
        await CreateWorld();
    }

    void Update()
    {

        if (Math.Abs(transform.position.x)<100)
        {
            int3 mousePos = GetMousePos();

            foreach (Block block in allBlocks)
            {
                block.SetLightPos(mousePos);
                block.SetBlockOffset(new Vector3(0, Mathf.Sin(Time.time + (floatX?block.blockPos.x:0) + (floatY?block.blockPos.y:0))*floatDistance, 0));
            }
            selectionBlock.SetBlockPos(mousePos);

        }
        
        MushPerSecText.text = "MPS: " + GetMushPerSec();
    }

    private float GetMushPerSec()
    {
        float mushPerSec = 0;
        float growthTimeWithHarvestTime;
        switch (currentMushroomType)
        {
            case MushroomBlock.MushroomType.Brown:
                growthTimeWithHarvestTime = 6f;
                break;
            case MushroomBlock.MushroomType.Red:
                growthTimeWithHarvestTime = 8f;
                break;
            case MushroomBlock.MushroomType.Blue:
                growthTimeWithHarvestTime = 13f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (GameMaster.instance.SaveSystem.autoHarvest[(int)currentMushroomType])
        {
            mushPerSec = ((GameMaster.instance.SaveSystem.autoHarvestSpeed[(int)currentMushroomType]*0.1f)
                          +(GameMaster.instance.SaveSystem.growthSpeedBonus[(int)currentMushroomType]*5*0.1f)
                          +(GameMaster.instance.SaveSystem.mushroomSpeed*0.05f)
                          
                          +(1 + GameMaster.instance.SaveSystem.mushroomBlockCount[(int)currentMushroomType])
                          
                          * GameMaster.instance.SaveSystem.mushroomBlockCount[(int)currentMushroomType]) / growthTimeWithHarvestTime;
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
        uint cost = (uint)Math.Pow(GameMaster.instance.SaveSystem.mushroomBlockCount[(int)currentMushroomType], 2);
        // Debug.Log(cost);
        if (GameMaster.instance.SaveSystem.mushrooms[(int)currentMushroomType] < cost)
        {
            return;
        }
        bool success = false;

        if (dirtBlocks.Count > 0)
        {
            BiomeBlock biomeBlock = dirtBlocks[0];

            BiomeBlock newBlock = Instantiate(biomeBlockPrefab, transform.position, Quaternion.identity);
            newBlock.transform.SetParent(this.transform);
            newBlock.PlaceBlock(biomeBlock.blockPos, Biome.Grass);
            biomeBlocks.Add(newBlock);
            biomeBlocks.Remove(biomeBlock);
            dirtBlocks.Remove(biomeBlock);
            allBlocks.Remove(biomeBlock);
            allBlocks.Add(newBlock);
            biomeBlock.RemoveBlock();

            MushroomBlock block =
                Instantiate(mushroomBlockPrefab, new Vector3(-10000, -10000, 0), Quaternion.identity, transform);
            block.PlaceBlock(biomeBlock.blockPos + new int3(0, 0, 1), currentMushroomType);
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
        ScoreMaster.instance.SpendMushrooms(currentMushroomType,cost);
        GameMaster.instance.SaveSystem.mushroomBlockCount[(int)currentMushroomType]++;
        switch (currentMushroomType)
        {
            case MushroomBlock.MushroomType.Brown:
                ScoreMaster.instance.UpdateBrownText();
                break;
            case MushroomBlock.MushroomType.Red:
                ScoreMaster.instance.UpdateRedText();
                break;
            case MushroomBlock.MushroomType.Blue:
                ScoreMaster.instance.UpdateBlueText();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (dirtBlocks.Count == 0)
        {
            enrichButton.SetActive(false);
        }
    }
    
    void FixedUpdate()
    {

    }

    public async Task CreateWorld()
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
                } else if (GameMaster.instance.SaveSystem.mushroomBlockCount[(int)currentMushroomType]>blockCount)
                {
                    biome = Biome.Grass;                    
                    blockCount++;
                    MushroomBlock mushroomBlock = Instantiate(mushroomBlockPrefab, new Vector3(-10000, -10000, 0), Quaternion.identity, transform);
                    mushroomBlock.PlaceBlock(new int3(x, y,1),currentMushroomType);
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

        switch (currentMushroomType)
        {
            case MushroomBlock.MushroomType.Brown:
                ScoreMaster.instance.UpdateBrownText();
                break;
            case MushroomBlock.MushroomType.Red:
                ScoreMaster.instance.UpdateRedText();
                break;
            case MushroomBlock.MushroomType.Blue:
                ScoreMaster.instance.UpdateBlueText();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (dirtBlocks.Count ==0)
        {
            enrichButton.SetActive(false);
            return;
        }
        enrichButton.SetActive(true);
        isWorldCreated = true;

    }

    public async Task DissolveWorld()
    {
        isWorldCreated = false;
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
        GameMaster.instance.SaveSystem.mushroomBlockCount[(int)currentMushroomType] = 1;
        
    }
}

