using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuCardDisplayScript : MonoBehaviour
{
    public Player player;
    public int cardNumber;
    public RecipeCard recipeCard;

    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI priceText;

    public TextMeshProUGUI dicetext;
    public Image DiceArtwork;
    public Image typeBorderColor;

    private Vector3 position;
    private Collider2D col;
    public ExpandedRecipeCardDisplay expandedRecipeCardDisplay;
    private ExpandedRecipeCardDisplay cardDisplay;
    private bool isExpanded = false;

    // Start is called before the first frame update
    void Start()
    {
        // set middle position of card
        position = this.transform.position;
        col = GetComponent<Collider2D>();

        // set the recipe card of the menu display
        recipeCard = player.playerRecipeCards[cardNumber - 1];

        // set up card display based on recipe card
        setRecipeDisplays();

        cardNameText.text = recipeCard.name;
        priceText.text = recipeCard.basePrice.ToString();
        SetDicePrices();
    }

    private void Update()
    {
        // set the recipe card of the menu display
        recipeCard = player.playerRecipeCards[cardNumber - 1];

        // set up card display based on recipe card
        setRecipeDisplays();

        cardNameText.text = recipeCard.name;
        priceText.text = recipeCard.basePrice.ToString();
        SetDicePrices();

    }

    void setRecipeDisplays()
    {
        switch (recipeCard.tier)
        {
            case RecipeCard.cardTier.Bottom:
                priceText.color = Color.red;
                this.transform.position = position + (-295 * Vector3.up);
                break;
            case RecipeCard.cardTier.Mid:
                priceText.color = Color.white;
                this.transform.position = position;
                break;
            case RecipeCard.cardTier.Top:
                priceText.color = Color.blue;
                this.transform.position = position + (295 * Vector3.up);
                break;
        }

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
