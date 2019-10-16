using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class PlayerMoveEvent : UnityEvent<Vector2Int> { }
public class Board : MonoBehaviour
{
    public Vector2Int Size;
    public BoardElem[,] Elems;
    public PlayerMoveEvent PlayerMove;
    public BoardElem ElemPrefab;
    public Vector2Int Stride;
    public ElemType Side = ElemType.BLACK;
    public bool Enable
    {
        get;
        set;
    }
    private void Awake()
    {
        Elems = new BoardElem[Size.x, Size.y];
        for(int j = 0; j < Size.y; j++)
        {
            for (int i = 0; i < Size.x; i++)
            {
                var elem = Instantiate(ElemPrefab, transform);
                elem.Position = new Vector2Int(i, j);
            }
        }
    }
    public void AIMove(Vector2Int pos)
    {
        Elems[pos.x, pos.y].elemType = Side;
        Side = ElemTypeUtil.Flip(Side);
    }
    public void RandomMove()
    {
        var pos = new Vector2Int(UnityEngine.Random.Range(0, Size.x), UnityEngine.Random.Range(0, Size.y));
        PlayerMove.Invoke(pos);
    }
}
