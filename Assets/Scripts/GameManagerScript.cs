using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // set up inputs
    public RecipeCardDisplay recipeCardDisplay;
    
    // list trackers
    public List<Player> players = new List<Player>();
    public List<bool> playersReady = new List<bool>();
    public List<RecipeCard> listofRecipeCards = new List<RecipeCard>();

    // Decks of Cards
    public List<RecipeCard> starterCards = new List<RecipeCard>();    public List<RecipeCard> recipeDeck = new List<RecipeCard>();
    public List<RecipeCard> shop = new List<RecipeCard>();

    // user vairables
    public List<RecipeCardDisplay> userHand = new List<RecipeCardDisplay>();
    public int user;

    // singleton
    public static GameManagerScript singleton;

    // 0 - setUp, 1 - Ready to Start Round, 2 - Buy Dice, Roll Dice, and Make Recipes, 3 - Resolve Recipes, Gain Money 4 - Buy Recipes, reset to 1
    public int currentPhase = 0;

    // current round
    public int round = 1;

    // Startup of Game
    private void Start()
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

        // set current user from players
        user = 1;

        // set player name
        players[user].playerName = PlayerInfoController.playerName;

        // set current phase to 0
        currentPhase = 0;

        // set round to 1
        round = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPhase != 0)
        {
            // update current player's balance on screen
            PlayerDisplay.singleton.UpdatePlayerBalance();

            // update current player's cards on screen
            PlayerDisplay.singleton.UpdatePlayerDieCount();

            // update user hand
            UpdateUserHand();
        }
    }


    // On Game Start - currentphase at 0
    // On Finish set currentpahse to 1
    public void OnNewGameStart()
    {
        for (int i = 0; i < 5;  i++)
        {
            players[0].playerRecipeCards[i] = starterCards[i].newRecipeCard();
        }

        // set players all to start up player
        ResetPlayers();

        // set round to 1
        round = 1;

        // Set up recipe cards for this player invoked - player enter start
        SetUpUserHand();

        // Set up recipe card deck
        SetUpRecipeDeck();

        // Set up event deck
        SetUpEventDeck();

        // Set up critic deck
        SetUpCriticDeck();

        // When finished set up, start first round
        Debug.Log("Game set up complete");

        // set current phase to 1
        currentPhase = 1;

    }

    // Reset players to default
    void ResetPlayers()
    {
        Player player = players[0];
        for (int i = 1; i < players.Count; i++)
        {
            players[i].playerBalance = player.playerBalance;
            players[i].playerDieCount = player.playerDieCount;
            players[i].SetRecipesFromPlayer(player);
            players[i].SetDiceFromPlayer(player);
            players[i].SetMadeFromPlayer(player);
        }
    }

    // Set up player 1 card display
    void SetUpUserHand()
    {
        // Set parent position
        Vector3 parent = this.transform.position;
        parent += new Vector3(-160, 25, -60);
        
        // Set up cards in hand
        for (int i = 0; i < 5; i++)
        {
            RecipeCardDisplay card = Instantiate(recipeCardDisplay);
            card.transform.position = parent;
            card.recipeCard = players[user].playerRecipeCards[i];
            userHand.Add(card);
            parent += new Vector3(75, 0, 0);
        }
    }

    // Update player 1 card display 
    void UpdateUserHand()
    {
        int i = 0;
        // Set up cards in hand
        foreach (RecipeCardDisplay curr in userHand) 
        {
            curr.recipeCard = players[user].playerRecipeCards[i];
            i++;
        }
    }

    // reset recipe deck
    void SetUpRecipeDeck()
    {
        recipeDeck.Clear();

        foreach (RecipeCard card in listofRecipeCards)
        {
            recipeDeck.Add(card.newRecipeCard());
        }
    }

    // reset event deck
    void SetUpEventDeck()
    {
        Debug.Log("Set up event card deck");
    }

    // reset critic deck
    void SetUpCriticDeck()
    {
        Debug.Log("Set up critic card deck");
    }
    
    // Initiate beginning of round
    public void StartRound()
    {
        // Fill shop with 5 cards from recipe card deck
        FillShop();

        // Reset dice
        ResetDice();

        // Flip event card
        FlipEventCard();

        // Flip critic card when needed
        FlipCriticCard();

        // Commence buy dice phase - 2
        currentPhase = 2;
    }

    // Fill all empty slots in shop
    void FillShop()
    {
        Debug.Log("Fill recipe shop");
        for (int i = shop.Count; i < 5; i++) // from index where shop starts building up until shop is at length 5
        {
            drawRecipeCard();
        }
    }

    // reset all dice counts back to 3 and display back to 3
    void ResetDice()
    {
        Player player = players[0];
        for (int i = 1; i < players.Count; i++)
        {
            players[i].playerDieCount = player.playerDieCount;
            players[i].SetDiceFromPlayer(player);
        }
    }

    void FlipEventCard()
    {
        Debug.Log("Flip Event Card");
    }

    void FlipCriticCard()
    {
        Debug.Log("Flip Critic Card");
    }

    void drawRecipeCard()
    {
        int i = Random.Range(0, recipeDeck.Count);
        RecipeCard r = recipeDeck[i];
        recipeDeck.Remove(r);
        shop.Add(r);
    }

    public RecipeCard BuyFromDeck()
    {
        int i = Random.Range(0, recipeDeck.Count);
        RecipeCard r = recipeDeck[i];
        recipeDeck.Remove(r);
        return r;
    }
}
