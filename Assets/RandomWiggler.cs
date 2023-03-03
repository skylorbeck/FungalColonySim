using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class RandomWiggler : MonoBehaviour
{
    public uint time = 0;
    public uint checkTime = 150;
    public uint checkTimeBase = 100;
    public uint checkTimeRandomAdd = 100;
    public float wiggleRandomMin = 0.5f;
    public float wiggleRandomMax = 1.5f;
    public float wiggleAmount = 25f;
    public float wiggleTime = 1f;
    public float wiggleChance = 0.25f;
    void FixedUpdate()
    {
        time++;
        if (time >= checkTime)
        {
            time = 0;
            checkTime = checkTimeBase + (uint)Random.Range(0, checkTimeRandomAdd);
            if (Random.value <= wiggleChance)
            {
                transform.DOShakeRotation(wiggleTime, wiggleAmount*Random.Range(wiggleRandomMin, wiggleRandomMax));
            }
        }
    }
}
