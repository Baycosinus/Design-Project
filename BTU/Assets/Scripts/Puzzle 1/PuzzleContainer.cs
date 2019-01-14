using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleContainer : MonoBehaviour {

    public enum PuzzleStatus { Empty, Ruby, Emerald, Both };
    public PuzzleStatus pStatus;
    public List<string> containerRequirements;
    public bool interactable = false;
    public bool isComplete = false;
    public GameObject leftEye, rightEye;
    private FPSController player;
#pragma warning disable IDE0044 // Add readonly modifier
    private GameController controller;


    public Material leftEyeMat, rightEyeMat;
    public Color left, right;

    private bool isInitialized = false;
	void Start ()
    {
        pStatus = PuzzleStatus.Empty;
        Initialize();
	}
	
	void Update ()
    {
        if (isInitialized && !isComplete)
        {
            CheckInteraction();
            CheckPuzzleStatus();
        }
    }
    private void Initialize()
    {
        leftEye = gameObject.transform.Find("LeftEye").gameObject;
        rightEye = gameObject.transform.Find("RightEye").gameObject;
        leftEyeMat = leftEye.GetComponent<Renderer>().material;
        rightEyeMat = rightEye.GetComponent<Renderer>().material;
        left = leftEyeMat.color;
        right = rightEyeMat.color;
        player = GameObject.Find("Player").GetComponent<FPSController>();
        containerRequirements.Clear();
        containerRequirements.Add("Ruby Gem");
        containerRequirements.Add("Emerald Gem");
        isInitialized = true;
    }
    private void CheckPuzzleStatus()
    {
        
        switch (pStatus)
        {
            case PuzzleStatus.Empty:
                leftEyeMat.color = new Color(left.r, left.g, left.b, 0.0f);
                rightEyeMat.color = new Color(right.r, right.g, right.b, 0.0f);
                isComplete = false;
                break;
            case PuzzleStatus.Ruby:
                rightEyeMat.color = new Color(right.r, right.g, right.b, 1.0f);
                break;
            case PuzzleStatus.Emerald:
                leftEyeMat.color = new Color(left.r, left.g, left.b, 1.0f);
                isComplete = false;
                break;
            case PuzzleStatus.Both:
                leftEyeMat.color = new Color(left.r, left.g, left.b, 1.0f);
                rightEyeMat.color = new Color(right.r, right.g, right.b, 1.0f);
                isComplete = true;
                break;
        }
    }
    private void CheckInteraction()
    {
        if(interactable && player.isInteracting && player.inventory.Count != 0)
        {
            if (containerRequirements.Contains(player.inventory[0]))
            {
                containerRequirements.Remove(player.inventory[0]);
                player.inventory.Remove(player.inventory[0]);
                CheckState();
            }
        }
    }

    private void CheckState()
    {
        if(containerRequirements.Count == 2)
        {
            pStatus = PuzzleStatus.Empty;
        }
        else if(containerRequirements.Count == 1)
        {
            switch (containerRequirements[0])
            {
                case "Ruby Gem":
                    pStatus = PuzzleStatus.Ruby;
                    break;
                case "Emerald Gem":
                    pStatus = PuzzleStatus.Emerald;
                    break;
            }
        }
        else
        {
            pStatus = PuzzleStatus.Both;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            interactable = true;
            //Debug.Log("You can now interact with " + gameObject.name);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player") { interactable = false; }
    }
}
