using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct IngredientAmount
{
    public Ingredient ingredient;
    public int amount;
}

[System.Serializable]
public struct OptionalBonusEntry
{
    public Ingredient ingredient;
    public int vpBonus;
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewMenuCard", menuName = "Waroong/Menu Card")]
public class MenuCardSO : ScriptableObject
{
    public string menuName;
    public int baseVP;

    [Header("Deck Configuration")]
    [Tooltip("How many copies of this card are in the deck?")]
    [Range(1, 10)]
    public int quantityInDeck = 1;

    public Sprite icon;

    [Header("Recipe (MANDATORY)")]
    public List<IngredientAmount> requirements;

    [Header("Optionals (Supports Multiple)")]
    public List<OptionalBonusEntry> optionalBonuses;

    public MenuCardData ToRuntimeData()
    {
        MenuCardData d = new MenuCardData();
        d.Name = menuName;
        d.BaseVP = baseVP;
        d.Icon = icon;

        d.Req = new Dictionary<Ingredient, int>();
        foreach (var item in requirements)
        {
            if (d.Req.ContainsKey(item.ingredient)) d.Req[item.ingredient] += item.amount;
            else d.Req.Add(item.ingredient, item.amount);
        }

        d.Optionals = new List<OptionalBonusData>();
        if (optionalBonuses != null)
        {
            foreach (var opt in optionalBonuses)
            {
                d.Optionals.Add(new OptionalBonusData
                {
                    Ingredient = opt.ingredient,
                    VP = opt.vpBonus
                });
            }
        }

        return d;
    }
}

[CreateAssetMenu(fileName = "NewCustomerCard", menuName = "Waroong/Customer Card")]
public class CustomerCardSO : ScriptableObject
{
    public string customerName;
    public string effectCode;
    public int vpBonus;
    public Sprite icon;

    public CustomerCardData ToRuntimeData()
    {
        return new CustomerCardData
        {
            Name = customerName,
            Effect = effectCode,
            VP = vpBonus,
            Icon = icon
        };
    }
}