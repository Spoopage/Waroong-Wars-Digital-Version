using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerCardUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text title;
    public Image iconDisplay;
    public Button useBtn;
    private CustomerCardData data;
    private PlayerState owner;
    private UIController ui;

    public void Bind(CustomerCardData c, PlayerState p, UIController U)
    {
        data = c;
        owner = p;
        ui = U;

        if (title != null) title.text = data.Name;

        if (iconDisplay != null && data.Icon != null)
        {
            iconDisplay.sprite = data.Icon;
            iconDisplay.enabled = true;
        }

        if (useBtn != null)
        {
            bool isUsable = data.Effect.StartsWith("Add:") || data.Effect.StartsWith("Action:");

            useBtn.gameObject.SetActive(isUsable);
            useBtn.onClick.RemoveAllListeners();
            useBtn.onClick.AddListener(() => { ui.ActivateCustomer(data); });
        }
    }
}