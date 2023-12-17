using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe Card", menuName = "Cards/Recipe")]
public class RecipeCard : ScriptableObject
{
    public new string name;
    public string instant;
    public string effect;

    public Sprite artwork;

    public int coins;
    public cardTier tier;
    public List<recipeType> type;
    public List<diceType> cost;

    public bool made = false;

    public enum cardTier
    {
        Bottom,Mid,Top
    }

    public void MoveDown()
    {
        switch (this.tier)
        {
            case cardTier.Mid:
                this.tier =  cardTier.Bottom;
                break;
            case cardTier.Top:
                this.tier = cardTier.Mid;
                break;
            default:
                this.tier = cardTier.Bottom;
                break;

        }
    }

    public void MoveUp()
    {
        switch (this.tier)
        {
            case cardTier.Mid:
                this.tier = cardTier.Top;
                break;
            case cardTier.Bottom:
                this.tier = cardTier.Mid;
                break;
            default:
                this.tier = cardTier.Top;
                break;

        }
    }

    public enum recipeType
    {
        Entree,Side,Appetizer,Dessert
    }

    public enum diceType
    {
        One,Two,Three,Four,Five,Six,X,Y,Z,XplusOne,XplusTwo,XplusThree,XplusFour,XplusFive,Question
    }

    public RecipeCard newRecipeCard()
    {
        RecipeCard card = new RecipeCard();
        card.name = name;
        card.instant = instant;
        card.effect = effect;
        card.coins = coins;
        card.tier = tier;
        List<recipeType> t = new List<recipeType>();
        foreach (recipeType type in type)
        {
            t.Add(type);
        }
        card.type = t;

        List<diceType> d = new List<diceType>();
        foreach (diceType dice in cost)
        {
            d.Add(dice);
        }
        card.cost = d;

        return card;
    }
    public static string stringFromDiceType(diceType diceType)
    {
        switch (diceType)
        {
            case diceType.One:
                return "1";
            case diceType.Two:
                return "2";
            case diceType.Three:
                return "3";
            case diceType.Four:
                return "4";
            case diceType.Five:
                return "5";
            case diceType.Six:
                return "6";
            case diceType.X:
                return "X";
            case diceType.Y:
                return "Y";
            case diceType.Z:
                return "Z";
            case diceType.XplusOne:
                return "X+1";
            case diceType.XplusTwo:
                return "X+2";
            case diceType.XplusThree:
                return "X+3";
            case diceType.XplusFour:
                return "X+4";
            case diceType.XplusFive:
                return "X+5";
            default:
                return "?";
        }
    }

    public static diceType DiceTypeFromString(string s)
    {
        switch (s)
        {
            case "1":
                return diceType.One;
            case "2":
                return diceType.Two;
            case "3":
                return diceType.Three;
            case "4":
                return diceType.Four;
            case "5":
                return diceType.Five;
            case "6":
                return diceType.Six;
            case "X":
                return diceType.X;
            case "Y":
                return diceType.Y;
            case "Z":
                return diceType.Z;
            case "X+1":
                return diceType.XplusOne;
            case "X+2":
                return diceType.XplusTwo;
            case "X+3":
                return diceType.XplusThree;
            case "X+4":
                return diceType.XplusFour;
            case "X+5":
                return diceType.XplusFive;
            default:
                return diceType.Question;
        }
    }

    public bool VerifyDice(List<diceType> dice)
    {
        // make dictionary of die rolls
        Dictionary<diceType, int> rollsDict = makeDict(dice);

        // make dictionary of die costs
        Dictionary<diceType, int> diceDict = makeDict(cost);

        // for each specific number diceCost find and remove from the unused list
        // if unable to find number then return false

        // check each number case in the roll
        int count;
        diceType curr = diceType.One;

        for (int i = 0; i < 6; i++)
        {
            if (diceDict.ContainsKey(curr))
            {
                rollsDict.TryGetValue(curr, out count);

                if (count == diceDict[curr])
                {
                    diceDict.Remove(curr);
                    rollsDict.Remove(curr);
                }
                else if (count > diceDict[curr])
                {
                    rollsDict[curr] -= diceDict[curr];
                    diceDict.Remove(curr);
                }
                else
                {
                    return false;
                }
            }
            curr = PlusPlus(curr);
        }

        List<diceType> keys = new List<diceType>();
        foreach (KeyValuePair<diceType,int> roll in rollsDict)
        {
            keys.Add(roll.Key);
        }

        // check if this recipe is looking for any striaghts/Pluses
        List<diceType> straights = CheckXPlusExists(diceDict);
        if (straights.Count > 0 )
        {
            List<int> indices = FindStraightExists(straights, keys);

            if (indices.Count > 0 )
            {
                foreach (int i in indices)
                {
                    rollsDict.Remove(keys[i]);
                }

                diceDict.Remove(diceType.X);

                foreach (diceType d in straights)
                {
                    diceDict.Remove(d);
                }
            } else { return false; }
        }

        // Check X in roll
        diceType x = diceType.Question;

        if (diceDict.ContainsKey(diceType.X))
        {
            foreach (KeyValuePair<diceType,int> num in rollsDict)
            {
                if (num.Value == diceDict[diceType.X])
                {
                    diceDict.Remove(diceType.X);
                    rollsDict.Remove(num.Key);
                    x = num.Key;
                    break;
                }
                else if (num.Value > diceDict[diceType.X])
                {
                    rollsDict[num.Key] -= diceDict[diceType.X];
                    diceDict.Remove(diceType.X);
                    x = num.Key;
                    break;
                }
            }

            if (x == diceType.Question)
            {
                return false;
            }
        }

        // Check Y in roll
        diceType y = diceType.Question;

        if (diceDict.ContainsKey(diceType.Y))
        {
            foreach (KeyValuePair<diceType, int> num in rollsDict)
            {
                if (num.Value == diceDict[diceType.Y] && num.Key != x)
                {
                    diceDict.Remove(diceType.Y);
                    rollsDict.Remove(num.Key);
                    y = num.Key;
                    break;
                }
                else if (num.Value > diceDict[diceType.Y] && num.Key != x)
                {
                    rollsDict[num.Key] -= diceDict[diceType.Y];
                    diceDict.Remove(diceType.Y);
                    y = num.Key;
                    break;
                }
            }

            if (y == diceType.Question)
            {
                return false;
            }
        }

        // Check Z in roll
        diceType z = diceType.Question;

        if (diceDict.ContainsKey(diceType.Z))
        {
            foreach (KeyValuePair<diceType, int> num in rollsDict)
            {
                if (num.Value == diceDict[diceType.Z] && num.Key != x && num.Key != y)
                {
                    diceDict.Remove(diceType.Z);
                    rollsDict.Remove(num.Key);
                    z = num.Key;
                    break;
                }
                else if (num.Value > diceDict[diceType.Z] && num.Key != x && num.Key != y)
                {
                    rollsDict[num.Key] -= diceDict[diceType.Z];
                    diceDict.Remove(diceType.Z);
                    z = num.Key;
                    break;
                }
            }

            if (z == diceType.Question)
            {
                return false;
            }
        }

        if (diceDict.ContainsKey(diceType.Question))
        {
            foreach (KeyValuePair<diceType, int> num in rollsDict)
            {
                if (num.Value == diceDict[diceType.Question])
                {
                    diceDict.Remove(diceType.Question);
                    rollsDict.Remove(num.Key);
                    break;
                }
                else if (diceDict[diceType.Question] > num.Value)
                {
                    diceDict[diceType.Question] -= num.Value;
                    rollsDict.Remove(num.Key);
                }
                else { break; }
            }
        }

        if (rollsDict.Count == 0 && diceDict.Count == 0)
        {
            return true;
        }
        return false;
    }

