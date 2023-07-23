using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public BlockMaster brownBlockMaster;
    public BlockMaster redBlockMaster;
    public BlockMaster blueBlockMaster;
    public Camera camera;
    public Tooltip tooltip;
    public Toggle fpsToggle;
    public DayNightSystem dayNightSystem;

    public ModeMaster ModeMaster;
    public UpgradeMaster brownUpgradeMaster;
    public UpgradeMaster redUpgradeMaster;
    public UpgradeMaster blueUpgradeMaster;

    public ConvergenceMaster convergenceMaster;
    [SerializeField] private bool isConverging;

    public Hivemind Hivemind;
    public Cauldron Cauldron;
    public Marketplace Marketplace;

    public int saveTimer = 0;

    public float inputDelay = 1f;

    public Color[] mushroomColors = new Color[3];

    //TODO harvest mushrooms by clicking on floor
    //TODO reset button
    //TODO red and blue upgrades have different scaling?
    //TODO Regenerate, a second layer of prestige that resets hivemind levels and gives a flat bonus to mushroom production

    IEnumerator Start()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate =
            PlayerPrefs.GetInt("targetFPS", (int)Screen.currentResolution.refreshRateRatio.value);
        DOTween.SetTweensCapacity(500, 50);
        fpsToggle.SetIsOnWithoutNotify(Application.targetFrameRate != 30);
        camera = Camera.main;
        yield return new WaitUntil(() => SaveSystem.instance != null);
        SaveSystem.instance.Load();
    }

    void Update()
    {
        //quit on escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (inputDelay > 0)
        {
            inputDelay -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ModeMaster.PreviousMode();
            inputDelay = 1f;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ModeMaster.NextMode();
            inputDelay = 1f;
        }
    }

    void FixedUpdate()
    {
        TickSaver();
    }

    public void OnDestroy()
    {
        SaveSystem.SaveS();
    }

    public void ToggleFPS(bool value)
    {
        if (value)
        {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            PlayerPrefs.SetInt("targetFPS", (int)Screen.currentResolution.refreshRateRatio.value);
        }
        else
        {
            Application.targetFrameRate = 30;
            PlayerPrefs.SetInt("targetFPS", 30);
        }
    }

    public IEnumerator Prestige()
    {
        isConverging = true;
        Coroutine brown = StartCoroutine(brownBlockMaster.DissolveWorld());
        Coroutine red = StartCoroutine(redBlockMaster.DissolveWorld());
        Coroutine blue = StartCoroutine(blueBlockMaster.DissolveWorld());
        yield return brown;
        yield return red;
        yield return blue;

        Coroutine brown2 = StartCoroutine(brownBlockMaster.CreateWorld());
        Coroutine red2 = StartCoroutine(redBlockMaster.CreateWorld());
        Coroutine blue2 = StartCoroutine(blueBlockMaster.CreateWorld());
        yield return brown2;
        yield return red2;
        yield return blue2;

        SaveSystem.instance.GetSaveFile().statsTotal.converges++;
        SaveSystem.SaveS();
        ModeMaster.UpdateButton();
        isConverging = false;
    }

    private void TickSaver()
    {
        saveTimer++;
        if (saveTimer > 500 && !convergenceMaster.inStore && !isConverging && (brownBlockMaster.isWorldCreated &&
                                                                               redBlockMaster.isWorldCreated &&
                                                                               blueBlockMaster.isWorldCreated))
        {
            saveTimer = 0;
            SaveSystem.SaveS();
        }
    }
}