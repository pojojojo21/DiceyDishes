using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartGame : MonoBehaviour
{
    public TMP_InputField playerNameText;
    public TextMeshProUGUI playerCountText;

    public void OnCall()
    {

        if (string.IsNullOrEmpty(playerNameText.text))
        {
            return;
        }
        else {
            // Set player name
            PlayerInfoController.playerName = playerNameText.text;

            // Set player count
            switch(playerCountText.text)
            {
                case "One":
                    PlayerInfoController.playerCount = 1;
                    break;
                case "Two":
                    PlayerInfoController.playerCount = 2;
                    break;
                case "Three":
                    PlayerInfoController.playerCount = 3;
                    break;
                case "Four":
                    PlayerInfoController.playerCount = 4;
                    break;
                case "Five":
                    PlayerInfoController.playerCount = 5;
                    break;
                case "Six":
                    PlayerInfoController.playerCount = 6;
                    break;
                default:
                    PlayerInfoController.playerCount = 1;
                    break;
            }

            // Change Current Scene to GameScene
            SceneManager.LoadScene("GameScene");
        }
    }
}
