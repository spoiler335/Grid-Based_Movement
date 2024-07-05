using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAI
{
    private List<List<SingleTile>> grid => DI.di.gridGenerator.grid;
    private int currX = -1;
    private int currY = -1;

    private void Awake() => SubscribeEvents();

    private void SubscribeEvents()
    {
        EventsModel.TILE_CLICKED += MoveTowardsPlayer;
    }

    private void UnsubscribeEvents()
    {
        EventsModel.TILE_CLICKED -= MoveTowardsPlayer;
    }


    public void MoveTowardsPlayer(int x, int y)
    {
        SingleTile playerTarget = grid[x][y];
        var playerNeighbors = DI.di.gridGenerator.GetNeighbors(playerTarget);
        SingleTile myTarget = playerNeighbors[Random.Range(0, playerNeighbors.Count)];
        SingleTile currentTile = grid[currX][currY];
        var path = DI.di.gridGenerator.FindPath(currentTile, myTarget);
        StartCoroutine(MoveToardsTarget(path));
    }

    public IEnumerator MoveToardsTarget(List<SingleTile> path)
    {
        foreach (SingleTile tile in path)
        {
            yield return StartCoroutine(MoveToPosition(new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z)));
        }
        currX = path[path.Count - 1].gridX;
        currY = path[path.Count - 1].gridY;
        Debug.Log($"Player moved to ({currX},{currY})");
        EventsModel.PLAYER_MOVEMNT_COMPLETED?.Invoke();
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        while ((transform.position - target).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, IAI.moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void SetInitialCoordinates(int x, int y)
    {
        currX = x;
        currY = y;
    }

    private void OnDestroy() => UnsubscribeEvents();
}
