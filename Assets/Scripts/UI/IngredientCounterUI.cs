using UnityEngine;
using UnityEngine.UI; // <--- Wajib ada untuk mengakses komponen Image
using TMPro;

public class IngredientCounterUI : MonoBehaviour {
    [Header("UI Components")]
    public TMP_Text label;
    public Image iconDisplay; // <--- Slot baru untuk menaruh Image Icon

    // Fungsi ini dipanggil saat UI di-refresh
    public void Set(Ingredient k, int v){ 
        // 1. Update Teks Jumlah
        label.text = $"{k} ×{v}"; 

        // 2. Ambil Gambar dari DeckManager
        // Kita cari script DeckManager yang aktif di scene
        DeckManager deckManager = Object.FindFirstObjectByType<DeckManager>();
        
        if (deckManager != null) {
            // Panggil fungsi helper yang sudah kita buat di DeckManager
            Sprite spriteBahan = deckManager.GetSpriteForIngredient(k);
            
            if (spriteBahan != null) {
                iconDisplay.sprite = spriteBahan;
                
                // Opsional: Pastikan gambar aspek rasionya benar
                // iconDisplay.preserveAspect = true; 
            } else {
                // Debugging jika gambar lupa dimasukkan
                // Debug.LogWarning($"Sprite untuk bahan '{k}' belum di-set di DeckManager!");
            }
        }
    }
}