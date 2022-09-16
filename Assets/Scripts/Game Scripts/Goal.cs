using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    bool active = false;
    SpriteRenderer sR;
    // Start is called before the first frame update
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    //Every frame check if there are anymore enemies, if not, change tile color to gold
    void Update()
    {
        //Debug.Log(GameObject.FindGameObjectsWithTag("Enemy").Length);
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            active = true;
            sR.color = new Color(255, 200, 0, 255);
        }
        //Debug.Log(active);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Meep");
        if (active)
        {
            //Debug.Log("Load next level");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }
}
