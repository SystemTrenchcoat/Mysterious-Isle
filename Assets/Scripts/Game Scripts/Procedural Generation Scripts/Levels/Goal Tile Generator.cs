using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class GoalTileGenerator
{
    public static void CreateGoals(List<HashSet<Vector2Int>> roomPositions, TilemapVisualizer tilemapVisualizer)
    {
        HashSet<Vector2Int> basicGoalPositions = ConnectRooms(FindRoomCenters(roomPositions), roomPositions);
        //Debug.Log(FindRoomCenters(roomPositions).Count);
        //foreach (var position in basicGoalPositions)
        //{
        //    //Debug.Log(position);
        //    tilemapVisualizer.PaintGoals(position);
        //}
    }

    //public static void CreateGoals(Vector2Int position, TilemapVisualizer tilemapVisualizer)
    //{
    //    tilemapVisualizer.PaintGoals(position);
    //}

    private static List<Vector2Int> FindRoomCenters(List<HashSet<Vector2Int>> roomPositions)
    {
        List<Vector2Int> centers = new List<Vector2Int>();

        foreach (HashSet<Vector2Int> room in roomPositions)
        {
            int maxX = 0;
            int maxY = 0;
            var tiles = room.ToArray();
            for(int i = 0; i < room.Count; i++)
            {
                for(int j = 0; j < room.Count; j++)
                {
                    int distanceX = tiles[i].x - tiles[j].x;
                    int distanceY = tiles[i].y - tiles[j].y;
                    if (distanceX > maxX)
                    {
                        maxX = distanceX;
                    }

                    if (distanceY > maxY)
                    {
                        maxY = distanceY;
                    }
                }
            }
            //Debug.Log("X: " + maxX / 2 + "\nY: " + maxY / 2);
            centers.Add(new Vector2Int(maxX / 2, maxY / 2));
        }

        return centers;
    }

    public static HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters, List<HashSet<Vector2Int>> roomPositions)
    {
        var currentRoomCenter = roomCenters[0];
        HashSet<Vector2Int> goalTiles = new HashSet<Vector2Int>();
        int roomIndex = roomCenters.IndexOf(currentRoomCenter);
        roomCenters.Remove(currentRoomCenter);

        //while (roomCenters.Count > 0)
        for (int y = 0; y < roomCenters.Count; y++)
        {
            float closestDistanceSqr = Mathf.Infinity;
            for (int x = 0; x < roomCenters.Count; x++)
            {
                Vector2Int directionToTarget = roomCenters[y] - currentRoomCenter;
                float dSqrToTile = directionToTarget.sqrMagnitude;
                if (dSqrToTile < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTile;
                }
                if (dSqrToTile == closestDistanceSqr)
                {
                    currentRoomCenter = roomCenters[y];
                    int currentRoom = roomIndex;
                    foreach(var position in roomPositions)
                    {
                        if (position.Contains(currentRoomCenter))
                        {
                            roomIndex = roomPositions.IndexOf(position);
                        }
                    }
                    goalTiles = FindGoals(roomPositions[currentRoom], roomPositions[roomIndex]);
                    roomCenters.Remove(roomCenters[y]);
                    break;
                }
                if (x == roomCenters.Count)
                {
                    x = 0;
                }
            }
        }

        return goalTiles;
    }

    //public static Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter)

    private static HashSet<Vector2Int> FindGoals(HashSet<Vector2Int> current, HashSet<Vector2Int> next)
    {
        HashSet<Vector2Int> goalPositions = new HashSet<Vector2Int>();
        Vector2Int closestCurrent = Vector2Int.zero;
        Vector2Int closestNext = Vector2Int.zero;
        foreach (var tile in current)
        {
            float closestDistanceSqr = Mathf.Infinity;
            foreach (var pos in next)
            {
                Vector2Int directionToTarget = tile - pos;
                float dSqrToTile = directionToTarget.sqrMagnitude;
                //Debug.Log(dSqrToTile);
                if (dSqrToTile < closestDistanceSqr)// && !current.Contains(pos))
                {
                    closestDistanceSqr = dSqrToTile;
                    closestCurrent = tile;
                    closestNext = pos;
                }
            }
        }

        goalPositions.Add(closestCurrent);
        goalPositions.Add(closestNext);
        //Debug.Log("Current: " + closestCurrent);
        //Debug.Log("Closest: " + closestNext);


        //foreach (var compRoom in roomPos)
        //{
        //    if (compRoom != roomSet)
        //    {
        //        float closestDistanceSqr = Mathf.Infinity;
        //        foreach (var position in compRoom)
        //        {
        //            Vector2Int directionToTarget = position - pos;
        //            float dSqrToTile = directionToTarget.sqrMagnitude;
        //            if (dSqrToTile < closestDistanceSqr)
        //            {
        //                closestDistanceSqr = dSqrToTile;
        //            }
        //        }
        //    }
        //}

        return goalPositions;
    }
}
