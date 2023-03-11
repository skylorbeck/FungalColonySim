using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectionShelf : MonoBehaviour
{
    public CollectionItem itemPrefab;
    public List<CollectionItem> items = new List<CollectionItem>();
    public int itemsPerRow = 5;
    public float itemDistance = 5f;
    public float rowDistance = 2f;
    
    public float curScroll = 0;
    
    public string[] itemNames;
    public string[] itemDescriptions;
    public Sprite[] itemSprites;
    
    
    void Start()
    {
        var datas = SaveSystem.instance.GetSaveFile().collectionItems;
        for (var i = 0; i < datas.Length; i++)
        {
            var itemSaveData = datas[i];
            CollectionItem item = Instantiate(itemPrefab, transform);
            item.InsertSaveData(itemSaveData);
            item.timeOffset = i * 3;
            items.Add(item);
        }

        for (int i = 0; i < items.Count; i++)
        {
            int row = i / itemsPerRow;
            int column = i % itemsPerRow;
            items[i].transform.localPosition = new Vector3(column * itemDistance, -row * rowDistance, 0);
        }
        
        this.transform.localPosition = new Vector3(-Mathf.FloorToInt(itemsPerRow * 0.5f) * itemDistance, 0, 0);
    }

    public float GetY()
    {
        return Mathf.FloorToInt(items.Count / (float)itemsPerRow) * rowDistance+50f;
    }

 
    void Update()
    {
        if (!(GameMaster.instance.ModeMaster.currentMode == ModeMaster.Gamemode.Hivemind && GameMaster.instance.Hivemind.mode == Hivemind.HiveMindMode.Collection))return;
        if (Input.mouseScrollDelta.y > 0)
        {
            curScroll -= 1;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            curScroll += 1;
        }
        curScroll = Mathf.Clamp(curScroll, 0, Mathf.Max(0, Mathf.CeilToInt(items.Count / (float)itemsPerRow) - 1));

    }

    void FixedUpdate()
    {
        for (int i = 0; i < items.Count; i++)
        {
            int row = i / itemsPerRow;
            int column = i % itemsPerRow;
            Vector3 targetPos = new Vector3(column * itemDistance, -row * rowDistance + curScroll * rowDistance, 0);
            items[i].transform.localPosition = Vector3.Lerp(items[i].transform.localPosition, targetPos, Time.fixedDeltaTime * 10);
        }
    }
}
