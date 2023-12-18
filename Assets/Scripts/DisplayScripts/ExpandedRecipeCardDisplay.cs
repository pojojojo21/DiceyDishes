using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandedRecipeCardDisplay : MonoBehaviour
{
    public RecipeCard recipeCard;


    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI typeText;

    public TextMeshProUGUI diceText;
    public Image typeBorderColor;

    
    // Start is called before the first frame update
    void Start()
    {
        cardNameText.text = recipeCard.name;
        priceText.text = recipeCard.basePrice.ToString();
        SetDescriptionText();

        switch (recipeCard.tier)
        {
            case RecipeCard.cardTier.Bottom:
                priceText.color = Color.red;
                break;
            case RecipeCard.cardTier.Mid:
                priceText.color = Color.white;
                break;
            case RecipeCard.cardTier.Top:
                priceText.color = Color.blue;
                break;
        }

        switch (recipeCard.type[0])
        {
            case RecipeCard.recipeType.Entree:
                typeBorderColor.color = Color.blue;
                typeText.text = "Entree";
                break;
            case RecipeCard.recipeType.Appetizer:
                typeBorderColor.color = Color.red;
                typeText.text = "Appetizer";
                break;
            case RecipeCard.recipeType.Side:
                typeBorderColor.color = Color.green;
                typeText.text = "Side";
                break;
            case RecipeCard.recipeType.Dessert:
                typeBorderColor.color = Color.magenta;
                typeText.text = "Dessert";
                break;
        }

        SetDicePrices();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetDicePrices()
    {
        foreach (RecipeCard.diceType num in recipeCard.cost)
        {
            string newText = diceText.text + RecipeCard.stringFromDiceType(num) + " ";
            diceText.text = newText;
        }

    }

    void SetDescriptionText()
    {
        descriptionText.text = "";

        // set instant
        if (recipeCard.instant.Length > 0)
        {
            descriptionText.text += "Instant: " + recipeCard.instant.ToString() + "\n";
        }

        // set effect
        if (recipeCard.effect.Length > 0)
        {
            descriptionText.text += "Effect: " + recipeCard.effect.ToString() + "\n";
        }

        // set ability
        if (recipeCard.ability.Length > 0)
        {
            descriptionText.text += recipeCard.instant.ToString() + "\n";
        }
    }
}
