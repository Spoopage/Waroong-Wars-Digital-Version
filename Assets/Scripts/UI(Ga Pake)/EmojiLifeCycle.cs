using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class EmojiLifecycle : MonoBehaviour
{
    public Image displayImage;
    public CanvasGroup canvasGroup; // Wajib ada untuk efek transparan (Fade out)

    private Vector3 targetPosition;
    private bool shouldMove = false;
    private float moveSpeed = 10f; // Kecepatan geser ke bawah

    void Awake()
    {
        // Otomatis cari komponen jika lupa drag
        if (displayImage == null) displayImage = GetComponent<Image>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        // Set posisi target awal sama dengan posisi spawn agar tidak teleport
        targetPosition = transform.position;
    }

    public void Init(Sprite sprite, float lifeTime)
    {
        displayImage.sprite = sprite;
        // Mulai hitung mundur kematian
        StartCoroutine(LifeRoutine(lifeTime));
    }

    // Fungsi ini dipanggil Manager untuk menyuruh emoji geser ke bawah
    public void SetTargetPosition(Vector3 newPos)
    {
        targetPosition = newPos;
        shouldMove = true;
    }

    void Update()
    {
        // Logika animasi pergerakan mulus (Lerp)
        if (shouldMove)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            // Jika sudah sangat dekat, stop kalkulasi (optimasi)
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                shouldMove = false;
            }
        }
    }

    IEnumerator LifeRoutine(float duration)
    {
        // 1. Tunggu selama durasi (misal 3 detik)
        yield return new WaitForSeconds(duration);

        // 2. Animasi Fade Out (Meredup)
        float fadeSpeed = 2f;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // 3. Hancurkan diri sendiri dan beri tahu Manager (opsional, tapi aman dicover di Manager)
        Destroy(gameObject);
    }
}