using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleAnimator : MonoBehaviour
{
    public float inputDelay = 3f;
    public float inputTimer = 0f;
    public TextMeshPro pressStartText;
    public TextMeshProUGUI versionText;

    public TextMeshPro TitleText;
    public TextMeshPro TitleText2;
    public TextMeshPro TitleText3;
    public TextMeshPro VersionText;
    public TextMeshPro NewContentText;
    public SpriteRenderer titlePop;
    public SpriteRenderer statue;
    public SpriteRenderer book;
    public SpriteRenderer cauldron;
    public SpriteRenderer potion1;
    public SpriteRenderer potion2;
    public SpriteRenderer mushroom1;
    public SpriteRenderer mushroom2;
    public SpriteRenderer mushroom3;
    public SpriteRenderer hoodedFigure;

    public List<float> finalScale;
    public bool inputReady => inputTimer >= inputDelay;

    void Start()
    {
        StoreFinalScales();
        ZeroAll();
        StartAnimation();
        versionText.text = "v" + Application.version;
    }

    public void Update()
    {
        if (inputTimer < inputDelay)
        {
            inputTimer += Time.deltaTime;
            return;
        }

        pressStartText.gameObject.SetActive(true);
        pressStartText.transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 2f) * .1f);
        if (Input.anyKeyDown)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void StartAnimation()
    {
        KillAll();
        ZeroAll();
        mushroom1.transform.DOScale(finalScale[0], 2f)
            .SetEase(Ease
                .OutElastic) /*.onComplete = () => { mushroom1.transform.DOScale(finalScale[0]*1.1f, 1f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo); }*/
            ;
        mushroom2.transform.DOScale(finalScale[1], 1.5f).SetEase(Ease.OutBounce)
            .SetDelay(.5f) /*.onComplete = () => { mushroom2.transform.DOScale(finalScale[1]*1.1f, 1f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo); }*/
            ;
        mushroom3.transform.DOScale(finalScale[2], 1.5f).SetEase(Ease.OutBounce)
            .SetDelay(.5f) /*.onComplete = () => { mushroom3.transform.DOScale(finalScale[2]*1.1f, 1f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo); }*/
            ;
        titlePop.transform.DOScale(finalScale[3], 1.5f).SetEase(Ease.OutElastic).SetDelay(.25f).onComplete = () =>
        {
            titlePop.transform.DOScale(finalScale[3] * 1.1f, 1f).SetEase(Ease.OutElastic)
                .SetLoops(-1, LoopType.Yoyo);
        };
        statue.transform.DOScale(finalScale[4], 1.5f).SetEase(Ease.OutBounce)
            .SetDelay(1.15f) /*.onComplete = () => { statue.transform.DOScale(finalScale[4]*1.1f, 1f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo); }*/
            ;
        book.transform.DOScale(finalScale[5], 1.5f).SetEase(Ease.OutBounce)
            .SetDelay(1.25f) /*.onComplete = () => { book.transform.DOScale(finalScale[5]*1.1f, 1f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo); }*/
            ;
        cauldron.transform.DOScale(finalScale[6], 1.5f).SetEase(Ease.OutBounce)
            .SetDelay(1.35f) /*.onComplete = () => { cauldron.transform.DOScale(finalScale[6]*1.1f, 1f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo); }*/
            ;
        potion1.transform.DOScale(finalScale[7], 1.5f).SetEase(Ease.OutBounce)
            .SetDelay(1.45f) /*.onComplete = () => { potion1.transform.DOScale(finalScale[7]*1.1f, 1f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo); }*/
            ;
        potion2.transform.DOScale(finalScale[8], 1.5f).SetEase(Ease.OutBounce)
            .SetDelay(1.55f) /*.onComplete = () => { potion2.transform.DOScale(finalScale[8]*1.1f, 1f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo); }*/
            ;
        hoodedFigure.DOColor(Color.white, 1f).SetEase(Ease.Flash).SetDelay(2f);
        TitleText.transform.DOScale(1f, 1f).SetEase(Ease.OutElastic).SetDelay(1f);
        TitleText2.transform.DOScale(1f, 1f).SetEase(Ease.OutElastic).SetDelay(1.5f);
        TitleText3.transform.DOScale(1f, 1f).SetEase(Ease.OutElastic).SetDelay(2f);
        VersionText.DOColor(Color.white, 1f).SetEase(Ease.Flash).SetDelay(2.5f);
        NewContentText.transform.DOScale(1f, 1f).SetEase(Ease.OutElastic).SetDelay(3f).onComplete = () =>
        {
            NewContentText.transform.DOScale(1.25f, 1f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Yoyo);
        };
    }

    private void StoreFinalScales()
    {
        finalScale.Add(mushroom1.transform.localScale.x);
        finalScale.Add(mushroom2.transform.localScale.x);
        finalScale.Add(mushroom3.transform.localScale.x);
        finalScale.Add(titlePop.transform.localScale.x);
        finalScale.Add(statue.transform.localScale.x);
        finalScale.Add(book.transform.localScale.x);
        finalScale.Add(cauldron.transform.localScale.x);
        finalScale.Add(potion1.transform.localScale.x);
        finalScale.Add(potion2.transform.localScale.x);
    }

    private void KillAll()
    {
        mushroom1.transform.DOKill();
        mushroom2.transform.DOKill();
        mushroom3.transform.DOKill();
        titlePop.transform.DOKill();
        statue.transform.DOKill();
        book.transform.DOKill();
        cauldron.transform.DOKill();
        potion1.transform.DOKill();
        potion2.transform.DOKill();
        hoodedFigure.transform.DOKill();
        TitleText.transform.DOKill();
        TitleText2.transform.DOKill();
        TitleText3.transform.DOKill();
        VersionText.transform.DOKill();
        NewContentText.transform.DOKill();
    }

    private void ZeroAll()
    {
        TitleText.transform.localScale = Vector3.zero;
        TitleText2.transform.localScale = Vector3.zero;
        TitleText3.transform.localScale = Vector3.zero;
        // VersionText.transform.localScale = Vector3.zero;
        VersionText.color = Color.clear;
        NewContentText.transform.localScale = Vector3.zero;
        titlePop.transform.localScale = Vector3.zero;
        statue.transform.localScale = Vector3.zero;
        book.transform.localScale = Vector3.zero;
        cauldron.transform.localScale = Vector3.zero;
        potion1.transform.localScale = Vector3.zero;
        potion2.transform.localScale = Vector3.zero;
        mushroom1.transform.localScale = Vector3.zero;
        mushroom2.transform.localScale = Vector3.zero;
        mushroom3.transform.localScale = Vector3.zero;
        // hoodedFigure.transform.localScale = Vector3.zero;
        hoodedFigure.color = Color.clear;
    }
}