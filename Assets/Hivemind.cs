using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hivemind : MonoBehaviour, IPointerClickHandler
{
    SpriteRenderer spriteRenderer;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    public void ShakeStuff()
    {
        
        OnPointerClick(null);//eventData is never used 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOComplete();
        transform.DOShakeScale(0.5f, 0.5f, 15, 50);
    }
}
