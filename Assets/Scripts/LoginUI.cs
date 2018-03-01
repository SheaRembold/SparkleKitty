using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField]
    Text userNameLabel;
    [SerializeField]
    InputField userNameInput;
    [SerializeField]
    InputField passwordInput;

    private void Start()
    {
        userNameLabel.text = GameSparksManager.Instance.UserName;
        GameSparksManager.Instance.onUserLogin += OnUserLogin;
    }

    void OnUserLogin(string userName)
    {
        userNameLabel.text = userName;
    }

    public void Register()
    {
        GameSparksManager.Instance.Register(userNameInput.text, passwordInput.text);
    }

    public void Login()
    {
        GameSparksManager.Instance.Login(userNameInput.text, passwordInput.text);
    }
}