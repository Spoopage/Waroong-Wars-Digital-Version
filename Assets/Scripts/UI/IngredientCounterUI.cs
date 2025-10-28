using UnityEngine;
using TMPro;

public class IngredientCounterUI : MonoBehaviour {
    public TMP_Text label;
    public void Set(Ingredient k,int v){ label.text = $"{k} ×{v}"; }
}
