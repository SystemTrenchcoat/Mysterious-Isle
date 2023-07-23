using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;


//place at the end of each room in procedural generation script
public class ObjectInstantiator : ObjectInstantiatorAbstract
{
    public GameObject[] entities;

    public Tilemap dangers;
    public Tilemap barriers;
    public Tilemap paths;


    public int radius;// = 60;
    public int amount;// = 10;//create thing for literals (like in the procedural generation program)
    public GameObject[] producedObjects;

    protected override void RunProceduralGeneration()
    {
        dangers = GameObject.Find("Dangers").GetComponent<Tilemap>();
        barriers = GameObject.Find("Barriers").GetComponent<Tilemap>();
        paths = GameObject.Find("Paths").GetComponent<Tilemap>();
        producedObjects = new GameObject[amount];

        for (int i = 0; i < amount; i++)
        {
            Vector3Int next = new Vector3Int(Random.Range(-1 * radius, radius + 1), Random.Range(-1 * radius, radius + 1), -1);

            Vector3Int barrierMapTile = barriers.WorldToCell(next);
            Vector3Int dangerMapTile = dangers.WorldToCell(next);
            Vector3Int pathMapTile = barriers.WorldToCell(next);

            var collider = Physics2D.OverlapCircle(new Vector2(next.x, next.y), .5f);

            while (!(barriers.GetTile(barrierMapTile) == null && (dangers.GetTile(dangerMapTile) != null || paths.GetTile(pathMapTile))
                && (collider == null || collider != null && collider.GetComponent<BoxCollider2D>() != null)))
            {
                next = new Vector3Int(Random.Range(0, radius), Random.Range(0, radius), -1);

                barrierMapTile = barriers.WorldToCell(next);
                dangerMapTile = dangers.WorldToCell(next);
                pathMapTile = barriers.WorldToCell(next);

                collider = Physics2D.OverlapCircle(new Vector2(next.x, next.y), .5f);
            }

            Debug.Log("x: " + next.x + "\ny: " + next.y + "\nz: " + next.z);
            producedObjects[i] = Instantiate(entities[Random.Range(0, entities.Length)], next, Quaternion.identity);
        }
    }
}
