using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float fadeTime = 1.0f;

    private TMP_Text textComp;
    private float timer;
    private Color startColor;

    void Awake()
    {
        textComp = GetComponent<TMP_Text>();
        if (textComp != null) startColor = textComp.color;
    }

    public void Init(string text, Color color)
    {
        if (textComp == null) textComp = GetComponent<TMP_Text>();
        textComp.text = text;
        textComp.color = color;
        startColor = color;
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);

        if (textComp != null)
        {
            textComp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }

        if (timer >= fadeTime) Destroy(gameObject);
    }
}