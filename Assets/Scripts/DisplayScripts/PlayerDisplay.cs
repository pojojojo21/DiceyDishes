using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviour
{
    public Player player;

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerBalanceText;
    public TextMeshProUGUI playerDieCountText;

    public static PlayerDisplay singleton;

    // Start is called before the first frame update
    void Start()
    {
        // Set up singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }

        playerNameText.text = player.playerName;
        playerBalanceText.text = player.playerBalance.ToString();
        playerDieCountText.text = player.playerDieCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePlayerBalance()
    {
        playerBalanceText.text = player.playerBalance.ToString();
    }

    public void UpdatePlayerDieCount()
    {
        playerDieCountText.text = player.playerDieCount.ToString();
    }
}
