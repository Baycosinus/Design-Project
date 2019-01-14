using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject targetPuzzleObject;
    public bool isLocked;
    public bool isOpen;
    
    // Start is called before the first frame update
    void Start()
    {
        isLocked = true;
        switch(gameObject.name)
        {
            case "OfficeDoor":
                targetPuzzleObject = GameObject.Find("OwlStatue");
            break;
            case "DinnerDoor":
                targetPuzzleObject = GameObject.Find("CanvasContainer_Final");
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(targetPuzzleObject != null)
        {
            if (targetPuzzleObject.GetComponent<PuzzleContainer>() != null)
            {
                isLocked = !targetPuzzleObject.GetComponent<PuzzleContainer>().isComplete;
            }
            else if (targetPuzzleObject.GetComponent<CanvasContainer>() != null)
            {
                isLocked = !targetPuzzleObject.GetComponent<CanvasContainer>().isComplete;
            }
        }
    }
    public void Trigger()
    {
        if (!isLocked && !isOpen)
        {
            Open();            
        }
        else
        {
            Debug.Log("The door is locked");
        }
    }
    public void Open()
    {
        
        Debug.Log("Open sesame!");
        transform.parent.gameObject.GetComponent<Animator>().SetBool("Open",true);
        isOpen = true;

    }
    
}
