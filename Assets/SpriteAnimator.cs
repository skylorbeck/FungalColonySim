using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] protected Sprite[] sprites;
    protected SpriteRenderer spriteRenderer;
    [SerializeField] protected int currentSpriteIndex = 0;
    [SerializeField] protected float timeBetweenSprites = 1f;
    [SerializeField] protected float timeSinceLastSprite = 0f;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void FixedUpdate()
    {
        timeSinceLastSprite += Time.fixedDeltaTime;
        if (timeSinceLastSprite < timeBetweenSprites) return;
        timeSinceLastSprite = 0f;
        NextSprite();
    }

    protected virtual void NextSprite()
    {
           currentSpriteIndex++;
            if (currentSpriteIndex >= sprites.Length)
            {
                currentSpriteIndex = 0;
            }
            spriteRenderer.sprite = sprites[currentSpriteIndex];
    }

    protected virtual void PreviousSprite()
    {
        currentSpriteIndex--;
        if (currentSpriteIndex < 0)
        {
            currentSpriteIndex = sprites.Length - 1;
        }
        spriteRenderer.sprite = sprites[currentSpriteIndex];
    }

    public void SetSprites(Sprite[] newSprites)
    {
        sprites = newSprites;
    }
}