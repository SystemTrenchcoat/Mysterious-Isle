using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleRandomWalkGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    private SimpleRandomWalkSO randomWalkParameters;


    protected override void RunProceduralGeneration()
    {
        tilemapVisualizer.Clear();
        List<HashSet<Vector2Int>> baseMap = RunRandomWalk();
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        foreach (var position in baseMap)
        {
            floorPositions.AddRange(position.ToArray());
        }

        tilemapVisualizer.PaintFLoorTile(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        GoalTileGenerator.CreateGoals(baseMap, tilemapVisualizer);
    }

    protected List<HashSet<Vector2Int>> RunRandomWalk()
    {
        Debug.Log("things");
        var currentPosition = startPosition;
        List<HashSet<Vector2Int>> tilePositions = new List<HashSet<Vector2Int>>();
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomMarkers = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        for (int x = 0; x < randomWalkParameters.rooms; x++)
        {
            for (int i = 0; i < randomWalkParameters.iterations; i++)
            {
                var path = ProceduralGeneratedRoomScript.SimpleRandomWalk(currentPosition, randomWalkParameters.walkLength, randomWalkParameters.areaDangerChance, tilemapVisualizer);

                floorPositions.UnionWith(path);
                roomPositions.UnionWith(path);
                //Debug.Log(currentPosition);
                if (i == randomWalkParameters.iterations - 1)
                {
                    Debug.Log("Goal");
                    roomMarkers.Add(path.ElementAt(path.Count - 1));
                }

                if (randomWalkParameters.startRandomlyEachIteration)
                {
                    currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
                    //GoalTileGenerator.CreateGoals(floorPositions.ElementAt(Random.Range(0, floorPositions.Count)), tilemapVisualizer);
                }
            }

            tilePositions.Add(roomPositions);
            roomPositions = new HashSet<Vector2Int>();
            //tilePositions.Add(roomCenters);

            while (floorPositions.Contains(currentPosition))
            //|| floorPositions.Contains(currentPosition + (Direction2D.DirectionList.ElementAt(0) * randomWalkParameters.roomSeperation))
            // || floorPositions.Contains(currentPosition + (Direction2D.DirectionList.ElementAt(1) * randomWalkParameters.roomSeperation))
            //  || floorPositions.Contains(currentPosition + (Direction2D.DirectionList.ElementAt(2) * randomWalkParameters.roomSeperation))
            //   || floorPositions.Contains(currentPosition + (Direction2D.DirectionList.ElementAt(3) * randomWalkParameters.roomSeperation)))
            {
                currentPosition = floorPositions.ElementAt(floorPositions.Count - 1) + (Direction2D.GetRandomDirection() * randomWalkParameters.roomSeperation);
            }
        }

        HashSet<Vector2Int> paths = ProceduralGeneratedRoomScript.ConnectRooms(roomMarkers, randomWalkParameters.pathWalk, randomWalkParameters.pathThickness, randomWalkParameters.pathDangerChance, tilemapVisualizer);

        tilePositions.Add(paths);

        //Debug.Log(tilePositions[0] == tilePositions[1]);

        //HashSet<Vector2Int> n = new HashSet<Vector2Int>();
        //foreach (HashSet<Vector2Int> position in tilePositions)
        //{
        //    n.UnionWith(position);
        //}
        Debug.Log(paths.Count);
        return tilePositions;
    }
}
