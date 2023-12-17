using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static RecipeCard;

public class UiInteractionsScript : MonoBehaviour
{
    public GameObject canvas_UI;

    GraphicRaycaster ui_raycaster;
    PointerEventData click_data;
    List<RaycastResult> click_results;

    bool isDiceDisplayOpen;
    public GameObject diceDisplay;
    bool isCardsDisplayOpen;
    public GameObject cardsDisplay;
    bool isShopDisplayOpen;
    public GameObject shopsDisplay;

    public List<GameObject> diceDisplayList;
    public List<GameObject> cardsDisplayList;
    public TextMeshProUGUI textInput;
    public TextMeshProUGUI userMessages;

    public Player player;
    public static UiInteractionsScript singleton;

    private string textParser;
    List<int> selectedDiceIndices = new List<int>();
    RecipeCard cardBought;

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

        ui_raycaster = canvas_UI.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();
        isDiceDisplayOpen = false;
        isCardsDisplayOpen = false;
        isShopDisplayOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetElementsClicked();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Start New Game on input when currentphase is 0
            if (textInput.text.Contains("Start") && GameManagerScript.singleton.currentPhase == 0)
            {
                ClearUserMessage();

                GameManagerScript.singleton.OnNewGameStart();
                
                // set player 1 active dice display
                SetActiveDice();

                // update any dice changes
                updateDice();

                // user message to start round
                userMessages.text = "Enter \"Start Round\"";
            }

            // start round when currentPhase = 1; means waiting for a round start
            if (textInput.text.Contains("Start Round") && GameManagerScript.singleton.currentPhase == 1)
            {
                ClearUserMessage();

                GameManagerScript.singleton.StartRound();
            }

