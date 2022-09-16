using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmfulTerrain : MonoBehaviour
{
    public int damage = 2;
    public float delay = 2f;
    public float countdown = 2f;
    public bool trigger = false;
    public Entities entity;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Walkable";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (trigger)
        {
            //Debug.Log(countdown);

            if (countdown <= 0)
            {
                entity.Damage(damage);

                countdown = delay;
            }

            countdown -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Damage");

        if (collision.GetComponent<Entities>() != null)
        {
            entity = collision.GetComponent<Entities>();

            if (entity.canFly)
            {
                entity.isFlying = true;
            }

            else
            {
                entity.Damage(damage);
                trigger = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        entity.isFlying = false;
        trigger = false;
    }
}
