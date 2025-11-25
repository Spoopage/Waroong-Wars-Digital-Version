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

    public void Init(){
        ingDeck = BuildIng();
        menuDeck = BuildMenus();
        custDeck = BuildCustomers();
    }

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

    Stack<MenuCardData> BuildMenus(){
        var L = new List<MenuCardData>();
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
        var L = new List<CustomerCardData>{
            new CustomerCardData{ Name="Grocer", Effect="Add:Sayur" },
            new CustomerCardData{ Name="Rice Vendor", Effect="Add:Nasi" },
            new CustomerCardData{ Name="Butcher", Effect="Add:Daging" },
            new CustomerCardData{ Name="Tofu Seller", Effect="Add:Tahu" },
            new CustomerCardData{ Name="Spicemonger", Effect="Add:Bumbu" },
            new CustomerCardData{ Name="Thief", Effect="StealIng" },
            new CustomerCardData{ Name="Political Candidate", Effect="Draw2Keep1" },
            new CustomerCardData{ Name="Singer", Effect="VP:+3", VP=3 },
            new CustomerCardData{ Name="Travelling Merchant", Effect="Swap" },
            new CustomerCardData{ Name="Mobster", Effect="VP:-1", VP=-1 },
            new CustomerCardData{ Name="Foodie", Effect="VP:+2", VP=2 },
            new CustomerCardData{ Name="Security", Effect="BlockSteal" },
            new CustomerCardData{ Name="Primadonna", Effect="VP:+4", VP=4 },
            new CustomerCardData{ Name="Influencer", Effect="StealSkill" }
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