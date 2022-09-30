using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectInstantiatorAbstract : MonoBehaviour
{
    public void GenerateDungeon()
    {
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
