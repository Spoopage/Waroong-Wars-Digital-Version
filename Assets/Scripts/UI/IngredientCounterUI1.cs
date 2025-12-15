using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientCounterUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text label;
    public Image iconDisplay;

    public void Set(Ingredient k, int v, Sprite icon)
    {
        label.text = $"{k} x{v}";

        if (iconDisplay != null && icon != null)
        {
            iconDisplay.sprite = icon;
            iconDisplay.enabled = true;
        }
        else if (iconDisplay != null)
        {

        }
    }
}