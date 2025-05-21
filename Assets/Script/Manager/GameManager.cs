using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public TMP_InputField inputField;
    public Button gameStartButton;


    // Start is called before the first frame update
    void Start()
    {
        gameStartButton.onClick.AddListener(OnGameStartButtonClicked);
    }

    private void OnGameStartButtonClicked()
    {
        string playerName = inputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("플레이어 이름이 없어!");
            return;
        }
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        Debug.Log("플레이어 이름 저장 됨 :" + playerName);
        SceneManager.LoadScene("Stage1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
