using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProceduralGeneratedRoomScript
{

    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength, double dangerChance, TilemapVisualizer tilemapVisualizer)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomDirection();
            path.Add(newPosition);

            if (Random.Range(0, 1f) <= dangerChance)
            {
                tilemapVisualizer.PaintDangers(newPosition);
            }
            previousPosition = newPosition;
        }
        return path;
    }

    public static HashSet<Vector2Int> ConnectRooms(HashSet<Vector2Int> roomMarkers, int walkLength, double thickness, double dangerChance, TilemapVisualizer tilemapVisualizer)
    {
        HashSet<Vector2Int> path = roomMarkers;
        List<Vector2Int> rooms = roomMarkers.ToList();

        foreach (var roomMarker in rooms)
        {
            //Debug.Log(roomMarker);
            foreach (var room in rooms)
            {
                int x;
                int y;
                if (rooms.IndexOf(roomMarker) > rooms.IndexOf(room))
                {
                    x = roomMarker.x - room.x;
                    y = roomMarker.y - room.y;
                }
                else
                {
                    x = room.x - roomMarker.x;
                    y = room.y - roomMarker.y;
                }

                Debug.Log("x: " + x + "\ny: " + y);

                Vector2Int previousPosition = roomMarker;
                int extra = 0;

                for (int i = 0; i < Mathf.Abs(x) + Mathf.Abs(y) + walkLength;)
                {
                    Vector2Int currentPosition;

                    currentPosition = previousPosition + Direction2D.GetRandomDirection();

                    if (Mathf.Abs(room.x - currentPosition.x) < Mathf.Abs(room.x - previousPosition.x) || Mathf.Abs(room.y - currentPosition.y) < Mathf.Abs(room.y - previousPosition.y))
                    {
                        //i++;
                    }

                    else if (extra < walkLength)
                    {
                        extra++;
                    }

                    else
                    {
                        if (currentPosition.x != room.x)// && Random.Range(0,2) == 1)//xDiff != x)
                        {
                            currentPosition = new Vector2Int(previousPosition.x - (int)Mathf.Sign(x), previousPosition.y);
                        }
                        if (currentPosition.y != room.y)// && Random.Range(0, 2) == 0)
                        {
                            currentPosition = new Vector2Int(previousPosition.x, previousPosition.y - (int)Mathf.Sign(y));
                        }
                        i++;
                    }

                    if (Random.Range(0, 1f) <= dangerChance)
                    {
                        tilemapVisualizer.PaintDangers(currentPosition);
                    }

                    previousPosition = currentPosition;


                    path.Add(currentPosition);
                    for (int d = 0; d < Direction2D.DirectionList.Count * thickness; d++)
                    {
                        path.Add(currentPosition + Direction2D.GetRandomDirection());
                    }
                }
            }
        }
        //Debug.Log("Connected");
        return path;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> DirectionList = new List<Vector2Int>()
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0)
    };

    public static Vector2Int GetRandomDirection()
    {
        return DirectionList[Random.Range(0, DirectionList.Count)];
    }
}