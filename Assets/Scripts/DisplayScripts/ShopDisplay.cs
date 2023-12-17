using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopDisplay : MonoBehaviour
{
    public int cardNumber;
    public RecipeCard recipeCard;
    public GameObject cardDisplay;

    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI priceText;

    public TextMeshProUGUI dicetext;
    public Image dicePic;
    public Image typeBorderColor;

    // Start is called before the first frame update
    void Start()
    {
        // set the recipe card of the shop display
        if (GameManagerScript.singleton.shop.Count >= cardNumber)
        {
            cardDisplay.SetActive(true);
            recipeCard = GameManagerScript.singleton.shop[cardNumber - 1];

            // set up card display based on recipe card
            setRecipeDisplays();
            cardNameText.text = recipeCard.name;
            priceText.text = recipeCard.coins.ToString();
            SetDicePrices();
        }
        else
        {
            cardDisplay.SetActive(false);
        }
    }

    private void Update()
    {
        // check if there are any updates
        if (GameManagerScript.singleton.shop.Count >= cardNumber)
        {
            cardDisplay.SetActive(true);
            recipeCard = GameManagerScript.singleton.shop[cardNumber - 1];
            setRecipeDisplays();
            SetDicePrices();
        }
        else
        {
            cardDisplay.SetActive(false);
        }
    }
    
    void setRecipeDisplays()
    {
        switch (recipeCard.type[0])
        {
            case RecipeCard.recipeType.Entree:
                typeBorderColor.color = Color.blue;
                break;
            case RecipeCard.recipeType.Appetizer:
                typeBorderColor.color = Color.red;
                break;
            case RecipeCard.recipeType.Side:
                typeBorderColor.color = Color.green;
                break;
            case RecipeCard.recipeType.Dessert:
                typeBorderColor.color = Color.magenta;
                break;
        }
    }

    void SetDicePrices()
    {
        dicetext.text = "";

        foreach (RecipeCard.diceType num in recipeCard.cost)
        {
            string newText = dicetext.text + RecipeCard.stringFromDiceType(num) + " ";
            dicetext.text = newText;
        }

    }
}
