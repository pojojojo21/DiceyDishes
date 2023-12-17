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
        priceText.text = recipeCard.coins.ToString();
    }

    private void Update()
    {
        // check if there are any updates
        recipeCard = player.playerRecipeCards[cardNumber - 1];
        setRecipeDisplays();

        if (Input.GetMouseButton(0) && isExpanded)
        {
            Destroy(cardDisplay.gameObject);
            isExpanded = false;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (Input.GetMouseButtonDown(0) && hit.collider == col)
        {
            Debug.Log("Clicked" + recipeCard.name); // open recipe card
            // open viewer
            Vector3 parent = this.transform.position;
            parent.x = 0;
            Quaternion q = this.transform.rotation;
            parent += new Vector3(0, 75, 50);
            q *= Quaternion.Euler(-15, 0, 0);
            cardDisplay = Instantiate(expandedRecipeCardDisplay);
            cardDisplay.transform.position = parent;
            cardDisplay.transform.rotation = q;
            cardDisplay.recipeCard = recipeCard;
            StartCoroutine(DeleteCardCoroutine());
        }
    }

    IEnumerator DeleteCardCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        isExpanded = true;
    }

    void setRecipeDisplays()
    {
        switch (recipeCard.tier)
        {
            case RecipeCard.cardTier.Bottom:
                priceText.color = Color.red;
                this.transform.position = position + (-330 * Vector3.up);
                break;
            case RecipeCard.cardTier.Mid:
                priceText.color = Color.white;
                this.transform.position = position;
                break;
            case RecipeCard.cardTier.Top:
                priceText.color = Color.blue;
                this.transform.position = position + (330 * Vector3.up);
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
}
