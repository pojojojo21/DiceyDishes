using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

[CreateAssetMenu(fileName = "New Player", menuName = "Player")]
public class Player : ScriptableObject
{
    public string playerName;
    public int playerBalance = 20;
    public int playerDieCount = 3;
    public List<RecipeCard> playerRecipeCards = new List<RecipeCard>(5);
    public List<RecipeCard.diceType> playerDiceList = new List<RecipeCard.diceType>(8);
    public List<bool> recipeMade = new List<bool>(5);

    private void OnEnable()
    {
        playerBalance = 20;
        playerDieCount = 3;

        InitializeDiceList();
    }

    private void InitializeDiceList()
    {
        playerDiceList.Clear();

        // Initialize the playerDiceList list with default diceNums items
        for (int i = 0; i < 3; i++)
        {
            this.playerDiceList.Add(RecipeCard.diceType.Question);
        }
    }

    public void SetRecipesFromPlayer(Player player)
    {
        for (int i = 0; i < 5; i++)
        {
            this.playerRecipeCards[i] = player.playerRecipeCards[i].newRecipeCard();
        }
    }

    public void SetDiceFromPlayer(Player player)
    {
        this.playerDiceList.Clear();

        for (int i = 0; i < player.playerDieCount; i++)
        {
            this.playerDiceList.Add(player.playerDiceList[i]);
        }
    }

    public void SetMadeFromPlayer(Player player)
    {
        this.recipeMade.Clear();

        for (int i = 0; i < player.recipeMade.Count; i++)
        {
            this.recipeMade.Add(player.recipeMade[i]);
        }
    }

    public int ResolveRecipes()
    {
        int moneyEarned = 0;

        for (int i = 0;i < this.recipeMade.Count;i++)
        {
            if (this.recipeMade[i])
            {
                RecipeCard r = this.playerRecipeCards[i];

                // calculate multiplier from tier
                float multiplier = 1.0f;
                if (r.tier == RecipeCard.cardTier.Bottom)
                {
                    multiplier = 0.5f;
                } else if (r.tier == RecipeCard.cardTier.Top)
                {
                    multiplier = 1.5f;
                }

                // add recipe amount to total
                moneyEarned += (int)Mathf.Ceil(r.coins * multiplier);

                // move card tier down
                 r.MoveDown();

                // set recipe back to unmade
                this.recipeMade[i] = false;
                r.made = false;
            }
        }

        playerBalance += moneyEarned;
        return moneyEarned;
    }

}
