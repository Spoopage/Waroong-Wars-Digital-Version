using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    [Header("Refs")]
    public TurnManager turn;
    public DeckManager deck;
    public CookingSystem cooking;

    [Header("Panels")]
    public RectTransform DeckPanel;
    public RectTransform MenuPanel;
    public RectTransform CookedPanel;
    public RectTransform IngredientPanel;
    public RectTransform CustomerPanel;
    public Button EndTurnButton;

    [Header("Prefabs")]
    public MenuCardUI MenuCardPrefab;
    public CustomerCardUI CustomerCardPrefab;
    public IngredientCounterUI IngredientCounterPrefab;

    void Start(){
        turn.Init();
        deck.Init();

        for(int p=0;p<turn.Players.Length;p++){
            var pl = turn.Players[p];
            // HAPUS BARIS INI:
            // for(int i=0;i<3;i++){ var m = deck.DrawMenu(); if (m!=null) pl.MenuHand.Add(m); } 
            
            // Baris-baris ini biarkan (untuk inventory & customer pribadi)
            for(int i=0;i<6;i++){ var ing = deck.DrawIng(); if (ing.HasValue) pl.Add(ing.Value,1); }
            for(int i=0;i<2;i++){ var cc = deck.DrawCust(); if (cc!=null) pl.CustHand.Add(cc); }
        }

        // TAMBAHKAN BLOK INI:
        // Ambil referensi ke daftar bersama (bisa dari pemain mana saja)
        var menuHand = turn.Players[0].MenuHand;
        // Isi daftar bersama satu kali (misal 5 kartu)
        for(int i=0;i<5;i++){ 
            var m = deck.DrawMenu(); 
            if (m!=null) menuHand.Add(m); 
        }


        EndTurnButton.onClick.AddListener(()=>{ turn.Next(); Refresh(); });
        Refresh();
    }

    public void Refresh(){
        var pl = turn.Players[turn.Active];
        Clear(MenuPanel); Clear(CustomerPanel); Clear(CookedPanel); Clear(IngredientPanel);

        // Baris ini sekarang akan menampilkan 'sharedMenuHand'
        // karena pl.MenuHand menunjuk ke sana.
        foreach (var m in pl.MenuHand)
            Instantiate(MenuCardPrefab, MenuPanel).Bind(m, pl, cooking, this);

        foreach (var c in pl.CustHand)
            Instantiate(CustomerCardPrefab, CustomerPanel).Bind(c, pl, this);

        foreach (var m in pl.Cooked){
            var item = Instantiate(IngredientCounterPrefab, CookedPanel);
            item.Set((Ingredient)999, 0); 
            item.label.text = m.Name;
        }

        foreach (var kv in pl.Inv.Where(kv=>kv.Value>0))
            Instantiate(IngredientCounterPrefab, IngredientPanel).Set(kv.Key, kv.Value);
    }

    public void UseCustomer(PlayerState self, CustomerCardData c){
        if (c.Effect.StartsWith("Draw:")){
            var keyStr = c.Effect.Split(':')[1];
            if (System.Enum.TryParse<Ingredient>(keyStr, out var key)) self.Add(key,1);
            else { var ing = deck.DrawIng(); if(ing.HasValue) self.Add(ing.Value,1); }
        } else if (c.Effect=="VP:+3" || c.Effect=="VP:+2"){
            self.VP += c.VP;
        }
        self.CustHand.Remove(c);
        Refresh();
    }

    void Clear(Transform t){ for(int i=t.childCount-1;i>=0;i--) Destroy(t.GetChild(i).gameObject); }
}