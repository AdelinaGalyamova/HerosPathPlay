using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public GameObject player;
    public Tile startTile;
    public int levelId = 1;

    public float moveSpeed = 8f;

    private Tile currentTile;
    private bool isMoving = false;
    private UIScript uiScript;
    private int currentMoves = 0;
    private List<Tile> allTiles = new List<Tile>();

    private void Start()
    {
        uiScript = FindFirstObjectByType<UIScript>();
        allTiles.AddRange(FindObjectsByType<Tile>(FindObjectsSortMode.None));

        if (GameManager.Instance != null)
        {
            currentMoves = GameManager.Instance.GetCarryOverMoves();
            GameManager.Instance.ResetCarryOverMoves();

            if (uiScript != null) uiScript.UpdateMoveCount(currentMoves);
        }

        if (startTile != null)
        {
            player.transform.position = startTile.transform.position;
            currentTile = startTile;
        }
        else
        {
            currentTile = GetTileAtPosition(player.transform.position);
        }
    }

    public Tile GetTileAtPosition(Vector3 position)
    {
        foreach (Tile tile in allTiles)
        {
            if (Vector3.Distance(tile.transform.position, position) < 0.1f)
                return tile;
        }
        return null;
    }

    public Tile GetTileAtGrid(int x, int y)
    {
        foreach (Tile tile in allTiles)
        {
            if (tile.x == x && tile.y == y)
                return tile;
        }
        return null;
    }

    public void MovePlayerToTile(Tile targetTile)
    {
        if (isMoving) return;
        if (targetTile == null) return;
        if (targetTile == currentTile) return;

        Vector3 diff = targetTile.transform.position - currentTile.transform.position;
        if (Mathf.Abs(diff.x) > 0.1f && Mathf.Abs(diff.y) > 0.1f)
        {
            Debug.Log("Нельзя ходить по диагонали!");
            return;
        }

        float distance = Vector3.Distance(currentTile.transform.position, targetTile.transform.position);
        if (distance > 1.5f)
        {
            Debug.Log("Можно ходить только на соседние клетки!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(targetTile.transform.position, 0.3f);
        GameObject box = null;
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("BoxOne"))
            {
                box = hit.gameObject;
                break;
            }
        }

        if (box != null)
        {
            Vector3 pushDirection = targetTile.transform.position - currentTile.transform.position;
            Vector3 newBoxPos = targetTile.transform.position + pushDirection;

            Collider2D[] pushHits = Physics2D.OverlapCircleAll(newBoxPos, 0.3f);
            bool canPush = true;
            foreach (Collider2D hit in pushHits)
            {
                if (hit.CompareTag("BoxOne") || hit.CompareTag("Wall") || hit.CompareTag("DoorOne"))
                {
                    canPush = false;
                    break;
                }
            }

            if (canPush)
            {
                box.transform.position = newBoxPos;
                AudioManager.Instance?.PlayPushBox();
                StartCoroutine(MoveToTile(targetTile));
            }
            else
            {
                Debug.Log("Нельзя толкнуть ящик — путь заблокирован!");
            }
        }
        else
        {
            StartCoroutine(MoveToTile(targetTile));
        }
    }

    private IEnumerator MoveToTile(Tile target)
    {
        isMoving = true;
        Vector3 startPos = player.transform.position;
        Vector3 endPos = target.transform.position;
        float duration = Vector3.Distance(startPos, endPos) / moveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            player.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        player.transform.position = endPos;
        AudioManager.Instance?.PlayStep();
        currentTile = target;
        currentMoves++;
        if (uiScript != null) uiScript.UpdateMoveCount(currentMoves);

        CheckTileContent(target);
        isMoving = false;
    }

    private void CheckTileContent(Tile tile)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(tile.transform.position, 0.3f);
        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit.gameObject == player) continue;
            if (hit.CompareTag("Key")) hit.GetComponent<KeyScript>()?.Collect();
            else if (hit.CompareTag("GoldenCircle")) hit.GetComponent<GoldenCircle>()?.Collect();
            else if (hit.CompareTag("BoxTwo")) hit.GetComponent<BoxTwo>()?.TryOpen();
            else if (hit.CompareTag("DoorOne")) hit.GetComponent<DoorScript>()?.TryEnter();
            else if (hit.CompareTag("Coffin")) hit.GetComponent<CoffinScript>()?.TryEnter();
        }
    }

    public void ResetMoves()
    {
        currentMoves = 0;
        if (uiScript != null) uiScript.UpdateMoveCount(0);
    }

    public int GetCurrentMoves() => currentMoves;
}