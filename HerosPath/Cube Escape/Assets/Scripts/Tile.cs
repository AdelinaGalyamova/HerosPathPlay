using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public bool isWall = false;

    private TileManager tileManager;

    private void Start()
    {
        tileManager = FindFirstObjectByType<TileManager>();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }
    }

    private void OnMouseDown()
    {
        if (isWall)
        {
            Debug.Log($"Клетка {x},{y} - стена, нельзя пройти");
            return;
        }

        if (tileManager != null)
        {
            tileManager.MovePlayerToTile(this);
        }
    }
}