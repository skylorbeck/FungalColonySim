using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatioBar : MonoBehaviour
{
    public float maxWidth = 450f;
    public Image[] bars;
    public List<int> amounts = new List<int>();
    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI[] percentTexts;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI potionTotalText;
    public TextMeshProUGUI potionRatioText;

    public float ratio = 100;
    public Color[] colors => GameMaster.instance.mushroomColors;

    void FixedUpdate()
    {
        float total = 0;
        foreach (int amount in amounts)
        {
            total += amount;
        }

        Vector2 size;
        for (int i = 0; i < bars.Length; i++)
        {
            if (i >= amounts.Count)
            {
                size = new Vector2(0, bars[i].rectTransform.sizeDelta.y);
            }
            else
            {
                float width = (amounts[i] / total) * maxWidth;
                size = new Vector2(width, bars[i].rectTransform.sizeDelta.y);
            }

            bars[i].rectTransform.sizeDelta = Vector2.Lerp(bars[i].rectTransform.sizeDelta, size, Time.deltaTime * 10);
        }
    }

    public void SetAmounts(List<int> amounts, List<MushroomBlock.MushroomType> types)
    {
        this.amounts = amounts;
        for (int i = 0; i < bars.Length; i++)
        {
            if (i >= types.Count)
            {
                bars[i].color = Color.clear;
                texts[i].text = "";
            }
            else
            {
                bars[i].color = colors[(int)types[i]];
                texts[i].text = amounts[i].ToString("N0");
                texts[i].color = colors[(int)types[i]];
                percentTexts[i].color = colors[(int)types[i]];
            }
        }

        int total = 0;
        foreach (int amount in amounts)
        {
            total += amount;
        }

        if (total > 0)
        {
            totalText.text = total.ToString("N0");
            potionTotalText.text = Mathf.FloorToInt(total / ratio).ToString("N0");
            potionRatioText.text = "/" + ratio.ToString("N0") + "=";
            potionRatioText.enabled = true;
        }
        else
        {
            totalText.text = "";
            potionTotalText.text = "";
            potionRatioText.enabled = false;
        }


        for (int i = 0; i < percentTexts.Length; i++)
        {
            if (i >= types.Count)
            {
                percentTexts[i].text = "";
            }
            else
            {
                percentTexts[i].text = Mathf.RoundToInt((amounts[i] / (float)total) * 100) + "%";
            }
        }
    }
}