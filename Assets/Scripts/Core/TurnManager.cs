using UnityEngine;

public enum Phase { Drafting, Cooking, Scoring }

public class TurnManager : MonoBehaviour {
    public int PlayerCount = 4;
    public int Active = 0;
    public Phase Current = Phase.Cooking;
    public PlayerState[] Players;

    public void Init(){
        Players = new PlayerState[PlayerCount];
        for(int i=0;i<PlayerCount;i++) Players[i]=new PlayerState{ Index=i };
        Active = 0;
        Current = Phase.Cooking;
    }
    public void Next(){ Active = (Active+1)%PlayerCount; }
}
