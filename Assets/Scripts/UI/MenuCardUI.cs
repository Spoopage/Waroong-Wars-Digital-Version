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
        cookBtn.onClick.AddListener(()=>{ if(cooking.Cook(owner,data)) ui.Refresh(); });
        cookBtn.interactable = cooking.CanCook(owner, data);
    }
}
