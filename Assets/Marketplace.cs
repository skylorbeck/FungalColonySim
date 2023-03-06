using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Marketplace : MonoBehaviour
{
    public int ticksToRefresh = 30000;
    public int ticksToRefreshDefault = 30000;
    public int tickRange = 10000;
    public MarketPreview buyPreview;
    public MarketPreview sellPreview;
    
    public TextMeshProUGUI timerText;
    
    void Start()
    {
        Refresh();
    }

    

    void FixedUpdate()
    {
        
        ticksToRefresh--;
        if (ticksToRefresh <= 0)
        {
            Refresh();
        }

        if (GameMaster.instance.ModeMaster.currentMode != ModeMaster.Gamemode.Marketplace) return;
        buyPreview.CheckButton();
        sellPreview.CheckButton();
        //format tickstoRefresh to minutes and seconds. 50 ticks = 1 second
        int minutes = ticksToRefresh / 3000;
        int seconds = (ticksToRefresh % 3000) / 50;
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        
    }

    private void Refresh()
    {
        ticksToRefresh = ticksToRefreshDefault + Random.Range(-tickRange, tickRange);
        do
        {
            buyPreview.Refresh();
            sellPreview.Refresh();
        } while (buyPreview.currency == sellPreview.currency);
    }
}
