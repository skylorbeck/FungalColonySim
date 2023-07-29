using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PlinkoMachine : MonoBehaviour
{
    public PlinkoBall ballPrefab;
    public GameObject ballHolder;
    public List<PlinkoBall> plinkoBalls;

    public GameObject pegPrefab;
    public GameObject pegHolder;
    public List<GameObject> plinkoPegs;

    public PlinkoPrizeAwarder prizeAwarder;
    public TextMeshProUGUI prizeText;
    public CollectionItem prizePreview;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI softCapText;

    public float spawnForce = 50;
    public float spawnRate = 60f;

    public float autoFireTimer = 0;
    public float autoFireRate = 1f;
    public float fireTimer = 0;
    public float fireRate = 0.5f;

    public Image spawnTimerBar;

    public int pegRows = 6;
    public int pegsPerRow = 10;
    public float pegDistance = 1.5f;
    public float rowDistance = 2f;

    public float[] prizeWeights = new float[3];

    public DamageNumber numberPrefab;
    public RectTransform rectParent;
    public AudioClip jackpotSound;
    public AudioClip winSound;
    public ObjectPool<PlinkoBall> ballPool;
    public ObjectPool<GameObject> pegPool;

    public uint score
    {
        get => SaveSystem.save.plinkoSave.score;
        set => SaveSystem.save.plinkoSave.score = value;
    }

    public float spawnTimer
    {
        get => SaveSystem.save.plinkoSave.ballProgress;
        set => SaveSystem.save.plinkoSave.ballProgress = value;
    }

    public bool canFire => fireTimer <= 0;


    public IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveSystem.instance.loaded);
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
                score += ball.GetScore();
                DamageNumber damageNumber = numberPrefab.Spawn(Vector3.zero, ball.GetScore());
                damageNumber.SetAnchoredPosition(rectParent, new Vector2(0, -100));
                UpdateScoreText();
                ball.SetScore(0);
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
        UpdateScoreText();
        UpdateSoftCap();
        // GeneratePegs();//use this to generate pegs, then copy them into the editor
    }

    public void Update()
    {
        if (!(GameMaster.instance.ModeMaster.IsMode(ModeMaster.Gamemode.Hivemind) &&
              GameMaster.instance.Hivemind.mode == Hivemind.HiveMindMode.Plinko)) return;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBall();
        }
    }

    public void FixedUpdate()
    {
        // if (GameMaster.instance.ModeMaster.currentMode!=ModeMaster.Gamemode.Hivemind) return;
        if (fireTimer > 0)
        {
            fireTimer -= Time.fixedDeltaTime;
        }

        if (SaveSystem.save.plinkoSave.autofireUnlocked)
        {
            autoFireTimer += Time.fixedDeltaTime; //TODO place for upgrades
            if (autoFireTimer > autoFireRate)
            {
                autoFireTimer = 0;
                SpawnBall(true);
            }
        }

        if (SaveSystem.save.plinkoSave.balls >=
            SaveSystem.save.plinkoSave.ballSoftCap) return;

        spawnTimer += Time.fixedDeltaTime * SaveSystem.save.plinkoSave.ballRegenSpeed;
        if (spawnTimer > spawnRate)
        {
            spawnTimer = 0;
            SaveSystem.save.plinkoSave.balls +=
                SaveSystem.save.plinkoSave.ballRegenAmount;
        }

        spawnTimerBar.fillAmount = spawnTimer / spawnRate;
    }

    public void AwardCollectible()
    {
        if (GameMaster.instance.ModeMaster.IsMode(ModeMaster.Gamemode.Hivemind))
        {
            SFXMaster.instance.PlayOneShot(jackpotSound);
        }

        CollectionItemSaveData saveData = prizeAwarder.currentPrize;
        prizePreview.InsertSaveData(saveData);
        prizePreview.gameObject.transform.DOKill();
        prizePreview.gameObject.transform.localScale = Vector3.zero;
        prizePreview.gameObject.transform.DOScale(2, 0.5f).SetEase(Ease.OutBack).onComplete = () =>
        {
            prizePreview.gameObject.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetDelay(1);
        };
        SaveSystem.save.collectionItems.Add(saveData);
        // SaveSystem.SaveS();
        GameMaster.instance.Hivemind.collectionShelf.AddItem(saveData);
        prizeAwarder.UpdatePrize();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString("N0");
    }

    public void UpdateSoftCap()
    {
        softCapText.text = "Soft Cap: " + SaveSystem.save.plinkoSave.ballSoftCap.ToString("N0");
    }

    public void GeneratePegs()
    {
        pegHolder.transform.localPosition = new Vector3(-((pegsPerRow - 1) * pegDistance * 0.5f), 0, 0);
        for (int i = 0; i < pegRows; i++)
        {
            for (int j = 0; j < pegsPerRow; j++)
            {
                var peg = pegPool.Get();
                peg.transform.localPosition = new Vector3(j * pegDistance + (i % 2 == 0 ? 0 : pegDistance * 0.5f),
                    i * -rowDistance, 0);
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

    public void SpawnBall(bool overrideTimer = false)
    {
        if (!overrideTimer)
        {
            if (!canFire) return;
        }

        fireTimer = fireRate;
        if (SaveSystem.save.plinkoSave.balls > 0)
        {
            SaveSystem.save.plinkoSave.balls--;
        }
        else
        {
            return;
        }

        if (GameMaster.instance.ModeMaster.IsMode(ModeMaster.Gamemode.Hivemind))
        {
            SFXMaster.instance.PlayMenuClick();
        }

        var ball = ballPool.Get();
        if (SaveSystem.save.plinkoSave.goldenBallsUnlocked)
        {
            ball.SetGolden(Random.Range(0, 100) < ball.goldChance);
        }

        ball.rb.AddForce(Vector2.up * spawnForce);
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
        if (GameMaster.instance.ModeMaster.currentMode == ModeMaster.Gamemode.Hivemind)
        {
            SFXMaster.instance.PlayOneShot(winSound);
        }

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
                        uint spores = (uint)Random.Range(1, 5); //TODO good place for upgrades
                        SaveSystem.save.stats.spores += spores;
                        prizeText.text = "Spores +" + spores;
                        break;
                    case 2:
                        uint hivemindPoints = (uint)Random.Range(1, 5); //TODO good place for upgrades
                        SaveSystem.save.stats.skillPoints += hivemindPoints;
                        prizeText.text = "Skill Points +" + hivemindPoints;
                        break;
                    case 3:
                        uint bpotions = (uint)Random.Range(1, 5); //TODO good place for upgrades
                        SaveSystem.save.marketSave.potionsCount[0] += bpotions;
                        prizeText.text = "Brown Potions +" + bpotions;
                        break;
                    case 4:
                        uint rpotions = (uint)Random.Range(1, 5); //TODO good place for upgrades
                        SaveSystem.save.marketSave.potionsCount[0] += rpotions;
                        prizeText.text = "Red Potions +" + rpotions;
                        break;
                    case 5:
                        uint bupotions = (uint)Random.Range(1, 5); //TODO good place for upgrades
                        SaveSystem.save.marketSave.potionsCount[0] += bupotions;
                        prizeText.text = "Blue Potions +" + bupotions;
                        break;
                    default:
                        uint balls = (uint)Random.Range(1, 5); //TODO good place for upgrades
                        SaveSystem.save.plinkoSave.balls += balls;
                        prizeText.text = "Balls +" + balls;
                        break;
                }

                break;
            }
        }

        // SaveSystem.SaveS();

        prizeText.gameObject.transform.DOKill();
        prizeText.gameObject.transform.localScale = Vector3.zero;
        prizeText.gameObject.transform.DOScale(2, 0.5f).SetEase(Ease.OutBack).onComplete = () =>
        {
            prizeText.gameObject.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetDelay(1);
        };
    }
}