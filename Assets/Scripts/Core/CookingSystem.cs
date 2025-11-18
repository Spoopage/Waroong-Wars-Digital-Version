using System.Linq;
using UnityEngine;

public class CookingSystem : MonoBehaviour {
    public DeckManager deck; 

    public bool CanCook(PlayerState p, MenuCardData m){
        foreach (var kv in m.Req){
            int need = kv.Value;
            int have = p.Get(kv.Key);
            int wild = p.Get(Ingredient.Goboy);
            if (have + wild < need) return false;
        }
        return true;
    }

    public bool Cook(PlayerState p, MenuCardData m){
        if (!CanCook(p, m)) return false;

        foreach (var kv in m.Req){
            int need = kv.Value;
            int own = Mathf.Min(p.Get(kv.Key), need);
            if (own > 0) p.Spend(kv.Key, own);
            int rem = need - own;
            if (rem > 0) p.Spend(Ingredient.Goboy, rem);
        }

        int bonus = 0;
        if (m.OptionalKey.HasValue && m.OptionalMax > 0 && m.OptionalVP != 0){
            if (p.Character == CharacterType.JengSastro && m.Req.Count(r => r.Key == m.OptionalKey) == 0) {
                 bonus += m.OptionalVP; 
            } else {
                int can = Mathf.Min(p.Get(m.OptionalKey.Value) + p.Get(Ingredient.Goboy), m.OptionalMax);
                for(int i=0; i<can; i++){
                    if (!p.Spend(m.OptionalKey.Value, 1)) p.Spend(Ingredient.Goboy, 1);
                    bonus += m.OptionalVP;
                }
            }
        }

        p.VP += m.BaseVP + bonus;
        p.Cooked.Add(m);
        p.MenuHand.Remove(m);
        p.CookedThisPhase = true;

        if (p.Character == CharacterType.BuPrasojo) {
            int totalIng = m.Req.Values.Sum();
            if (totalIng >= 4) {
                var card = deck.DrawCust();
                if (card != null) p.CustHand.Add(card);
            }
        }

        return true;
    }
}