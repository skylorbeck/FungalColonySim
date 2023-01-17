using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public BlockMaster blockMaster;
    [SerializeField] private Texture2D cursorSprite;
    [SerializeField] private Texture2D cursorClicking;
    [SerializeField] private Texture2D cursorTransparent;
    public Camera camera;
    public Tooltip tooltip;
    
    public SaveSystem SaveSystem;
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
    
    public async void Prestige()
    {
    
        await blockMaster.DissolveWorld();
        ScoreMaster.instance.Reset();
        UpgradeMaster.instance.Reset();
        await blockMaster.CreateTestWorld();
        SaveSystem.hivemindPoints++;
        SaveSystem.totalConverges++;
        SaveSystem.Save();
    }

    void FixedUpdate()
    {
        
    }

    public void OnDestroy()
    {
        SaveSystem.Save();
    }
}
