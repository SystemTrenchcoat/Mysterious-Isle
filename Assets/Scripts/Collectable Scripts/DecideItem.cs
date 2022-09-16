using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DecideItem : MonoBehaviour
{
    public enum Type { Weapon, Class, Ability, Stats }
    public Type type;

    public Collectable item;
    public GameObject player;

    public GameObject[] items;
    public int index;
    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        item = this.GetComponent<Collectable>();
        type = (Type)(int)item.colType;
        if (type == Type.Weapon)
        {
            items = player.GetComponent<CharacterCollectables>().Weapons;
        }
        else if (type == Type.Ability)
        {
            items = player.GetComponent<CharacterCollectables>().Abilities;
        }

        if (type != Type.Stats)
        {
            item.prefab = items[index];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
