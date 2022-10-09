using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Damage : MonoBehaviour
{
    public enum Direction { Right, Up, Left, Down, UL, UR, DL, DR };
    public enum Type { Melee, Ranged };
    public enum Effect { None, Poison, Disoriented, Stunned, Skunked };
    public Entities attacker;

    public Direction direction;// = Direction.Down;
    public Type type;
    public Effect effect;

    public GameObject instanceCreated;
    public int instanceAmount;
    public float instanceX;
    public float instanceY;
    public float[] instancesXs;
    public float[] instancesYs;

    public string special;
    public bool needAttackerDirection;
    public bool trigger;
    public bool ability;

    public int effectDamage;
    public float effectDuration;
    public float effectDamageCooldown;

    public int damage;// = 5;
    public float dCooldown;
    public float dCount;

    public float cooldown;// = 1f;
    public float count;// = 1f;

    public float damageBoost;
    public float critChance;
    public float critDamage;

    public float speed;// = .2f;
    public float xOffset;
    public float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        //changeDirection(attacker.GetComponent<Entities>().direction.ToString());
        //Debug.Log(direction);
        //Debug.Log(attacker.GetComponent<Entities>().direction);
        findAttacker();
        

        damageBoost += attacker.damageBonus + 1;
        critChance = attacker.crit;
        damage = (int)(damage * damageBoost);
        //Debug.Log(damage);
        if (critChance > 0)
        {
            critical();
            Debug.Log(damage+"\n"+critDamage);
        }

        if (ability)
        {
            instanceCreated = attacker.weapon;
        }

        if (special == "Lunge")
        {
            Lunge();
        }

        if (special == "Tri-Shot")
        {
            TriShot();

           // instancesXs = xs.ToArray();
            //instancesYs = ys.ToArray();
        }

        else
        {
            for (int i = 0; i < instanceAmount; i++)
            {
                //checks if there are any specific coordinates to put things
                if (instancesXs.Length > 0)
                {
                    int e = i;
                    //checks if i is too big to be in the list of xs
                    //if so, subtracts by length until it is within range and makes offset whatever that number is
                    while (e >= instancesXs.Length)
                    {
                        e -= instancesXs.Length;
                    }

                    instanceX = instancesXs[e];
                }

                if (instancesYs.Length > 0)
                {
                    int e = i;
                    //checks if i is too big to be in the list of ys
                    //if so, subtracts by length until it is within range and makes offset whatever that number is
                    while (e >= instancesYs.Length)
                    {
                        e -= instancesYs.Length;
                    }

                    instanceY = instancesYs[e];
                }

                if (trigger)
                {
                    instanceX = xOffset * -1;
                    instanceY = yOffset * -1;
                }

                if (special == "Tongue")
                {
                    instanceX = xOffset * (i + 1);
                    instanceY = yOffset * (i + 1);
                }

                //Debug.Log("X: " + instanceX + "\nY: " + instanceY);
                var attack = Instantiate(instanceCreated, new Vector3(transform.position.x + instanceX, transform.position.y + instanceY, -1), Quaternion.identity);
                if (special == "Lunge")
                {
                    attack.GetComponent<Damage>().damageBoost += (float).5;
                }
                else if (special == "Rapid Shot")
                {
                    attack.GetComponent<Damage>().damageBoost -= (float).25;
                }
            }

            if (trigger)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void TriShot()
    {
        float xs;// = new List<float>();
        float ys;// = new List<float>();

        int originalDirection = (int)attacker.direction;

        for (int i = 1; i <= instanceAmount; i++)
        {
            int ind = i;

            while (ind > 4)
            {
                ind -= 4;
            }

            int off = (ind % 2 == 1) ?
                ind / 2 * -1 :
                ind / 2;

            int newDirection = (int)attacker.direction + off;
            //attacker.changeDirection(newDirection);
            //Debug.Log(attacker.direction);


            while (newDirection > 3)
            {
                newDirection -= 4;
            }
            while (newDirection < 0)
            {
                newDirection += 4;
            }
            changeDirection(newDirection);
            //Debug.Log(direction);

            xs = changeXOffset();
            ys = changeYOffset();

            //Debug.Log("X: " + transform.position.x + "\nY: " + transform.position.y);

            var shot = Instantiate(instanceCreated, new Vector3(transform.position.x + xs, transform.position.y + ys, -1), Quaternion.identity);
            shot.GetComponent<Damage>().changeDirection(newDirection);
            shot.GetComponent<Damage>().special = "Keep Direction";
            Debug.Log(shot.GetComponent<Damage>().direction);
            changeDirection(originalDirection);
        }
    }

    private void Lunge()
    {
        attacker.transform.Translate(new Vector3(xOffset, yOffset, 0));
        instanceX = xOffset;
        instanceY = yOffset;
        Debug.Log("X: " + instanceX + "\nY: " + instanceY);
    }

    private void RapidShot()
    {
        attacker.isAttacking = true;
        if (count <= cooldown / 2)
        {
            instanceX = xOffset;
            instanceY = yOffset;
            //Debug.Log("X: " + instanceX + "\nY: " + instanceY);
            var attack = Instantiate(instanceCreated, new Vector3(transform.position.x + instanceX, transform.position.y + instanceY, -1), Quaternion.identity);
            attack.GetComponent<Damage>().damageBoost -= (float).25;
            count = cooldown;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (special == "Rapid Shot")
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                RapidShot();
            }
        }
        //Debug.Log("Scream");
        gameObject.transform.Translate(new Vector3(xOffset, yOffset, -1) * speed);
        //Debug.Log(xOffset + "\n" + yOffset);
        if (count <= 0)
        {
            Destroy(this.gameObject);
        }
        count -= Time.deltaTime;
        dCount -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision);
        //findAttacker();
        if (collision.GetComponent<Entities>() != null && collision.GetComponent<Entities>() != attacker && dCount <= 0)
        {
            Entities entity = collision.GetComponent<Entities>();
            //Debug.Log(effect);
            if (entity.isFlying)
            {
                //Debug.Log("Uh oh");
                if (type != Type.Melee)
                {
                    entity.Damage(damage);
                }
            }
            else
            {
                //Debug.Log("hmmm");
                entity.Damage(damage);
            }

            if (effect != Effect.None)
            {
                if (effectDamage > 0)
                {
                    //Debug.Log("Poisn");
                    entity.inflictEffect((int)effect, effectDamage, effectDuration, effectDamageCooldown);
                }

                else
                {
                    entity.inflictEffect((int)effect, 0, effectDuration, effectDamageCooldown);
                }
            }

            dCount = dCooldown;
            //Debug.Log(entity.health);
            if (special != "Trigger")
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void findAttacker()
    {
        Entities attack = null;
        Entities[] entities = FindObjectsOfType<Entities>();
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Entities potentialAttacker in entities)
        {
            //Debug.Log(potentialAttacker);
            Transform potentialPosition = potentialAttacker.GetComponent<Transform>();
            Vector3 directionToTarget = potentialPosition.position - currentPosition;
            float dSqrToAttacker = directionToTarget.sqrMagnitude;
            if (potentialAttacker.isAttacking)
            {
                if (dSqrToAttacker < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToAttacker;
                    attack = potentialAttacker;
                    attacker = attack;
                    //Debug.Log(attack.GetComponent<Transform>());
                    if (special != "Keep Direction")
                        changeDirection(attacker.direction.ToString());
                    //Debug.Log(attack);
                }
            }
        }
    }

    public void critical()
    {
        float critMod = 10;
        float crit = critChance;

        while (crit/2 > 10)
        {
            crit -= 10;
            critMod += 10;
        }

        float chance = Random.Range(0, critMod);

        if (chance < critChance / 2)
        {
            Debug.Log("Crit!");
            critDamage = (critChance + 1) * damage + 1;
            damage += (int)critDamage;
        }
    }

    public void changeDirection(int dir)
    {
        direction = (Direction)dir;
        switch(dir)
        {
            case 0:
                yOffset = 0;
                xOffset = 1;
                break;
            case 1:
                yOffset = 1;
                xOffset = 0;
                break;
            case 2:
                yOffset = 0;
                xOffset = -1;
                break;
            case 3:
                yOffset = -1;
                xOffset = 0;
                break;
            case 4:
                yOffset = 1;
                xOffset = -1;
                break;
            case 5:
                yOffset = 1;
                xOffset = 1;
                break;
            case 6:
                yOffset = -1;
                xOffset = 1;
                break;
            case 7:
                yOffset = 1;
                xOffset = 1;
                break;
        }
    }

    private void changeDirection(string dir)
    {
        if (dir.Equals("Up"))
        {
            direction = Direction.Up;
            yOffset = 1;
            xOffset = 0;
        }
        else if (dir.Equals("Down"))
        {
            direction = Direction.Down;
            yOffset = -1;
            xOffset = 0;
        }
        else if (dir.Equals("Right"))
        {
            direction = Direction.Right;
            yOffset = 0;
            xOffset = 1;
        }
        else if (dir.Equals("Left"))
        {
            direction = Direction.Left;
            yOffset = 0;
            xOffset = -1;
        }
        else if (dir.Equals("UL"))
        {
            direction = Direction.UL;
            yOffset = 1;
            xOffset = -1;
        }
        else if (dir.Equals("UR"))
        {
            direction = Direction.UR;
            yOffset = 1;
            xOffset = 1;
        }
        else if (dir.Equals("DL"))
        {
            direction = Direction.DL;
            yOffset = -1;
            xOffset = -1;
        }
        else if (dir.Equals("DR"))
        {
            direction = Direction.DR;
            yOffset = -1;
            xOffset = 1;
        }
    }

    public int changeXOffset()
    {
        int x = 0;

        if (direction == Direction.Left || direction == Direction.DL || direction == Direction.UL)
        {
            x = -1;
        }

        else if (direction == Direction.Right || direction == Direction.DR || direction == Direction.UR)
        {
            x = 1;
        }

        return x;
    }

    public int changeYOffset()
    {
        int y = 0;

        if (direction == Direction.Down || direction == Direction.DL || direction == Direction.DR)
        {
            y = -1;
        }

        else if (direction == Direction.Up || direction == Direction.UL || direction == Direction.UR)
        {
            y = 1;
        }

        return y;
    }
}