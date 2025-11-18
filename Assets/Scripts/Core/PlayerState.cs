using System.Collections.Generic;

public class PlayerState {
    public int Index;
    public CharacterType Character;
    public bool IsAI = false;
    public List<MenuCardData> MenuHand; 
    public List<CustomerCardData> CustHand = new List<CustomerCardData>();
    public List<MenuCardData> Cooked = new List<MenuCardData>();
    public Dictionary<Ingredient, int> Inv = new Dictionary<Ingredient, int> {
        {Ingredient.Nasi,0}, {Ingredient.Daging,0}, {Ingredient.Sayur,0},
        {Ingredient.Tahu,0}, {Ingredient.Bumbu,0}, {Ingredient.Krupuk,0},
        {Ingredient.Cabe,0}, {Ingredient.Goboy,0}
    };
    public int VP = 0;
    public bool CookedThisPhase = false;

    public int Get(Ingredient k) => Inv.TryGetValue(k, out var v) ? v : 0;
    public void Add(Ingredient k, int q) { Inv[k] = Get(k) + q; }
    public bool Spend(Ingredient k, int q) {
        int have = Get(k);
        if (have >= q) { Inv[k] = have - q; return true; }
        return false;
    }
    
    public void DiscardAllIngredients(){
        Inv = new Dictionary<Ingredient, int> {
            {Ingredient.Nasi,0}, {Ingredient.Daging,0}, {Ingredient.Sayur,0},
            {Ingredient.Tahu,0}, {Ingredient.Bumbu,0}, {Ingredient.Krupuk,0},
            {Ingredient.Cabe,0}, {Ingredient.Goboy,0}
        };
    }
}