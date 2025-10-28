using System.Linq;
using UnityEngine;

public class CookingSystem : MonoBehaviour {
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
        if (!CanCook(p,m)) return false;

        // pay required using own first then Goboy
        foreach (var kv in m.Req){
            int need = kv.Value;
            int own = Mathf.Min(p.Get(kv.Key), need);
            if (own>0) p.Spend(kv.Key, own);
            int rem = need - own;
            if (rem>0) p.Spend(Ingredient.Goboy, rem);
        }

        // optional
        int bonus = 0;
        if (m.OptionalKey.HasValue && m.OptionalMax>0 && m.OptionalVP!=0){
            int can = Mathf.Min(p.Get(m.OptionalKey.Value) + p.Get(Ingredient.Goboy), m.OptionalMax);
            for(int i=0;i<can;i++){
                if (!p.Spend(m.OptionalKey.Value,1)) p.Spend(Ingredient.Goboy,1);
                bonus += m.OptionalVP;
            }
        }

        p.VP += m.BaseVP + bonus;
        p.Cooked.Add(m);
        p.MenuHand.Remove(m);
        return true;
    }
}
