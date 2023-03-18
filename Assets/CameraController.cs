using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    //TODO disable this in certain modes
    private Camera camera;

    public int minCamDist = 5;
    public int maxCamDist = 20;
    public int camDist = 10;

    public bool isDragging = false;
    public Vector3 dragOrigin;
    public Vector2 bounds;

    [SerializeField] private Image grabCursor;
    [SerializeField] private Image grabCursorPointer;
    public float dragSpeed = 2;

    [SerializeField] private int camDistLast = 10;
    [SerializeField] private bool isDisabled = false;
    void Start()
    {
        camera = GameMaster.instance.camera;
    }

    void Update()
    {
        if (isDisabled)
        {
            transform.position =new Vector3(0, -5, -10);
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, camDist, Time.deltaTime * 10);
            return;
        }
        
        int larger = Mathf.Max(SaveSystem.instance.GetSaveFile().farmSave.farmSize.x, SaveSystem.instance.GetSaveFile().farmSave.farmSize.y);
        bounds = new Vector2(larger, larger);
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
        } else if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
            grabCursor.enabled =false;
            grabCursorPointer.enabled = false;
        }

        if (isDragging)
        {
            Vector3 pos = camera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
            Transform transform1;
            (transform1 = transform).Translate(move, Space.World);
            var position = transform1.position;
            position = new Vector3(Mathf.Clamp(position.x, -bounds.x*2, bounds.x*2), Mathf.Clamp(position.y, -bounds.y*2-5, bounds.y-5), position.z);
            transform.position = position;
            grabCursorPointer.transform.rotation = Quaternion.Euler(0, 0,Mathf.Atan2(Input.mousePosition.y - dragOrigin.y, Input.mousePosition.x - dragOrigin.x) * Mathf.Rad2Deg );
        }
    }

    public void Disable()
    {
        if (isDisabled)return;
        isDisabled = true;
        camDistLast = camDist;
        camDist = 10;
    }
    
    public void Enable()
    {
        if (!isDisabled)return;
        isDisabled = false;
        camDist = camDistLast;
    }
}
