using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    [FormerlySerializedAs("blockMaster")] public BlockMaster brownBlockMaster;
    public BlockMaster redBlockMaster;
    public BlockMaster blueBlockMaster;
    [SerializeField] private Texture2D cursorSprite;
    [SerializeField] private Texture2D cursorClicking;
    [SerializeField] private Texture2D cursorTransparent;
    public Camera camera;
    public Tooltip tooltip;
    
    public SaveSystem SaveSystem;
    public ModeMaster ModeMaster;
    public UpgradeMaster brownUpgradeMaster;
    public UpgradeMaster redUpgradeMaster;
    public UpgradeMaster blueUpgradeMaster;
    void Start()
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
        SaveSystem.Load();
        camera = Camera.main;
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        //quit on escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursorClicking, Vector2.zero, CursorMode.Auto);
        }/* else if (Input.GetMouseButtonDown(1))
        {
            Cursor.SetCursor(cursorTransparent, Vector2.zero, CursorMode.Auto);
        }*/ else if (Input.GetMouseButtonUp(0) /*|| Input.GetMouseButtonUp(1)*/)
        {
            Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
        }

        
    }
    
    public async Task Prestige()
    {
        await Task.WhenAll(brownBlockMaster.DissolveWorld(), redBlockMaster.DissolveWorld(), blueBlockMaster.DissolveWorld());
        ScoreMaster.instance.Reset();
        brownUpgradeMaster.Reset();
        redUpgradeMaster.Reset();
        blueUpgradeMaster.Reset();
        await Task.WhenAll(brownBlockMaster.CreateWorld(), redBlockMaster.CreateWorld(), blueBlockMaster.CreateWorld());
        SaveSystem.totalConverges++;
        SaveSystem.Save();
        ModeMaster.UpdateButton();
    }

    void FixedUpdate()
    {
        
    }

    public void OnDestroy()
    {
        SaveSystem.Save();
    }
}