            if (textInput.text.Contains("Buy") && GameManagerScript.singleton.currentPhase == 2)
            {
                // Find dice value
                int diceValue;
                textParser = textInput.text.Replace("Buy ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out diceValue);

                BuyDice(diceValue);
            }

            if (textInput.text.Contains("Roll") && GameManagerScript.singleton.currentPhase == 2)
            {
                RollDice();
            }

            if (textInput.text.Contains("Make") && GameManagerScript.singleton.currentPhase == 2)
            {
                // Find recipe card value
                int cardIndex;
                textParser = textInput.text.Replace("Make ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out cardIndex);

                List<diceType> list = new List<diceType>();
                foreach (int i in selectedDiceIndices)
                {
                    diceType t = DiceTypeFromString(diceDisplayList[i].GetComponentInChildren<TextMeshProUGUI>().text);
                    list.Add(t);
                }
                // check if dice selected meet recipe requirements
                bool check = player.playerRecipeCards[cardIndex].VerifyDice(list);
                if (check)
                {
                    Debug.Log("Make Recipe");
                    foreach (int i in selectedDiceIndices)
                    {
                        diceDisplayList[i].SetActive(false);
                    }
                    selectedDiceIndices.Clear();
                    player.recipeMade[cardIndex] = true;
                    player.playerRecipeCards[cardIndex].made = true;

                }
                else
                {
                    userMessages.text = "Required Dice Not Selected";

                    Invoke("ClearUserMessage", 3);
                }
            }

            if (textInput.text.Contains("Select") && GameManagerScript.singleton.currentPhase == 2)
            {
                // Find dice value
                int diceIndex;
                textParser = textInput.text.Replace("Select ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out diceIndex);

                // check if selected dice is active
                if (!diceDisplayList[diceIndex].activeSelf)
                {
                    userMessages.text = "Not an Active Dice";

                    Invoke("ClearUserMessage", 3);

                } else
                {
                    // add selected dice to list
                    selectedDiceIndices.Add(diceIndex);

                    // set selected dice green
                    diceDisplayList[diceIndex].GetComponentInChildren<Image>().color = Color.green;
                }
            }

            if (textInput.text.Contains("Deselect") && GameManagerScript.singleton.currentPhase == 2)
            {
                // Find dice value
                int diceIndex;
                textParser = textInput.text.Replace("Deselect ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out diceIndex);

                // deselect dice

                // remove selected dice from list
                if (selectedDiceIndices.Contains(diceIndex))
                {
                    selectedDiceIndices.Remove(diceIndex);
                    // set selected dice green
                    diceDisplayList[diceIndex].GetComponentInChildren<Image>().color = Color.black;
                } else
                {
                    userMessages.text = "Dice Not Selected";

                    Invoke("ClearUserMessage", 3);
                }
            }

            if (textInput.text.Contains("Ready") && GameManagerScript.singleton.currentPhase == 2)
            {
                // phase 2 over 

                // set this player as ready

                // when all players are ready or in our single player version currently end phase
                GameManagerScript.singleton.currentPhase = 3;

                // initialized Recipe Resolution - turn into loop through all players
                int moneyEarned = player.ResolveRecipes();

                userMessages.text = "You earned " + moneyEarned + " coins.";

                Invoke("ClearUserMessage", 3);

                GameManagerScript.singleton.currentPhase = 4;
            }

            if ((textInput.text.Contains("Buy")) && (GameManagerScript.singleton.currentPhase == 4) && (!cardBought))
            {
                // Find recipe card value
                int cardIndex;
                textParser = textInput.text.Replace("Buy ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out cardIndex);

                cardBought = GameManagerScript.singleton.shop[cardIndex];
                GameManagerScript.singleton.shop.Remove(cardBought);

                userMessages.text = "Discard a card from your hand.";
            }

            if ((textInput.text.Contains("Discard")) && (GameManagerScript.singleton.currentPhase == 4) && (cardBought))
            {
                // Find recipe card value
                int cardIndex;
                textParser = textInput.text.Replace("Discard ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out cardIndex);

                player.playerRecipeCards[cardIndex] = cardBought;
                cardBought = null;

                GameManagerScript.singleton.currentPhase = 1;
                GameManagerScript.singleton.round++;

                userMessages.text = "Round Over: \"Start Round\"";

                Invoke("ClearUserMessage", 3);
            }
        }
    }

    void GetElementsClicked()
    {
        click_data.position = Input.mousePosition;
        click_results.Clear();

        ui_raycaster.Raycast(click_data, click_results);

        foreach (RaycastResult result in click_results)
        {
            GameObject ui_element = result.gameObject;
            // Debug.Log(ui_element.name);
            
            if (ui_element.name == "OpenDiceDisplay" && GameManagerScript.singleton.currentPhase != 0)
            {
                diceDisplay.SetActive(!isDiceDisplayOpen);
                isDiceDisplayOpen = !isDiceDisplayOpen;
            }
            if (ui_element.name == "OpenCardsDisplay" && GameManagerScript.singleton.currentPhase != 0)
            {
                cardsDisplay.SetActive(!isCardsDisplayOpen);
                isCardsDisplayOpen = !isCardsDisplayOpen;
            }
            if (ui_element.name == "OpenShopDisplay" && GameManagerScript.singleton.currentPhase > 1)
            {
                shopsDisplay.SetActive(!isShopDisplayOpen);
                isShopDisplayOpen = !isShopDisplayOpen;
            }
        }
    }

    public void SetActiveDice()
    {
        switch (player.playerDieCount)
        {
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    diceDisplayList[i].SetActive(true);
                }
                for (int i = 3; i < 8; i++)
                {
                    diceDisplayList[i].SetActive(false);
                }
                break;
            case 4:
                for (int i = 0; i < 4; i++)
                {
                    diceDisplayList[i].SetActive(true);
                }
                for (int i = 4; i < 8; i++)
                {
                    diceDisplayList[i].SetActive(false);
                }
                break;
            case 5:
                for (int i = 0; i < 5; i++)
                {
                    diceDisplayList[i].SetActive(true);
                }
                for (int i = 5; i < 8; i++)
                {
                    diceDisplayList[i].SetActive(false);
                }
                break;
            case 6:
                for (int i = 0; i < 6; i++)
                {
                    diceDisplayList[i].SetActive(true);
                }
                for (int i = 6; i < 8; i++)
                {
                    diceDisplayList[i].SetActive(false);
                }
                break;
            case 7:
                for (int i = 0; i < 7; i++)
                {
                    diceDisplayList[i].SetActive(true);
                }
                for (int i = 7; i < 8; i++)
                {
                    diceDisplayList[i].SetActive(false);
                }
                break;
            case 8:
                for (int i = 0; i < 8; i++)
                {
                    diceDisplayList[i].SetActive(true);
                }
                break;
        }
    }

    void SetDiceInactive()
    {
        for (int i = 0; i < 8; i++)
        {
            diceDisplayList[i].SetActive(false);
        }
    }

    public void updateDice()
    {
        for (int i = 0; i < player.playerDieCount; i++)
        {
            diceDisplayList[i].GetComponentInChildren<TextMeshProUGUI>().text = RecipeCard.stringFromDiceType(player.playerDiceList[i]);
        }
    }

    // Player Actions

    // BUY DICE
    // Input: int for number of dice to buy
    // * if player has the money and dice number is a valid input *
    // Output: add input to player's dice count 
    void BuyDice(int diceNumber)
    {
        switch (diceNumber)
        {
            case 1:
                player.playerDieCount = 4;
                break;
            case 2:
                player.playerDieCount = 5;
                break;
            case 3:
                player.playerDieCount = 6;
                break;
            case 4:
                player.playerDieCount = 7;
                break;
            case 5:
                player.playerDieCount = 8;
                break;
            default:
                // not a valid dice count
                break;
        }

        // add number of dice to list
        for (int i = 0; i < diceNumber; i++)
        {
            player.playerDiceList.Add(RecipeCard.diceType.Question);
        }
        
        // set UI to display new number of dice
        SetActiveDice();
    }

    // ROLL DICE
    // Roll all dice for current player
    // Add n
    void RollDice()
    {
        Debug.Log("Rolling Dice");

        // set playerListDice to hold new rolled dice values
        for (int i = 0; i < player.playerDieCount; i++)
        {
            player.playerDiceList[i] = RandomDiceEnumGen();
        }

        // updates dice displayed to the screen
        updateDice();
    }

    // REROLL SPECIFIC DIE
    void ReRollDie(int id)
    {
        player.playerDiceList[id] = RandomDiceEnumGen();

        // updates dice displayed to the screen
        updateDice();
    }

    // Generates a random dice enum
    RecipeCard.diceType RandomDiceEnumGen()
    {
        switch (Random.Range(1, 7))
        {
            case 1:
                return RecipeCard.diceType.One;
            case 2:
                return RecipeCard.diceType.Two;
            case 3:
                return RecipeCard.diceType.Three;
            case 4:
                return RecipeCard.diceType.Four;
            case 5:
                return RecipeCard.diceType.Five;
            case 6:
                return RecipeCard.diceType.Six;
            default:
                return RecipeCard.diceType.Question;
        }
    }

    void ClearUserMessage()
    {
        userMessages.text = "";
    }
}
