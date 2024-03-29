using UnityEngine;

public class PlinkoBall : PlinkoPiece
{
    [SerializeField] private uint score = 0;
    public bool hasWon = false;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Peg") &&
            GameMaster.instance.ModeMaster.currentMode == ModeMaster.Gamemode.Hivemind)
        {
            SFXMaster.instance.PlayPlinkoHit();
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