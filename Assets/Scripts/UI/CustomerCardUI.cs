using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerCardUI : MonoBehaviour {
    public TMP_Text title;
    public Button useBtn;
    CustomerCardData data; PlayerState owner; UIController ui;

    public void Bind(CustomerCardData c, PlayerState p, UIController U){
        data=c; owner=p; ui=U;
        title.text = data.Name;
        useBtn.onClick.RemoveAllListeners();
        useBtn.onClick.AddListener(()=>{ ui.UseCustomer(owner,data); });
    }
}
