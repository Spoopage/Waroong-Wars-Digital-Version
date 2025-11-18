using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections; 

public enum Phase { Setup, Drafting, Cooking, Scoring }

public class TurnManager : MonoBehaviour {
    public int PlayerCount = 4;
    public int Active = 0;
    public Phase Current = Phase.Setup;
    public PlayerState[] Players;
    public DeckManager deck;
    public UIController uiController; 
    
    public List<Ingredient>[] DraftHands;
    List<MenuCardData> sharedMenuHand = new List<MenuCardData>();

    public void Init(){
        Players = new PlayerState[PlayerCount];
        sharedMenuHand.Clear();
        deck.Init();
        DraftHands = new List<Ingredient>[PlayerCount];

        for(int i=0; i<PlayerCount; i++){
            Players[i] = new PlayerState{ Index=i };
            Players[i].MenuHand = sharedMenuHand;
            Players[i].Character = (CharacterType)(i + 1); 
            if (i > 0) Players[i].IsAI = true; 
        }
        
        StartNewRound();
        HandleTurn(); 
    }

    void StartDrafting(){
        Current = Phase.Drafting;
        Active = 0;
        for(int i=0; i<PlayerCount; i++){
            DraftHands[i] = new List<Ingredient>();
            for(int k=0; k<6; k++){
                var ing = deck.DrawIng();
                if(ing.HasValue) DraftHands[i].Add(ing.Value);
            }
        }
    }

    public void DraftPick(int playerIdx, Ingredient card){
        if(Current != Phase.Drafting || playerIdx != Active) return;
        if(!DraftHands[playerIdx].Contains(card)) return;

        DraftHands[playerIdx].Remove(card);
        Players[playerIdx].Add(card, 1);

        NextPlayer(); 
        if(Active == 0) { 
            RotateHands();
        }
        uiController.Refresh();
        HandleTurn(); 
    }

    void RotateHands(){
        var last = DraftHands[PlayerCount-1];
        for(int i=PlayerCount-1; i>0; i--) DraftHands[i] = DraftHands[i-1];
        DraftHands[0] = last;

        if(DraftHands[0].Count == 0) {
            EndDrafting();
        }
    }

    void EndDrafting(){
        foreach(var p in Players){
            if(p.Character == CharacterType.CakLondho){
                var extra = deck.DrawIng();
                if(extra.HasValue) p.Add(extra.Value, 1);
            }
        }
        Current = Phase.Cooking;
        Active = 0;
        uiController.Refresh();
        HandleTurn();
    }

    // FUNGSI BARU: Cek apakah ada pemain yang masih bisa memasak menu yang tersedia
    private bool CheckIfAnyPlayerCanCook(){
        var cookingSystem = GetComponent<CookingSystem>();
        // Cek semua pemain terhadap semua menu di shared hand
        foreach(var p in Players){
            foreach(var m in sharedMenuHand){
                if(cookingSystem.CanCook(p, m)){
                    return true;
                }
            }
        }
        return false; 
    }

    public void Next(){
        if(Current == Phase.Cooking){
            EndPlayerTurn();
        }
    }
    
    void NextPlayer(){
        Active = (Active + 1) % PlayerCount;
    }

    public void HandleTurn(){
        if (Current == Phase.Scoring) return;

        var activePlayer = Players[Active];
        uiController.Refresh(); 

        if (activePlayer.IsAI) {
            StartCoroutine(AIPlay(activePlayer));
        }
    }

    IEnumerator AIPlay(PlayerState ai){
        yield return new WaitForSeconds(1.0f); 

        if (Current == Phase.Drafting){
            if (DraftHands[ai.Index].Count > 0){
                var pick = DraftHands[ai.Index].OrderBy(ing => ai.Get(ing)).First();
                DraftPick(ai.Index, pick); 
            }
        }
        else if (Current == Phase.Cooking){
            var cookingSystem = GetComponent<CookingSystem>(); 
            bool canCookMore = true;
            
            while (canCookMore){
                canCookMore = false;
                MenuCardData bestMenu = null;
                int maxVP = -1;

                foreach(var menu in ai.MenuHand){ 
                    if(cookingSystem.CanCook(ai, menu)){
                        int currentVP = menu.BaseVP; 
                        if (menu.OptionalKey.HasValue) currentVP += menu.OptionalVP; 

                        if (currentVP > maxVP){
                            maxVP = currentVP;
                            bestMenu = menu;
                        }
                    }
                }

                if (bestMenu != null){
                    cookingSystem.Cook(ai, bestMenu);
                    ai.CookedThisPhase = true;
                    canCookMore = true; 
                    uiController.Refresh();
                    yield return new WaitForSeconds(0.8f); 
                }
            }
            
            EndPlayerTurn();
        }
    }

    void EndPlayerTurn(){
        var activePlayer = Players[Active];
        
        if (!activePlayer.CookedThisPhase) {
            activePlayer.Add(Ingredient.Goboy, 1);
        }
        activePlayer.CookedThisPhase = false; 
        
        if(activePlayer.Character == CharacterType.PakBas){
            int count = activePlayer.Cooked.Count(c => c.Req.Values.Sum() >= 2 && c.Req.Values.Sum() <= 3);
            if(count == 1) activePlayer.Add(Ingredient.Goboy, 1);
        }
        
        NextPlayer();
        
        // Cek Transisi Fase/Ronde: Jika semua menu habis ATAU tidak ada yang bisa masak, mulai ronde baru
        if (sharedMenuHand.Count == 0 || CheckIfAnyPlayerCanCook() == false) { 
            StartNewRound();
        } else {
            HandleTurn();
        }
    }

    void StartNewRound(){
        // 1. Cek Akhir Game
        if (deck.IngCount < PlayerCount * 6 && sharedMenuHand.Count == 0 && deck.MenuCount == 0) { 
            EndGame();
            return;
        }
        
        // 2. LOGIKA RESET INGREDIENTS
        foreach(var p in Players){
            p.DiscardAllIngredients();
        }
        
        // 3. Isi 5 kartu Menu baru ke shared hand
        sharedMenuHand.Clear(); // Pastikan clear lagi sebelum ditarik
        for(int i=0; i<5; i++){
            var m = deck.DrawMenu();
            if(m!=null) sharedMenuHand.Add(m);
        }
        
        // 4. Jika menu yang bisa ditarik tidak ada, Game Over
        if (sharedMenuHand.Count == 0 && deck.MenuCount == 0) {
            EndGame();
            return;
        }

        // 5. Mulai Drafting
        StartDrafting();
    }

    void EndGame(){
        Current = Phase.Scoring;
        // FIX FREEZE BUG: Hentikan semua Coroutine agar game tidak macet
        StopAllCoroutines(); 
        Scoring();
        uiController.Refresh(); 
    }

    void Scoring(){
        Current = Phase.Scoring;
        
        foreach(var p in Players){
            foreach(var c in p.CustHand){
                if(c.Effect.StartsWith("VP:")) {
                    p.VP += c.VP; 
                }
                else {
                    p.VP += 1; 
                }
            }
            
            if(p.Character == CharacterType.GengBajoel){
                var groups = p.Cooked.GroupBy(m => m.Name);
                foreach(var g in groups){
                    if(g.Count() > 1){ 
                        p.VP += (g.Count() * 4); 
                    }
                }
            }
        }
    }
}