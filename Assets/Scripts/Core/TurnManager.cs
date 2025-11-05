using UnityEngine;
// Tambahkan using ini untuk List<>
using System.Collections.Generic; 

public enum Phase { Drafting, Cooking, Scoring }

public class TurnManager : MonoBehaviour {
    public int PlayerCount = 4;
    public int Active = 0;
    public Phase Current = Phase.Cooking;
    public PlayerState[] Players;

    // Buat satu daftar yang akan dipakai bersama
    List<MenuCardData> sharedMenuHand = new List<MenuCardData>();

    public void Init(){
        Players = new PlayerState[PlayerCount];
        sharedMenuHand.Clear(); // Kosongkan daftar jika game di-restart
        for(int i=0;i<PlayerCount;i++){
            Players[i]=new PlayerState{ Index=i };
            // Atur MenuHand setiap pemain agar menunjuk ke daftar bersama
            Players[i].MenuHand = sharedMenuHand; 
        }
        Active = 0;
        Current = Phase.Cooking;
    }
    public void Next(){ Active = (Active+1)%PlayerCount; }
}