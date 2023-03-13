using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegSpinner : MonoBehaviour
{
    public Rigidbody2D rb;
    public float spinSpeed = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        rb.angularVelocity = spinSpeed;
    }
}
