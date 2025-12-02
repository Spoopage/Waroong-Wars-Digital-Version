//using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Image = UnityEngine.UI.Image;

public class EmojiManager : MonoBehaviour
{
    public GameObject emojiWheelPanel;
    public Button clickAwayBlockerButton;

    [Header("Emoji Settings")]
    public GameObject emojiPrefab;       // Prefab yang ada script EmojiLifecycle
    public Transform emojiSpawnPoint;    // Titik muncul (di bawah wheel, kanan layar)
    public float emojiSpacing = 80f;     // Jarak antar emoji (pixel)
    public int maxEmojiCount = 4;        // Batas spam
    public float emojiDuration = 3.0f;   // Berapa lama tampil

    // List untuk menyimpan emoji yang sedang aktif di layar
    private List<EmojiLifecycle> activeEmojis = new List<EmojiLifecycle>();

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
    public void OnEmojiClicked(GameObject emojiButtonObj)
    {
        UnityEngine.UI.Image img = emojiButtonObj.GetComponent<Image>();
        if (img != null)
        {
            SpawnEmoji(img.sprite);
            CloseEmojiWheel();
            Debug.Log("Memainkan animasi untuk: " + emojiButtonObj.name);
        }
        else
        {
            Debug.LogError("Tombol ini tidak punya komponen Image!");
        }

        // Tampilkan animasi emoji
        Debug.Log("Memainkan animasi untuk: " + emojiButtonObj.name);

        // Panggil Coroutine untuk animasi
        //StartCoroutine(AnimateEmoji(emojiImage.sprite));
    }

    void SpawnEmoji(Sprite sprite)
    {
        // 1. Bersihkan List dari emoji yang sudah hancur (null) karena durasi habis
        activeEmojis.RemoveAll(x => x == null);

        // 2. Cek Limit Spam (Max 4)
        if (activeEmojis.Count >= maxEmojiCount)
        {
            // Hapus yang paling lama (index terakhir / paling bawah)
            EmojiLifecycle oldest = activeEmojis[activeEmojis.Count - 1];
            if (oldest != null) Destroy(oldest.gameObject);
            activeEmojis.RemoveAt(activeEmojis.Count - 1);
        }

        // 3. Instantiate Emoji Baru di titik Spawn (Posisi Teratas)
        // Kita instantiate di parent dari spawnPoint (biasanya Canvas utama) agar tidak ikut gerak spawnPoint
        GameObject newObj = Instantiate(emojiPrefab, emojiSpawnPoint.position, Quaternion.identity, emojiSpawnPoint.parent);
        EmojiLifecycle emojiScript = newObj.GetComponent<EmojiLifecycle>();

        // Setup gambar dan durasi
        emojiScript.Init(sprite, emojiDuration);

        // 4. Masukkan ke urutan pertama (Index 0 = Paling Baru/Atas)
        activeEmojis.Insert(0, emojiScript);

        // 5. Update Posisi Semua Emoji (Geser ke bawah)
        UpdateEmojiPositions();
    }

    void UpdateEmojiPositions()
    {
        // Loop semua emoji yang aktif
        for (int i = 0; i < activeEmojis.Count; i++)
        {
            if (activeEmojis[i] != null)
            {
                // Rumus: Posisi Awal - (Vector Bawah * Index * Jarak)
                // Index 0 (Baru) = Posisi Spawn
                // Index 1 = Posisi Spawn - 80px
                // Index 2 = Posisi Spawn - 160px ... dst
                Vector3 targetPos = emojiSpawnPoint.position - (Vector3.up * i * emojiSpacing);

                // Suruh script emoji bergerak ke target tersebut
                activeEmojis[i].SetTargetPosition(targetPos);
            }
        }
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