    Dictionary<diceType, int> makeDict(List<diceType> dice)
    {
        Dictionary<diceType, int> dict = new Dictionary<diceType, int>();

        foreach (diceType num in dice)
        {
            if (!dict.ContainsKey(num))
            {
                dict[num] = 1;
            }
            else
            {
                dict[num]++;
            }
        }

        return dict;
    }

    // check given progression in given dice for indices of the given progression (else return emtpty)
    List<diceType> CheckXPlusExists(Dictionary<diceType,int> dict)
    {
        List<diceType> list = new List<diceType>();

        foreach (diceType type in dict.Keys)
        {
            if ((type == diceType.XplusOne) ||  (type == diceType.XplusTwo) || (type == diceType.XplusThree) || (type == diceType.XplusFour) || (type == diceType.XplusFive))
            {
                list.Add(type);
            }
        }

        return list;
    }

    // return list if indexs in roll that makes up straight or empty
    List<int> FindStraightExists(List<diceType> cost, List<diceType> rolls)
    {
        List<int> list = new List<int>();

        if (rolls.Count < cost.Count)
        { return list; }

        rolls.Sort();

        int count = 0;

        foreach (diceType roll in rolls)
        {
            diceType x = roll;
            list.Add(rolls.FindIndex((y) => y == x));

            foreach (diceType costType in cost)
            {
                diceType curr = FindXPlus(x, costType);
                if (!rolls.Contains(curr))
                {
                    list.Clear();
                    count = 0;
                    break;
                }
                list.Add(rolls.FindIndex((x) => x == curr));
                count++;
            }

            if (count == cost.Count)
            {
                break;
            }
        }

        return list;
    }

    int toInt(diceType diceType)
    {
        switch (diceType)
        {
            case diceType.One:
                return 1;
            case diceType.Two:
                return 2;
            case diceType.Three:
                return 3;
            case diceType.Four:
                return 4;
            case diceType.Five:
                return 5;
            case diceType.Six:
                return 6;
            default:
                return 0;
        }
    }

    diceType PlusPlus(diceType diceType)
    {
        switch (diceType)
        {
            case diceType.One:
                return diceType.Two;
            case diceType.Two:
                return diceType.Three;
            case diceType.Three:
                return diceType.Four;
            case diceType.Four:
                return diceType.Five;
            case diceType.Five:
                return diceType.Six;
            case diceType.X:
                return diceType.XplusOne;
            case diceType.XplusOne:
                return diceType.XplusTwo;
            case diceType.XplusTwo:
                return diceType.XplusThree;
            case diceType.XplusThree:
                return diceType.XplusFour;
            case diceType.XplusFour:
                return diceType.XplusFive;
            default:
                return diceType.Question;
        }
    }

    diceType FindXPlus(diceType dice, diceType type)
    {
        switch (type)
        {
            case diceType.XplusOne:
                return PlusPlus(dice);
            case diceType.XplusTwo:
                dice = PlusPlus(dice);
                return PlusPlus(dice);
            case diceType.XplusThree:
                dice = PlusPlus(dice);
                dice = PlusPlus(dice);
                return PlusPlus(dice);
            case diceType.XplusFour:
                dice = PlusPlus(dice);
                dice = PlusPlus(dice);
                dice = PlusPlus(dice);
                return PlusPlus(dice);
            case diceType.XplusFive:
                dice = PlusPlus(dice);
                dice = PlusPlus(dice);
                dice = PlusPlus(dice);
                dice = PlusPlus(dice);
                return PlusPlus(dice);
            default:
                return diceType.Question;
        }
    }
}
