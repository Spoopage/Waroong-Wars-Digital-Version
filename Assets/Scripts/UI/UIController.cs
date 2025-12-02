//using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Pastikan ini ada jika belum ada!

public class UIController : MonoBehaviour {
    [Header("Refs")]
    public TurnManager turn;
    public DeckManager deck;
    public CookingSystem cooking;

    [Header("Turn UI")] // Tambahkan header baru ini
    public TMP_Text TurnInfoText;

    [Header("Leaderboard Refs")]
    public LeaderboardManager leaderboard;

    [Header("Panels")]
    public RectTransform DeckPanel;
    public RectTransform MenuPanel;
    public RectTransform CookedPanel;
    public RectTransform IngredientPanel;
    public RectTransform CustomerPanel;
    public RectTransform DraftPanel; 
    public Button EndTurnButton;

    [Header("Prefabs")]
    public MenuCardUI MenuCardPrefab;
    public CustomerCardUI CustomerCardPrefab;
    public IngredientCounterUI IngredientCounterPrefab;
    public Button DraftCardPrefab; 

    void Start(){
        turn.uiController = this; 
        turn.Init();
        EndTurnButton.onClick.AddListener(()=>{ turn.Next(); Refresh(); });
        Refresh();
        //turn.HandleTurn(); 
    }

    void Update()
    {
        // Cek jika tombol TAB ditekan (untuk menampilkan/menyembunyikan live leaderboard)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Panggil fungsi toggle yang sekaligus merefresh data skor
            leaderboard.ToggleAndRefresh(turn.Players);
        }

