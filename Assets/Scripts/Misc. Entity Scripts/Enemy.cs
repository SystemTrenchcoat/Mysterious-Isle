using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using static UnityEditor.FilePathAttribute;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    public enum Action { Move, Attack, Defend };

    Entities entity;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Vector3 nextLocation;
    Vector3 playerLocation;

    public Action nextAction = Action.Move;
    public float actionDelay = 2f;
    public float attackDelay = 2f;
    public float blockDuration = 1f;
    public float timer = 2f;
    public float timerA = 2f; //Attack
    public float timerB = 1f; //Block
    public float speed = .2f;

    public Tilemap dangers;
    public Tilemap barriers;
    public Tilemap paths;

    public bool camo;
    public bool active = true;
    public bool ignoreDistance;
    public bool omniAttacker;
    public bool omniTracker;

    public float xOffset = 0;
    public float yOffset = -1f;

    //public int[] xs = {1, 0, -1, 0, 2, 0, -2, 0 };
    //public int[] ys = {0, 1, 0, -1, 0, 2, 0, -2 };

    public int checkRadius = 2;
    public int lowestX = 1;
    public int lowestY = 1;

    public float maxAtkDistX = 1;
    public float maxAtkDistY = 1;

    //public int[] noX;
    //public int[] noY;

    public int numActions = 1;
    public int actionsRemaining = 1;
    public float visualDelay = .5f;
    public float visualCounter;

    public bool canDefend = false;
    public int abilityChance;// = 25;
    public bool abilityOverload = false; //ability instead of attack
    public int altChance;// = 50;
    public bool altOverload = false;
    public bool hybridAttacker;
    public bool canDiag = false;
    public string special;

    public GameObject instance;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entity = GetComponent<Entities>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        //if (entity.hp <= 0 && special == "Brood")
        //{

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
            if (actionsRemaining > 0 && visualCounter <= 0 && entity.effect != Entities.Effect.Stunned)
            {
                DecideAction();

                //Debug.Log(nextAction);
                if (nextAction == Action.Attack)
                {
                    //Debug.Log("Attack");
                    entity.isAttacking = true;
                    //Debug.Log(xOffset + "\n" + yOffset);

                    var attack = Instantiate(instance, new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, -1), Quaternion.identity);
                    attack.GetComponent<Damage>().target = GameObject.FindGameObjectWithTag("Player").GetComponent<Entities>();
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

                actionsRemaining -= 1;
                visualCounter = visualDelay;
            }

            visualCounter -= Time.deltaTime;

            if (actionsRemaining <= 0)
            {
                actionsRemaining = numActions;
                timer = actionDelay;
            }
        }

        //Debug.Log(timer);
        timer -= Time.deltaTime;
        timerA -= Time.deltaTime;
    }

    private void DecideAction()
    {
        Action act = Action.Move;

        entity.isDefending = false;
        bool skip = false;
        bool canMove = true;
        Vector3 check;
        Vector3 next;
        xOffset = 0;
        yOffset = 0;

        List<Vector3> locations = new List<Vector3>();

        //string[] directions = { "Down", "Up", "Right", "Left"};

        for (int x = -checkRadius; x <= checkRadius; x++)
        {
            for (int y = -checkRadius; y <= checkRadius; y++)
            {
                canMove = true;

                if (!canDiag && Math.Sign(x) != 0 && Math.Sign(y) != 0
                    || (x < lowestX - 1 || y < lowestY - 1)
                    || (special == "Only Diag" && (x == 0 || y == 0)))
                {
                    canMove = false;
                }
                //if (noX.Contains<int>(Math.Abs(x)))
                //{
                //    canMove = false;
                //}
                //if (noY.Contains<int>(Math.Abs(y)))
                //{
                //    canMove = false;
                //}

                check = new Vector3(transform.position.x + x, transform.position.y + y, 0);
                next = new Vector3(transform.position.x + (Math.Sign(x) * lowestX), transform.position.y + (Math.Sign(y) * lowestY), 0);



                if (canMove)
                {
                    canMove = CheckLocation(check, next);
                }

                if (canMove)
                {
                    locations.Add(next);
                }

                else
                {
                    var collider = Physics2D.OverlapCircle(check, .5f);
                    //Debug.Log(collider);
                    //is the next position occupied by anyone? if yes, player or enemy? if player, change to attack, if enemy, return current position, if neither, return next coordinate
                    if (collider != null && collider.GetComponent<BoxCollider2D>() != null)// && collider.GetComponent<BoxCollider2D>())// != rb.GetComponent<BoxCollider2D>())
                    {
                        //Debug.Log("Something near...");
                        if (collider.CompareTag("Player") && timerA <= 0
                            || collider.GetComponent<Grappler>() != null && collider.GetComponent<Grappler>().isGrappling)
                        {
                            //Debug.Log(Math.Abs(check.x - transform.position.x) + "\n" + Math.Abs(check.y - transform.position.y));
                            bool inAtkRng = Math.Abs(check.x - transform.position.x) <= maxAtkDistX && Math.Abs(check.y - transform.position.y) <= maxAtkDistY;
                            if (inAtkRng || ignoreDistance
                                || (special == "Create Grapple" && !GameObject.FindGameObjectWithTag("Player").GetComponent<Entities>().isGrappled))
                            {
                                act = Action.Attack;
                                xOffset = Math.Sign(x) * maxAtkDistX;
                                yOffset = Math.Sign(y) * maxAtkDistY;
                                entity.changeDirection(Math.Sign(xOffset), Math.Sign(yOffset));
                                timerA = attackDelay;
                                if ((hybridAttacker && !inAtkRng) || (inAtkRng && entity.attack.GetComponent<Damage>().special == "Lunge")
                                    || (!inAtkRng && special == "Create Grapple" && !GameObject.FindGameObjectWithTag("Player").GetComponent<Entities>().isGrappled))
                                {
                                    altOverload = true;
                                    xOffset = playerLocation.x - transform.position.x;
                                    yOffset = playerLocation.y - transform.position.y;
                                }

                                //Debug.Log(x + "\n" + y);
                                //Debug.Log(xOffset + "\n" + yOffset);
                                //i = 10; //end loop
                                Debug.Log("Next action: Attack");
                            }

                            else if (canDefend && !inAtkRng)
                            {
                                act = Action.Defend;
                                //Debug.Log("Next action: Defend");
                            }

                            else if (collider != null &&
                                next != new Vector3(collider.transform.position.x, collider.transform.position.y, next.z))
                            {
                                act = Action.Move;
                                skip = true;
                                xOffset = Math.Sign(x) * lowestX;
                                yOffset = Math.Sign(y) * lowestY;
                                if (!canDiag && Math.Sign(xOffset) != 0 && Math.Sign(yOffset) != 0)
                                {
                                    switch(Random.Range(1,3))
                                    {
                                        case 1: xOffset = 0; break;
                                        
                                        case 2: yOffset = 0; break;
                                    }
                                }
                                entity.changeDirection(Math.Sign(xOffset), Math.Sign(yOffset));
                                if (camo && !active)
                                {
                                    camo = false;
                                }
                                //i = 10; //end loop
                                Debug.Log(xOffset + " " + yOffset);
                            }
                        }
                    }
                }
            }
        }

        if (act == Action.Attack)
        {
            //Debug.Log(altOverload);
            if (entity.isBurrowing)
            {
                entity.isBurrowing = false;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                act = Action.Move;
                xOffset = 0;
                yOffset = 0;
                skip = true;
            }

            else
            {
                instance = entity.weapon;

                if (abilityOverload)
                {
                    instance = entity.attack;
                }
                if (altOverload)
                {
                    instance = entity.alt;
                }
                if (abilityChance > 0 && Random.Range(1, 101) <= abilityChance)
                {
                    instance = entity.attack;
                }
                if (altChance > 0 && Random.Range(1, 101) <= altChance)
                {
                    instance = entity.alt;
                }
            }

            if (hybridAttacker || entity.attack.GetComponent<Damage>().special == "Lunge" || special == "Create Grapple")
            {
                altOverload = false;
            }
        }

        else if (act == Action.Move && entity.canBurrow && !entity.isBurrowing && !camo)
        {
            xOffset = 0;
            yOffset = 0;
            entity.isBurrowing = true;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, .5f);
            locations.Clear();
        }

        else if (act == Action.Move && !skip && !camo)
        {
            if (locations.Count > 0)
            {
                //Debug.Log(xOffset + " " + yOffset);
                nextLocation = locations[Random.Range(0, locations.Count)];

                xOffset = nextLocation.x - transform.position.x;
                yOffset = nextLocation.y - transform.position.y;
                entity.changeDirection(Math.Sign(xOffset), Math.Sign(yOffset));
            }
            else
            {
                nextLocation = new Vector3(transform.position.x, transform.position.y, 0);
                //Debug.Log(nextLocation);
            }
        }

        if (entity.effect == Entities.Effect.Disoriented)
        {
            xOffset *= -1;
            yOffset *= -1;

            if (!CheckLocation(new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, 0)))
            {
                xOffset = 0;
                yOffset = 0;
            }
        }

        nextAction = act;
    }

    private bool CheckLocation(Vector3 check, Vector3 next)
    {
        bool canMove = true;

        Vector3Int barrierMapTile = barriers.WorldToCell(next);
        Vector3Int dangerMapTile = dangers.WorldToCell(next);
        Vector3Int pathMapTile = barriers.WorldToCell(next);

        //is next tile a wall? if no, return coordinate, if yes, return current position
        if (barriers.GetTile(barrierMapTile) == null && (dangers.GetTile(dangerMapTile) != null || paths.GetTile(pathMapTile)))
        {
            //Debug.Log("No wall\n" + i);
            var collider = Physics2D.OverlapCircle(check, .5f);
            //Debug.Log(collider);
            //is the next position occupied by anyone? if yes, player or enemy? if player, change to attack, if enemy, return current position, if neither, return next coordinate
            if (collider != null && collider.GetComponent<BoxCollider2D>() != null)// && collider.GetComponent<BoxCollider2D>())// != rb.GetComponent<BoxCollider2D>())
            {
                //Debug.Log("Something near...");

                canMove = false;
            }
        }

        else
        {
            canMove = false;
            //Debug.Log("There's a wall");
        }

        return canMove;
    }

    private bool CheckLocation(Vector3 next)
    {
        bool canMove = CheckLocation(next, next);

        return canMove;
    }

    ////private void DecideBlock()
    //{
    //    for (int i = 0; i < xs.Length; i++)
    //    {
    //        int x = xs[i];
    //        int y = ys[i];
    //        Vector3 check = new Vector3(transform.position.x + x, transform.position.y + y, 0);
    //        var collider = Physics2D.OverlapCircle(check, .5f);
    //        //is the next position occupied by anyone? if yes, player or enemy? if player, change to attack, if enemy, return current position, if neither, return next coordinate
    //        if (collider != null && collider.GetComponent<BoxCollider2D>() != null && collider.GetComponent<BoxCollider2D>())// != rb.GetComponent<BoxCollider2D>())
    //        {
    //            //Debug.Log("Something near...");
    //            if (collider.CompareTag("Player"))
    //            {
    //                nextAction = Action.Defend;
    //                entity.isDefending = true;
    //            }
    //            else
    //            {
    //                entity.isDefending = false;
    //            }
    //        }
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x + 2, transform.position.y + 0, 0), .3f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + -2, transform.position.y + 0, 0), .3f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + 0, transform.position.y + 2, 0), .3f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + 0, transform.position.y + -2, 0), .3f);
    }
}
