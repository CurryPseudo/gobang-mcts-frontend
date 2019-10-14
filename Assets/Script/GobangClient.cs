using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameResult
{
    PLAYERWIN,
    AIWIN,
    DRAW
}
[System.Serializable]
public class GameResultEvent : UnityEvent<GameResult> { }
public class GobangClient : StateMachine<GobangClient>
{
    public int TryCount;
    public GameResultEvent GREvent;
    private void Start()
    {
        GREvent.AddListener((r) => ChangeState(new GameEnd()));
        var client = GetComponent<TcpClientProxy>();
        client.Send(string.Format("try_count {0}", TryCount));
        ChangeState(new ChangeSide());
    }
    public class ChangeSide : State
    {
        public override IEnumerator Main()
        {
            var board = SceneSingleton.Get<Board>();
            var client = Machine.GetComponent<TcpClientProxy>();
            client.Send("change_side");
            board.Enable = false;
            RegisterStateOnlyEvent(client.ReceiveEvent, (s) =>
            {
                {
                    var rx = new Regex(@"ai_move (\d+) (\d+)");
                    var match = rx.Match(s);
                    if (match.Success)
                    {
                        var groups = match.Groups;
                        board.AIMove(new Vector2Int(int.Parse(groups[1].Value), int.Parse(groups[2].Value)));
                        var winrx = new Regex(@"win");
                        if(winrx.Match(s).Success)
                        {
                            Machine.GREvent.Invoke(GameResult.AIWIN);
                        }
                        else
                        {
                            Machine.ChangeState(new WaitPlayerMove());
                        }
                    }
                }
            });
            yield break;
        }

    }
    public class WaitPlayerMove : State
    {
        public override IEnumerator Main()
        {
            var board = SceneSingleton.Get<Board>();
            board.Enable = true;
            RegisterStateOnlyEvent(board.PlayerMove, (pos) =>
            {
                var client = Machine.GetComponent<TcpClientProxy>();
                client.Send(string.Format("move {0} {1}", pos.x, pos.y));
                Machine.ChangeState(new WaitPlayerMoveAnswer());
            });
            yield break;
        }
    }
    public class WaitPlayerMoveAnswer : State
    {
        public override IEnumerator Main()
        {
            RegisterStateOnlyEvent(Machine.GetComponent<TcpClientProxy>().ReceiveEvent, (s) =>
            {
                switch (s) {
                    case "player_win":
                        Machine.GREvent.Invoke(GameResult.PLAYERWIN);
                        break;
                    case "draw":
                        Machine.GREvent.Invoke(GameResult.DRAW);
                        break;
                    case "player_continue":
                        Machine.ChangeState(new ChangeSide());
                        break;
                }
            });
            yield break;
        }
    }
    public class GameEnd : State
    {
        public override IEnumerator Main()
        {
            yield break;
        }
    }
}
