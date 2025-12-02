using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class DeckManager : MonoBehaviour {
    [Header("Data Sources")]
    public List<MenuCardSO> allMenuCards; 
    public List<CustomerCardSO> allCustomerCards;
    
    [Header("Sprite Assets")]
    public List<IngredientSpriteData> ingredientSprites;
    private Dictionary<Ingredient, Sprite> ingSpriteDict;

    private Stack<Ingredient> ingDeck;
    private Stack<MenuCardData> menuDeck;
    private Stack<CustomerCardData> custDeck;

    public List<MenuCardData> ActiveMarket { get; private set; } = new List<MenuCardData>();

    [System.Serializable]
    public struct IngredientSpriteData {
        public Ingredient type;
        public Sprite icon;
    }
    
    void Awake() { InitializeSpriteDictionaries(); }

    public void Init(int playerCount){
        InitializeSpriteDictionaries();
        
        // 1. Build Decks
        ResetIngredientDeck(); // <--- NEW FUNCTION
        custDeck = BuildCustomersFromSO();
        menuDeck = BuildMenusFromSO(playerCount);

        // 2. Setup Market
        ActiveMarket.Clear();
        RefillMarket(); 
    }

    // --- NEW: CALL THIS AT START OF EVERY ROUND ---
    public void ResetIngredientDeck() {
        ingDeck = BuildIng(); // Rebuilds the perfect 39-card deck
        Debug.Log($"Ingredient Deck Reset: {ingDeck.Count} cards ready.");
    }

    public void RefillMarket() {
        while (ActiveMarket.Count < 6 && menuDeck.Count > 0) {
            ActiveMarket.Add(menuDeck.Pop());
        }
    }

    public MenuCardData TakeFromMarket(int marketIndex) {
        if (marketIndex < 0 || marketIndex >= ActiveMarket.Count) return null;
        MenuCardData taken = ActiveMarket[marketIndex];
        ActiveMarket.RemoveAt(marketIndex);
        RefillMarket();
        return taken;
    }

    // --- BUILDERS ---

    Stack<MenuCardData> BuildMenusFromSO(int playerCount) {
        var list = new List<MenuCardData>();
        foreach(var so in allMenuCards) {
            if (so != null) {
                for(int i = 0; i < so.quantityInDeck; i++) list.Add(so.ToRuntimeData());
            }
        }
        Shuffle(list);
        int limit = (playerCount == 3) ? 21 : (playerCount == 4) ? 28 : 35;
        if (list.Count > limit) list = list.GetRange(0, limit);
        return new Stack<MenuCardData>(list);
    }

    Stack<CustomerCardData> BuildCustomersFromSO() {
        var list = new List<CustomerCardData>();
        foreach(var so in allCustomerCards) if (so != null) list.Add(so.ToRuntimeData());
        Shuffle(list);
        return new Stack<CustomerCardData>(list);
    }

    Stack<Ingredient> BuildIng(){
        var list = new List<Ingredient>();
        
        // EXACT RULEBOOK NUMBERS
        // Total = 39 cards. Perfect balance.
        list.AddRange(Fill(Ingredient.Cabe, 2));
        list.AddRange(Fill(Ingredient.Krupuk, 2));
        list.AddRange(Fill(Ingredient.Bumbu, 5));
        list.AddRange(Fill(Ingredient.Tahu, 6));
        list.AddRange(Fill(Ingredient.Sayur, 7));
        list.AddRange(Fill(Ingredient.Nasi, 8));
        list.AddRange(Fill(Ingredient.Daging, 9));
        
        Shuffle(list);
        return new Stack<Ingredient>(list);
    }

    // --- HELPERS ---
    void InitializeSpriteDictionaries() {
        if (ingSpriteDict != null && ingSpriteDict.Count > 0) return;
        ingSpriteDict = new Dictionary<Ingredient, Sprite>();
        if (ingredientSprites != null) {
            foreach(var item in ingredientSprites) {
                if(!ingSpriteDict.ContainsKey(item.type)) ingSpriteDict.Add(item.type, item.icon);
            }
        }
    }
    
    public Sprite GetSpriteForIngredient(Ingredient ing) {
        if (ingSpriteDict == null) InitializeSpriteDictionaries();
        return ingSpriteDict.ContainsKey(ing) ? ingSpriteDict[ing] : null;
    }

    public Ingredient? DrawIng() => ingDeck.Count > 0 ? ingDeck.Pop() : null;
    public CustomerCardData DrawCust() => custDeck.Count > 0 ? custDeck.Pop() : null;

    static void Shuffle<T>(IList<T> a){
        var r = new Random();
        for(int i=a.Count-1; i>0; i--){ int j=r.Next(i+1); (a[i], a[j]) = (a[j], a[i]); }
    }
    static IEnumerable<Ingredient> Fill(Ingredient k, int n){ for(int i=0; i<n; i++) yield return k; }

    public int MenuDeckCount => menuDeck != null ? menuDeck.Count : 0;
    public int IngDeckCount => ingDeck != null ? ingDeck.Count : 0;
}