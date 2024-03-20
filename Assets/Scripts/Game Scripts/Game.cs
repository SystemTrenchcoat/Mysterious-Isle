using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField]
    protected ObjectInstantiator enemyGenerator;
    [SerializeField]
    protected SimpleRandomWalkGenerator levelGenerator;

    //blanket parameters
    [SerializeField]
    protected List<ObjectInstatiatorInfo> enemyParameters = new List<ObjectInstatiatorInfo>();
    [SerializeField]
    protected List<SimpleRandomWalkSO> levelParameters = new List<SimpleRandomWalkSO>();

    //Miniboss rooms
    [SerializeField]
    protected List<SimpleRandomWalkSO> bossRoomParameters = new List<SimpleRandomWalkSO>();

    //Forest Enemies
    [SerializeField]
    protected List<ObjectInstatiatorInfo> forestEnemies = new List<ObjectInstatiatorInfo>();
    [SerializeField]
    protected List<ObjectInstatiatorInfo> forestBosses = new List<ObjectInstatiatorInfo>();

    //Valley Enemies
    [SerializeField]
    protected List<ObjectInstatiatorInfo> valleyEnemies = new List<ObjectInstatiatorInfo>();
    [SerializeField]
    protected List<ObjectInstatiatorInfo> valleyBosses = new List<ObjectInstatiatorInfo>();

    //Mountain Enemies
    [SerializeField]
    protected List<ObjectInstatiatorInfo> mountainEnemies = new List<ObjectInstatiatorInfo>();
    [SerializeField]
    protected List<ObjectInstatiatorInfo> mountainBosses = new List<ObjectInstatiatorInfo>();

    //Cave Enemies
    [SerializeField]
    protected List<ObjectInstatiatorInfo> caveEnemies = new List<ObjectInstatiatorInfo>();
    //[SerializeField]
    //protected List<ObjectInstatiatorInfo> caveBosses = new List<ObjectInstatiatorInfo>();

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

        if (GameObject.FindGameObjectWithTag("Enemy") == null && !GameObject.FindGameObjectWithTag("Goal"))
        {
            player.GetComponent<Controller>().roomNum += 1;
            int levelNumber = player.GetComponent<Controller>().roomNum;

            //change generator parameters

            //level parameter change
            if (levelNumber % 10 == 0)
                levelGenerator.ChangeParameters(bossRoomParameters[Random.Range(0, bossRoomParameters.Count)]);
            else
                levelGenerator.ChangeParameters(levelParameters[Random.Range(0, levelParameters.Count)]);

            //forest
            if (levelNumber > 2 && levelNumber < 10)
            {
                enemyGenerator.ChangeParameters(forestEnemies[Random.Range(0, forestEnemies.Count)]);
            }
            else if (levelNumber == 10)
            {
                enemyGenerator.ChangeParameters(forestBosses[Random.Range(0, forestBosses.Count)]);
            }

            //valley
            else if (levelNumber > 10 && levelNumber < 20)
            {
                enemyGenerator.ChangeParameters(valleyEnemies[Random.Range(0, valleyEnemies.Count)]);
            }
            else if (levelNumber == 20)
            {
                enemyGenerator.ChangeParameters(valleyBosses[Random.Range(0, valleyBosses.Count)]);
            }

            //mountain
            else if (levelNumber > 20 && levelNumber < 32)
            {
                enemyGenerator.ChangeParameters(mountainEnemies[Random.Range(0, mountainEnemies.Count)]);
            }
            else if (levelNumber == 32)
            {
                enemyGenerator.ChangeParameters(mountainBosses[Random.Range(0, mountainBosses.Count)]);
            }

            //cave
            else if (levelNumber > 33 && levelNumber < 40)
            {
                enemyGenerator.ChangeParameters(caveEnemies[Random.Range(0, caveEnemies.Count)]);
            }
            //else if (levelNumber == 40)
            //{
            //    enemyGenerator.ChangeParameters(caveBosses[Random.Range(0, caveBosses.Count)]);
            //}

            //end generator change

            //generate dungeon
            levelGenerator.GenerateDungeon();

            //set player position
            player.gameObject.transform.position = Vector3.zero;

            //generate enemy
            enemyGenerator.GenerateDungeon();
        }
    }
}
