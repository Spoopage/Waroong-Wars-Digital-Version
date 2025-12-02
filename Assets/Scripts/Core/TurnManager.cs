using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Phase { Setup, Drafting, Cooking, Scoring }

public class TurnManager : MonoBehaviour
{
    public int PlayerCount = 4;
    public int Active = 0;
    public Phase Current = Phase.Setup;
    public int RoundNumber = 0;

    public PlayerState[] Players;
    public DeckManager deck;
    public UIController uiController;
    public CookingSystem cookingSystem;

    public List<Ingredient>[] DraftHands;
    private int consecutivePasses = 0;

    public void Init()
    {
        Players = new PlayerState[PlayerCount];
        DraftHands = new List<Ingredient>[PlayerCount];
        deck.Init(PlayerCount);

        for (int i = 0; i < PlayerCount; i++)
        {
            Players[i] = new PlayerState { Index = i };
            Players[i].Character = (CharacterType)(i + 1);
            if (i > 0) Players[i].IsAI = true;
        }
        StartNewRound();
    }

    void StartNewRound()
    {
        RoundNumber++;
        consecutivePasses = 0;

        if (deck.ActiveMarket.Count == 0 && deck.MenuDeckCount == 0)
        {
            EndGame();
            return;
        }

        foreach (var p in Players) p.DiscardAllIngredients();

        deck.ResetIngredientDeck();

        StartDrafting();
    }
    void StartDrafting()
    {
        Current = Phase.Drafting;
        Active = 0;

        for (int i = 0; i < PlayerCount; i++)
        {
            DraftHands[i] = new List<Ingredient>();
            for (int k = 0; k < 6; k++)
            {
                var ing = deck.DrawIng();
                if (ing.HasValue) DraftHands[i].Add(ing.Value);
            }
        }

        uiController.Refresh();
        HandleTurn();
    }

    public void DraftPick(int playerIdx, Ingredient card)
    {
        if (Current != Phase.Drafting || playerIdx != Active) return;

        if (DraftHands[playerIdx].Contains(card))
        {
            DraftHands[playerIdx].Remove(card);
            Players[playerIdx].Add(card, 1);
            NextPlayer();

            if (Active == 0) RotateHands();
            uiController.Refresh();
            HandleTurn();
        }
    }

    void RotateHands()
    {
        bool allEmpty = true;
        foreach (var h in DraftHands) if (h.Count > 0) allEmpty = false;

        if (allEmpty)
        {
            EndDrafting();
            return;
        }

        var first = DraftHands[0];
        for (int i = 0; i < PlayerCount - 1; i++) DraftHands[i] = DraftHands[i + 1];
        DraftHands[PlayerCount - 1] = first;
    }

    void EndDrafting()
    {
        Current = Phase.Cooking;
        Active = 0;
        consecutivePasses = 0;
        uiController.Refresh();
        HandleTurn();
    }

    public void Next() { if (Current == Phase.Cooking) EndPlayerTurn(); }

    void NextPlayer() => Active = (Active + 1) % PlayerCount;

    public void HandleTurn()
    {
        if (Current == Phase.Scoring) return;
        uiController.Refresh();
        if (Players[Active].IsAI) StartCoroutine(AIPlay(Players[Active]));
    }

    IEnumerator AIPlay(PlayerState ai)
    {
        yield return new WaitForSeconds(0.8f);

        if (Current == Phase.Drafting)
        {
            if (DraftHands[ai.Index].Count > 0)
            {
                DraftPick(ai.Index, DraftHands[ai.Index][0]);
            }
            else
            {
                Debug.LogWarning($"AI {ai.Character} has 0 cards to draft. Skipping turn.");
                NextPlayer();
                if (Active == 0) RotateHands();
                uiController.Refresh();
                HandleTurn();
            }
        }
        else if (Current == Phase.Cooking)
        {
            bool aiCooked = false;
            for (int i = 0; i < deck.ActiveMarket.Count; i++)
            {
                var menu = deck.ActiveMarket[i];
                // Pass index 'i' to Cook
                if (cookingSystem.Cook(ai, menu, i))
                {
                    aiCooked = true;
                    break;
                }
            }
            EndPlayerTurn();
        }
    }

    void EndPlayerTurn()
    {
        var activePlayer = Players[Active];

        if (activePlayer.CookedThisPhase)
        {
            consecutivePasses = 0;
            activePlayer.CookedThisPhase = false;
        }
        else
        {
            if (activePlayer.Get(Ingredient.Goboy) == 0) activePlayer.Add(Ingredient.Goboy, 1);
            consecutivePasses++;
        }

        NextPlayer();

        if (consecutivePasses >= PlayerCount)
        {
            StartNewRound();
        }
        else
        {
            HandleTurn();
        }
    }

    void EndGame()
    {
        Current = Phase.Scoring;
        StopAllCoroutines();
        uiController.Refresh();
    }
}