using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCardUI : MonoBehaviour {
    public TMP_Text title;
    public Button cookBtn;
    MenuCardData data; PlayerState owner; CookingSystem cooking; UIController ui;

    public void Bind(MenuCardData m, PlayerState p, CookingSystem c, UIController U){
        data=m; owner=p; cooking=c; ui=U;
        title.text = $"{m.Name} (+{m.BaseVP})";
        cookBtn.onClick.RemoveAllListeners();

        // UBAH LISTENER INI:
        cookBtn.onClick.AddListener(()=>{ 
            // Kita asumsikan cooking.Cook() akan menghapus 'data' dari 'owner.MenuHand'
            if(cooking.Cook(owner,data)) {
                
                // TAMBAHKAN INI:
                // Jika berhasil memasak, ambil kartu baru untuk menggantikan
                var newCard = ui.deck.DrawMenu();
                if (newCard != null){
                    // 'owner.MenuHand' adalah daftar bersama, jadi kita tambahkan ke sana
                    owner.MenuHand.Add(newCard); 
                }
                
                ui.Refresh(); // Refresh UI untuk menampilkan kartu baru
            }
        });
        cookBtn.interactable = cooking.CanCook(owner, data);
    }
}