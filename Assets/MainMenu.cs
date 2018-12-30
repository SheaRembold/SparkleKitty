using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject contButton;
    [SerializeField]
    GameObject newButton;
    [SerializeField]
    GameObject loading;

    private void Awake()
    {
        /*if (!File.Exists(Application.persistentDataPath + "/cats_" + DataManager.saveVersion + ".txt")
            && !File.Exists(Application.persistentDataPath + "/help_" + DataManager.saveVersion + ".txt")
            && !File.Exists(Application.persistentDataPath + "/mailbox_" + DataManager.saveVersion + ".txt")
            && !File.Exists(Application.persistentDataPath + "/PlayArea(Clone)_" + DataManager.saveVersion + ".txt")
            && !File.Exists(Application.persistentDataPath + "/inventory_" + DataManager.saveVersion + ".txt"))
        {
            contButton.SetActive(false);
        }*/
        loading.SetActive(false);
    }

    public void Continue()
    {
        contButton.SetActive(false);
        newButton.SetActive(false);
        loading.SetActive(true);
        SceneManager.LoadSceneAsync(1);
    }

    public void NewGame()
    {
        if (File.Exists(Application.persistentDataPath + "/cats_" + DataManager.saveVersion + ".txt"))
            File.Delete(Application.persistentDataPath + "/cats_" + DataManager.saveVersion + ".txt");
        if (File.Exists(Application.persistentDataPath + "/help_" + DataManager.saveVersion + ".txt"))
            File.Delete(Application.persistentDataPath + "/help_" + DataManager.saveVersion + ".txt");
        if (File.Exists(Application.persistentDataPath + "/mailbox_" + DataManager.saveVersion + ".txt"))
            File.Delete(Application.persistentDataPath + "/mailbox_" + DataManager.saveVersion + ".txt");
        if (File.Exists(Application.persistentDataPath + "/PlayArea(Clone)_" + DataManager.saveVersion + ".txt"))
            File.Delete(Application.persistentDataPath + "/PlayArea(Clone)_" + DataManager.saveVersion + ".txt");
        if (File.Exists(Application.persistentDataPath + "/inventory_" + DataManager.saveVersion + ".txt"))
            File.Delete(Application.persistentDataPath + "/inventory_" + DataManager.saveVersion + ".txt");

        contButton.SetActive(false);
        newButton.SetActive(false);
        loading.SetActive(true);
        SceneManager.LoadSceneAsync(1);
    }
}