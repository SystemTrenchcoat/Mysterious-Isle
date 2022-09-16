using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null && player != null)
        {
            Instantiate(player);
        }
    }
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        DontDestroyOnLoad(player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(GameObject.FindGameObjectWithTag("Player") == null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            DontDestroyOnLoad(player.transform);
        }
    }
}
