using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public Slider stepDelaySlider;
    public Slider nodeSizeSlider;
    public Button selectStartButton;
    public Button selectTargetButton;
    public Button stopStartButton;
    public GameObject toastPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

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
