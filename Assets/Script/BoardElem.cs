using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ElemType
{
    BLACK,
    WHITE,
}
public static class ElemTypeUtil
{
    public static ElemType Flip(ElemType elemType)
    {
        switch (elemType)
        {
            case ElemType.BLACK:
                return ElemType.WHITE;
            default:
                return ElemType.BLACK;
        }
    }
}
public class BoardElem : MonoBehaviour
{
    public Vector2Int Position;
    public ElemType? elemType = null;
    private Board board;
    private Button button;
    private Text text;
    private void Start()
    {

        board = SceneSingleton.Get<Board>();
        board.Elems[Position.x, Position.y] = this;

        var rectTransform = GetComponent<RectTransform>();
        Vector2 pos = Position;
        pos -= new Vector2(board.Size.x - 1, board.Size.y - 1) / 2;
        pos.Scale(board.Stride);
        rectTransform.localPosition = pos;
        gameObject.name = string.Format("BoardElem[{0}, {1}]", Position.x, Position.y);

        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        text = GetComponentInChildren<Text>();
    }
    public void OnClick()
    {
        elemType = board.Side;
        board.Side = ElemTypeUtil.Flip(board.Side);
        board.PlayerMove.Invoke(Position);
    }
    private void Update()
    {
        switch (elemType)
        {
            case ElemType.BLACK:
                button.enabled = false;
                text.text = "O";
                break;
            case ElemType.WHITE:
                button.enabled = false;
                text.text = "X";
                break;
            case null:
                button.enabled = true;
                text.text = "";
                break;
        }
        if(!board.Enable)
        {
            button.enabled = false;
        }
    }
}
