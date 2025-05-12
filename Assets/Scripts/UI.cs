using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    [Header("Map Controls")]
    public TMP_Dropdown mapSelector;
    public Slider brushSizeSlider;
    public Toggle brushToggle;
    public Toggle eraseToggle;

    [Header("Simulation Controls")]
    public Button selectStartButton;
    public Button selectTargetButton;
    public Button stopStartButton;
    public Slider stepDelaySlider;
    public Slider nodeSizeSlider;

    [Header("Visal Controls")]
    public Slider fCostFactorSlider;
    public Slider gCostFactorSlider;
    public Slider hCostFactorSlider;

    [Header("User Notification")]
    public GameObject toastPanel;

    public void ShowToast(String toastText, Color toastColor)
    {
        toastPanel.SetActive(true);
        toastPanel.GetComponent<Image>().color = toastColor;
        toastPanel.GetComponentInChildren<TextMeshProUGUI>().text = toastText;

    }

    public void ShowToastForSeconds(String toastText, Color toastColor, int seconds)
    {
        ShowToast(toastText, toastColor);
        StartCoroutine(HideToastAfter(seconds));
    }

    public void HideToast()
    {
        toastPanel.SetActive(false);

    }

    IEnumerator HideToastAfter(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideToast();
    }
}
