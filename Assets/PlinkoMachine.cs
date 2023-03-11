using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class PlinkoMachine : MonoBehaviour
{
    public PlinkoBall ballPrefab;
    public GameObject ballHolder;
    public List<PlinkoBall> plinkoBalls;
    public ObjectPool<PlinkoBall> ballPool;

    public GameObject pegPrefab;
    public GameObject pegHolder;
    public List<GameObject> plinkoPegs;
    public ObjectPool<GameObject> pegPool;

    public float spawnForce = 500;
    public float spawnRate = 1;
    public float spawnTimer = 0;

    public int pegRows = 4;
    public int pegsPerRow = 5;
    public float pegDistance = 1.5f;
    public float rowDistance = 2f;

    public void Start()
    {
        ballPool = new ObjectPool<PlinkoBall>(
            () =>
            {
                var ball = Instantiate(ballPrefab, ballHolder.transform);
                ball.gameObject.SetActive(false);
                return ball;
            },
            ball =>
            {
                ball.transform.position = ballHolder.transform.position;
                ball.gameObject.SetActive(true);
            },
            ball =>
            {
                ball.score = 0;
                plinkoBalls.Remove(ball);
                ball.gameObject.SetActive(false);
            },
            ball => { Destroy(ball); }
        );
        
        pegPool = new ObjectPool<GameObject>(
            () =>
            {
                var peg = Instantiate(pegPrefab, pegHolder.transform);
                peg.SetActive(false);
                return peg;
            },
            peg =>
            {
                peg.transform.localPosition = Vector3.zero;
                peg.SetActive(true);
            },
            peg =>
            {
                plinkoPegs.Remove(peg);
                peg.SetActive(false);
            },
            peg => { Destroy(peg); }
        );
        
        GeneratePegs();
    }

    public void GeneratePegs()
    {
        pegHolder.transform.localPosition = new Vector3(-((pegsPerRow-1)*pegDistance*0.5f), 0, 0);
        for (int i = 0; i < pegRows; i++)
        {
            for (int j = 0; j < pegsPerRow; j++)
            {
                var peg = pegPool.Get();
                peg.transform.localPosition = new Vector3(j * pegDistance + (i%2==0?0:pegDistance*0.5f), i * -rowDistance, 0);
                plinkoPegs.Add(peg);
            }
        }
    }
    
    public void ClearPegs()
    {
        foreach (var peg in plinkoPegs)
        {
            pegPool.Release(peg);
        }
    }
    
    public void RegeneratePegs()
    {
        ClearPegs();
        GeneratePegs();
    }
    
    public void FixedUpdate()
    {
        if (GameMaster.instance.ModeMaster.currentMode!=ModeMaster.Gamemode.Hivemind) return;
        spawnTimer += Time.fixedDeltaTime;
        if (spawnTimer > spawnRate)
        {
            spawnTimer = 0;
            SpawnBall();
        }
    }

    public void SpawnBall()
    {
        var ball = ballPool.Get();
        ball.SetGolden(Random.Range(0, 100) < 5);
        ball.rb.AddForce(Vector2.up*spawnForce);
        plinkoBalls.Add(ball);
    }
    
    public void SpawnPeg()
    {
        var peg = pegPool.Get();
        plinkoPegs.Add(peg);
    }
    
    public void RemoveBall(PlinkoBall ball)
    {
        ballPool.Release(ball);
    }
    
    public void RemovePeg(GameObject peg)
    {
        pegPool.Release(peg);
    }
    
}
