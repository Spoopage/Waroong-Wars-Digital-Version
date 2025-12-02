using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerCardUI : MonoBehaviour {
    [Header("UI Components")]
    public TMP_Text title;
    public Button useBtn;
    public Image iconDisplay; // <--- Slot baru untuk menaruh Image Icon Customer

    private CustomerCardData data; 
    private PlayerState owner; 
    private UIController ui;

    public void Bind(CustomerCardData c, PlayerState p, UIController U){
        data = c; 
        owner = p; 
        ui = U;
        
        // 1. Set Text Nama
        title.text = data.Name;

        // 2. Set Gambar (Icon)
        // Kita ambil langsung dari 'data.Icon' yang sudah diisi oleh DeckManager
        if (iconDisplay != null) {
            if (data.Icon != null) {
                iconDisplay.sprite = data.Icon;
                iconDisplay.enabled = true; // Pastikan gambar terlihat
            } else {
                // Jika gambar kosong/lupa di-set, bisa dimatikan atau biarkan putih
                // iconDisplay.enabled = false; 
                Debug.LogWarning($"Gambar untuk customer '{data.Name}' kosong/null!");
            }
        }

        // 3. Setup Tombol
        useBtn.onClick.RemoveAllListeners();
        useBtn.onClick.AddListener(()=>{ ui.UseCustomer(owner, data); });
    }
}