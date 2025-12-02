using System.Linq;
using UnityEngine;

public class CookingSystem : MonoBehaviour
{
    public DeckManager deck;

    public bool CanCook(PlayerState p, MenuCardData m)
    {
        foreach (var kv in m.Req)
        {
            int need = kv.Value;
            int have = p.Get(kv.Key);
            int wild = p.Get(Ingredient.Goboy);

            if (have + wild < need) return false;
        }
        return true;
    }

    public bool Cook(PlayerState p, MenuCardData m, int marketIndex)
    {
        if (!CanCook(p, m)) return false;

        foreach (var kv in m.Req)
        {
            int need = kv.Value;
            int own = Mathf.Min(p.Get(kv.Key), need);
            if (own > 0) p.Spend(kv.Key, own);

            int rem = need - own;
            if (rem > 0) p.Spend(Ingredient.Goboy, rem);
        }

        int totalBonusVP = 0;
        if (m.Optionals != null)
        {
            foreach (var opt in m.Optionals)
            {
                if (p.Get(opt.Ingredient) > 0)
                {
                    p.Spend(opt.Ingredient, 1);
                    totalBonusVP += opt.VP;
                }
                else if (p.Get(Ingredient.Goboy) > 0)
                {
                    p.Spend(Ingredient.Goboy, 1);
                    totalBonusVP += opt.VP;
                }
            }
        }

        p.VP += (m.BaseVP + totalBonusVP);

        MenuCardData takenCard = deck.TakeFromMarket(marketIndex);
        if (takenCard != null) p.Cooked.Add(takenCard);

        var cust = deck.DrawCust();
        if (cust != null)
        {
            p.CustHand.Add(cust);
            Debug.Log($"SUCCESS: Drawn Customer {cust.Name}");
        }
        else
        {
            Debug.LogWarning("WARNING: No Customer drawn! Is the Customer Deck empty?");
        }

        p.CookedThisPhase = true;
        return true;
    }
}