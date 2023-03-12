using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoBall : PlinkoPiece
{
    [SerializeField]private int score = 0;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("PlinkoRemove"))
        {
            GameMaster.instance.Hivemind.plinkoMachine.RemoveBall(this);
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int i)
    {
        score = i;
    }

    public void AddScore(int i)
    {
        score += i * (isGolden ? 2 : 1);
    }
}
