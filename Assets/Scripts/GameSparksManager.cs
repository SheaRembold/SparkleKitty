using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class GameSparksManager : MonoBehaviour
{
    public static GameSparksManager Instance;

    string userName;
    public string UserName { get { return userName; } }

    public delegate void OnUserLogin(string userName);
    public OnUserLogin onUserLogin;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GetAccount();
    }

    void SetUser(string userName)
    {
        this.userName = userName;
        if (onUserLogin != null)
            onUserLogin(userName);
    }

    void GetAccount()
    {
        AccountDetailsRequest request = new AccountDetailsRequest();
        request.Send((response) =>
        {
            if (response.HasErrors)
            {
                Debug.Log("AccountDetailsRequest HasErrors");
            }
            else
            {
                Debug.Log("AccountDetailsRequest Success");
                SetUser(response.DisplayName);
            }
        });
    }

    public void Register(string userName, string password)
    {
        RegistrationRequest request = new RegistrationRequest().SetUserName(userName).SetDisplayName(userName).SetPassword(password);
        request.Send((response) =>
        {
            if (response.HasErrors)
            {
                Debug.Log("RegistrationRequest HasErrors");
            }
            else
            {
                Debug.Log("RegistrationRequest Success");
                SetUser(response.DisplayName);
            }
        });
    }

    public void Login(string userName, string password)
    {
        AuthenticationRequest request = new AuthenticationRequest().SetUserName(userName).SetPassword(password);
        request.Send((response) =>
        {
            if (response.HasErrors)
            {
                Debug.Log("AuthenticationRequest HasErrors");
            }
            else
            {
                Debug.Log("AuthenticationRequest Success");
                SetUser(response.DisplayName);
            }
        });
    }
}