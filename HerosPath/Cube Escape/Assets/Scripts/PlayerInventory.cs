using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddKey()
    {
        if (UIScript.Instance != null)
            UIScript.Instance.AddKey();
        Debug.Log("Ключ добавлен в инвентарь!");
    }
}