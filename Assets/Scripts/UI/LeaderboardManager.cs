using UnityEngine;
using System.Linq; 
using TMPro; 
// Pastikan tidak ada fungsi Update() yang menangani KeyCode.Escape di sini lagi!

public class LeaderboardManager : MonoBehaviour
{
    public GameObject leaderboardPanel;

    [Header("Scoreboard Refs")]
    public Transform ScoreEntryParent; 
    public ScoreboardRowUi ScoreEntryPrefab;

    [Header("Winner Display")]
    public TMP_Text WinnerText; 

    void Start()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    public void OpenPanel()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    // FUNGSI BARU untuk Toggle Live Scoreboard (dipanggil oleh UIController)
    public void ToggleAndRefresh(PlayerState[] players)
    {
        bool willBeActive = !leaderboardPanel.activeSelf;
        
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(willBeActive);
        }

        if (willBeActive)
        {
            // Refresh skor, isFinalScoring = FALSE untuk live view
            DisplayScores(players, false); 
        }
    }

    // FUNGSI DisplayScores DIMODIFIKASI untuk menerima flag isFinalScoring
    public void DisplayScores(PlayerState[] players, bool isFinalScoring)
    {
        // 1. Clear entries lama
        if (ScoreEntryParent != null)
        {
            foreach (Transform child in ScoreEntryParent)
            {
                Destroy(child.gameObject);
            }
        }
        
        if (ScoreEntryPrefab == null)
        {
            Debug.LogError("ScoreEntryPrefab belum dihubungkan di Inspector!");
            return;
        }

        // 2. Sort Players berdasarkan VP (menurun/descending)
        var sortedPlayers = players.OrderByDescending(p => p.VP).ToArray();

        // 3. Tampilkan Pemenang secara eksplisit HANYA JIKA isFinalScoring TRUE
        if (WinnerText != null)
        {
            if (isFinalScoring && sortedPlayers.Length > 0)
            {
                 var winner = sortedPlayers[0];
                 // (Logika pengecekan seri dari modifikasi sebelumnya)
                 var tiedWinners = sortedPlayers.Where(p => p.VP == winner.VP).Select(p => p.Character.ToString()).ToList();

                 if (tiedWinners.Count > 1)
                 {
                     string winnersList = string.Join(" & ", tiedWinners);
                     WinnerText.text = $"PERMAINAN BERAKHIR SERI! Pemenang: {winnersList} dengan {winner.VP} VP!";
                 }
                 else
                 {
                     WinnerText.text = $"SELAMAT! PEMENANGNYA ADALAH: {winner.Character} dengan {winner.VP} VP!";
                 }
                 WinnerText.gameObject.SetActive(true);
            } else {
                 // Sembunyikan pesan pemenang saat live score (TAB)
                 WinnerText.gameObject.SetActive(false);
            }
        }

        // 4. Instantiate dan Bind data (Leaderboard)
        int rank = 1;
        int previousVP = int.MaxValue;
        
        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            var p = sortedPlayers[i];
            
            if (p.VP < previousVP)
            {
                rank = i + 1;
            }
            previousVP = p.VP;

            var row = Instantiate(ScoreEntryPrefab, ScoreEntryParent);
            row.SetData(rank, p.Character.ToString(), p.VP);
        }
    }
}