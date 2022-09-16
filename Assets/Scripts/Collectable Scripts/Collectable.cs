using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public enum Type { Weapon, Class, Ability, Stats }
    public enum Stat { Damage, Defense, Crit }
    public Type colType;
    public Stat stat;
    public float amount;
    public GameObject prefab;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            if (colType == Type.Ability)
            {
                player.GetComponent<Entities>().attack = prefab;
            }

            else if (colType == Type.Weapon)
            {
                player.GetComponent<Entities>().weapon = prefab;

                if (player.GetComponent<Entities>().attack == player.GetComponent<CharacterCollectables>().Abilities[0])
                {
                    player.GetComponent<Entities>().attack = prefab;
                }
            }
            
            else if (colType == Type.Class)
            {
                Instantiate(prefab);
                Destroy(player);

            }

            else if (colType == Type.Stats)
            {
                if (stat == Stat.Damage)
                {
                    player.GetComponent<Entities>().damageBonus += amount;
                }

                else if (stat == Stat.Defense)
                {
                    player.GetComponent<Entities>().defenseBonus += amount;
                }

                else if (stat == Stat.Crit)
                {
                    player.GetComponent<Entities>().crit += amount;
                }
            }
            
            Destroy(this.gameObject);
        }
    }
}
