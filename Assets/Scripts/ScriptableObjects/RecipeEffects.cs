using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface RecipeEffects
{
    void Effect(RecipeCard r, Player p);
    // void End(RecipeCard r, Player p);
}

public class Milkshake : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        if (GameManagerScript.singleton.currentPhase == 2)
        {
            // INSTANT
            // Next reroll costs 1 coin.
            p.rerollBoost = 2;

        }
        else if (GameManagerScript.singleton.currentPhase == 3)
        {
            //EFFECT
            // If this is the last recipe you make this round,
            if (p.recipeOrderMade[p.recipeOrderMade.Count - 1].name == r.name)
            {
                // its value is increased by 6 coins.
                r.totalValue += 6;
            }
        }
    }
}

public class FishandChips : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // The next recipe you purchase this round costs 3 coins less
        p.shopBoost = 3;
    }
}

public class GarbagePlate : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        // INSTANT
        // Next reroll costs 0 coins.
        p.rerollBoost = 3;
    }
}

public class SideSalad : RecipeEffects
{
    // REQUIRES INPUT
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Move another Recipe up 1 track on your menu
        r.inputCard.MoveUp();
    }
}

public class ScrambledEggs : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Purchase the top card of the recipe deck immediately for 0 coins
        // it must replace this card when purchasing recipes
        p.cardBought = GameManagerScript.singleton.BuyFromDeck();
        
        // find index of this card
        for (int i = 0; i < 5; i++)
        {
            if (p.playerRecipeCards[i].name == r.name)
            {
                p.cardReplaced = i;
                break;
            }
        }
    }
}

public class Calamari : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // the value of the next Entree you make this round is increased by 6 coins.
            for (int i = 1; i < 5; i++)
            {
                foreach (RecipeCard.recipeType t in p.recipeOrderMade[i].type)
                {
                    if (t == RecipeCard.recipeType.Entree)
                    {
                        p.recipeOrderMade[i].totalValue += 6;
                        return;
                    }
                }
            }
        }
    }
}

public class ChipsandDip : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        // INSTANT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // you may change the result of one of your remaining dice to a result of you choosing.
            
            // NOT IMPLEMENTED
        }
    }
}

public class DeviledEggs : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        // INSTANT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // Next reroll costs 0 coins.
            p.rerollBoost = 3;

            // NOT FULLY IMMPLEMENTED
            // immediately reroll up to 3 of your remaining dice.
        }
    }
}

public class FriedShrimp : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        // INSTANT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // Next reroll costs 0 coins.
            p.rerollBoost = 3;
        }
    }
}

public class LoadedNachos : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // its value this round is 22 coins.
            r.totalValue = 22;
        }
    }
}

public class LoadedTots : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //INSTANT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // gain 6 coins.
            p.playerBalance += 6;
        }
    }
}

public class MozzarellaSticks : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //INSTANT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // you may place a die with a result of 2 on this card
            // to increase its value to 20 coins this round.
            r.totalValue = 20;

            // NO CHECK FOR DIE INPUT YET
        }
    }
}

public class PretzelBites : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // the value of each Entree you make this round is increased by 6 coins.
            foreach (RecipeCard card in p.recipeOrderMade)
            {
                foreach (RecipeCard.recipeType t in card.type)
                {
                    if (t == RecipeCard.recipeType.Entree)
                    {
                        card.totalValue += 6;
                    }
                }
            }
        }
    }
}

public class Sliders : RecipeEffects
{
    // If this is the first recipe you make this round,
    // this recipe counts as both an Appetizer and an Entree for all purposes this round.
    public void Effect(RecipeCard r, Player p)
    {
        // EFFECT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // this recipe counts as both an Appetizer and an Entree for all purposes this round.
            // r.tier.Add(RecipeCard.recipeType.Entree)
            // need a way of removing tier at end of round
            // NOT IMPLEMENTED
        }
    }
}

