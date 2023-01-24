using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class Block : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public int3 blockPos;
    private int3 blockPosPrevious;
    [SerializeField] private Vector3 blockOffset;
    private Vector3 blockOffsetPrevious;
    [SerializeField] private float blockDistance= 2.75f;
    [SerializeField] protected SpriteRenderer spriteRenderer; 
    [SerializeField] private float shadowDistance = 7.5f;
    
    [SerializeField] private String tooltipText;
    [SerializeField] private int3 lightPos;
    [SerializeField] private int3 lastLightPos;
    [SerializeField] private bool isPlacing = false;

    void Start()
    {
        
    }

    protected virtual void Update()
    {
        
        if (blockPos.x != blockPosPrevious.x || blockPos.y != blockPosPrevious.y || blockPos.z != blockPosPrevious.z)
        {
            blockPosPrevious = blockPos;
            UpdateWorldPos();
        }

        if (blockOffsetPrevious != blockOffset)
        {
            blockOffsetPrevious = blockOffset;
            UpdateWorldPos();
        }

        if (lastLightPos.x != lightPos.x || lastLightPos.y != lightPos.y || lastLightPos.z != lightPos.z)
        {
            UpdateLighting();
        }

    }
    private void UpdateWorldPos()
    {
        transform.localPosition =
            new Vector3(
                (blockPos.x * -blockDistance) + (blockPos.y * blockDistance),
                (blockPos.x * (blockDistance * -0.5f)) + (blockPos.y * (blockDistance * -0.5f)) + (blockPos.z * blockDistance),
                (blockPos.x + blockPos.y + blockPos.z) * -1)
            + blockOffset;
        UpdateLighting();
    }
    private void UpdateLighting()
    {
        lastLightPos = lightPos;
        // spriteRenderer.DOComplete();
        //use lightpos to calculate the distance between the light and the block
        float color = 1 - (Vector3.Distance(new Vector3(lightPos.x,lightPos.y,lightPos.z), new Vector3(blockPos.x,blockPos.y,blockPos.z)) / shadowDistance);
        color = Mathf.Clamp(color, 0.25f, 1);
        
        // float color = 1- Mathf.Abs(Math.Abs(blockPos.x) + Math.Abs(blockPos.y) + Math.Abs(blockPos.z)) / shadowDistance;
        spriteRenderer.DOColor(new Color(color, color, color, 1), 0.5f);
    }
    void FixedUpdate()
    {
        
    }


    public virtual void OnPointerClick(PointerEventData eventData)
    {
        GameMaster.instance.tooltip.SetTarget(gameObject);
        transform.DOComplete();
        transform.lossyScale.Set(1, 1, 1);
        transform.DOPunchScale(new Vector3(-0.25f, -0.25f, -0.25f), 0.5f, 1, 0.5f);
    }

    public virtual void PlaceBlock(int3 int3)
    {
        this.isPlacing = true;
        SFXMaster.instance.PlayBlockPlace();
        this.blockPos = int3;
        this.blockOffset = new Vector3(0, 50, 0);
        DOTween.To(() => blockOffset, x => blockOffset = x, new Vector3(0, 0, 0), 0.5f).onComplete += () =>
        {
            this.isPlacing = false;
        };
        // UpdateWorldPos();
    }
    public void RemoveBlock()
    {
        isPlacing = true;
        DOTween.To(() => blockOffset, x => blockOffset = x, new Vector3(0, -50, 0), 1f).onComplete += () => Destroy(gameObject);
        SFXMaster.instance.PlayBlockDestroy();
    }

    public void SetLightPos(int3 LightPos)
    {
        lightPos = LightPos;
    }

    public void SetBlockOffset(Vector3 vector3)
    {
        if (isPlacing)
        {
            return;
        }
        blockOffset = vector3;
    }
}
