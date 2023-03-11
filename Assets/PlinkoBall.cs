using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoBall : PlinkoPiece
{
    public float score = 0;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("PlinkoRemove"))
        {
            GameMaster.instance.Hivemind.plinkoMachine.RemoveBall(this);
        }
    }
}
