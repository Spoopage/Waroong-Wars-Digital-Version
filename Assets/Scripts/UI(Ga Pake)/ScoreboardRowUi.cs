using UnityEngine;
using TMPro;

public class ScoreboardRowUi : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text rankText;
    public TMP_Text characterText;
    public TMP_Text vpText;
    
    // Fungsi untuk mengisi data baris
    public void SetData(int rank, string characterName, int vp)
    {
        rankText.text = $"{rank}.";
        characterText.text = characterName;
        vpText.text = vp.ToString() + " VP";
        
        // Opsional: Highlight pemenang (Rank 1)
        if (rank == 1)
        {
            // Misalnya, ganti warna teks atau background untuk menyorot pemenang
            rankText.color = Color.yellow;
            characterText.color = Color.yellow;
            vpText.color = Color.yellow;
        }
        else
        {
            // Reset warna untuk pemain lain
            rankText.color = Color.white;
            characterText.color = Color.white;
            vpText.color = Color.white;
        }
    }
}