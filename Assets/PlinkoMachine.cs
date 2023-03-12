using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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

    public PlinkoPrizeAwarder prizeAwarder;
    public TextMeshProUGUI prizeText;
    public CollectionItem prizePreview;

    public float spawnForce = 500;
    public float spawnRate = 1;
    public float spawnTimer = 0;

    public int pegRows = 4;
    public int pegsPerRow = 5;
    public float pegDistance = 1.5f;
    public float rowDistance = 2f;
    
    public float[] prizeWeights = new float[3];

    public void AwardCollectible()
    {
        CollectionItemSaveData saveData = prizeAwarder.currentPrize;
        prizePreview.InsertSaveData(saveData);
        prizePreview.gameObject.transform.DOKill();
        prizePreview.gameObject.transform.localScale = Vector3.zero;
        prizePreview.gameObject.transform.DOScale(2, 0.5f).SetEase(Ease.OutBack).onComplete = () =>
        {
            prizePreview.gameObject.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetDelay(1);
        };
        SaveSystem.instance.GetSaveFile().collectionItems.Add(saveData);
        SaveSystem.SaveS();
        GameMaster.instance.Hivemind.collectionShelf.AddItem(saveData);
        prizeAwarder.UpdatePrize();
    }

  

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
        
        // GeneratePegs();//use this to generate pegs, then copy them into the editor
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
        /*if (GameMaster.instance.ModeMaster.currentMode!=ModeMaster.Gamemode.Hivemind) return;
        spawnTimer += Time.fixedDeltaTime;
        if (spawnTimer > spawnRate)
        {
            spawnTimer = 0;
            SpawnBall();
        }*/
    }

    public void SpawnBall()
    {
        if (SaveSystem.instance.GetSaveFile().plinkoBalls>0)
        {
            SaveSystem.instance.GetSaveFile().plinkoBalls--;
        }
        else
        {
            return;
        }
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

    public void AwardNonCollectible()
    {
        float totalWeight = 0;
        foreach (var weight in prizeWeights)
        {
            totalWeight += weight;
        }
        
        float random = Random.Range(0, totalWeight);
        float currentWeight = 0;
        
        for (int i = 0; i < prizeWeights.Length; i++)
        {
            currentWeight += prizeWeights[i];
            if (random < currentWeight)
            {
                switch (i)
                {
                    case 1:
                        uint spores = (uint)Random.Range(1, 5);//TODO good place for upgrades
                        SaveSystem.instance.GetSaveFile().sporeCount +=spores;
                        prizeText.text = "Spores +" + spores;
                        break;
                    case 2:
                        uint hivemindPoints = (uint)Random.Range(1, 5);//TODO good place for upgrades
                        SaveSystem.instance.GetSaveFile().hivemindPoints +=hivemindPoints;
                        prizeText.text = "Skill Points +" + hivemindPoints;
                        break;
                    default:
                        uint balls = (uint)Random.Range(1, 5);//TODO good place for upgrades
                        SaveSystem.instance.GetSaveFile().plinkoBalls += balls;
                        prizeText.text = "Balls +" + balls;
                        break;
                }
                break;
            }
        }
        
        SaveSystem.SaveS();
        
        prizeText.gameObject.transform.DOKill();
        prizeText.gameObject.transform.localScale = Vector3.zero;
        prizeText.gameObject.transform.DOScale(2, 0.5f).SetEase(Ease.OutBack).onComplete = () =>
        {
            prizeText.gameObject.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetDelay(1);
        };
    }
}
