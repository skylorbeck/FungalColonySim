using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectionShelf : MonoBehaviour
{
    public CollectionItem itemPrefab;
    public List<CollectionItem> items = new List<CollectionItem>();
    public int itemsPerRow = 5;
    public float itemDistance = 5f;
    public float rowDistance = 2f;

    public float curScroll = 0;

    public ItemVariableCluster[] itemVariableCluster;
    public TextMeshProUGUI collectiblePercentText;


    void Start()
    {
        var datas = SaveSystem.save.collectionItems;
        for (var i = 0; i < datas.Count; i++)
        {
            var itemSaveData = datas[i];
            CollectionItem item = Instantiate(itemPrefab, transform);
            item.InsertSaveData(itemSaveData);
            item.timeOffset = i * itemsPerRow;
            items.Add(item);
        }

        UpdatePercentText();
        this.transform.localPosition = new Vector3(-Mathf.FloorToInt(itemsPerRow * 0.5f) * itemDistance, 0, 0);
    }

    void Update()
    {
        if (!(GameMaster.instance.ModeMaster.currentMode == ModeMaster.Gamemode.Hivemind &&
              GameMaster.instance.Hivemind.mode == Hivemind.HiveMindMode.Collection)) return;
        if (Input.mouseScrollDelta.y > 0)
        {
            curScroll -= 1;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            curScroll += 1;
        }

        if (Input.GetMouseButton(0))
        {
            curScroll += Input.GetAxis("Mouse Y") * 0.5f;
        }

        curScroll = Mathf.Clamp(curScroll, 0,
            Mathf.Max(0, Mathf.CeilToInt((items.Count - (itemsPerRow - 1) * 3) / (float)itemsPerRow) - 1));
    }

    void FixedUpdate()
    {
        for (int i = 0; i < items.Count; i++)
        {
            int row = i / itemsPerRow;
            int column = i % itemsPerRow;
            Vector3 targetPos = new Vector3(column * itemDistance, -row * rowDistance + curScroll * rowDistance,
                items[i].mouseOver ? -5f : 0);
            items[i].transform.localPosition =
                Vector3.Lerp(items[i].transform.localPosition, targetPos, Time.fixedDeltaTime * 10);
        }
    }

    public void AddItem(CollectionItemSaveData saveData)
    {
        CollectionItem item = Instantiate(itemPrefab, transform);
        item.InsertSaveData(saveData);
        item.timeOffset = items.Count * itemsPerRow;
        items.Add(item);
        item.transform.localPosition = new Vector3(items.Count * itemDistance, 0, 0);
        Vector3 thisPos = transform.position;
        thisPos.y = GetY();
        transform.position = thisPos;
        UpdatePercentText();
    }

    public void RemoveItem(CollectionItemSaveData saveData)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i].saveData.name == saveData.name)
            {
                Destroy(items[i].gameObject);
                items.RemoveAt(i);
                break;
            }
        }

        UpdatePercentText();
    }

    public float GetY()
    {
        return Mathf.FloorToInt(items.Count / (float)itemsPerRow) * rowDistance + 50f;
    }

    public CollectionItemSaveData GenerateSaveData()
    {
        ItemVariableCluster cluster = itemVariableCluster[Random.Range(0, itemVariableCluster.Length)];
        CollectionItemSaveData saveData = new CollectionItemSaveData();
        saveData.name = cluster.itemNames[Random.Range(0, cluster.itemNames.Length)];
        saveData.spriteName = cluster.itemSprites[Random.Range(0, cluster.itemSprites.Length)].name;
        return saveData;
    }

    public void UpdatePercentText()
    {
        collectiblePercentText.text = "+" + SaveSystem.GetCollectionMultiplier() + "%";
    }
}

[Serializable]
public class ItemVariableCluster
{
    public string[] itemNames;
    public Sprite[] itemSprites;
}