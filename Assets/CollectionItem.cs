using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectionItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SpriteRenderer sr;
    public SpriteRenderer frameSr;
    public Sprite unlockedSprite;
    public Sprite lockedSprite;
    public TextMeshPro nameText;
    public TextMeshPro descriptionText;
    public float timeOffset = 0;//TODO create patterns by offsetting time
    public float roationRange = 5;
    public float rotationSpeed = 1;
    public float bobRange = 0.1f;
    public float bobSpeed = 1;
    public float zoomRange = 0.1f;
    public float zoomSpeed = 1;
    public bool mouseOver = false;
    public float mouseOverMultiplier = 1.5f;
    public CollectionItemSaveData saveData;
    public bool isUnlocked = false;

    public Color lockedColor = Color.gray;
    
    public void SetUnlocked(bool isUnlocked)
    {
        this.isUnlocked = isUnlocked;
        sr.color = isUnlocked ? Color.white :lockedColor;
        frameSr.sprite = isUnlocked ? unlockedSprite : lockedSprite;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // if (!saveData.isUnlocked)return;
        Transform thisTransform = sr.transform;
        var position = thisTransform.localPosition;
        position = new Vector3(position.x, Mathf.Sin(Time.time * bobSpeed * (mouseOver?mouseOverMultiplier:1) + timeOffset) * bobRange, position.z);
        thisTransform.localPosition = position;
        thisTransform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time*rotationSpeed * (mouseOver?mouseOverMultiplier:1) + timeOffset) * roationRange);
        thisTransform.localScale = new Vector3(1 + Mathf.Sin(Time.time * zoomSpeed * (mouseOver?mouseOverMultiplier:1) + timeOffset) * zoomRange, 1 + Mathf.Sin(Time.time * zoomSpeed * (mouseOver?mouseOverMultiplier:1) + timeOffset) * zoomRange, 1);
    }


    public void InsertSaveData(CollectionItemSaveData itemSaveData)
    {
        this.saveData = itemSaveData;
        SetUnlocked(isUnlocked);
        SetSprite(itemSaveData.spriteName);
        SetText(itemSaveData.name, itemSaveData.description);
    }

    private void SetSprite(string spriteName)
    {
        sr.sprite = Resources.Load<Sprite>("Sprites/Collection/" + spriteName);
    }

    private void SetText(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        descriptionText.gameObject.SetActive(true);
        SetUnlocked(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        descriptionText.gameObject.SetActive(false);
        SetUnlocked(false);
    }
}
[Serializable]
public class CollectionItemSaveData
{
    public string name = "Item";//TODO make this a dictionary SO of names
    public string description = "This is an item";//TODO make this a dictionary SO of descriptions
    public string spriteName = "Item";//TODO make this a dictionary SO of sprites
    //below are Copilot generated, not implemented yet
    public bool isGolden = false;
    public bool isEquipped = false;
    public bool isEquippable = false;
}