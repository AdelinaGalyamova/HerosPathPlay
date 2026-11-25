using UnityEngine;

public class KeyScript : MonoBehaviour
{
    private bool collected = false;

    public void Collect()
    {
        if (collected) return;
        collected = true;

        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
        if (inventory != null)
            inventory.AddKey();
        AudioManager.Instance?.PlayCollectKey();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }
}