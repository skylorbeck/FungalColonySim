using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public RectTransform settingsMenu;
    public RectTransform statsMenu;
    public RectTransform offlineMenu;
    public GameObject settingsRayBlocker;
    public Toggle confirmDelete;
    public Button resetButton;

    public bool settingsOpen => settingsMenu.gameObject.activeSelf;
    public bool statsOpen => statsMenu.gameObject.activeSelf;

    public bool offlineOpen => offlineMenu.gameObject.activeSelf;

    public void Start()
    {
        confirmDelete.isOn = false;
    }

    public void OpenSettings()
    {
        SFXMaster.instance.PlayMenuClick();
        settingsMenu.gameObject.SetActive(true);
        settingsMenu.DOLocalMoveY(0, 0.5f);
        settingsRayBlocker.SetActive(true);
    }

    public void CloseSettings()
    {
        SFXMaster.instance.PlayMenuClick();
        PlayerPrefs.Save();
        confirmDelete.isOn = false;
        settingsMenu.DOLocalMoveY(2000, 0.5f).OnComplete(() => settingsMenu.gameObject.SetActive(false));
        settingsRayBlocker.SetActive(false);
    }

    public void ToggleDelete(bool value)
    {
        resetButton.interactable = value;
        SFXMaster.instance.PlayMenuClick();
    }

    public void ConfirmDelete()
    {
        if (!confirmDelete.isOn) return;
        SFXMaster.instance.PlayMenuClick();
        SaveSystem.instance.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenStats()
    {
        SFXMaster.instance.PlayMenuClick();
        statsMenu.gameObject.SetActive(true);
        statsMenu.DOLocalMoveY(0, 0.5f);
        settingsRayBlocker.SetActive(true);
    }

    public void CloseStats()
    {
        SFXMaster.instance.PlayMenuClick();
        statsMenu.DOLocalMoveY(2000, 0.5f).OnComplete(() => statsMenu.gameObject.SetActive(false));
        settingsRayBlocker.SetActive(false);
    }

    public void OpenOffline()
    {
        offlineMenu.gameObject.SetActive(true);
        offlineMenu.DOLocalMoveY(0, 0.5f);
        settingsRayBlocker.SetActive(true);
    }

    public void CloseOffline()
    {
        offlineMenu.DOLocalMoveY(2000, 0.5f).OnComplete(() => offlineMenu.gameObject.SetActive(false));
        settingsRayBlocker.SetActive(false);
    }

    public void EmailFeedback()
    {
        string email = "mailto:FCS@skylorbeck.website?subject=Feedback";
        Application.OpenURL(email);
    }
}