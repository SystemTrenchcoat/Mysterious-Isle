using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public enum Type { Weapon, Class, Ability, Stats }
    public enum Stat { Damage, Defense, Crit, Health }
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
                player.GetComponent<Entity>().attack = prefab;
            }

            else if (colType == Type.Weapon)
            {
                player.GetComponent<Entity>().weapon = prefab;

                if (player.GetComponent<Entity>().attack == player.GetComponent<CharacterCollectables>().Abilities[0])
                {
                    player.GetComponent<Entity>().attack = prefab;
                }
            }
            
            else if (colType == Type.Class)
            {
                Instantiate(prefab);
                Destroy(player);

            }

            else if (colType == Type.Stats)
            {
                Debug.Log(stat.ToString());
                if (stat == Stat.Damage)
                {
                    player.GetComponent<Entity>().damageBonus += amount;
                }

                else if (stat == Stat.Defense)
                {
                    player.GetComponent<Entity>().defenseBonus += amount;
                }

                else if (stat == Stat.Crit)
                {
                    player.GetComponent<Entity>().crit += amount;
                }

                else if (stat == Stat.Health)
                {
                    player.GetComponent<Entity>().health += (int)amount;
                    player.GetComponent<Entity>().hp += (int)amount;
                }
            }
            
            Destroy(this.gameObject);
        }
    }
}
