using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
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
    public float maxTime = 3;

    public bool isWorldCreated = false;

    public float floatDistance = 0.5f;
    public bool floatX = true;
    public bool floatY = true;
    public bool floatFlipped = false;

    public int3 lastMousePos;

    public UpgradeContainer enrichButton;

    public TextMeshProUGUI MushPerSecText;
    public Image mushPerSecTimerImage;
    public float timer = 0;
    public float seconds = 0;
    public float mushPerSec = 0;
    public uint mushOneSecondAgo = 0;

    public MushroomBlock.MushroomType currentMushroomType = MushroomBlock.MushroomType.Brown;
    public ObjectPool<BiomeBlock> biomeBlockPool;
    public ObjectPool<MushroomBlock> mushroomBlockPool;
    private bool isInstant => Vector3.Distance(transform.position, GameMaster.instance.camera.transform.position) >= 50;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveSystem.instance != null);
        // yield return new WaitUntil(() => SaveSystem.save != null);
        yield return new WaitUntil(() => SaveSystem.instance.loaded);
        biomeBlockPool = new ObjectPool<BiomeBlock>(() => Instantiate(biomeBlockPrefab),
            block => { block.gameObject.SetActive(true); },
            block => { block.gameObject.SetActive(false); },
            block => { Destroy(block.gameObject); });

        mushroomBlockPool = new ObjectPool<MushroomBlock>(() => Instantiate(mushroomBlockPrefab),
            block => { block.gameObject.SetActive(true); },
            block => { block.gameObject.SetActive(false); },
            block => { Destroy(block.gameObject); });
        biomeBlocks = new List<BiomeBlock>();
        mushroomBlocks = new List<MushroomBlock>();
        allBlocks = new List<Block>();
        dirtBlocks = new List<BiomeBlock>();
        enrichButton.gameObject.SetActive(false);
        yield return StartCoroutine(CreateWorld());
        UpdateMushPerSec();
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
                if (block.isPlacing) continue;
                block.SetBlockOffset(new Vector3(0,
                    Mathf.Sin(((floatFlipped ? 1 : -1) * Time.time) + (floatX ? block.blockPos.x : 0) +
                              (floatY ? block.blockPos.y : 0)) * floatDistance, 0));
            }

            selectionBlock.SetBlockPos(mousePos);
        }

        if (timer > 1f) //TODO make this a time manager class and combine it with the timer in the marketplace
        {
            timer = 0;
            UpdateMushPerSec();
        }
        else
        {
            timer += Time.deltaTime;
        }

        DOTween.To(() => mushPerSecTimerImage.fillAmount, x => mushPerSecTimerImage.fillAmount = x,
            (seconds / 10f) * 0.725f, 1f);

        // MushPerSecText.text = "MPS: " + GetMushPerSec();
    }

    void FixedUpdate()
    {
    }

    private void UpdateMushPerSec()
    {
        mushPerSec = (SaveSystem.save.statsTotal.mushrooms[(int)currentMushroomType] -
                      mushOneSecondAgo);
        mushOneSecondAgo = SaveSystem.save.statsTotal.mushrooms[(int)currentMushroomType];
        MushPerSecText.text = "MPS: " + SaveSystem.GetMushroomsPerSecond((int)currentMushroomType).ToString("F1") +
                              " (" + mushPerSec + ")";
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

        if (SaveSystem.save.farmSave.upgrades.autoHarvest[(int)currentMushroomType])
        {
            mushPerSec = ((SaveSystem.save.farmSave.upgrades
                              .autoHarvestSpeed[(int)currentMushroomType] * 0.1f)
                          + (SaveSystem.save.farmSave.upgrades
                              .growthSpeedBonus[(int)currentMushroomType] * 5 * 0.1f)
                          + (SaveSystem.save.farmSave.upgrades.mushroomSpeed * 0.05f)
                          + (1 + SaveSystem.save.farmSave
                              .mushroomBlockCount[(int)currentMushroomType])
                          * SaveSystem.save.farmSave.mushroomBlockCount[(int)currentMushroomType]) /
                         growthTimeWithHarvestTime;
        }

        return (float)Math.Round(mushPerSec, 2);
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

    public void StartEnrichBlock()
    {
        StartCoroutine(EnrichBlock());
    }

    public IEnumerator EnrichBlock()
    {
        enrichButton.ToggleButton(false);
        uint cost = (uint)Math.Pow(
            SaveSystem.save.farmSave.mushroomBlockCount[(int)currentMushroomType], 2);
        // Debug.Log(cost);
        if (SaveSystem.save.stats.mushrooms[(int)currentMushroomType] < cost)
        {
            yield break;
        }

        bool success = false;

        if (dirtBlocks.Count > 0)
        {
            BiomeBlock biomeBlock = dirtBlocks[0];
            BiomeBlock newBlock = biomeBlockPool.Get();
            newBlock.gameObject.SetActive(false);
            Transform transform1;
            (transform1 = newBlock.transform).SetParent(transform);
            transform1.position = transform.position;
            biomeBlocks.Add(newBlock);
            biomeBlocks.Remove(biomeBlock);
            dirtBlocks.Remove(biomeBlock);
            allBlocks.Remove(biomeBlock);
            allBlocks.Add(newBlock);
            biomeBlock.RemoveBlockRandom(isInstant);
            ScoreMaster.instance.SpendMushrooms(currentMushroomType, cost);
            SaveSystem.save.farmSave.mushroomBlockCount[(int)currentMushroomType]++;
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

            yield return new WaitUntil(() => !biomeBlock.isPlacing);
            newBlock.gameObject.SetActive(true);
            newBlock.PlaceBlock(biomeBlock.blockPos, Biome.Grass, isInstant);
            MushroomBlock block = mushroomBlockPool.Get();
            Transform transform2;
            (transform2 = block.transform).SetParent(transform);
            transform2.position = new Vector3(-10000, -10000, 0);
            block.PlaceBlock(biomeBlock.blockPos + new int3(0, 0, 1), currentMushroomType, isInstant);
            block.isGrowing = true;
            mushroomBlocks.Add(block);
            allBlocks.Add(block);
            success = true;
        }

        // enrichButton.ToggleButton(true);

        if (dirtBlocks.Count == 0 || !success)
        {
            enrichButton.gameObject.SetActive(false);
        }
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
        float timePerBlock = maxTime / ((xMax + SaveSystem.save.farmSave.farmSize.x) * (yMax +
            SaveSystem.save.farmSave.farmSize.y));
        for (int x = 0; x < xMax + SaveSystem.save.farmSave.farmSize.x; x++)
        {
            for (int y = 0; y < yMax + SaveSystem.save.farmSave.farmSize.y; y++)
            {
                BiomeBlock block = biomeBlockPool.Get();
                Transform transform1;
                (transform1 = block.transform).SetParent(transform);
                transform1.position = new Vector3(-10000, -10000, 0);
                Biome biome = Biome.Dirt;
                if (x == 0 || y == 0 || x == xMax + SaveSystem.save.farmSave.farmSize.x - 1 ||
                    y == yMax + SaveSystem.save.farmSave.farmSize.y - 1)
                {
                    biome = Biome.Rock;
                }
                else if (SaveSystem.save.farmSave.mushroomBlockCount[(int)currentMushroomType] >
                         blockCount)
                {
                    biome = Biome.Grass;
                    blockCount++;
                    MushroomBlock mushroomBlock = mushroomBlockPool.Get();
                    Transform transform2;
                    (transform2 = mushroomBlock.transform).SetParent(transform);
                    transform2.position = new Vector3(-10000, -10000, 0);
                    mushroomBlock.PlaceBlock(new int3(x, y, 1), currentMushroomType, isInstant);
                    mushroomBlock.isGrowing = true;
                    mushroomBlocks.Add(mushroomBlock);
                    allBlocks.Add(mushroomBlock);
                }

                block.PlaceBlock(new int3(x, y, 0), biome, isInstant);
                if (biome == Biome.Dirt)
                {
                    dirtBlocks.Add(block);
                }

                allBlocks.Add(block);
                biomeBlocks.Add(block);
                yield return new WaitForSeconds(timePerBlock);
            }
        }

        enrichButton.gameObject.SetActive(dirtBlocks.Count > 0);
        yield return new WaitForSeconds(1f);
        isWorldCreated = true;
    }

    public IEnumerator DissolveWorld()
    {
        isWorldCreated = false;
        enrichButton.gameObject.SetActive(false);
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
                    if (x.blockPos.y != y.blockPos.y) return x.blockPos.y.CompareTo(y.blockPos.y);
                    if (x.blockPos.x == y.blockPos.x)
                    {
                        return x.blockPos.z == y.blockPos.z ? 0 : x.blockPos.z.CompareTo(y.blockPos.z);
                    }

                    return Random.value > 0.5f ? 1 : -1;
                });
                break;
            case 2:
                //sort by smallest x, then random y, then smallest z
                allBlocks.Sort((x, y) =>
                {
                    if (x.blockPos.x != y.blockPos.x) return x.blockPos.x.CompareTo(y.blockPos.x);
                    if (x.blockPos.y == y.blockPos.y)
                    {
                        return x.blockPos.z == y.blockPos.z ? 0 : x.blockPos.z.CompareTo(y.blockPos.z);
                    }

                    return Random.value > 0.5f ? 1 : -1;
                });
                break;
            case 3:
                //sort smallest x, then smallest y, then smallest z 
                allBlocks.Sort((x, y) =>
                {
                    if (x.blockPos.x != y.blockPos.x) return x.blockPos.x.CompareTo(y.blockPos.x);
                    return x.blockPos.y == y.blockPos.y
                        ? x.blockPos.z.CompareTo(y.blockPos.z)
                        : x.blockPos.y.CompareTo(y.blockPos.y);
                });
                break;
            case 4:
                //sort by smallest y, then smallest x, then smallest z
                allBlocks.Sort((x, y) =>
                {
                    if (x.blockPos.y != y.blockPos.y) return x.blockPos.y.CompareTo(y.blockPos.y);
                    return x.blockPos.x == y.blockPos.x
                        ? x.blockPos.z.CompareTo(y.blockPos.z)
                        : x.blockPos.x.CompareTo(y.blockPos.x);
                });
                break;
        }

        if (Random.value > 0.5f)
        {
            allBlocks.Reverse();
        }

        int mode = Random.Range(0, 2);
        float timePerBlock = maxTime / allBlocks.Count;

        foreach (var block in allBlocks)
        {
            block.RemoveBlock(mode, isInstant);
            yield return new WaitForSeconds(timePerBlock);
        }

        yield return new WaitUntil(() => allBlocks.TrueForAll(x => x.isPlacing == false));

        foreach (var block in biomeBlocks)
        {
            biomeBlockPool.Release(block);
        }

        foreach (var block in mushroomBlocks)
        {
            mushroomBlockPool.Release(block);
        }

        biomeBlocks.Clear();
        mushroomBlocks.Clear();
        allBlocks.Clear();
        dirtBlocks.Clear();
        SaveSystem.save.farmSave.mushroomBlockCount[(int)currentMushroomType] = 1;
    }
}