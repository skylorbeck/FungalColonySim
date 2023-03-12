using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRightMover : MonoBehaviour
{
    bool isMovingLeft = true;
    public float speed = 1;
    public float leftBound = -5;
    public float rightBound = 5;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (isMovingLeft)
        {
            transform.localPosition += Vector3.left * (speed * Time.fixedDeltaTime);
            if (transform.localPosition.x < leftBound)
            {
                isMovingLeft = false;
            }
        }
        else
        {
            transform.localPosition += Vector3.right * (speed * Time.fixedDeltaTime);
            if (transform.localPosition.x > rightBound)
            {
                isMovingLeft = true;
            }
        }
    }
}