public class Wings : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // increase the value of your other recipes by 2 coins this round.
            for (int i = 1; i < 5; i++)
            {
                p.playerRecipeCards[i].totalValue += 2;
            }
        }
    }
}

public class Brownies : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        if (GameManagerScript.singleton.currentPhase == 2)
        {
            // INSTANT
            // Reroll all of your remaining dice for 0 coins.
            p.rerollBoost = 3;

            // NOT FULLY IMPLEMENTED

        }
        else if (GameManagerScript.singleton.currentPhase == 3)
        {
            //EFFECT
            // If this is the last recipe you make this round,
            if (p.recipeOrderMade[p.recipeOrderMade.Count - 1].name == r.name)
            {
                // increase its value by 4 this round
                r.totalValue += 4;
            }
        }
    }
}

public class ChocolateCake : RecipeEffects
{
    
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Move all of your other Desserts down one track on your Menu.
        for (int i = 0; i < 5; i++)
        {
            RecipeCard card = p.playerRecipeCards[i];
            
            foreach (RecipeCard.recipeType t in card.type)
            {
                if (t == RecipeCard.recipeType.Dessert && card.name != r.name)
                {
                    card.MoveDown();
                }
            }
        }
    }
}

public class ChocolateChipCookies : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the last recipe you make this round,
        if (p.recipeOrderMade[p.recipeOrderMade.Count - 1].name == r.name)
        {
            // gain 5 coins, otherwise, give each player 1 coin.
            p.playerBalance += 5;
            // no multiplayer yet
        }
    }
}

public class ChocolateMilk : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the last recipe you make this round,
        if (p.recipeOrderMade[p.recipeOrderMade.Count - 1].name == r.name)
        {
            // increase the value of your Dino Nuggets by 12 coins this round.
            
            // NOT IMPLEMENTED
        }
    }
}

public class Churros : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the last recipe you make this round,
        if (p.recipeOrderMade[p.recipeOrderMade.Count - 1].name == r.name)
        {
            // gain 5 coins.
            p.playerBalance += 5;
        }
    }
}

public class Macarons : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the last recipe you make this round,
        if (p.recipeOrderMade[p.recipeOrderMade.Count - 1].name == r.name)
        {
            // gain 10 coins if you did not make any sides or Appetizers this round.
            foreach (RecipeCard card in p.recipeOrderMade)
            {
                foreach (RecipeCard.recipeType t in card.type)
                {
                    if (t == RecipeCard.recipeType.Side || t == RecipeCard.recipeType.Appetizer)
                    {
                        return;
                    }
                }
            }
            // gain 5 coins.
            p.playerBalance += 5;
        }
    }
}

public class Mochi : RecipeEffects
{
    // REQUIRES INPUT
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the last recipe you make this round,
        if (p.recipeOrderMade[p.recipeOrderMade.Count - 1].name == r.name)
        {
            // Move another Recipe up 1 track on your menu
            r.inputCard.MoveUp();
        }
    }
}

public class StrawberrySundae : RecipeEffects
{
    // If this is the last recipe you make this round,
    // take one recipe from the shop. Place it on your Menu when purchasing recipes.
    // REQUIRES INPUT
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Purchase the top card of the recipe deck immediately for 0 coins
        // it must replace this card when purchasing recipes
        //p.cardBought = GameManagerScript.singleton.BuyFromDeck();

        //// find index of this card
        //for (int i = 0; i < 5; i++)
        //{
        //    if (p.playerRecipeCards[i].name == r.name)
        //    {
        //        p.cardReplaced = i;
        //        break;
        //    }
        //}

        // NOT IMPLEMENTED
    }
}

public class VanillaScoop : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this is the last recipe you make this round
        if (p.recipeOrderMade[p.recipeOrderMade.Count - 1].name == r.name)
        {
            // gain 2 coins for every recipe you made this round.
            p.playerBalance += 2 * p.recipeOrderMade.Count;
        }
    }
}

