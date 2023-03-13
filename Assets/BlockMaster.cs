using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public bool floatFlipped = false;
    
    public int3 lastMousePos;

    public GameObject enrichButton;
    
    public TextMeshProUGUI MushPerSecText;
    public Image mushPerSecTimerImage;
    public float timer = 0;
    public float seconds = 0;
    public float mushPerSec = 0;
    public float mushPerTenSec = 0;
    public uint mushOneSecondAgo = 0;
    public uint mushTenSecondAgo = 0;

    public MushroomBlock.MushroomType currentMushroomType = MushroomBlock.MushroomType.Brown;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveSystem.instance != null);
        // yield return new WaitUntil(() => SaveSystem.instance.GetSaveFile() != null);
        yield return new WaitUntil(() => SaveSystem.instance.loaded);
        biomeBlocks = new List<BiomeBlock>();
        mushroomBlocks = new List<MushroomBlock>();
        allBlocks = new List<Block>();
        dirtBlocks = new List<BiomeBlock>();
        enrichButton.SetActive(false);
        yield return StartCoroutine(CreateWorld());
        UpdateMushPerSec();
        UpdateMushPerTenSec();
        UpdateMushPerTenSec();
        
    }

    void Update()
    {
        //Is this more performant than just checking for the absolute x pos?
        // if (Math.Abs(transform.position.x)<100)
        var thisMode = currentMushroomType switch
        {
            MushroomBlock.MushroomType.Brown => ModeMaster.Gamemode.BrownFarm,
            MushroomBlock.MushroomType.Red => ModeMaster.Gamemode.RedFarm,
            MushroomBlock.MushroomType.Blue => ModeMaster.Gamemode.BlueFarm,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (thisMode == GameMaster.instance.ModeMaster.currentMode)
        {
            int3 mousePos = GetMousePos();

            foreach (Block block in allBlocks)
            {
                block.SetLightPos(mousePos);
                block.SetBlockOffset(new Vector3(0,
                    Mathf.Sin(((floatFlipped ? 1 : -1) * Time.time) + (floatX ? block.blockPos.x : 0) +
                              (floatY ? block.blockPos.y : 0)) * floatDistance, 0));
            }

            selectionBlock.SetBlockPos(mousePos);
        }

        if (timer>1f)//TODO make this a time manager class and combine it with the timer in the marketplace
        {
            timer = 0;
            seconds++;
            if (seconds>10)
            {
                seconds = 0;
                UpdateMushPerTenSec();
            }
            UpdateMushPerSec();
        }
        else
        {
            timer += Time.deltaTime;
        }
        DOTween.To(()=>mushPerSecTimerImage.fillAmount, x=>mushPerSecTimerImage.fillAmount = x, (seconds/10f)*0.725f, 1f);
            
        // MushPerSecText.text = "MPS: " + GetMushPerSec();
    }

    private void UpdateMushPerSec()
    {
        mushPerSec = (SaveSystem.instance.GetSaveFile().statsTotal.mushrooms[(int)currentMushroomType] - mushOneSecondAgo);
        mushOneSecondAgo = SaveSystem.instance.GetSaveFile().statsTotal.mushrooms[(int)currentMushroomType];
        MushPerSecText.text = "MPS: " + mushPerTenSec + " (" + mushPerSec + ")";
    }

    public void UpdateMushPerTenSec()
    {
        mushPerTenSec = (SaveSystem.instance.GetSaveFile().statsTotal.mushrooms[(int)currentMushroomType] - mushTenSecondAgo)/10f;
        mushTenSecondAgo = SaveSystem.instance.GetSaveFile().statsTotal.mushrooms[(int)currentMushroomType];
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
        if (SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvest[(int)currentMushroomType])
        {
            mushPerSec = ((SaveSystem.instance.GetSaveFile().farmSave.upgrades.autoHarvestSpeed[(int)currentMushroomType]*0.1f)
                          +(SaveSystem.instance.GetSaveFile().farmSave.upgrades.growthSpeedBonus[(int)currentMushroomType]*5*0.1f)
                          +(SaveSystem.instance.GetSaveFile().farmSave.upgrades.mushroomSpeed*0.05f)
                          
                          +(1 + SaveSystem.instance.GetSaveFile().farmSave.mushroomBlockCount[(int)currentMushroomType])
                          
                          * SaveSystem.instance.GetSaveFile().farmSave.mushroomBlockCount[(int)currentMushroomType]) / growthTimeWithHarvestTime;
        }
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
        uint cost = (uint)Math.Pow(SaveSystem.instance.GetSaveFile().farmSave.mushroomBlockCount[(int)currentMushroomType], 2);
        // Debug.Log(cost);
        if (SaveSystem.instance.GetSaveFile().stats.mushrooms[(int)currentMushroomType] < cost)
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
        SaveSystem.instance.GetSaveFile().farmSave.mushroomBlockCount[(int)currentMushroomType]++;
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

    public IEnumerator CreateWorld()
    {
        floatX = false;
        floatY = false;
        floatFlipped = Random.value > 0.5f;

        while (!floatX && !floatY)
        {
            floatX = Random.value > 0.5f;
            floatY = Random.value > 0.5f;
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
        
        int blockCount = 0;
        for (int x = 0; x < xMax+SaveSystem.instance.GetSaveFile().farmSave.farmSize.x; x++)
        {
            for (int y = 0; y < yMax+SaveSystem.instance.GetSaveFile().farmSave.farmSize.y; y++)
            {
                BiomeBlock block = Instantiate(biomeBlockPrefab, new Vector3(-10000,-10000, 0), Quaternion.identity, transform);
                Biome biome = Biome.Dirt;
                if (x == 0 || y == 0 || x == xMax+SaveSystem.instance.GetSaveFile().farmSave.farmSize.x - 1 || y == yMax+SaveSystem.instance.GetSaveFile().farmSave.farmSize.y - 1)
                {
                    biome = Biome.Rock;
                } else if (SaveSystem.instance.GetSaveFile().farmSave.mushroomBlockCount[(int)currentMushroomType]>blockCount)
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
                yield return new WaitForSeconds(0.1f);
            }
        }

        
        yield return new WaitForSeconds(1f);
        isWorldCreated = true;
        enrichButton.SetActive(dirtBlocks.Count >0);
    }

    public IEnumerator DissolveWorld()
    {
        isWorldCreated = false;
        foreach (var block in mushroomBlocks)
        {
            block.isGrowing = false;
            block.isGrown = false;
        }

        switch (Random.Range(0, 5))
        {
            case 0:
                //sort random
                allBlocks.Sort((x, y) => Random.value > 0.5f ? 1 : -1);
                break;
            case 1:
                //sort by smallest y, then random x, then smallest z 
                allBlocks.Sort((x, y) =>
                {
                    if (x.blockPos.y == y.blockPos.y)
                    {
                        if (x.blockPos.x == y.blockPos.x)
                        {
                            return x.blockPos.z.CompareTo(y.blockPos.z);
                        }
                        return Random.value > 0.5f ? 1 : -1;
                    }
                    return x.blockPos.y.CompareTo(y.blockPos.y);
                });
                break;
            case 2:
                //sort by smallest x, then random y, then smallest z
                allBlocks.Sort((x, y) =>
                {
                    if (x.blockPos.x == y.blockPos.x)
                    {
                        if (x.blockPos.y == y.blockPos.y)
                        {
                            return x.blockPos.z.CompareTo(y.blockPos.z);
                        }
                        return Random.value > 0.5f ? 1 : -1;
                    }
                    return x.blockPos.x.CompareTo(y.blockPos.x);
                });
                break;
            case 3:
                //sort smallest x, then smallest y, then smallest z 
                allBlocks.Sort((x, y) =>
                {
                    if (x.blockPos.x == y.blockPos.x)
                    {
                        if (x.blockPos.y == y.blockPos.y)
                        {
                            return x.blockPos.z.CompareTo(y.blockPos.z);
                        }
                        return x.blockPos.y.CompareTo(y.blockPos.y);
                    }
                    return x.blockPos.x.CompareTo(y.blockPos.x);
                });
            break;
            case 4:
                //sort by smallest y, then smallest x, then smallest z
                allBlocks.Sort((x, y) =>
                {
                    if (x.blockPos.y == y.blockPos.y)
                    {
                        if (x.blockPos.x == y.blockPos.x)
                        {
                            return x.blockPos.z.CompareTo(y.blockPos.z);
                        }
                        return x.blockPos.x.CompareTo(y.blockPos.x);
                    }
                    return x.blockPos.y.CompareTo(y.blockPos.y);
                });
                break;
        }

        if (Random.value > 0.5f)
        {
            allBlocks.Reverse();
        }

        foreach (var block in allBlocks)
        {
            block.RemoveBlock();
            yield return new WaitForSeconds(0.1f);
        }
        biomeBlocks.Clear();
        mushroomBlocks.Clear();
        allBlocks.Clear();
        dirtBlocks.Clear();
        SaveSystem.instance.GetSaveFile().farmSave.mushroomBlockCount[(int)currentMushroomType] = 1;
        
    }
}

