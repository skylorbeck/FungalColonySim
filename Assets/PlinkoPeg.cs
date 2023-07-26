using DG.Tweening;
using UnityEngine;

public class PlinkoPeg : PlinkoPiece
{
    public float punchScale = 0.25f;
    public bool doPunch = true;

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
            ball.AddScore((uint)(scoreAddition * (isGolden ? 2 : 1)));
            col.rigidbody.AddForce(col.contacts[0].normal * -1 * (force * Random.Range(1 - variance, 1 + variance)));
            if (SaveSystem.save.plinkoSave.goldenPegsUnlocked)
            {
                this.SetGolden(Random.Range(0, 100) < this.goldChance);
            }

            if (!doPunch) return;
            sr.transform.DOComplete();
            sr.transform.DOPunchScale(Vector3.one * punchScale, 0.1f);
        }
    }
}