using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GobangClient : StateMachine<GobangClient>
{
    public int TryCount;
    public UnityEvent PlayerWinEvent;
    public UnityEvent AIWinEvent;
    public UnityEvent DrawEvent;
    public UnityEvent ChangeSideEvent;
    public UnityEvent WaitPlayerEvent;
    private void Start()
    {
        PlayerWinEvent.AddListener(() => ChangeState(new GameEnd()));
        AIWinEvent.AddListener(() => ChangeState(new GameEnd()));
        DrawEvent.AddListener(() => ChangeState(new GameEnd()));
        var client = GetComponent<TcpClientProxy>();
        client.Send(string.Format("try_count {0}", TryCount));
        ChangeState(new WaitPlayerMove());
    }
    public void ChangeToChangeSideState()
    {
        ChangeState(new ChangeSide());
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public class ChangeSide : State
    {
        public override IEnumerator Main()
        {
            Machine.ChangeSideEvent.Invoke();
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
                            Machine.AIWinEvent.Invoke();
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
            Machine.WaitPlayerEvent.Invoke();
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
                        Machine.PlayerWinEvent.Invoke();
                        break;
                    case "draw":
                        Machine.DrawEvent.Invoke();
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
