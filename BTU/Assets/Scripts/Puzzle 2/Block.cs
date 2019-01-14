using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public bool isMissing;
    public bool inOriginalPosition;
    public int[] gridCoordinates;
    public Vector3 coordinates;
    public Vector3 startingCoords;
    private int gridSize;
    public CanvasContainer container;

    public enum MoveDirection { Up, Down, Left, Right, None};
    public MoveDirection moveDirection;

    public event System.Action<Block> OnBlockPressed;
    public event System.Action OnFinishedMoving;
    void Start()
    {
        startingCoords = transform.position;
    }
    void Update()
    {
        inOriginalPosition = (transform.position == startingCoords);
        //Debug.Log(gameObject.name + ":" + inOriginalPosition);
    }
    void Initialize()
    {
        
        container = transform.parent.gameObject.GetComponent<CanvasContainer>();
        gridSize = CanvasContainer.size;
    }
	public bool CanMove()
    {
        Initialize();
        MoveDirection result = new MoveDirection();
        if (container != null)
        {
            if(gridCoordinates[0] > 0 && container.canvasGrid[gridCoordinates[0]-1, gridCoordinates[1]].isMissing) 
            { result = MoveDirection.Left; }
            else if(gridCoordinates[0] < gridSize-1 &&container.canvasGrid[gridCoordinates[0]+1, gridCoordinates[1]].isMissing) 
            { result = MoveDirection.Right; }
            else if(gridCoordinates[1] > 0 && container.canvasGrid[gridCoordinates[0], gridCoordinates[1] - 1].isMissing) 
            { result = MoveDirection.Down;  }
            else if (gridCoordinates[1] < gridSize-1 && container.canvasGrid[gridCoordinates[0], gridCoordinates[1] + 1].isMissing) 
            { result = MoveDirection.Up; }
            else { result = MoveDirection.None; }
        }
        else
        {
            //Debug.Log("Container is null");
            result = MoveDirection.None;
        }
        //Debug.Log(name + "can move to " + result.ToString());
        moveDirection = result;
        return !(result == MoveDirection.None);
    }
    
    public void Trigger()
    {
        if(OnBlockPressed != null)
        {
            OnBlockPressed(this);
        }
    }
   
    public void MoveToPosition(Vector3 target, float duration)
    {
        StartCoroutine(AnimateMove(target, duration));
    }
    IEnumerator AnimateMove(Vector3 target, float duration)
    {
        Vector3 initialPosition = transform.position;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(initialPosition, target, percent);
            yield return null;
        }
        if(OnFinishedMoving != null)
        {
            OnFinishedMoving();
        }
    }
}
