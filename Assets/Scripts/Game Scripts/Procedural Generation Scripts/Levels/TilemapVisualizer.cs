using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap, dangerTilemap;//, goalTilemap;
    [SerializeField]
    private TileBase floorTile, wallTile, dangerTile;//, goalTile, activeGoalTile;

    public void PaintFLoorTile(IEnumerable<Vector2Int> floorPositions)
    {
        Debug.Log("Paint");
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            //Debug.Log("Paint");
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        dangerTilemap.ClearAllTiles();
        //goalTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTile, position);
    }

    //internal void PaintGoals(Vector2Int position)
    //{
    //    PaintSingleTile(goalTilemap, goalTile, position);
    //    //Debug.Log(position);
    //}

    internal void PaintDangers(Vector2Int position)
    {
        PaintSingleTile(dangerTilemap, dangerTile, position);
        //Debug.Log(position);
    }
}
