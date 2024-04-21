using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEditor.FilePathAttribute;

public class Controller : MonoBehaviour
{
    Entity entity;

    Rigidbody2D rb;

    //public float speed = .2f;

    public enum charClass { Warrior, Archer };

    public charClass chara;

    public Tilemap dangers;
    public Tilemap barriers;

    public int roomNum = 0;

    public float xOffset = 0;
    public float yOffset = 0;

    //private void Awake()
    //{
    //    Debug.Log("Awake");
    //    rb.GetComponent<Rigidbody2D>();
    //    entity.GetComponent<Entities>();
    //}

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entity = GetComponent<Entity>();
        dangers = GameObject.Find("Dangers").GetComponent<Tilemap>();
        barriers = GameObject.Find("Barriers").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dangers == null)
        {
            dangers = GameObject.Find("Dangers").GetComponent<Tilemap>();
            barriers = GameObject.Find("Barriers").GetComponent<Tilemap>();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (entity.effect != Entity.Effect.Stunned && entity.effect != Entity.Effect.Skunked)
        {
            //Tile-based movement
            //Debug.Log(entity.isAttacking);
            if (!entity.isAttacking && !entity.isGrappled)
                tileMovement();
            //Debug.Log(entity.direction);

            var collider = Physics2D.OverlapCircle(new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, 0), .25f);
            //Debug.Log(!(NextTile(barriers)) && collider != null && collider.tag == "Walkable");
            if (!(NextTile(barriers)) && collider != null && collider.isTrigger//(collider.tag == "Walkable" || collider.tag == "Collectable")
                || !(NextTile(barriers)) && collider == null)
            {
                gameObject.transform.Translate(new Vector3(xOffset, yOffset, 0));
            }


            //end of movement

            //attack using ability
            int x = entity.changeXOffset();
            int y = entity.changeYOffset();

            if (entity.attack.GetComponent<Damage>().special == "Rapid Shot" && !Input.GetKey(KeyCode.LeftControl) || entity.attack.GetComponent<Damage>().special != "Rapid Shot")
            {
                entity.isAttacking = false;
            }

            if (Input.GetKeyDown("left ctrl"))
            {
                //Debug.Log("Attack");
                entity.isAttacking = true;
                Instantiate(entity.attack, new Vector3(transform.position.x + x, transform.position.y + y, -1), Quaternion.identity);
            }
            //end of attack using ability

            //attack using weapon
            if (Input.GetKeyDown("left alt"))
            {
                //Debug.Log("Attack");
                entity.isAttacking = true;
                Instantiate(entity.weapon, new Vector3(transform.position.x + x, transform.position.y + y, -1), Quaternion.identity);
            }
            //end of attack using weapon
        }
    }

    private void tileMovement()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            entity.changeDirection("Left");
            xOffset = -1;
            yOffset = 0;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            entity.changeDirection("Down");
            xOffset = 0;
            yOffset = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            entity.changeDirection("Right");
            xOffset = 1;
            yOffset = 0;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            entity.changeDirection("Up");
            xOffset = 0;
            yOffset = 1;
        }
        else
        {
            xOffset = 0;
            yOffset = 0;
        }

        if (entity.effect == Entity.Effect.Disoriented)
        {
            xOffset *= -1;
            yOffset *= -1;
        }
    }

    void FixedUpdate()
    {
        //Movement
        //Smooth movement
        /*float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (vertical > 0)
        {
            entity.changeDirection("Up");
            yOffset = 1;
            xOffset = 0;
        }
        else if (vertical < 0)
        {
            entity.changeDirection("Down");
            yOffset = -1;
            xOffset = 0;
        }
        
        if (horizontal > 0)
        {
            entity.changeDirection("Right");
            xOffset = 1;
            yOffset = 0;
        }
        else if (horizontal < 0)
        {
            entity.changeDirection("Left");
            xOffset = -1;
            yOffset = 0;
        }

        gameObject.transform.Translate(new Vector3(horizontal, vertical, 0) * speed);*/


    }

    public bool NextTile(Tilemap tiles)
    {
        bool tileExist = true;

        Vector3 next = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, 0);
        Vector3Int tilesMapTile = tiles.WorldToCell(next);

        //is next tile a wall? if no, return coordinate, if yes, return current position
        if (barriers.GetTile(tilesMapTile) == null)
        {
            //Debug.Log("No wall\n");
            tileExist = false;
        }

        return tileExist;
    }
}
