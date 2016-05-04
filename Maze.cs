using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
    //Author @Connor Sidwell

    [System.Serializable]
    /*public class cell is used to create to designate each wall to North,East,South,West so that they can be distinguished so that 
     when the maze generates it knows which wall it is taking down to make sure atleast 1 wall on each cell is always standing*/
    public class Cell
    {
        public bool visited;
        public GameObject north;//1
        public GameObject east;//2
        public GameObject west;//3
        public GameObject south;//4
    }

    public GameObject wall;
    public float wallLength = 1.0f;
    public int xSize = 5;
    public int ySize = 5;
    private Vector3 initalPos;
    private GameObject WallHolder;
    private Cell[] cells;
    public int currentCell;
    private int totalCells;
    private int visitedCells;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;

	// Use this for initialization
	void Start () {
        //starts the create walls for loops to create the maze
        CreateWalls();
	}

    void CreateWalls(){
        //Creating an empty game object that holds all of the tempwall game objects so the hierarchy looks cleaner
        WallHolder = new GameObject();
        WallHolder.name = "Maze";

        //Setting the initial position of the wall generation using the X and Y Vectors 
        initalPos = new Vector3((-xSize / 2) + wallLength / 2, 1.0f, (-ySize / 2) + wallLength / 2);
        Vector3 myPos = initalPos;

        //Creating the tempWall GameObject
        GameObject tempWall;
        
        //**FOR X AXIS** This is the X Axis generation that loops and creates walls along the X axis until the wallLength variable is met
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++)
            {
				//create the vector for the wall generation on the x axis depending on wallLength variable
                myPos = new Vector3(initalPos.x + (j * wallLength) - wallLength / 2, 1.0f, initalPos.z + (i * wallLength) - wallLength / 2);
                //This instantiates the wall as a game object on the position that has been generated above
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                //This just places the wall into the WallHolder empty GameObject for tidyness
                tempWall.transform.parent = WallHolder.transform;
            }
        }

        //**FOR Y AXIS** This is the Y Axis generation that loops and creates walls along the Y axis until the wallLength variable is met
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
				//create the vector for the wall generation on the y axis depending on wallLength variable
                myPos = new Vector3(initalPos.x + (j * wallLength), 1.0f, initalPos.z + (i * wallLength) - wallLength);
                //This instantiates the wall as a game object on the position that has been generated above
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f,90.0f,0.0f)) as GameObject;
                //This just places the wall into the WallHolder empty GameObject for tidyness
                tempWall.transform.parent = WallHolder.transform;

            }
        }

        //Goto the create cells void
        CreateCells();
    }

    void CreateCells()
    {
        //Instantiating the LastCells List
        lastCells = new List<int>();
        lastCells.Clear();
        //Times the X Axis by Y Axis for the total number of cells
        totalCells = xSize * ySize;

        //Creating a Gameobject that holds all of the walls
        GameObject[] allWalls;
        //create an int that counts all of the walls within wallholder
        int children = WallHolder.transform.childCount;
        //Store all the children in the all walls object
        allWalls = new GameObject[children];
        //times the x axis size by y axis size to create the cells
        cells = new Cell[xSize*ySize];
        //set everything to 0 by default
        int eastWestProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        //Gets all the children of WallHolder
        for (int i = 0; i < children; i++) {
            allWalls[i] = WallHolder.transform.GetChild(i).gameObject;
        }

        //Assigns walls to the cells
        for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++)
        {
            if (termCount == xSize)
            {
                eastWestProcess++;
                termCount = 0;
            }

            cells[cellprocess] = new Cell ();
            cells[cellprocess].west = allWalls[eastWestProcess];
            cells[cellprocess].south = allWalls[childProcess + (xSize + 1) * ySize];

            eastWestProcess++;

            termCount++;
            childProcess++;
            cells[cellprocess].east = allWalls[eastWestProcess];
            cells[cellprocess].north = allWalls[(childProcess + (xSize + 1) * ySize)+xSize-1];
        }

        //goto createmaze function
        CreateMaze();
    }

    void CreateMaze()
    {
        while(visitedCells < totalCells)
        {
            if(startedBuilding)
            {
                GiveMeNeighbour();
                //If the currenNeighbour has not been visited and the currentCell is visited then break the wall between the neighbour and the current cell to create a space
                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    //sets the cell currentNeighbour that the statement goes too to visited
                    cells[currentNeighbour].visited = true;
                    //increments the visited cells so that the maze knows how many cells have already been visited
                    visitedCells++;
                    //Adds the current cell to the list of cells last visited incase it needs to go back
                    lastCells.Add(currentCell);
                    // sets the currentcell to the neighbour so it knows where it is
                    currentCell = currentNeighbour;
                    //resets the backingup int
                    if(lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                //start the generation by picking a random cell within the totalCells
                currentCell = Random.Range(0, totalCells);
                //set the cell choosen at random to visited
                cells[currentCell].visited = true;
                //increment the visitedcells by 1 because 1 has been visited
                visitedCells++;
                //set the startedbuilding bool to true
                startedBuilding = true;
            }

          
        }
        //this is a log that shows when the maze is finished for debugging
        Debug.Log("Finished");
    }

    
    void BreakWall()
    {
        /*This switch is what destroys a wall between neighbours this is choosen at random from a cell that is not ticked as already visited unless
         the program had to backup because there were no possible routes to take via neighbouring cells*/
        switch (wallToBreak)
        {
            case 1:
                Destroy(cells[currentCell].north);
                break;
            case 2:
                Destroy(cells[currentCell].east);
                break;
            case 3:
                Destroy(cells[currentCell].west);
                break;
            case 4:
                Destroy(cells[currentCell].south);
                break;

        }
    }

    void GiveMeNeighbour()
    {
        //set the ints for neighbours because there are only 4 possible neighbours each int has a total of 4
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;

        //all the possible checks that can be done for neighbouring cells
        check = ((currentCell+1)/xSize);
        check -= 1;
        check *= xSize;
        check += xSize;

        //**EAST CHECK* This IF checks if there is a neighbour to the east incase there is no cell next to the current cell on the East I.E. The last cell on the X Axis
        if(currentCell + 1 < totalCells && (currentCell + 1) != check)
        {
            if (cells[currentCell+1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 2;
                length++;
            }
        }

        //**WEST CHECK* This IF checks if there is a neighbour to the west incase there is no cell next to the current cell on the west I.E. The last cell on the X Axis
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 3;
                length++;
            }
        }

        //**NORTH CHECK* This IF checks if there is a neighbour to the north incase there is no cell next to the current cell on the north I.E. The last cell on the X Axis
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }

        //**SOUTH CHECK* This IF checks if there is a neighbour to the south incase there is no cell next to the current cell on the south I.E. The last cell on the X Axis
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }
        //If the length is not equal to 0 then make currentNeighbour the number choosen at random from the neighbouring cells
        if (length != 0)
        {
            int thechosenOne = Random.Range(0, length);
            currentNeighbour = neighbours[thechosenOne];
            wallToBreak = connectingWall[thechosenOne];
        }
        //If all neighbours have been visited on the current cell backup to previous cell and check for a unvisited neighbour
        else
        {
            if (backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
