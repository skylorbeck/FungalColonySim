using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public RectTransform settingsMenu;
    public RectTransform statsMenu;
    public GameObject settingsRayBlocker;
    public Toggle confirmDelete;
    public Button resetButton;

    public void Start()
    {
        confirmDelete.isOn = false;
    }

    public void OpenSettings()
    {
        settingsMenu.gameObject.SetActive(true);
        settingsMenu.DOLocalMoveY(0, 0.5f);
        settingsRayBlocker.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayerPrefs.Save();
        confirmDelete.isOn = false;
        settingsMenu.DOLocalMoveY(2000, 0.5f).OnComplete(() => settingsMenu.gameObject.SetActive(false));
        settingsRayBlocker.SetActive(false);
    }

    public void ToggleDelete(bool value)
    {
        resetButton.interactable = value;
    }

    public void ConfirmDelete()
    {
        if (!confirmDelete.isOn) return;
        SaveSystem.instance.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenStats()
    {
        statsMenu.gameObject.SetActive(true);
        statsMenu.DOLocalMoveY(0, 0.5f);
        settingsRayBlocker.SetActive(true);
    }

    public void CloseStats()
    {
        statsMenu.DOLocalMoveY(2000, 0.5f).OnComplete(() => statsMenu.gameObject.SetActive(false));
        settingsRayBlocker.SetActive(false);
    }
}