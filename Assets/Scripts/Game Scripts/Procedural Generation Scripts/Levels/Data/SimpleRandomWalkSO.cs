using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters_")]
public class SimpleRandomWalkSO : ScriptableObject
{
    public int iterations = 10, walkLength = 10, rooms = 10, roomSeperation = 20, pathWalk = 4, dangerMin = 8, dangerMax = 10;
    public double pathThickness = .5, pathDangerChance = .001, areaDangerChance = .001;
    public bool startRandomlyEachIteration = true;
}
