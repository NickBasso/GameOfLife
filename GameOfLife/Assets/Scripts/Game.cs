using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // 1 unit is set to 16px
    private static int SCREEN_WIDTH = 64;   //64 units = 1024px
    private static int SCREEN_HEIGHT = 48;  //48 units = 768px
    private float timer = 0;

    public float speed = 0.1f;

    Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start()
    {
        PlaceCells();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= speed)
        {
            timer = 0f;

            CountNeighbors();
            PopulationControl();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    void PopulationControl()
    {
        for(int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for(int x = 0; x < SCREEN_WIDTH; x++)
            {
                /*
                    Conway's Game of Life rules:
                    1. live cell with 2 or 3 neighbors => lives
                    2. dead cell with 3 neighbors => lives
                    3. others => die or remain dead
                */

                if(grid[x,y].isAlive)
                {
                    if(grid[x,y].numNeighbors != 2 && grid[x,y].numNeighbors != 3)
                    {
                        grid[x,y].SetAlive(false);
                    }
                }
                else
                {
                    if(grid[x, y].numNeighbors == 3)
                    {
                        grid[x, y].SetAlive(true);
                    }
                }
            }
        }
    }

    void CountNeighbors()
    {
        for(int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for(int x = 0; x < SCREEN_WIDTH; x++)
            {
                int numNeighbors = 0;

                // North
                if(y+1 < SCREEN_HEIGHT)
                {
                    if(grid[x, y+1].isAlive){
                        numNeighbors++;
                    }
                }

                // East
                if(x+1 < SCREEN_WIDTH)
                {
                    if(grid[x+1, y].isAlive){
                        numNeighbors++;
                    }
                }

                // South
                if(y-1 >= 0)
                {
                    if(grid[x, y-1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // West
                if(x-1 >= 0)
                {
                    if(grid[x-1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // NorthEast
                if(x+1 < SCREEN_WIDTH && y+1 < SCREEN_HEIGHT)
                {
                    if(grid[x+1, y+1].isAlive){
                        numNeighbors++;
                    }
                }

                // NorthWest
                if(x-1 >= 0 && y+1 < SCREEN_HEIGHT)
                {
                    if(grid[x-1, y+1].isAlive){
                        numNeighbors++;
                    }
                }

                // SouthEast
                if(x+1 < SCREEN_WIDTH && y-1 >= 0)
                {
                    if(grid[x+1, y-1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // SouthWest
                if(x-1 >= 0 && y-1 >= 0)
                {
                    if(grid[x-1, y-1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                grid[x, y].numNeighbors = numNeighbors;
            }
        }
    }

    void PlaceCells()
    {
        for(int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for(int x = 0; x < SCREEN_WIDTH; x++)
            {
                Cell cell = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x,y), Quaternion.identity) as Cell;
                grid[x, y] = cell;

                // fill the grid with a random state (alive or dead)
                grid[x, y].SetAlive(RandomAliveCell());
            }
        }
    }

    bool RandomAliveCell()
    {
        int rand = UnityEngine.Random.Range(0, 100);

        if(rand > 75)
            return true;
        
        return false;
    }
}
