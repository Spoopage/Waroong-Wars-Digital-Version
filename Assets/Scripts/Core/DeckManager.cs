using System;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using Random = System.Random;

public class DeckManager : MonoBehaviour {
    Random rng = new Random();

    Stack<Ingredient> ingDeck;
    Stack<MenuCardData> menuDeck;
    Stack<CustomerCardData> custDeck;

    [Header("Sprite Assets")]
    [SerializeField] private List<IngredientSpriteData> ingredientSprites;
    [SerializeField] private List<CardSpriteData> menuSprites;
    [SerializeField] private List<CardSpriteData> customerSprites;

    // Dictionary untuk pencarian cepat (Nama -> Gambar)
    private Dictionary<Ingredient, Sprite> ingSpriteDict;
    private Dictionary<string, Sprite> menuSpriteDict;
    private Dictionary<string, Sprite> custSpriteDict;

    [System.Serializable]
    public struct IngredientSpriteData {
        public Ingredient type;
        public Sprite icon;
    }

    [System.Serializable]
    public struct CardSpriteData {
        public string idName; // Harus sama persis dengan nama di kode (contoh: "Nasi Goreng")
        public Sprite icon;
    }

    void Awake() {
        InitializeSpriteDictionaries();
    }

    public void Init(){
        // Pastikan dictionary ada (jaga-jaga jika Awake tidak jalan karena script mati)
        if (ingSpriteDict == null) InitializeSpriteDictionaries(); 

        ingDeck = BuildIng();
        menuDeck = BuildMenus();
        custDeck = BuildCustomers();
    }

    // Mengubah List dari Inspector menjadi Dictionary agar mudah diambil
   void InitializeSpriteDictionaries() {
        // Tambahkan null check untuk list inspector agar tidak error jika kosong
        ingSpriteDict = new Dictionary<Ingredient, Sprite>();
        if (ingredientSprites != null) {
            foreach(var item in ingredientSprites) {
                if(!ingSpriteDict.ContainsKey(item.type)) ingSpriteDict.Add(item.type, item.icon);
            }
        }

        menuSpriteDict = new Dictionary<string, Sprite>();
        if (menuSprites != null) {
            foreach(var item in menuSprites) {
                if(!menuSpriteDict.ContainsKey(item.idName)) menuSpriteDict.Add(item.idName, item.icon);
            }
        }

        custSpriteDict = new Dictionary<string, Sprite>();
        if (customerSprites != null) {
            foreach(var item in customerSprites) {
                if(!custSpriteDict.ContainsKey(item.idName)) custSpriteDict.Add(item.idName, item.icon);
            }
        }
    }

    // Helper untuk mengambil gambar dengan aman
    Sprite GetIngSprite(Ingredient type) => ingSpriteDict.ContainsKey(type) ? ingSpriteDict[type] : null;
    Sprite GetMenuSprite(string name) => menuSpriteDict.ContainsKey(name) ? menuSpriteDict[name] : null;
    Sprite GetCustSprite(string name) => custSpriteDict.ContainsKey(name) ? custSpriteDict[name] : null;

