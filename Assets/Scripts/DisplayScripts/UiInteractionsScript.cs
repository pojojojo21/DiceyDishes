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
                SetActiveDice();
                updateDice();
            }

            if (textInput.text.Contains("Buy") && GameManagerScript.singleton.currentPhase == 2 && player.playerDieCount < 4)
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

            if (textInput.text.Contains("Reroll") && GameManagerScript.singleton.currentPhase == 2)
            {
                if (player.playerBalance >= 3 - player.rerollBoost && selectedDiceIndices.Count > 0)
                {
                    player.playerBalance -= 3 - player.rerollBoost;

                    foreach (int i in selectedDiceIndices)
                    {
                        ReRollDie(i);
                    }
                    DeSelectAllDice();

                    // updates dice displayed to the screen
                    updateDice();

                    player.rerollBoost = 0;
                }
                else
                {
                    userMessages.text = "Not enough coins";
                    Invoke("ClearUserMessage", 2);
                }
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

                RecipeCard r = player.playerRecipeCards[cardIndex];
                // check if dice selected meet recipe requirements
                bool check = r.VerifyDice(list);
                if (check)
                {
                    Debug.Log("Make Recipe");
                    foreach (int i in selectedDiceIndices)
                    {
                        diceDisplayList[i].SetActive(false);
                    }
                    DeSelectAllDice();

                    player.recipeMade[cardIndex] = true;
                    r.made = true;
                    player.recipeOrderMade.Add(r);

                    // evaluate instants
                    if (r.instant.Length > 0)
                    {
                        r.effectFunction.Effect(r, player);
                    }
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

                // select die
                SelectDie(diceIndex);
            }

            if (textInput.text.Contains("Deselect") && GameManagerScript.singleton.currentPhase == 2)
            {
                // Find die value
                int diceIndex;
                textParser = textInput.text.Replace("Deselect ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out diceIndex);

                // deselect dice
                DeSelectDie(diceIndex);
            }

            if (textInput.text.Contains("Ready") && GameManagerScript.singleton.currentPhase == 2)
            {
                // phase 2 over 
                SetDiceInactive();

                // set this player as ready

                // when all players are ready or in our single player version currently end phase
                GameManagerScript.singleton.currentPhase = 3;

                // evaluate all recipe effects that do not take inputs
                // if evaluation waits on input finish effect in Select
                List<RecipeCard> made = new List<RecipeCard>();
                foreach (RecipeCard card in player.recipeOrderMade)
                {
                    if (card.effect.Length > 0)
                    {
                        if (!card.input)
                        {
                            card.effectFunction.Effect(card, player);
                            made.Add(card);
                        }
                    }
                    else
                    {
                        made.Add(card);
                    }
                }

                foreach (RecipeCard card in made)
                {
                    player.recipeOrderMade.Remove(card);
                }

                // else resolve recipes here
                if (player.recipeOrderMade.Count == 0)
                {
                    // initialized Recipe Resolution - turn into loop through all players
                    int moneyEarned = player.ResolveRecipes();

                    userMessages.text = "You earned " + moneyEarned + " coins.";

                    Invoke("ClearUserMessage", 3);

                    GameManagerScript.singleton.currentPhase = 4;
                } else
                {
                    userMessages.text = "Select Recipe to use effect from " + player.recipeOrderMade[0].name;

                    Invoke("ClearUserMessage", 2);
                }
            }

            if (textInput.text.Contains("Select") && GameManagerScript.singleton.currentPhase == 3)
            {
                // Find recipe card value
                int cardIndex;
                textParser = textInput.text.Replace("Select ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out cardIndex);

                if (cardIndex < 0 || cardIndex > 4)
                {
                    userMessages.text = "Card input out of range.";
                    Invoke("ClearUserMessage", 2);
                    return;
                }

                // set input 
                RecipeCard card = player.recipeOrderMade[0];
                card.inputCard = player.playerRecipeCards[cardIndex];

                // run effect
                card.effectFunction.Effect(card, player);

                card.inputCard = null;
                player.recipeOrderMade.Remove(card);

                // else resolve recipes here
                if (player.recipeOrderMade.Count == 0)
                {
                    // initialized Recipe Resolution - turn into loop through all players
                    int moneyEarned = player.ResolveRecipes();

                    userMessages.text = "You earned " + moneyEarned + " coins.";

                    Invoke("ClearUserMessage", 3);

                    GameManagerScript.singleton.currentPhase = 4;
                }
                else
                {
                    userMessages.text = "Select Recipe to use effect from " + player.recipeOrderMade[0].name;

                    Invoke("ClearUserMessage", 2);
                }
            }

            if ((textInput.text.Contains("Buy")) && (GameManagerScript.singleton.currentPhase == 4) && (!player.cardBought))
            {
                // Find recipe card value
                int cardIndex;
                textParser = textInput.text.Replace("Buy ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out cardIndex);

                if (cardIndex < 0 || cardIndex > 4)
                {
                    userMessages.text = "Card input out of range.";
                    Invoke("ClearUserMessage", 2);
                    return;
                }
                BuyRecipe(cardIndex);
            }

            if ((textInput.text.Contains("Discard")) && (GameManagerScript.singleton.currentPhase == 4) && (player.cardBought))
            {
                // Find recipe card value
                int cardIndex;
                textParser = textInput.text.Replace("Discard ", "");
                textParser = textParser.Remove(textParser.Length - 1, 1);
                int.TryParse(textParser, out cardIndex);

                if (cardIndex < 0 || cardIndex > 4)
                {
                    userMessages.text = "Card input out of range.";
                    Invoke("ClearUserMessage", 2);
                } else
                {
                    player.playerRecipeCards[cardIndex] = player.cardBought;
                    player.cardBought = null;
                    player.shopBoost = 0;

                    GameManagerScript.singleton.currentPhase = 1;
                    GameManagerScript.singleton.round++;

                    userMessages.text = "Round Over: \"Start Round\"";

                    Invoke("ClearUserMessage", 3);
                }
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

    // BUY SHOP RECIPE
    // * if player has the money and recipe number is a valid input *
    // Output: set recipe bought 
    void BuyRecipe(int recipeNumber)
    {
        // buy card from shop 0-4, off top of deck 5
        if (recipeNumber == 5)
        {
            if (player.playerBalance >= 3 - player.shopBoost)
            {
                player.playerBalance -= 3 - player.shopBoost;
                player.cardBought = GameManagerScript.singleton.BuyFromDeck();
            } else
            {
                userMessages.text = "Not enoguh coins.";

                Invoke("ClearUserMessage", 2);
                return;
            }
            
        } else if (recipeNumber > 0 && recipeNumber < 5)
        {
            if (player.playerBalance >= 4 - player.shopBoost)
            {
                player.playerBalance -= 4 - player.shopBoost;
                player.cardBought = GameManagerScript.singleton.shop[recipeNumber];
                GameManagerScript.singleton.shop.Remove(player.cardBought);
            } else
            {
                userMessages.text = "Not enoguh coins.";

                Invoke("ClearUserMessage", 2);
                return;
            }
        }
        

        userMessages.text = "Discard a card from your hand.";
    }

    // BUY DICE
    // Input: int for number of dice to buy
    // * if player has the money and dice number is a valid input *
    // Output: add input to player's dice count 
    void BuyDice(int diceNumber)
    {
        switch (diceNumber)
        {
            case 1:
                if (player.playerBalance >= 3)
                {
                    player.playerBalance -= 3;
                    player.playerDieCount = 4;
                    break;
                }
                userMessages.text = "Not enough coins";
                Invoke("ClearUserMessage", 2);
                break;
            case 2:
                if (player.playerBalance >= 8)
                {
                    player.playerBalance -= 8;
                    player.playerDieCount = 5;
                    break;
                }
                userMessages.text = "Not enough coins";
                Invoke("ClearUserMessage", 2);
                break;
            case 3:
                if (player.playerBalance >= 15)
                {
                    player.playerBalance -= 15;
                    player.playerDieCount = 6;
                    break;
                }
                userMessages.text = "Not enough coins";
                Invoke("ClearUserMessage", 2);
                break;
            case 4:
                if (player.playerBalance >= 24)
                {
                    player.playerBalance -= 24;
                    player.playerDieCount = 7;
                    break;
                }
                userMessages.text = "Not enough coins";
                Invoke("ClearUserMessage", 2);
                break;
            case 5:
                if (player.playerBalance >= 35)
                {
                    player.playerBalance -= 35;
                    player.playerDieCount = 8;
                    break;
                }
                userMessages.text = "Not enough coins";
                Invoke("ClearUserMessage", 2);
                break;
            default:
                // not a valid dice count
                userMessages.text = "Not a valid dice purchase";
                Invoke("ClearUserMessage", 2);
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
    void ReRollDie(int die)
    {
        // check if selected dice is active
        if (!diceDisplayList[die].activeSelf)
        {
            userMessages.text = "Not an Active Dice";

            Invoke("ClearUserMessage", 3);

        }
        else
        {
            // generate new number
            player.playerDiceList[die] = RandomDiceEnumGen();
        }
    }

    void SelectDie(int die)
    {
        // check if selected dice is active
        if (!diceDisplayList[die].activeSelf)
        {
            userMessages.text = "Not an Active Dice";

            Invoke("ClearUserMessage", 3);

        }
        else
        {
            // add selected dice to list
            selectedDiceIndices.Add(die);

            // set selected dice green
            diceDisplayList[die].GetComponentInChildren<Image>().color = Color.green;
        }
    }

    void DeSelectDie(int die)
    {
        // remove selected dice from list
        if (selectedDiceIndices.Contains(die))
        {
            selectedDiceIndices.Remove(die);
            // set selected dice back to black
            diceDisplayList[die].GetComponentInChildren<Image>().color = Color.black;
        }
        else
        {
            userMessages.text = "Dice Not Selected";

            Invoke("ClearUserMessage", 3);
        }
    }

    void DeSelectAllDice()
    {
        foreach (int die in selectedDiceIndices)
        {
            // set selected dice back to black
            diceDisplayList[die].GetComponentInChildren<Image>().color = Color.black;
        }

        selectedDiceIndices.Clear();
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
