using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    
    [SerializeField] private GameObject target;
    [SerializeField] private float waitTimer;
    [SerializeField] private float fadeTimer;
    [SerializeField] private float waitTimerMax = 1f;
    [SerializeField] private float fadeTimerMax = 1f;

    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI text;
    
    [SerializeField] private Vector2 offset;

    void Start()
    {
        
    }

    void Update()
    {
        if (target != null)
        {
            if (waitTimer < waitTimerMax)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= 0)
                {
                    fadeTimer = 0;
                }
            }
            else
            {
                Vector3 targetPosition = GameMaster.instance.camera.WorldToScreenPoint(target.transform.position);
                transform.position = targetPosition + (Vector3)offset;

                if (fadeTimer < fadeTimerMax)
                {
                    fadeTimer += Time.deltaTime;
                    Color color = Color.gray;
                    color.a = fadeTimer / fadeTimerMax;
                    background.color = color;
                    color = Color.white;
                    color.a = fadeTimer / fadeTimerMax;
                    text.color = color;
                }
            }
        }
    }

    public void SetTarget(GameObject target)
    {
        if (this.target== target)
        {
            ClearTarget();
            return;
        }
        ToolTipData data = target.GetComponent<ToolTipData>();
        if (data != null)
        {
            text.text = data.GetToolTipText();
        } else {
            text.text = target.name;
        }
        this.target = target;
        waitTimer = 0;
        fadeTimer = 0;
        background.color = Color.clear;
            text.color = new Color(1, 1, 1, 0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(text.rectTransform);
            // Canvas.ForceUpdateCanvases();
            background.rectTransform.sizeDelta = new Vector2(text.preferredWidth + 10, text.preferredHeight + 10);
    }
    public void ClearTarget()
    {
        transform.position = new Vector3(-10000, -10000, 0);
        target = null;
    }
    
    public void ToggleTooltip(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    void FixedUpdate()
    {
        
    }
}
