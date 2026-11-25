using UnityEngine;

public class GoldenCircle : MonoBehaviour
{
    public BoxTwo boxTwo;
    private bool collected = false;

    private void Start()
    {
        if (GetComponent<Rigidbody2D>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }

        gameObject.tag = "GoldenCircle";
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;

        Debug.Log("Кольцо собрано!");

        if (boxTwo != null)
        {
            boxTwo.OnRingCollected();
        }
        AudioManager.Instance?.PlayCollectRing();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }
}