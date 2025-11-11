//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class EmojiManager : MonoBehaviour
{
    public GameObject emojiWheelPanel;
    public Button clickAwayBlockerButton;

    // Nanti kita akan tambahkan logika untuk animasi emoji di sini
    //public Image emojiAnimasiPrefab;
    //public Transform animationSpawnPoint;

    void Start()
    {
        emojiWheelPanel.SetActive(false);
        clickAwayBlockerButton.gameObject.SetActive(false);
    }

    // Fungsi ini akan dipanggil oleh TOMBOL EMOJI UTAMA (di header)
    public void ToggleEmojiWheel()
    {
        bool isActive = !emojiWheelPanel.activeSelf;

        emojiWheelPanel.SetActive(isActive);
        clickAwayBlockerButton.gameObject.SetActive(isActive);

        clickAwayBlockerButton.interactable = isActive;
    }

    // Fungsi ini akan dipanggil oleh CLICKAWAYBLOCKER
    public void CloseEmojiWheel()
    {
        emojiWheelPanel.SetActive(false);
        clickAwayBlockerButton.gameObject.SetActive(false);
    }

    // Fungsi ini akan dipanggil oleh 4 TOMBOL EMOJI di dalam panel
    public void OnEmojiClicked(Image emojiImage)
    {
        CloseEmojiWheel();

        // Tampilkan animasi emoji
        Debug.Log("Memainkan animasi untuk: " + emojiImage.sprite.name);

        // Panggil Coroutine untuk animasi
        StartCoroutine(AnimateEmoji(emojiImage.sprite));
    }

    // Ini adalah fungsi animasi sederhana (Ketentuan: keluar dari kanan layar)
    // Anda harus membuat 'emojiAnimasiPrefab' dan 'animationSpawnPoint' dulu
    System.Collections.IEnumerator AnimateEmoji(Sprite emojiSprite)
    {
        // Contoh: Membuat prefab gambar emoji di kanan layar
        // Image emojiInstance = Instantiate(emojiAnimasiPrefab, animationSpawnPoint.position, Quaternion.identity, transform);
        // emojiInstance.sprite = emojiSprite;

        // (Tambahkan logika animasi di sini... misal: gerak ke kiri, lalu fade out)

        // Placeholder
        yield return new WaitForSeconds(2.0f); // Tunggu 2 detik

        // Hancurkan emoji animasi
        // Destroy(emojiInstance.gameObject);
    }
}