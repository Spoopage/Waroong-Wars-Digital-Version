using System.Collections.Generic;

public class PlayerState {
    public int Index;
    // Hapus "= new()" agar kita bisa mengaturnya dari luar
    public List<MenuCardData> MenuHand; 
    public List<CustomerCardData> CustHand = new();
    public List<MenuCardData> Cooked = new();
    public Dictionary<Ingredient,int> Inv = new() {
        {Ingredient.Nasi,0},{Ingredient.Daging,0},{Ingredient.Sayur,0},
        {Ingredient.Tahu,0},{Ingredient.Bumbu,0},{Ingredient.Krupuk,0},
        {Ingredient.Cabe,0},{Ingredient.Goboy,0}
    };
    public int VP = 0;

    public int Get(Ingredient k)=> Inv.TryGetValue(k, out var v) ? v : 0;
    public void Add(Ingredient k,int q){ Inv[k]=Get(k)+q; }
    public bool Spend(Ingredient k,int q){
        int have = Get(k);
        if (have>=q){ Inv[k]=have-q; return true; }
        return false;
    }
}