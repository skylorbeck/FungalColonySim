using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoTeleporter : MonoBehaviour
{
    public GameObject portalIn;
    public GameObject portalOut;

    
    //TODO good place for upgrades. Score + on teleport?
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("PlinkoBall"))
        {
            col.gameObject.transform.position = portalOut.transform.position;
        }
    }

}
