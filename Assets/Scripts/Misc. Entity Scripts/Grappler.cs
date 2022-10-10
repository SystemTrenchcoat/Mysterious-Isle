using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    //public GameObject grappler;
    public Entities target;

    public bool isGrappling;
    public int hp;
    public int health;

    public int damage;
    public float damageCooldown;
    public float damageCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (health <= 0)
        {
            isGrappling = false;
            target.isGrappled = false;
            Destroy(gameObject);
        }

        if (damageCount <= 0)
        {
            target.Damage(damage);
            damageCount = damageCooldown;
        }

        damageCount -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (target == null)
        {
            isGrappling = true;
            if (collision.GetComponent<Entities>() != null && !collision.GetComponent<Entities>().isGrappled)
            {
                target = collision.GetComponent<Entities>();
                target.isGrappled = true;
            }
        }

        //if (collision != null && collision.GetComponent<Damage>() != null)
        //{
        //    Damage(collision.GetComponent<Damage>().damage);
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isGrappling)
        {
            isGrappling = false;
            if (collision.GetComponent<Entities>() != null)
            {
                target.isGrappled = false;
                target = null;
            }
        }
    }

    public void Damage(int damage)
    {
        health -= damage;
    }
}
