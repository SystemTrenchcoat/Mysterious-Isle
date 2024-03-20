using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;


//place at the end of each room in procedural generation script
public class ObjectInstantiator : ObjectInstantiatorAbstract
{
    [SerializeField]
    private ObjectInstatiatorInfo info;

    public Tilemap dangers;
    public Tilemap barriers;
    public Tilemap paths;

    public GameObject[] producedObjects;

    public void ChangeParameters(ObjectInstatiatorInfo parameters)
    {
        info = parameters;
    }

    protected override void RunProceduralGeneration()
    {
        dangers = GameObject.Find("Dangers").GetComponent<Tilemap>();
        barriers = GameObject.Find("Barriers").GetComponent<Tilemap>();
        paths = GameObject.Find("Paths").GetComponent<Tilemap>();
        producedObjects = new GameObject[info.amount];

        for (int i = 0; i < info.amount; i++)
        {
            Vector3 next = new Vector3(Random.Range(-1 * info.radius, info.radius + 1) + .5f, Random.Range(-1 * info.radius, info.radius + 1) + .5f, -1);

            Vector3Int barrierMapTile = barriers.WorldToCell(next);
            Vector3Int dangerMapTile = dangers.WorldToCell(next);
            Vector3Int pathMapTile = barriers.WorldToCell(next);

            var collider = Physics2D.OverlapCircle(new Vector2(next.x, next.y), .5f);

            while (!(barriers.GetTile(barrierMapTile) == null && (dangers.GetTile(dangerMapTile) != null || paths.GetTile(pathMapTile))
                && (collider == null || collider != null && collider.GetComponent<BoxCollider2D>() != null)))
            {
                next = new Vector3(Random.Range(0, info.radius) + .5f, Random.Range(0, info.radius) + .5f, -1);

                barrierMapTile = barriers.WorldToCell(next);
                dangerMapTile = dangers.WorldToCell(next);
                pathMapTile = barriers.WorldToCell(next);

                collider = Physics2D.OverlapCircle(new Vector2(next.x, next.y), .5f);
            }

            //Debug.Log("x: " + next.x + "\ny: " + next.y + "\nz: " + next.z);
            producedObjects[i] = Instantiate(info.entities[Random.Range(0, info.entities.Length)], next, Quaternion.identity);
        }
    }
}
