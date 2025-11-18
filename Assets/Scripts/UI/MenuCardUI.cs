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

        cookBtn.onClick.AddListener(()=>{ 
            if(cooking.Cook(owner,data)) {
                
                // Jika berhasil memasak, ambil kartu baru untuk menggantikan
                var newCard = ui.deck.DrawMenu();
                if (newCard != null){
                    owner.MenuHand.Add(newCard); 
                }
                
                ui.Refresh(); 
            }
        });
        cookBtn.interactable = cooking.CanCook(owner, data);
    }
}