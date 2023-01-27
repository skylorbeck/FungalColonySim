using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    
    [SerializeField] private float waitTimerMax = 1f;
    [SerializeField] private float fadeTimerMax = 1f;
    [SerializeField] private Vector2 offset;

    [SerializeField] private Image background;
    [SerializeField] private RectTransform rectTransform;
    
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI content;
    public int characterWrapLimit = 30;
    

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();    
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        if (mousePos.x > Screen.width * 0.5f)
        {
            mousePos.x += offset.x;
        }
        else
        {
            mousePos.x -= offset.x;
        }

        if (mousePos.y > Screen.height * 0.5f)
        {
            mousePos.y += offset.y;
        }
        else
        {
            mousePos.y -= offset.y;
        }

        if (mousePos.x + rectTransform.rect.width * 0.5f > Screen.width)
        {
            mousePos.x = Screen.width - rectTransform.rect.width * 0.5f;
        }
        else if (mousePos.x - rectTransform.rect.width * 0.5f < 0)
        {
            mousePos.x = rectTransform.rect.width * 0.5f;
        }

        if (mousePos.y + rectTransform.rect.height * 0.5f > Screen.height)
        {
            mousePos.y = Screen.height - rectTransform.rect.height * 0.5f;
        }
        else if (mousePos.y - rectTransform.rect.height * 0.5f < 0)
        {
            mousePos.y = rectTransform.rect.height * 0.5f;
        }

        // rectTransform.pivot = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
        rectTransform.position = mousePos;
    }

    public void ShowToolTip(string headerText, string contentText)
    {
        gameObject.SetActive(true);//TODO make fade instead of instant
        background.DOComplete();
        header.DOComplete();
        content.DOComplete();
        
        background.DOFade(0, 0);
        header.DOFade(0, 0);
        content.DOFade(0, 0);
        layoutElement.enabled = content.text.Length > characterWrapLimit;
        if (string.IsNullOrEmpty(headerText))
        {
            header.gameObject.SetActive(false);
        } else
        {
            header.gameObject.SetActive(true);
            header.text = headerText;
            header.DOFade(1, fadeTimerMax).SetDelay(waitTimerMax);
        }

        if (string.IsNullOrEmpty(contentText))
        {
            content.gameObject.SetActive(false);
        }
        else
        {
            content.gameObject.SetActive(true);
            content.text = contentText;
            content.DOFade(1, fadeTimerMax).SetDelay(waitTimerMax);
        }
        
        background.DOFade(1, fadeTimerMax).SetDelay(waitTimerMax);
        
    }
    void FixedUpdate()
    {
        
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}
