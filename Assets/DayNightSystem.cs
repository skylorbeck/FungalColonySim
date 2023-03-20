using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    public float dayLength = 10f;
    public float nightLength = 10f;
    public float currentTime = 0f;

    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentTime > dayLength + nightLength)
        {
            currentTime = 0;
        }
    }
    
    public float GetTime()
    {
        return currentTime;
    }

    public bool IsDay()
    {
        return currentTime < dayLength;
    }
    
    public float GetDayProgress()
    {
        return currentTime / (dayLength + nightLength);
    }

    public float GetLightProgress()
    {
        if (IsDay())
        {
            return currentTime / dayLength;
        }
        else
        {
            //inverted progress
            return 1 - ((currentTime - dayLength) / nightLength);
        }
    }
}
