using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoPeg : PlinkoPiece
{
    
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("PlinkoBall"))
        {
            PlinkoBall ball = col.gameObject.GetComponent<PlinkoBall>();
            ball.score += scoreAddition;
            // ball.score *= scoreMultiplier;
            col.rigidbody.AddForce(col.contacts[0].normal * -1 * (force*Random.Range(1-variance,1+variance)));
        }
    }
    
    
}