    Stack<Ingredient> BuildIng(){
        var list = new List<Ingredient>();
        list.AddRange(Fill(Ingredient.Cabe, 2));
        list.AddRange(Fill(Ingredient.Krupuk, 2));
        list.AddRange(Fill(Ingredient.Tahu, 6));
        list.AddRange(Fill(Ingredient.Nasi, 8));
        list.AddRange(Fill(Ingredient.Bumbu, 5));
        list.AddRange(Fill(Ingredient.Sayur, 7));
        list.AddRange(Fill(Ingredient.Daging, 9));
        
        Shuffle(list);
        return new Stack<Ingredient>(list);
    }
     // Fungsi helper tambahan untuk memudahkan akses sprite ingredient dari luar
    public Sprite GetSpriteForIngredient(Ingredient ing) {
        return GetIngSprite(ing);
    }
    Stack<MenuCardData> BuildMenus(){
        var L = new List<MenuCardData>();
        MenuCardData Create(string name, int vp, (Ingredient, int) req, params (Ingredient, int)[] more) {
             var m = Menu(name, vp, req, more);
             m.Icon = GetMenuSprite(name); // Set Gambar disini
             return m;
        }

        // Overload untuk yang ada optional ingredients
        MenuCardData CreateOpt(string name, int vp, (Ingredient, int, int) opt, (Ingredient, int) req, params (Ingredient, int)[] more) {
             var m = Menu(name, vp, opt, req, more);
             m.Icon = GetMenuSprite(name); // Set Gambar disini
             return m;
        }
        L.Add(Menu("Nasi Kecap", 2, opt:(Ingredient.Krupuk, 2, 1), req:(Ingredient.Sayur, 1)));
        L.Add(Menu("Bebek Goreng", 2, opt:(Ingredient.Cabe, 2, 1), req:(Ingredient.Daging, 2)));
        L.Add(Menu("Soto Daging", 2, opt:(Ingredient.Cabe, 2, 1), req:(Ingredient.Nasi, 1), (Ingredient.Daging, 1)));
        L.Add(Menu("Nasi Goreng", 2, req:(Ingredient.Nasi, 2)));
        L.Add(Menu("Semanggi", 2, req:(Ingredient.Sayur, 2)));
        L.Add(Menu("Tahu Campur", 2, req:(Ingredient.Sayur, 1), (Ingredient.Daging, 1), (Ingredient.Tahu, 1)));
        L.Add(Menu("Nasi Rawon", 2, opt:(Ingredient.Cabe, 2, 1), req:(Ingredient.Nasi, 1), (Ingredient.Daging, 2)));
        L.Add(Menu("Lontong Balap", 2, opt:(Ingredient.Krupuk, 2, 1), req:(Ingredient.Nasi, 1), (Ingredient.Tahu, 1), (Ingredient.Bumbu, 1)));
        L.Add(Menu("Kupang Lontong", 2, req:(Ingredient.Nasi, 1), (Ingredient.Daging, 1), (Ingredient.Bumbu, 1)));
        L.Add(Menu("Nasi Krawu", 2, opt:(Ingredient.Cabe, 2, 1), req:(Ingredient.Nasi, 1), (Ingredient.Daging, 1), (Ingredient.Tahu, 1)));
        L.Add(Menu("Nasi Cumi", 2, opt:(Ingredient.Cabe, 2, 1), req:(Ingredient.Nasi, 2), (Ingredient.Daging, 1), (Ingredient.Bumbu, 1)));
        L.Add(Menu("Pecel", 2, opt:(Ingredient.Krupuk, 2, 1), req:(Ingredient.Sayur, 3), (Ingredient.Bumbu, 1)));
        L.Add(Menu("Tempe Penyet", 2, req:(Ingredient.Sayur, 1), (Ingredient.Tahu, 2), (Ingredient.Bumbu, 1)));
        L.Add(Menu("Bakso", 2, req:(Ingredient.Sayur, 1), (Ingredient.Daging, 1), (Ingredient.Tahu, 1), (Ingredient.Bumbu, 1)));
        L.Add(Menu("Tahu Tek", 2, opt:(Ingredient.Krupuk, 2, 1), req:(Ingredient.Nasi, 1), (Ingredient.Sayur, 1), (Ingredient.Tahu, 1), (Ingredient.Bumbu, 1)));
        L.Add(Menu("Rujak Cingur", 2, opt:(Ingredient.Krupuk, 2, 1), req:(Ingredient.Nasi, 1), (Ingredient.Sayur, 1), (Ingredient.Daging, 1), (Ingredient.Tahu, 1), (Ingredient.Bumbu, 1)));
        L.Add(Menu("Nasi Campur", 2, req:(Ingredient.Nasi, 1), (Ingredient.Sayur, 2), (Ingredient.Daging, 2), (Ingredient.Tahu, 1)));
        L.Add(Menu("Sate Klopo", 2, req:(Ingredient.Nasi, 1), (Ingredient.Daging, 3), (Ingredient.Tahu, 1)));
        Shuffle(L);
        return new Stack<MenuCardData>(L);
    }

    Stack<CustomerCardData> BuildCustomers(){
        // Helper singkat untuk membuat customer + gambar
        CustomerCardData C(string name, string eff, int vp = 0) {
            return new CustomerCardData { 
                Name = name, 
                Effect = eff, 
                VP = vp,
                Icon = GetCustSprite(name) // <--- Ini baris kuncinya, dia mengambil gambar
            };
        }
        var L = new List<CustomerCardData>{
            C("Grocer", "Add:Sayur"),
            C("Rice Vendor", "Add:Nasi"),
            C("Butcher", "Add:Daging"),
            C("Tofu Seller", "Add:Tahu"),
            C("Spicemonger", "Add:Bumbu"),
            C("Thief", "StealIng"),
            C("Political Candidate", "Draw2Keep1"), // Sekarang gambar akan terisi
            C("Singer", "VP:+3", 3),
            C("Travelling Merchant", "Swap"),
            C("Mobster", "VP:-1", -1),
            C("Foodie", "VP:+2", 2),
            C("Security", "BlockSteal"),
            C("Primadonna", "VP:+4", 4),
            C("Influencer", "StealSkill")
        };
        
        Shuffle(L);
        return new Stack<CustomerCardData>(L);
    }

    static void Shuffle<T>(IList<T> a){
        var r = new Random();
        for(int i=a.Count-1; i>0; i--){ int j=r.Next(i+1); (a[i], a[j]) = (a[j], a[i]); }
    }
    static IEnumerable<Ingredient> Fill(Ingredient k, int n){ for(int i=0; i<n; i++) yield return k; }

    public void RefillIngredients()
    {
        Debug.Log("DeckManager: Refilling Ingredient Deck (Reshuffle)");
        ingDeck = BuildIng(); // Membuat ulang tumpukan ingredient penuh
    }

    MenuCardData Menu(string name, int vp, (Ingredient, int) req, params (Ingredient, int)[] more){
        var m = new MenuCardData{ Name=name, BaseVP=vp };
        m.Req[req.Item1] = req.Item2;
        foreach(var t in more) m.Req[t.Item1] = t.Item2;
        return m;
    }
    MenuCardData Menu(string name, int vp, (Ingredient, int, int) opt, (Ingredient, int) req, params (Ingredient, int)[] more){
        var m = Menu(name, vp, req, more);
        m.OptionalKey = opt.Item1; m.OptionalVP = opt.Item2; m.OptionalMax = opt.Item3;
        return m;
    }

    public Ingredient? DrawIng() => ingDeck.Count > 0 ? ingDeck.Pop() : null;
    public MenuCardData DrawMenu() => menuDeck.Count > 0 ? menuDeck.Pop() : null;
    public CustomerCardData DrawCust() => custDeck.Count > 0 ? custDeck.Pop() : null;

    public int MenuCount => menuDeck.Count;
    public int IngCount => ingDeck.Count;
}