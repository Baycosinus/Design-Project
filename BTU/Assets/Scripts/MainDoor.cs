using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDoor : MonoBehaviour
{

    public GameObject leftDoor,rightDoor;
    public bool open;
    // Start is called before the first frame update
    void Start()
    {
        open = false;
        leftDoor = transform.Find("Left").gameObject;
        rightDoor = transform.Find("Right").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(open)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        GetComponent<Animator>().SetBool("MainDoorOpen",true);
    }
}
