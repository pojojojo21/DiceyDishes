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
            // your next reroll this round costs 0 coins.
            p.rerollBoost = 3;
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
            // its value is increased to 20 coins this round.
            r.totalValue = 20;
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
    public void Effect(RecipeCard r, Player p)
    {
        // EFFECT
        // If this is the first recipe you make this round,
        if (p.recipeOrderMade[0].name == r.name)
        {
            // this recipe counts as both an Appetizer and an Entree for all purposes this round.
            r.type.Add(RecipeCard.recipeType.Entree);
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
            // Next reroll costs 0 coins.
            p.rerollBoost = 3;
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
            // gain 5 coins,
            p.playerBalance += 5;
        } else
        {
            // otherwise, lose 3 coins.
            p.playerBalance -= 3;
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
            for (int i = 0; i < 5; i++)
            {
                RecipeCard card = p.playerRecipeCards[i];

                if (card.name == "Dino Nuggets")
                {
                    card.totalValue += 12;
                    return;
                }
            }
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
            // gain 10 coins.
            p.playerBalance += 10;
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

public class BaconCheeseburger : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If this recipe is in your middle track.
        if (r.tier == RecipeCard.cardTier.Mid)
        {
            // This card's value is increased to 20 coins
            r.totalValue = 20;
        }
    }
}

public class BakedZiti : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Give the Start Player token to a player of your choice.
        
        // no multiplayer implementation yet
    }
}

public class Curry : RecipeEffects
{
    // INPUT
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Move one of your Recipes down 1 track on your Menu.
        // You cannot choose a Recipe already on the bottom track.
        r.inputCard.MoveDown();
    }
}

public class GlazedHam : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        // EFFECT
        // The values of each of your Appetizers and Desserts are increased by 4 coins this round.
        for (int i = 0; i < 5; i++)
        {
            RecipeCard card = p.playerRecipeCards[i];

            foreach (RecipeCard.recipeType t in card.type)
            {
                if (t == RecipeCard.recipeType.Dessert || t == RecipeCard.recipeType.Appetizer)
                {
                    card.totalValue += 4;
                }
            }
        }
    }
}

public class GoldenLobster : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        // EFFECT
        // Earn a star
        p.stars++;
    }
}

public class Goulash : RecipeEffects
{
    // This recipe can be made once per round with 1 or more dice.
    // Its base value is the sum of the dice used to make it this round.
    // When multiplying this card's value, round up.
    public void Effect(RecipeCard r, Player p)
    {
        r.totalValue = 0;
        int i;
        foreach (RecipeCard.diceType dice in r.cost)
        {
            int.TryParse(RecipeCard.stringFromDiceType(dice), out i);
            r.totalValue += i;
        }
        r.cost.Clear();
        r.cost.Add(RecipeCard.diceType.Question);
    }
}

public class PulledPork : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Increase the value of each of your Sides by 6 this round.
        for (int i = 0; i < 5; i++)
        {
            RecipeCard card = p.playerRecipeCards[i];

            foreach (RecipeCard.recipeType t in card.type)
            {
                if (t == RecipeCard.recipeType.Side)
                {
                    card.totalValue += 6;
                }
            }
        }
    }
}

public class StirFry : RecipeEffects
{
    // You may immediately purchase a Recipe from the discard pile for 5 coins
    // (this includes your own discarded starter Recipes)
    public void Effect(RecipeCard r, Player p)
    {
        // EFFECT

        // NOT IMPLEMENTED
    }
}

public class Sushi : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        // INSTANT
        // Next reroll costs 0 coins.
        p.rerollBoost = 3;
    }
}

public class AppleSlices : RecipeEffects
{

    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Increase the value of your Dino Nuggets by 8 coins this round.
        for (int i = 0; i < 5; i++)
        {
            RecipeCard card = p.playerRecipeCards[i];

            if (card.name == "Dino Nuggets")
            {
                card.totalValue += 8;
                return;
            }
        }
    }
}

public class BakedBeans : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If you made at least one Entree this round, this card's value is 10 coins.
        foreach (RecipeCard card in p.recipeOrderMade)
        {
            foreach (RecipeCard.recipeType t in card.type)
            {
                if (t == RecipeCard.recipeType.Entree)
                {
                    r.totalValue = 10;
                    return;
                }
            }
        }
    }
}

public class ChickenNoodleSoup : RecipeEffects
{
    // REQUIRES INPUT
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Take a recipe from the shop
        // it must replace this card when purchasing recipes in the same track this card was in
        // find index of this card

        for (int i = 0; i < 5; i++)
        {
            if (p.playerRecipeCards[i].name == r.name)
            {
                p.cardBought.tier = r.tier;
                p.cardReplaced = i;
                break;
            }
        }
    }
}

public class Chili : RecipeEffects
{
    // Move all of the recipes on your bottom track (excluding Chili) to your middle track. 
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Move all of the recipes on your bottom track (excluding Chili) to your middle track. 
        for (int i = 0; i < 5; i++)
        {
            if (p.playerRecipeCards[i].tier == RecipeCard.cardTier.Bottom && p.playerRecipeCards[i].name != r.name)
            {
                p.playerRecipeCards[i].MoveUp();
            }
        }
    }
}

public class MacnCheese : RecipeEffects
{
    // REQUIRES INPUT
    
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // Move one of your Entrees to the top track of your menu.
        r.inputCard.MoveUp();
        r.inputCard.MoveUp();
    }
}

public class MashedPotatoes : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // The values of each of your Entrees are increased by 6 this round.
        for (int i = 0; i < 5; i++)
        {
            RecipeCard card = p.playerRecipeCards[i];

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

public class SteakFries : RecipeEffects
{
    public void Effect(RecipeCard r, Player p)
    {
        //EFFECT
        // If you made High Quality Steak this round, gain 4 coins.
        foreach (RecipeCard card in p.recipeOrderMade)
        {
            if (card.name == "High Quality Steak")
            {
                p.playerBalance += 4;
            }
        }

        // If you made at least one Entree this round, gain 6 coins.
        foreach (RecipeCard card in p.recipeOrderMade)
        {
            foreach (RecipeCard.recipeType t in card.type)
            {
                if (t == RecipeCard.recipeType.Entree)
                {
                    p.playerBalance += 6;
                    return;
                }
            }
        }
    }
}