using UnityEngine;

public class BoxTwo : MonoBehaviour
{
    public GameObject keyPrefab;
    public Transform keySpawnPoint;
    public Sprite openedSprite;

    private bool isOpened = false;
    private bool ringCollected = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.tag = "BoxTwo";

        if (GetComponent<Rigidbody2D>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void OnRingCollected()
    {
        ringCollected = true;
        Debug.Log("Кольцо собрано! Сундук можно открыть.");
    }

    public void TryOpen()
    {
        Debug.Log($"TryOpen вызван, ringCollected={ringCollected}, isOpened={isOpened}");

        if (ringCollected && !isOpened)
        {
            OpenBox();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryOpen();
        }
    }

    private void OpenBox()
    {
        isOpened = true;
        Debug.Log("Сундук открывается!");
        AudioManager.Instance?.PlayOpenBox();

        if (spriteRenderer != null && openedSprite != null)
        {
            spriteRenderer.sprite = openedSprite;
        }

        if (keyPrefab != null && keySpawnPoint != null)
        {
            Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);
            Debug.Log("Ключ выпал из сундука!");
        }
    }
}