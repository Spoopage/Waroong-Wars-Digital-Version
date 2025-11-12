using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject leaderboardPanel;

    void Start()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    // Fungsi ini akan dipanggil oleh tombol 'Esc'
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (leaderboardPanel != null && leaderboardPanel.activeSelf)
            {
                ClosePanel();
            }
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

    // Fungsi untuk tombol header 
    public void TogglePanel()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(!leaderboardPanel.activeSelf);
        }
    }
}