using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Manager Refs")]
    public TurnManager turn;
    public DeckManager deck;
    public CookingSystem cooking;

    [Header("UI Containers")]
    public Transform MarketPanel;
    public Transform PlayerHandPanel;
    public Transform PlayerCustPanel;
    public Transform DraftPanel;

    public TMP_Text TurnInfoText;
    public Button EndTurnButton;

    [Header("Prefabs")]
    public MenuCardUI MenuCardPrefab;
    public IngredientCounterUI IngCardPrefab;
    public Button DraftCardPrefab;
    public CustomerCardUI CustomerCardPrefab;

    [Header("Visuals")]
    public FloatingText floatingTextPrefab;
    public GameObject GameOverPanel;
    public TMP_Text WinnerText;

    private Coroutine draftAnimCoroutine;

    void Start()
    {
        turn.uiController = this;
        turn.Init();
        EndTurnButton.onClick.AddListener(() => { turn.Next(); });
        Refresh();
    }

    public void SpawnFloatingText(string msg, Vector3 pos)
    {
        if (floatingTextPrefab != null)
        {
            // Instantiate inside the Canvas (transform) so it renders on top
            var txt = Instantiate(floatingTextPrefab, transform);
            txt.transform.position = pos;
            txt.Init(msg, new Color(1f, 0.8f, 0f)); // Gold Color
        }
    }

    public void Refresh()
    {
        var pl = turn.Players[turn.Active];

        string type = pl.IsAI ? "(AI)" : "(Human)";
        TurnInfoText.text = $"ROUND {turn.RoundNumber} | {turn.Current}\nTURN: {pl.Character} {type}";

        Clear(MarketPanel);
        for (int i = 0; i < deck.ActiveMarket.Count; i++)
        {
            var m = deck.ActiveMarket[i];
            var ui = Instantiate(MenuCardPrefab, MarketPanel);
            ui.BindToMarket(m, i, deck, turn, cooking);

            ui.transform.localScale = Vector3.zero;
            StartCoroutine(AnimateScale(ui.transform));
        }

        Clear(PlayerHandPanel);
        if (pl.Get(Ingredient.Goboy) > 0)
        {
            var ui = Instantiate(IngCardPrefab, PlayerHandPanel);
            ui.Set(Ingredient.Goboy, pl.Get(Ingredient.Goboy), deck.GetSpriteForIngredient(Ingredient.Goboy));
        }
        foreach (var kv in pl.Inv)
        {
            if (kv.Key == Ingredient.Goboy || kv.Value == 0) continue;
            var ui = Instantiate(IngCardPrefab, PlayerHandPanel);
            ui.Set(kv.Key, kv.Value, deck.GetSpriteForIngredient(kv.Key));
        }

        Clear(PlayerCustPanel);
        foreach (var c in pl.CustHand)
        {
            if (CustomerCardPrefab != null)
            {
                var ui = Instantiate(CustomerCardPrefab, PlayerCustPanel);
                ui.Bind(c, pl, this);
            }
        }

        bool showDraft = (turn.Current == Phase.Drafting && !pl.IsAI);
        DraftPanel.gameObject.SetActive(showDraft);

        if (draftAnimCoroutine != null) StopCoroutine(draftAnimCoroutine);
        Clear(DraftPanel);

        if (showDraft)
        {
            var hand = turn.DraftHands[turn.Active];
            draftAnimCoroutine = StartCoroutine(SpawnDraftCardsStaggered(hand));
        }

        EndTurnButton.gameObject.SetActive(turn.Current == Phase.Cooking && !pl.IsAI);

        if (turn.Current == Phase.Scoring && GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
            var winner = turn.Players.OrderByDescending(p => p.VP).First();
            if (WinnerText) WinnerText.text = $"WINNER:\n{winner.Character}\n({winner.VP} Points)";
        }
    }

    public void ActivateCustomer(CustomerCardData c)
    {
        var p = turn.Players[turn.Active];

        if (c.Effect.StartsWith("Add:"))
        {
            string ingName = c.Effect.Split(':')[1];
            if (System.Enum.TryParse(ingName, out Ingredient res))
            {
                p.Add(res, 1);
                p.CustHand.Remove(c);
                SpawnFloatingText($"+1 {res}", PlayerCustPanel.position); // Visual Pop
                Refresh();
            }
        }
        else if (c.Effect.StartsWith("Action:"))
        {
            PerformAction(p, c.Effect.Split(':')[1]);
            p.CustHand.Remove(c);
            Refresh();
        }
    }

    void PerformAction(PlayerState self, string action)
    {
        int targetIdx = (self.Index + 1) % turn.PlayerCount;
        var target = turn.Players[targetIdx];
        bool hasSecurity = target.CustHand.Any(x => x.Effect == "Passive:BlockSteal");

        if (action == "StealIng")
        {
            if (hasSecurity) { SpawnFloatingText("Blocked!", transform.position); return; }
            var available = target.Inv.Where(x => x.Value > 0).Select(x => x.Key).ToList();
            if (available.Count > 0)
            {
                Ingredient stolen = available[Random.Range(0, available.Count)];
                target.Spend(stolen, 1); self.Add(stolen, 1);
                SpawnFloatingText($"Stole {stolen}!", transform.position);
            }
        }
        else if (action == "Draw2Keep1")
        {
            var ing = deck.DrawIng();
            if (ing.HasValue)
            {
                self.Add(ing.Value, 1);
                SpawnFloatingText($"+1 {ing.Value}", transform.position);
            }
        }
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    IEnumerator SpawnDraftCardsStaggered(List<Ingredient> hand)
    {
        var handCopy = new List<Ingredient>(hand);
        foreach (var ing in handCopy)
        {
            if (!DraftPanel.gameObject.activeInHierarchy) yield break;
            var btn = Instantiate(DraftCardPrefab, DraftPanel);

            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.text = ing.ToString();
            var img = btn.GetComponent<Image>();
            if (img != null) img.sprite = deck.GetSpriteForIngredient(ing);

            var capIng = ing;
            btn.onClick.AddListener(() => { turn.DraftPick(turn.Active, capIng); });
            btn.transform.localScale = Vector3.zero;
            StartCoroutine(AnimateScale(btn.transform));
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator AnimateScale(Transform target)
    {
        float duration = 0.2f; float t = 0;
        while (t < duration && target != null)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.one * Mathf.SmoothStep(0, 1, t / duration);
            yield return null;
        }
        if (target != null) target.localScale = Vector3.one;
    }
    void Clear(Transform t) { foreach (Transform child in t) Destroy(child.gameObject); }
}