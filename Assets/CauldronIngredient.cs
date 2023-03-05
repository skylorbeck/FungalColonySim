using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronIngredient : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector2 bounds;
    public Vector2 offset;
    public float timeOffset = 0;
    public float rotationMax = 15;
    public float speed = 1;
    public bool hopping = false;

    void FixedUpdate()
    {
        Vector3 pos = new Vector3(Mathf.Sin(timeOffset+Time.fixedTime * (hopping?speed:speed*0.5f)) * bounds.x, Mathf.Cos(timeOffset+Time.fixedTime * (hopping?speed*2:speed*0.5f)) * bounds.y, Mathf.Cos(timeOffset+Time.fixedTime * (hopping?speed:speed*0.5f))-1f);
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(timeOffset+Time.fixedTime * (hopping?speed*2:speed*0.5f)) * rotationMax);
        transform.localPosition = pos + (Vector3)offset;
    }

    public void SetIngredient(MushroomBlock.MushroomType ingredient)
    {
        spriteRenderer.sprite = MushroomBlock.GetMushroomSprite(ingredient);
    }

    public void Disable()
    {
        spriteRenderer.enabled = false;
        hopping = false;
    }
    
    public void Enable()
    {
        spriteRenderer.enabled = true;
    }
}
