using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EntryUIManager : MonoBehaviour
{
    public Button startButton;   
    public TMP_InputField codeText;
    public GameObject codePanel;

    public string conditionText;


    void Start()
    {
        codePanel.SetActive(true);
        startButton.interactable = false;

    }

    void Update()
    {

    }

    public void CheckCode()
    {
        startButton.interactable = true;

        if (codeText.text == " ")
            startButton.interactable = false;

    }

    public void StartApp() 
    {
        //FindObjectOfType<DataManager>().questionnaireAnswers.Add("a01_userCode", codeText.text);
        //FindObjectOfType<DataManager>().questionnaireAnswers.Add("a02_condition", condition.text);
        SceneManager.LoadScene(1);
    }

    public void ExitApp() => Application.Quit();

}
