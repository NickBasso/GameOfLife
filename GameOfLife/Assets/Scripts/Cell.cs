using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell: MonoBehaviour {
    public bool isAlive = false;
    public int numNeighbors = 0;

    public void SetAlive(bool alive) {
        isAlive = alive;
        SetRender(alive);
    }

    private void SetRender(bool alive) {
        GetComponent < SpriteRenderer > ().enabled = alive ? true : false;
    }
}