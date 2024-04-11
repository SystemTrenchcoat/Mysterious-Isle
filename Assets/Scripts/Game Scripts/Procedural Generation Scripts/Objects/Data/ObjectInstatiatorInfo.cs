using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectInstantiatorParameters", menuName = "Procedural Generation")]
public class ObjectInstatiatorInfo : ScriptableObject
{
    public GameObject[] objects;
    public float[] chances;
    public int radius = 20, amount = 10;
}
