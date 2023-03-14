using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoBall : PlinkoPiece
{
    [SerializeField]private uint score = 0;
    public bool hasWon = false;
    public AudioClip popSound;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Peg") && GameMaster.instance.ModeMaster.currentMode == ModeMaster.Gamemode.Hivemind)
        { 
            SFXMaster.instance.PlayOneShot(popSound);
        }
        if (col.gameObject.CompareTag("PlinkoRemove"))
        {
            hasWon = false;
            GameMaster.instance.Hivemind.plinkoMachine.RemoveBall(this);
        }
    }

    public uint GetScore()
    {
        return score;
    }

    public void SetScore(uint i)
    {
        score = i;
    }

    public void AddScore(uint i)
    {
        score += (uint)(i * (isGolden ? 2 : 1));
    }
}
