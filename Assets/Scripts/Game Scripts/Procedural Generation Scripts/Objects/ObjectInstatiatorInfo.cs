using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectInstantiatorParameters", menuName = "Procedural Generation")]
public class ObjectInstatiatorInfo : ScriptableObject
{
    public int radius = 60, amount = 10;
}
