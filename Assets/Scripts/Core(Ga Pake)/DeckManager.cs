// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using Random = System.Random;

// public class DeckManager : MonoBehaviour {
//     Random rng = new Random();

//     // Runtime Decks (untuk logic game)
//     Stack<Ingredient> ingDeck;
//     Stack<MenuCardData> menuDeck;
//     Stack<CustomerCardData> custDeck;

//     [Header("--- DATA KARTU (Drag SO Kesini) ---")]
//     // KITA GANTI HARDCODE DENGAN INI:
//     public List<MenuCardSO> allMenuCards; 
//     public List<CustomerCardSO> allCustomerCards;

//     [Header("Sprite Ingredients")]
//     public List<IngredientSpriteData> ingredientSprites;
//     private Dictionary<Ingredient, Sprite> ingSpriteDict;

//     [System.Serializable]
//     public struct IngredientSpriteData {
//         public Ingredient type;
//         public Sprite icon;
//     }

//     public void Init(){
//         InitializeSpriteDictionaries(); // Setup gambar ingredient
        
//         ingDeck = BuildIng(); // Ingredient tetap logic kode (karena cuma angka)
//         menuDeck = BuildMenusFromSO(); // <--- BARU
//         custDeck = BuildCustomersFromSO(); // <--- BARU
//     }

//     // ... (Fungsi InitializeSpriteDictionaries, GetSpriteForIngredient, Shuffle tetap sama) ...
//     // ... (Copy dari kode DeckManager sebelumnya untuk bagian-bagian itu) ...

//     void InitializeSpriteDictionaries() {
//         ingSpriteDict = new Dictionary<Ingredient, Sprite>();
//         if (ingredientSprites != null) {
//             foreach(var item in ingredientSprites) {
//                 if(!ingSpriteDict.ContainsKey(item.type)) ingSpriteDict.Add(item.type, item.icon);
//             }
//         }
//     }
//     public Sprite GetSpriteForIngredient(Ingredient ing) => ingSpriteDict.ContainsKey(ing) ? ingSpriteDict[ing] : null;

//     // --- BUILDER BARU MENGGUNAKAN SO ---

//     Stack<MenuCardData> BuildMenusFromSO() {
//         var list = new List<MenuCardData>();
//         foreach(var so in allMenuCards) {
//             if (so != null) list.Add(so.ToRuntimeData());
//         }
//         Shuffle(list);
//         return new Stack<MenuCardData>(list);
//     }

//     Stack<CustomerCardData> BuildCustomersFromSO() {
//         var list = new List<CustomerCardData>();
//         foreach(var so in allCustomerCards) {
//             if (so != null) list.Add(so.ToRuntimeData());
//         }
//         Shuffle(list);
//         return new Stack<CustomerCardData>(list);
//     }

//     // Builder Ingredients tetap sama (simple math)
//     Stack<Ingredient> BuildIng(){
//         var list = new List<Ingredient>();
//         // Goboy tidak ada di deck
//         list.AddRange(Fill(Ingredient.Cabe, 2));
//         list.AddRange(Fill(Ingredient.Krupuk, 2));
//         list.AddRange(Fill(Ingredient.Tahu, 6));
//         list.AddRange(Fill(Ingredient.Nasi, 8));
//         list.AddRange(Fill(Ingredient.Bumbu, 5));
//         list.AddRange(Fill(Ingredient.Sayur, 7));
//         list.AddRange(Fill(Ingredient.Daging, 9));
//         Shuffle(list);
//         return new Stack<Ingredient>(list);
//     }

//     static void Shuffle<T>(IList<T> a){
//         var r = new Random();
//         for(int i=a.Count-1; i>0; i--){ int j=r.Next(i+1); (a[i], a[j]) = (a[j], a[i]); }
//     }
//     static IEnumerable<Ingredient> Fill(Ingredient k, int n){ for(int i=0; i<n; i++) yield return k; }

//     // Helper draw
//     public Ingredient? DrawIng() => ingDeck.Count > 0 ? ingDeck.Pop() : null;
//     public MenuCardData DrawMenu() => menuDeck.Count > 0 ? menuDeck.Pop() : null;
//     public CustomerCardData DrawCust() => custDeck.Count > 0 ? custDeck.Pop() : null;

//     public int MenuCount => menuDeck.Count;
//     public int IngCount => ingDeck.Count;
// }