        // Cek jika tombol ESCAPE ditekan (untuk menyembunyikan leaderboard live)
        // Jika sedang di fase Scoring, leaderboard tidak akan ditutup oleh ESC
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (leaderboard.leaderboardPanel.activeSelf && turn.Current != Phase.Scoring)
            {
                leaderboard.ClosePanel();
            }
        }
    }

    public void Refresh(){
        var pl = turn.Players[turn.Active];
        
        string phaseIndonesian;
        switch(turn.Current){
            case Phase.Setup: phaseIndonesian = "Setup"; break;
            case Phase.Drafting: phaseIndonesian = "Drafting"; break;
            case Phase.Cooking: phaseIndonesian = "Memasak"; break;
            case Phase.Scoring: phaseIndonesian = "Penghitungan Skor"; break;
            default: phaseIndonesian = turn.Current.ToString(); break;
        }

        string activeCharacter = pl.Character.ToString();
        // Menentukan apakah pemain aktif adalah pemain manusia atau AI
        string playerType = pl.IsAI ? "(AI)" : "(Pemain)"; 

        // Menggabungkan semua informasi
        string turnText = $"Ronde {turn.RoundNumber} | Fase: {phaseIndonesian}\nGiliran: {activeCharacter} {playerType}";
        
        // Menampilkan teks di UI
        if (TurnInfoText != null) {
            TurnInfoText.text = turnText;
        }
        
        if (turn.Current == Phase.Scoring) { //
            Debug.Log("Game Selesai. Pemenang: Hitung VP tertinggi.");
            // Panggil LeaderboardManager
            if (leaderboard != null) 
            {
                leaderboard.OpenPanel();
                // PERUBAHAN: Set isFinalScoring menjadi TRUE
                leaderboard.DisplayScores(turn.Players, true); // Kirim data pemain ke leaderboard
            }
        }

        Clear(MenuPanel); Clear(CustomerPanel); Clear(CookedPanel); Clear(IngredientPanel); Clear(DraftPanel);

        if(turn.Current == Phase.Drafting){
            DraftPanel.gameObject.SetActive(true);
            EndTurnButton.gameObject.SetActive(false);
            var hand = turn.DraftHands[turn.Active];

            if (pl.IsAI == false){
                if (hand.Count == 0)
                {
                    Debug.LogError("BUG: Player diminta draft, tapi tangan kosong! Memaksa EndDrafting...");
                    // Opsional: Panggil fungsi di TurnManager untuk force stop, atau tampilkan pesan error
                    // turn.ForceEndDrafting(); 
                    return;
                }
                foreach (var card in hand){
                    var btn = Instantiate(DraftCardPrefab, DraftPanel);
                    var txt = btn.GetComponentInChildren<TMPro.TMP_Text>();
                    if (txt != null) txt.text = card.ToString();
                    //btn.GetComponentInChildren<TMPro.TMP_Text>().text = card.ToString();

                    // COPY CAPTURE VARIABLE:
                    // Untuk keamanan di loop lambda (meski C# baru aman, ini best practice Unity lama)
                    var cardRef = card;
                    btn.onClick.AddListener(()=> {
                        turn.DraftPick(turn.Active, card);
                    });
                }
            }
        } else {
            DraftPanel.gameObject.SetActive(false);
            EndTurnButton.gameObject.SetActive(turn.Current == Phase.Cooking && pl.IsAI == false);
        }
        
        // Tampilkan semua kartu/inventori untuk player aktif
        
        foreach (var m in pl.MenuHand)
            Instantiate(MenuCardPrefab, MenuPanel).Bind(m, pl, cooking, this);

        foreach (var c in pl.CustHand)
            Instantiate(CustomerCardPrefab, CustomerPanel).Bind(c, pl, this);

        foreach (var m in pl.Cooked){
            var item = Instantiate(IngredientCounterPrefab, CookedPanel);
            item.Set((Ingredient)999, 0); 
            item.label.text = m.Name;
        }

        foreach (var kv in pl.Inv.Where(kv=>kv.Value>0))
            Instantiate(IngredientCounterPrefab, IngredientPanel).Set(kv.Key, kv.Value);
            
        // TODO: Tambahkan logic UI untuk menampilkan Phase.Scoring
        if (turn.Current == Phase.Scoring) {
            Debug.Log("Game Selesai. Pemenang: Hitung VP tertinggi.");
            // Di sini Anda bisa memanggil LeaderboardManager.OpenPanel()
        }
    }

    public void UseCustomer(PlayerState self, CustomerCardData c){
        if(c.Effect.StartsWith("Add:")){
            var ingName = c.Effect.Split(':')[1];
            if(System.Enum.TryParse<Ingredient>(ingName, out var ing)) self.Add(ing, 1);
        }
        else if(c.Effect == "Draw2Keep1"){
            var i1 = deck.DrawIng();
            var i2 = deck.DrawIng();
            if(i1.HasValue) self.Add(i1.Value, 1);
        }
        else if(c.Effect == "StealIng"){
            int targetIdx = (self.Index + 1) % turn.PlayerCount; 
            var target = turn.Players[targetIdx];
            bool blocked = target.CustHand.Any(x => x.Effect == "BlockSteal");
            
            if(!blocked && target.Inv.Any(kv=>kv.Value>0)){
                var key = target.Inv.First(kv=>kv.Value>0).Key; 
                target.Spend(key, 1);
                self.Add(key, 1);
            }
        }
        else if(c.Effect == "StealSkill"){
             int targetIdx = (self.Index + 1) % turn.PlayerCount;
             var target = turn.Players[targetIdx];
             bool blocked = target.CustHand.Any(x => x.Effect == "BlockSteal");

             if(!blocked && target.CustHand.Count > 0){
                 var stolen = target.CustHand.First(x=>!x.Effect.StartsWith("VP:")); 
                 target.CustHand.Remove(stolen);
                 self.CustHand.Add(stolen);
             }
        }
        else if (c.Effect == "Swap"){
            // Logic ini perlu penambahan UI interaktif.
        }
        
        if(!c.Effect.StartsWith("VP:") && c.Effect != "BlockSteal" && c.Effect != "Swap"){
            self.CustHand.Remove(c);
        }
        
        Refresh();
    }

    void Clear(Transform t){ for(int i=t.childCount-1;i>=0;i--) Destroy(t.GetChild(i).gameObject); }
}