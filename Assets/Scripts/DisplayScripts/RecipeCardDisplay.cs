using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCardDisplay : MonoBehaviour
{
    public RecipeCard recipeCard;
    

    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI priceText;

    public TextMeshProUGUI dicetext;
    public Image dicePic;
    public Image typeBorderColor;

    private Collider col;
    public ExpandedRecipeCardDisplay expandedRecipeCardDisplay;
    private ExpandedRecipeCardDisplay cardDisplay;
    private bool isExpanded = false;

    // Start is called before the first frame update
    private void Start()
    {
        col = GetComponent<Collider>();

        displaySet();
    }

    private void Update()
    {
        displaySet();

        if (Input.GetMouseButton(0) && isExpanded)
        {
            Destroy(cardDisplay.gameObject);
            isExpanded = false;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && col.Raycast(ray, out hit, 300))
        {
            // Debug.Log("Clicked" + recipeCard.name); // open recipe card
            // open viewer
            Vector3 parent = this.transform.position;
            parent.x = 0;
            Quaternion q = this.transform.rotation;
            parent += new Vector3(-10, 60, 35);
            q *= Quaternion.Euler(-15, 0, 0);
            cardDisplay = Instantiate(expandedRecipeCardDisplay);
            cardDisplay.transform.position = parent;
            cardDisplay.transform.rotation = q;
            cardDisplay.recipeCard = recipeCard;
            StartCoroutine(DeleteCardCoroutine());
        }

        if (recipeCard.made)
        {
            Color newColor = new Color(1.0f, 0.6666666f, 0.0f, 1.0f);
            dicePic.color = newColor;
        } else
        {
            Color newColor = new Color(0.9450981f, 0.7686275f, 0.4156863f, 1.0f);
            dicePic.color = newColor;
        }
    }

    IEnumerator DeleteCardCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        isExpanded = true;
    }
    
    void displaySet()
    {
        cardNameText.text = recipeCard.name;
        priceText.text = recipeCard.basePrice.ToString();

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

        SetDicePrices();
    }

    void SetDicePrices()
    {
        // clear text
        dicetext.text = "";

        foreach (RecipeCard.diceType num in  recipeCard.cost)
        {
            string newText = dicetext.text + RecipeCard.stringFromDiceType(num) + " ";
            dicetext.text = newText;
        }
        
    }
}
