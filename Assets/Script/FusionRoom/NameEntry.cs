using HeoWeb.Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameEntry : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] Button submitButton;

    public void SubmitName()
    {
        FusionConnection.instance.ConnectToLobby(nameInputField.text);
        canvas.SetActive(false);
    }

    public void ActivateButton()
    {
        submitButton.interactable = true;
    }
}
