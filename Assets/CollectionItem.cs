using System;
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
    public float timeOffset = 0;
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

    public bool preview = false;

    void Start()
    {
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (preview) return;
        Transform srTransform = sr.transform;
        var position = srTransform.localPosition;
        position = new Vector3(position.x,
            Mathf.Sin(Time.time * bobSpeed * (mouseOver ? mouseOverMultiplier : 1) + timeOffset) * bobRange,
            position.z);
        srTransform.localPosition = position;
        srTransform.rotation = Quaternion.Euler(0, 0,
            Mathf.Sin(Time.time * rotationSpeed * (mouseOver ? mouseOverMultiplier : 1) + timeOffset) * roationRange);
        srTransform.localScale =
            new Vector3(
                1 + Mathf.Sin(Time.time * zoomSpeed * (mouseOver ? mouseOverMultiplier : 1) + timeOffset) * zoomRange,
                1 + Mathf.Sin(Time.time * zoomSpeed * (mouseOver ? mouseOverMultiplier : 1) + timeOffset) * zoomRange,
                1);
        transform.localScale = Vector3.Lerp(mouseOver
            ? new Vector3(mouseOverMultiplier, mouseOverMultiplier, mouseOverMultiplier)
            : Vector3.one, transform.localScale, Time.fixedDeltaTime * 10);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (preview) return;
        mouseOver = true;
        SetUnlocked(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (preview) return;
        mouseOver = false;
        SetUnlocked(false);
    }

    public void SetUnlocked(bool isUnlocked)
    {
        if (preview) isUnlocked = true;
        this.isUnlocked = isUnlocked;
        sr.color = isUnlocked ? Color.white : lockedColor;
        frameSr.sprite = isUnlocked ? unlockedSprite : lockedSprite;
    }


    public void InsertSaveData(CollectionItemSaveData itemSaveData)
    {
        this.saveData = itemSaveData;
        SetUnlocked(isUnlocked);
        SetSprite(itemSaveData.spriteName);
        SetText(itemSaveData.name);
    }

    private void SetSprite(string spriteName)
    {
        sr.sprite = Resources.Load<Sprite>("Sprites/Collection/" + spriteName);
    }

    private void SetText(string name)
    {
        nameText.text = name;
    }
}

[Serializable]
public class CollectionItemSaveData
{
    public string name = "Item";

    public string spriteName = "Item";

    //below are Copilot generated, not implemented yet
    public bool isGolden = false;
    public bool isEquipped = false;
    public bool isEquippable = false;
}