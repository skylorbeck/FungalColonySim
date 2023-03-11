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
    public float scoreMultiplier = 1;
    public float scoreAddition = 1;
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
        this.isGolden = isGolden;
        sr.color = isGolden ? Color.yellow : Color.white;
    }
}
