using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasContainer : MonoBehaviour {

#pragma warning disable IDE0044 // Add readonly modifier


    public enum PuzzleState {Shuffling, InPlay, Completed};
    public PuzzleState puzzleState;

    public int shuffleMovesRemaining;
    Vector2Int prevShuffleOffset;
    public int shuffleLength = 20;
    public float defaultMoveDuration = .2f;
    public float shuffleMoveDuration = .1f;
    public static int size = 3;
    public bool isComplete;
    public bool isInteractive;

    public Texture2D image;
    private List<Texture2D> imageMatrixList = new List<Texture2D>();
    public Texture2D[,] imageMatrix;


    public Block missingBlock;
    public GameObject blockPrefab;

    public List<GameObject> canvasContainer;
    private List<Block> moveableBlocks;
    public Block[,] canvasGrid = new Block[size, size];
    public Block[,] solvedGrid = new Block[size, size];
    FPSController player;

    Queue<Block> inputs = new Queue<Block>();
    bool blockIsMoving;


	void Start ()
    {
        
        moveableBlocks = new List<Block>();
        player = GameObject.Find("Player").GetComponent<FPSController>();
        InitializeTexture();
        InitializeCanvas();
        PopRandom();
        puzzleState = PuzzleState.Shuffling;
        StartShuffling();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartShuffling();
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            Solve();
        }

        if(!isComplete && puzzleState == PuzzleState.InPlay){CheckIfSolved(); }
        if(puzzleState == PuzzleState.Completed)
        {
            GameObject.Find("DinnerDoor").GetComponent<Door>().isLocked = false;
        }
    }
    private void InitializeTexture()
    {
        //Image texture will be sliced into 3 different images and added to an array.
        int imageSize = Mathf.Min(image.width, image.height);
        int blockSize = imageSize / size;

        imageMatrix = new Texture2D[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Texture2D block = new Texture2D(blockSize, blockSize);
                block.SetPixels(image.GetPixels(x * blockSize, y * blockSize, blockSize, blockSize));
                block.Apply();
                imageMatrix[x, y] = block;
                imageMatrixList.Add(block);
                
            }
        }
    }
    private void InitializeCanvas()
    {

        EmptyCanvas();
        float xOffset =  .0055f;
        float yOffset =  .0055f;
        float zOffset =  .001f;

        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                GameObject obj = Instantiate(blockPrefab, transform);
                obj.name = "Block [" + x + "," + y + "]";
                
                if(x<1){ xOffset = .0055f;}
                else if (x > 1) { xOffset = -.0055f;}
                else { xOffset = 0;}
                if(y < 1){ yOffset = -.0055f;}
                else if(y > 1){ yOffset = .0055f;}
                else { yOffset = 0;}
                obj.transform.localPosition = new Vector3(xOffset, yOffset, zOffset);
                obj.GetComponent<Block>().coordinates = new Vector3Int(x, y, 0);
                obj.GetComponent<Block>().OnBlockPressed += PlayerMoveBlockInput;
                obj.GetComponent<Block>().OnFinishedMoving += OnBlockFinishedMoving;
                obj.GetComponent<Renderer>().material.mainTexture = imageMatrix[x, y];
                obj.GetComponent<Renderer>().material.SetFloat("_Metallic",1.0f);
                obj.GetComponent<Block>().gridCoordinates = new int[]{x,y};
                canvasContainer.Add(obj);
                canvasGrid[x, y] = obj.GetComponent<Block>();
                solvedGrid[x,y] = obj.GetComponent<Block>();
            }
        }
        solvedGrid = canvasGrid;
    }
    void EmptyCanvas()
    {
        foreach(Transform t in GameObject.Find("MissingBlockContainer").transform)
        {
            DestroyImmediate(t.gameObject);
        }
        foreach(Transform t in transform)
        {
            DestroyImmediate(t.gameObject);
        }
        foreach(GameObject g in canvasContainer)
        {
            DestroyImmediate(g);
        }
        canvasContainer.Clear();
    }
    private void PopRandom()
    {
        int x = UnityEngine.Random.Range(0, size);
        int y = UnityEngine.Random.Range(0, size);

         
        missingBlock = canvasGrid[x, y];
        missingBlock.container = GetComponent<CanvasContainer>();
        //emptyPosition = missingObject.transform.position;
        //missingObject.transform.SetParent(null);
        canvasGrid[x, y].isMissing = true;
        canvasGrid[x, y].gameObject.GetComponent<Renderer>().enabled = false;
        //canvasGrid[x, y] = null;

        GameObject clone = Instantiate(missingBlock.gameObject, GameObject.Find("MissingBlockContainer").transform);
        clone.name = canvasGrid[x, y].gameObject.name;
        clone.GetComponent<Renderer>().enabled = true;
        clone.transform.Rotate(new Vector3(90, 0, 0));
        clone.transform.localScale = new Vector3(.5f,.5f,0.05f);
        clone.transform.localPosition = new Vector3(0, 0.02f, -0.1f);
        clone.AddComponent<PickUpObject>();
        clone.AddComponent<SphereCollider>();
        clone.GetComponent<SphereCollider>().radius = 5f;
        clone.GetComponent<SphereCollider>().isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            isInteractive = true;
            //Debug.Log("You can now interact with " + gameObject.name);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            isInteractive = false;
        }
    }

    void PlayerMoveBlockInput(Block blockToMove)
    {
        //Debug.Log(blockToMove.name);
        if(!blockIsMoving)
        {
        inputs.Enqueue(blockToMove);
        }
        MakeNextBlockMove();
    }

    void MakeNextBlockMove()
    {
        while(inputs.Count > 0 && !blockIsMoving)
        {
            MoveBlock(inputs.Dequeue(), false);
        }
    }
    void MoveBlock(Block blockToMove, bool isShuffle)
    {
        if (blockToMove.CanMove())
        {
            canvasGrid[blockToMove.gridCoordinates[0], blockToMove.gridCoordinates[1]] = missingBlock;
            canvasGrid[missingBlock.gridCoordinates[0], missingBlock.gridCoordinates[1]] = blockToMove;

            int[] targetCoordinates = missingBlock.gridCoordinates;
            missingBlock.gridCoordinates = blockToMove.gridCoordinates;
            blockToMove.gridCoordinates = targetCoordinates;

            Vector3 targetPosition = missingBlock.transform.position;
            missingBlock.transform.position = blockToMove.transform.position;
            float duration;
            if (!isShuffle) { duration = shuffleMoveDuration; }
            else { duration = defaultMoveDuration; }
            blockToMove.MoveToPosition(targetPosition, duration);
            blockIsMoving = true;
        }
        foreach(Transform t in transform)
        {
            t.gameObject.GetComponent<Block>().CanMove();
        }
        CheckIfSolved();
    }
    void OnBlockFinishedMoving()
    {
        blockIsMoving = false;
        CheckIfSolved();
        Debug.Log(shuffleMovesRemaining);
        switch(puzzleState)
        {
            case PuzzleState.InPlay:
            MakeNextBlockMove();
            break;
            case PuzzleState.Shuffling:
                if (shuffleMovesRemaining > 0)
                {
                    Shuffle();
                    
                }
                else
                {
                    puzzleState = PuzzleState.InPlay;
                }
            break;
            case PuzzleState.Completed:
            break;
        }
    }

    void StartShuffling()
    {
        shuffleMovesRemaining = shuffleLength;
        Shuffle();
    }
    void Shuffle()
    {
        Vector2Int[] offsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        int randomIndex = UnityEngine.Random.Range(0, offsets.Length);

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            if (offset != prevShuffleOffset * -1)
            {
                Vector2Int moveBlockCoord = new Vector2Int(missingBlock.gridCoordinates[0] + offset.x, missingBlock.gridCoordinates[1] + offset.y);
                if (moveBlockCoord.x >= 0 && moveBlockCoord.x < size && moveBlockCoord.y >= 0 && moveBlockCoord.y < size)
                {
                    PlayerMoveBlockInput(canvasGrid[moveBlockCoord.x, moveBlockCoord.y]);
                    shuffleMovesRemaining--;
                    prevShuffleOffset = offset;
                    break;
                }
            }
        }
    }

    void Solve()
    {
        InitializeCanvas();
        PopRandom();
    }
    void CheckIfSolved()
    {
        foreach(GameObject g in canvasContainer)
        {
            if(g.GetComponent<Block>() != null)
            {
                string[] hash1 = g.name.Split('['); // hash1[1] = "1,2]"
                string[] hash2 = hash1[1].Split(','); // hash2[0] = "1"
                string[] hash3 = hash2[1].Split(']'); // hash3[0] = "2";

                //Debug.Log(hash2[0] + "/" + hash3[0]);
                int[] initialCoord = { Convert.ToInt32(hash2[0]) , Convert.ToInt32(hash3[0])};

                if(initialCoord[0] != g.GetComponent<Block>().gridCoordinates[0] || initialCoord[1] != g.GetComponent<Block>().gridCoordinates[1])
                {
                    //Debug.Log(initialCoord[0] + "," + initialCoord[1] + "!=" + g.GetComponent<Block>().gridCoordinates[0] + "," + g.GetComponent<Block>().gridCoordinates[1]);
                    return;
                }
            }
        }

        isComplete = true;
        puzzleState = PuzzleState.Completed;
    }
}
