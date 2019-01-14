using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour {

    public string objectId; //Red-Green
    public bool interactable = false;
    private FPSController player;
	void Start () {
        player = GameObject.Find("Player").GetComponent<FPSController>();
	}
	
	void Update ()
    {
        CheckInteraction();
	}

    void CheckInteraction()
    {
        if(interactable && player.isInteracting)
        {
            PickUp();
        }
    }

    void PickUp()
    {
        player.inventory.Add(gameObject.name);
        DestroyImmediate(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            interactable = true;
            //Debug.Log("You can now pick up " + gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player") { interactable = false; }
    }
}
