using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMe : MonoBehaviour
{
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
        Transform follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        this.transform.position = new Vector3(follow.position.x, follow.position.y, this.transform.position.z);
    }
}
