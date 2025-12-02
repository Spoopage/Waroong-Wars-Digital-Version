using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text titleText;
    public TMP_Text vpText;
    public Image iconImage;
    public Transform ingredientsContainer;
    public GameObject ingredientIconPrefab;
    public Button actionButton;

    private MenuCardData data;
    private int marketIndex;
    private DeckManager deck;
    private TurnManager turn;
    private CookingSystem cooking;

    public void BindToMarket(MenuCardData m, int index, DeckManager d, TurnManager t, CookingSystem c)
    {
        data = m;
        marketIndex = index;
        deck = d;
        turn = t;
        cooking = c;

        // Visuals
        if (titleText != null) titleText.text = m.Name;
        if (vpText != null) vpText.text = m.BaseVP.ToString();
        if (iconImage != null && m.Icon != null) iconImage.sprite = m.Icon;

        // Ingredients
        if (ingredientsContainer != null && ingredientIconPrefab != null)
        {
            foreach (Transform child in ingredientsContainer) Destroy(child.gameObject);
            foreach (var kv in m.Req)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    var iconObj = Instantiate(ingredientIconPrefab, ingredientsContainer);
                    var img = iconObj.GetComponent<Image>();
                    if (img != null) img.sprite = deck.GetSpriteForIngredient(kv.Key);
                }
            }
        }

        // Button Logic
        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                StartCoroutine(AnimateButtonPress());

                if (cooking.Cook(turn.Players[turn.Active], data, marketIndex))
                {

                    string msg = $"+{data.BaseVP} VP";
                    turn.uiController.SpawnFloatingText(msg, transform.position);

                    turn.uiController.Refresh();
                    turn.Next();
                }
            });
        }

        StartCoroutine(AnimatePopIn());
        UpdateState();
    }

    void UpdateState()
    {
        if (actionButton == null) return;
        if (turn.Current != Phase.Cooking) { actionButton.interactable = false; return; }

        var p = turn.Players[turn.Active];
        if (p.IsAI) { actionButton.interactable = false; return; }

        actionButton.interactable = cooking.CanCook(p, data);
    }

    IEnumerator AnimatePopIn()
    {
        transform.localScale = Vector3.zero;
        float duration = 0.3f; float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Sin((timer / duration) * Mathf.PI * 0.5f) * 1.1f;
            if (timer / duration >= 1f) scale = 1f;
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    IEnumerator AnimateButtonPress()
    {
        actionButton.transform.localScale = Vector3.one * 0.8f;
        yield return new WaitForSeconds(0.1f);
        actionButton.transform.localScale = Vector3.one;
    }
}