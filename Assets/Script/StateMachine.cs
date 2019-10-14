using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StateMachine<T> : MonoBehaviour where T : StateMachine<T>
{
    private IEnumerator currentStateCoroutine = null;
    private State currentState = null;
    public State LastState { get; private set; } = null;
    public string StateNameDebug;
    public virtual State OriginState
    {
        get
        {
            return null;
        }
    }
    public class State
    {
        public bool IsCurrentState {
            get {
                return Machine.currentState == this;
            }
        }
        public T Machine;
        public event Action BeforeExit;
        public virtual IEnumerator Main() {
            yield break;
        }
        public void InvokeBeforeExit() {
            if(BeforeExit != null) {
                BeforeExit.Invoke();
            }
        }
        public virtual IEnumerator Exit() {
            yield break;
        }
        public void RegisterStateOnlyEvent(UnityEvent e, UnityAction a)
        {
            e.AddListener(a);
            BeforeExit += () =>
            {
                e.RemoveListener(a);
            };
        }
        public void RegisterStateOnlyEvent<T>(UnityEvent<T> e, UnityAction<T> a)
        {
            e.AddListener(a);
            BeforeExit += () =>
            {
                e.RemoveListener(a);
            };
        }
    }
    public void ChangeState(State NewState)
    {
        if(currentState != null)
        {
            StopCoroutine(currentStateCoroutine);
            currentState.InvokeBeforeExit();
            StartCoroutine(currentState.Exit());
            currentStateCoroutine = null;
            currentState = null;
        }
        if(NewState != null)
        {
            NewState.Machine = this as T;
            currentState = NewState;
            currentStateCoroutine = NewState.Main();
            StartCoroutine(currentStateCoroutine);
            StateNameDebug = currentState.ToString();
        }
        LastState = currentState;
    }
    public void Main()
    {
        ChangeState(OriginState);
    }
}
