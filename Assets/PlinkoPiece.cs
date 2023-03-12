using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoPiece : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public float force = 50;
    public float variance = 0.5f;
    public bool isGolden = false;
    public bool canBeGolden = true;
    public int goldChance = 5;
    public Color defaultColor = Color.white;
    public float scoreMultiplier = 1;
    public int scoreAddition = 1;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }
    
    public void SetGolden(bool isGolden)
    {
        if (!canBeGolden) isGolden = false;
        this.isGolden = isGolden;
        sr.color = isGolden ? Color.yellow : defaultColor;
    }
}
