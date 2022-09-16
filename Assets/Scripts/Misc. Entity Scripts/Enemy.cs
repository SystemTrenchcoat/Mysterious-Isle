using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public enum Action { Move, Attack, Defend };

    Entities entity;
    Rigidbody2D rb;
    Vector3 nextLocation;
    Vector3 playerLocation;

    public Action nextAction = Action.Move;
    public float actionDelay = 2f;
    public float blockDuration = 1f;
    public float timer = 2f;
    public float timerB = 1f;
    public float speed = .2f;

    public Tilemap dangers;
    public Tilemap barriers;
    public Tilemap paths;

    public bool ignoreDistance;

    public float xOffset = 0;
    public float yOffset = -1f;

    public int[] xs = {1, 0, -1, 0, 2, 0, -2, 0 };
    public int[] ys = {0, 1, 0, -1, 0, 2, 0, -2 };

    public int lowestX = 1;
    public int lowestY = 1;

    public float maxAtkDistX = 1;
    public float maxAtkDistY = 1;

    public int[] noX;
    public int[] noY;

    public bool canDefend = false;
    public bool canDiag = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entity = GetComponent<Entities>();
        dangers = GameObject.Find("Dangers").GetComponent<Tilemap>();
        barriers = GameObject.Find("Barriers").GetComponent<Tilemap>();
        paths = GameObject.Find("Paths").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //Debug.Log(playerLocation.x + " " + transform.position.x + entity.defendDirectionOffsetX());
        //if (canDefend && (playerLocation.x == transform.position.x + entity.defendDirectionOffsetX() ||
        //    playerLocation.y == transform.position.y + entity.defendDirectionOffsetY()))
        //{
        //    nextAction = Action.Defend;
        //    entity.isDefending = true;  
        //}
        entity.isAttacking = false;
        playerLocation = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        if (entity.isDefending)
        {
            timerB -= Time.deltaTime;
        }
        
        if (timerB <= 0)
        {
            entity.isDefending = false;
        }

        if (timer <= 0)
        {
            //Debug.Log("Something");
            timerB = blockDuration;
            entity.isAttacking = false;
            entity.isDefending = false;
            DecideAction();

            //Debug.Log(nextAction);
            if (nextAction == Action.Attack)
            {
                //Debug.Log("Attack");
                entity.isAttacking = true;
                //Debug.Log(xOffset + "\n" + yOffset);

                Instantiate(entity.attack, new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, -1), Quaternion.identity);
            }

            //gradually moves enemy to location
            else if (nextAction == Action.Move)
            {
                gameObject.transform.Translate(new Vector3(xOffset, yOffset, 0));
                //if (Vector3.Distance(transform.position, nextLocation) <= 0)
            }

            else if (nextAction == Action.Defend)
            {
                entity.isDefending = true;
            }

            timer = actionDelay;
        }

        //Debug.Log(timer);
        timer -= Time.deltaTime;
    }

    private void DecideAction()
    {
        Action act = Action.Move;

        entity.isDefending = false;
        bool skip = false;
        bool canMove = true;
        Vector3 check;
        Vector3 next;

        List<Vector3> locations = new List<Vector3>();

        //string[] directions = { "Down", "Up", "Right", "Left"};

        for (int i = 0; i < xs.Length; i++)
        {
            canMove = true;

            int x = xs[i];
            int y = ys[i];
            int direct = i;
            if (!canDiag)
            {
                while (direct > 3)
                {
                    direct -= 4;
                }
            }

            else
            {
                while (direct > 7)
                {
                    direct -= 8;
                }
            }

            if (noX.Contains<int>(Math.Abs(x)))
            {
                canMove = false;
            }
            if (noY.Contains<int>(Math.Abs(y)))
            {
                canMove = false;
            }

            //Debug.Log(Mathf.Sign(x)+ " " + Mathf.Sign(y));
            check = new Vector3(transform.position.x + x, transform.position.y + y, 0);
            next = new Vector3(transform.position.x + (Math.Sign(x) * lowestX), transform.position.y + (Math.Sign(y) * lowestY), 0);
            Vector3Int barrierMapTile = barriers.WorldToCell(next);
            Vector3Int dangerMapTile = dangers.WorldToCell(next);
            Vector3Int pathMapTile = barriers.WorldToCell(next);

            //is next tile a wall? if no, return coordinate, if yes, return current position
            if (barriers.GetTile(barrierMapTile) == null && (dangers.GetTile(dangerMapTile) != null || paths.GetTile(pathMapTile)))
            {
                //Debug.Log("No wall\n" + i);
                var collider = Physics2D.OverlapCircle(check, .5f);
                //is the next position occupied by anyone? if yes, player or enemy? if player, change to attack, if enemy, return current position, if neither, return next coordinate
                if (collider != null && collider.GetComponent<BoxCollider2D>() != null && collider.GetComponent<BoxCollider2D>())// != rb.GetComponent<BoxCollider2D>())
                {
                    //Debug.Log("Something near...");
                    if (collider.CompareTag("Player"))
                    {
                        //Debug.Log(Math.Abs(check.x - transform.position.x) + "\n" + Math.Abs(check.y - transform.position.y));
                        bool inAtkRng = Math.Abs(check.x - transform.position.x) <= maxAtkDistX && Math.Abs(check.y - transform.position.y) <= maxAtkDistY;
                        if (inAtkRng || ignoreDistance)
                        {
                            act = Action.Attack;
                            entity.changeDirection(direct);
                            xOffset = Math.Sign(x) * maxAtkDistX;
                            yOffset = Math.Sign(y) * maxAtkDistY;
                            //Debug.Log(x + "\n" + y);
                            //Debug.Log(xOffset + "\n" + yOffset);
                            //i = 10; //end loop
                            //Debug.Log("Next action: Attack") ;
                        }

                        else if ((Math.Abs(next.x - playerLocation.x) == 0 || Math.Abs(playerLocation.x - next.x) == 0)
                            && (Math.Abs(next.y - playerLocation.y) == 0 || Math.Abs(playerLocation.y - next.y) == 0))
                        {
                            canMove = false;
                        }

                        else if (canDefend && !inAtkRng)
                        {
                            act = Action.Defend;
                            //Debug.Log("Next action: Defend");
                        }

                        else if (canMove)
                        {
                            act = Action.Move;
                            skip = true;
                            entity.changeDirection(direct);
                            xOffset = Math.Sign(x) * lowestX;
                            yOffset = Math.Sign(y) * lowestY;
                            //i = 10; //end loop
                            //Debug.Log(xOffset + " " + yOffset);
                        }

                        else
                        {
                            canMove = false;
                            //Debug.Log("Ally near\n" + collider);
                        }
                    }

                    else
                    {
                        canMove = false;
                        //Debug.Log("Ally near\n" + collider);
                    }
                    //change direction to this direction and attack
                }

                else
                {
                    if (canMove)
                    {
                        locations.Add(next);
                    }
                    //Debug.Log(next);
                }
            }

            else
            {
                canMove = false;
                //Debug.Log("There's a wall");
            }
        }

        if (act == Action.Move && !skip)
        {
            if (locations.Count > 0)
            {
                //Debug.Log(xOffset + " " + yOffset);
                nextLocation = locations[Random.Range(0, locations.Count)];
                int direct = locations.IndexOf(nextLocation);
                if (!canDiag)
                {
                    while (direct > 3)
                    {
                        direct -= 4;
                    }
                }

                else
                {
                    while (direct > 7)
                    {
                        direct -= 8;
                    }
                }
                entity.changeDirection(direct);
                xOffset = nextLocation.x - transform.position.x;
                yOffset = nextLocation.y - transform.position.y;
            }
            else
            {
                nextLocation = new Vector3(transform.position.x, transform.position.y, 0);
                //Debug.Log(nextLocation);
            }
        }

        nextAction = act;
    }

    private void DecideBlock()
    {
        for (int i = 0; i < xs.Length; i++)
        {
            int x = xs[i];
            int y = ys[i];
            Vector3 check = new Vector3(transform.position.x + x, transform.position.y + y, 0);
            var collider = Physics2D.OverlapCircle(check, .5f);
            //is the next position occupied by anyone? if yes, player or enemy? if player, change to attack, if enemy, return current position, if neither, return next coordinate
            if (collider != null && collider.GetComponent<BoxCollider2D>() != null && collider.GetComponent<BoxCollider2D>())// != rb.GetComponent<BoxCollider2D>())
            {
                //Debug.Log("Something near...");
                if (collider.CompareTag("Player"))
                {
                    nextAction = Action.Defend;
                    entity.isDefending = true;
                }
                else
                {
                    entity.isDefending = false;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x + 2, transform.position.y + 0, 0), .3f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + -2, transform.position.y + 0, 0), .3f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + 0, transform.position.y + 2, 0), .3f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + 0, transform.position.y + -2, 0), .3f);
    }
}
