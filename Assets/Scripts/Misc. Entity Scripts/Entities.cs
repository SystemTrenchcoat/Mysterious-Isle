using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour
{
    public enum Direction { Right, Up, Left, Down, UL, UR, DL, DR };
    public enum Effect { None, Poison, Disoriented, Stunned, Skunked };

    public Direction direction = Direction.Down;
    public Direction defendDirection = Direction.Up;

    public Effect effect;
    public int effectDamage;
    public float effectDamageCooldown;
    public float effectDamageCount;
    public float effectCooldown;
    public int effectsCount; //for things like skunked

    public bool canFly;
    public bool isFlying = false;
    public bool canBurrow = false;
    public bool isBurrowing = false;
    public bool isAttacking = false;
    public bool isDefending = false;

    public GameObject attack;
    public GameObject weapon;
    public GameObject alt;

    public float damageBonus;
    public float defenseBonus;
    public float crit;

    public int health = 20;
    public int hp = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //Debug.Log(health);
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }

        if (effect != Effect.None)
        {
            if (effectCooldown > 0)
            {
                effectCooldown -= Time.deltaTime;
                if (effectDamageCooldown > 0)
                {
                    if (effectDamageCount <= 0)
                    {
                        health -= effectDamage;
                        effectDamageCount = effectDamageCooldown;
                    }
                    effectDamageCount -= Time.deltaTime;
                }
            }

            else if (effect == Effect.Skunked && effectCooldown <= 0)
            {
                effect = Effect.Disoriented;
                effectCooldown = 3;
            }

            else
            {
                //Debug.Log(effect);
                effect = Effect.None;
                effectCooldown = 0;
                effectDamage = 0;
                effectDamageCooldown = 0;
                effectDamageCount = 0;
            }
        }
    }

    public void changeDirection(string dir)
    {
        if (dir.Equals("Up"))
        {
            direction = Direction.Up;
            defendDirection = Direction.Down;
        }
        else if (dir.Equals("Down"))
        {
            direction = Direction.Down;
            defendDirection = Direction.Up;
        }
        else if (dir.Equals("Right"))
        {
            direction = Direction.Right;
            defendDirection = Direction.Left;
        }
        else if (dir.Equals("Left"))
        {
            direction = Direction.Left;
            defendDirection = Direction.Right;
        }
        else if (dir.Equals("UL"))
        {
            direction = Direction.UL;
            defendDirection = Direction.DR;
        }
        else if (dir.Equals("UR"))
        {
            direction = Direction.UR;
            defendDirection = Direction.DL;
        }
        else if (dir.Equals("DL"))
        {
            direction = Direction.DL;
            defendDirection = Direction.UR;
        }
        else if (dir.Equals("DR"))
        {
            direction = Direction.DR;
            defendDirection = Direction.UL;
        }
    }

    public void changeDirection(int dir)
    {
        direction = (Direction)dir;
    }

    public int defendDirectionOffsetX()
    {
        int off = 0;

        if (defendDirection == Direction.Left)
        {
            off = -1;
        }

        else if (defendDirection == Direction.Right)
        {
            off = 1;
        }

        return off;
    }

    public int defendDirectionOffsetY()
    {
        int off = 0;

        if (defendDirection == Direction.Down)
        {
            off = -1;
        }

        else if (defendDirection == Direction.Up)
        {
            off = 1;
        }

        return off;
    }
    
    public void Damage(int damage)
    {
        if (!isDefending)
        {
            //Debug.Log(damage);
            damage -= (int)(damage * defenseBonus);
            health -= damage;
        }
    }

    public int changeXOffset()
    {
        int x = 0;

        if(direction == Direction.Left || direction == Direction.DL || direction == Direction.UL)
        {
            x = -1;
        }

        else if(direction == Direction.Right || direction == Direction.DR || direction == Direction.UR)
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

    public void inflictEffect (int eff, int dmg, float cooldown, float dmgCooldown)
    {
        effect = (Effect)eff;
        effectCooldown = cooldown;
        effectDamage = dmg;
        effectDamageCooldown = dmgCooldown;
        effectDamageCount = 0;
    }
}
