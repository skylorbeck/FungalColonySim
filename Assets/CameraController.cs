using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private Camera camera;

    public int minCamDist = 5;
    public int maxCamDist = 20;
    public int camDist = 10;
    
    public bool isDragging = false;
    public Vector3 dragOrigin;
    public Vector3 bounds;
    
    [SerializeField] private Image grabCursor;
    [SerializeField] private Image grabCursorPointer;
    
    public float dragSpeed = 2;
    void Start()
    {
        camera = GameMaster.instance.camera;
    }

    void Update()
    {
        int larger = Mathf.Max(GameMaster.instance.SaveSystem.farmSize.x, GameMaster.instance.SaveSystem.farmSize.y);
        bounds = new Vector3(larger, larger, 0);
        if (Input.mouseScrollDelta.y > 0)
        {
            camDist -= 1;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            camDist += 1;
        }
        camDist = Mathf.Clamp(camDist, minCamDist, maxCamDist); 
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, camDist, Time.deltaTime * 10);

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
            grabCursor.enabled = true;
            grabCursor.transform.position = Input.mousePosition;
            grabCursorPointer.enabled = true;
            Cursor.visible = false;
        } else if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
            grabCursor.enabled =false;
            grabCursorPointer.enabled = false;
            Cursor.visible = true;
        }

        if (isDragging)
        {
            Vector3 pos = camera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
            Transform transform1;
            (transform1 = transform).Translate(move, Space.World);
            var position = transform1.position;
            position = new Vector3(Mathf.Clamp(position.x, -bounds.x, bounds.x), Mathf.Clamp(position.y, -bounds.y-5, bounds.y-5), position.z);
            transform.position = position;
            grabCursorPointer.transform.rotation = Quaternion.Euler(0, 0,Mathf.Atan2(Input.mousePosition.y - dragOrigin.y, Input.mousePosition.x - dragOrigin.x) * Mathf.Rad2Deg );
        }
    }

    void FixedUpdate()
    {
        
    }
}
