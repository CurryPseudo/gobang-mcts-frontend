using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class PlayerMoveEvent : UnityEvent<Vector2Int> { }
public class Board : MonoBehaviour
{
    public Vector2Int Size;
    private BoardElem[,] elems;
    public PlayerMoveEvent PlayerMove;
    public GameObject ElemPrefab;
    public bool Enable
    {
        private get;
        set;
    }
    private void Awake()
    {
        elems = new BoardElem[Size.x, Size.y];
        for(int i = 0; i < Size.x; i++)
        {

        }
    }
    public void AIMove(Vector2Int pos)
    {

    }
    public void RandomMove()
    {
        var pos = new Vector2Int(UnityEngine.Random.Range(0, Size.x), UnityEngine.Random.Range(0, Size.y));
        PlayerMove.Invoke(pos);
    }
}
