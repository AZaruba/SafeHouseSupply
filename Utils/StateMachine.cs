
using System.Collections.Generic;
using Godot;

public class StateMachine
{
  public Dictionary<StateReference, State> States;
  private StateReference CurrentState;

  public StateMachine()
  {
    States = [];
  }

  public void Start(StateReference reference)
  {
    CurrentState = reference;
  }

  public void AddState(StateReference reference, State state)
  {
    if (!States.TryAdd(reference, state))
    {
      GD.Print("State Not Added");
    }
  }

  public void Act()
  {
    States[CurrentState].Act();
  }

  public void UpdateState(StateAction action)
  {
    State oldState = States[CurrentState];
    StateReference nextState = oldState.Transition(action);
    if (nextState != CurrentState)
    {
      oldState.Exit();
      States[nextState].Enter();
    }
    CurrentState = nextState;
  }

  public StateReference GetCurrentState()
  {
    return CurrentState;
  }
}

public class State
{
  public readonly StateReference ID;

  public State(StateReference id)
  {
    this.ID = id;
  }

  public virtual void Enter() { }
  public virtual void Act() { }

  public virtual void Exit() { }

  public virtual StateReference Transition(StateAction action)
  {
    return ID;
  }
}

public enum StateAction {
  ERROR = -1,
  NO_ACTION,

  // player actions
  MOVE,
  STOP,
  START_HIDING,
  HIDING_TRANSITION_END,
  STOP_HIDING,
  GET_CAUGHT,

  // enemy actions
  PATROL,
  CHASE,
  LOOK,
  RETURN_TO_PATROL
}

public enum StateReference {
  ERROR = -1,
  // Player States
  IDLE,
  MOVING,
  HIDING,
  HIDDEN,
  UNHIDING,
  CAUGHT,
  INACTIVE,

  // Enemy states
  PATROLING,
  CHASING,
  LOOKING,
  RETURNING
}