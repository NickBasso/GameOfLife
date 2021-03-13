using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game: MonoBehaviour {
    // 1 unit is set to 16px
    public
    const int SCREEN_WIDTH = 64; //64 units = 1024px
    public
    const int SCREEN_HEIGHT = 48; //48 units = 768px

    // hold user defined map size
    private int fromX = 0;
    private int fromY = 0;
    private static int toX = SCREEN_WIDTH;
    private static int toY = SCREEN_HEIGHT;

    private float timer = 0;

    // from - to coordinates block for grid overlay script 
    public static float xNoEdit;
    public static float yNoEdit;
    public static float toXNoEdit;
    public static float toYNoEdit;
    public static int gridSizeX;
    public static int gridSizeY;

    public float speed = 0.5f;
    public int mapSizeX = SCREEN_WIDTH;
    public int mapSizeY = SCREEN_HEIGHT;

    public bool simulationEnabled = false;

    Cell[, ] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start() {
        EstablishMapSize();
        FillMapWithDeadCells();
    }

    // Update is called once per frame
    void Update() {
        if (simulationEnabled) {
            if (timer >= speed) {
                timer = 0f;

                CountNeighbors();
                PopulationControl();
            } else {
                timer += Time.deltaTime;
            }
        }

        UserInput();
    }

    void PopulationControl() {
        for (int y = fromY; y < toY; y++) {
            for (int x = fromX; x < toX; x++) {
                /*
                    Conway's Game of Life rules:
                    1. live cell with 2 or 3 neighbors => lives
                    2. dead cell with 3 neighbors => lives
                    3. others => die or remain dead
                */

                // if oustide of bounds, set alive "outside of bounds" cells to see the bounds
                if (y < fromY || y >= toY || x < fromX || x >= toX) {
                    grid[x, y].SetAlive(true);
                } else if (grid[x, y].isAlive) {
                    if (grid[x, y].numNeighbors != 2 && grid[x, y].numNeighbors != 3) {
                        grid[x, y].SetAlive(false);
                    }
                } else {
                    if (grid[x, y].numNeighbors == 3) {
                        grid[x, y].SetAlive(true);
                    }
                }
            }
        }
    }

    void CountNeighbors() {
        for (int y = fromY; y < toY; y++) {
            for (int x = fromX; x < toX; x++) {
                int numNeighbors = 0;

                // North
                if (y + 1 < toY) {
                    if (grid[x, y + 1].isAlive) {
                        numNeighbors++;
                    }
                }

                // East
                if (x + 1 < toX) {
                    if (grid[x + 1, y].isAlive) {
                        numNeighbors++;
                    }
                }

                // South
                if (y - 1 >= fromY) {
                    if (grid[x, y - 1].isAlive) {
                        numNeighbors++;
                    }
                }

                // West
                if (x - 1 >= fromX) {
                    if (grid[x - 1, y].isAlive) {
                        numNeighbors++;
                    }
                }

                // NorthEast
                if (x + 1 < toX && y + 1 < toY) {
                    if (grid[x + 1, y + 1].isAlive) {
                        numNeighbors++;
                    }
                }

                // NorthWest
                if (x - 1 >= fromX && y + 1 < toY) {
                    if (grid[x - 1, y + 1].isAlive) {
                        numNeighbors++;
                    }
                }

                // SouthEast
                if (x + 1 < toX && y - 1 >= fromY) {
                    if (grid[x + 1, y - 1].isAlive) {
                        numNeighbors++;
                    }
                }

                // SouthWest
                if (x - 1 >= fromX && y - 1 >= fromY) {
                    if (grid[x - 1, y - 1].isAlive) {
                        numNeighbors++;
                    }
                }

                grid[x, y].numNeighbors = numNeighbors;
            }
        }
    }

    void FillMapWithDeadCells() {
        // fill the valid map cells with random beginning state
        for (int y = fromY; y < toY; y++)
            for (int x = fromX; x < toX; x++) {
                Cell cell = Instantiate(Resources.Load("Prefabs/Like", typeof (Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                grid[x, y] = cell;
                grid[x, y].SetAlive(false);
            }
    }

    void FillMapWithRandomCells() {
        // fill the valid map cells with random beginning state
        for (int y = fromY; y < toY; y++)
            for (int x = fromX; x < toX; x++) {
                Cell cell = Instantiate(Resources.Load("Prefabs/Like", typeof (Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                grid[x, y] = cell;
                grid[x, y].SetAlive(RandomAliveCell());
            }
    }

    void clearMap() {
        // fill the valid map cells with random beginning state
        for (int y = fromY; y < toY; y++)
            for (int x = fromX; x < toX; x++)
                grid[x, y].SetAlive(false);
    }

    void UserInput() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int x = Mathf.RoundToInt(mousePoint.x);
            int y = Mathf.RoundToInt(mousePoint.y);

            if (x >= fromX && y >= fromY && x < toX && y < toY) {
                grid[x, y].SetAlive(!grid[x, y].isAlive);
            }
        }

        if (Input.GetKeyUp(KeyCode.P)) {
            // Pause-Resume simulation
            simulationEnabled = !simulationEnabled;
        }

        if (Input.GetKeyUp(KeyCode.R)) {
            // Clear map from previosly existing cells
            clearMap();

            // Fill map with random state cells
            FillMapWithRandomCells();
        }

        if (Input.GetKeyUp(KeyCode.C)) {
            // Clear map from previosly existing cells
            clearMap();

            FillMapWithDeadCells();
        }
    }

    bool RandomAliveCell() {
        int rand = UnityEngine.Random.Range(0, 100);

        if (rand > 50)
            return true;

        return false;
    }

    void EstablishMapSize() {
        fromX = (SCREEN_WIDTH - mapSizeX) / 2;
        toX = fromX + mapSizeX;
        fromY = (SCREEN_HEIGHT - mapSizeY) / 2;
        toY = fromY + mapSizeY;

        xNoEdit = fromX;
        yNoEdit = fromY;
        toXNoEdit = toX;
        toYNoEdit = toY;
        gridSizeX = mapSizeX;
        gridSizeY = mapSizeY;
    }
}