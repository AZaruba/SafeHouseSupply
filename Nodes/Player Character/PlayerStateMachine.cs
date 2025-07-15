
using Godot;

public class PlayerData
{
  public PlayerData()
  {
    MotionDirection = Vector2.Zero;
    MotionVelocity = 0;
    CurrentAnimation = "Idle";
    delta = 0;
    IsOnDoor = false;
  }
  public Vector2 MotionDirection;
  public float MotionVelocity;
  public float HidingTransitionTime;
  public float delta; // comes from engine

  public string CurrentAnimation;
  public bool IsOnDoor;
}

public class PlayerState : State
{
  protected PlayerData pd;
  public PlayerState(StateReference id, ref PlayerData data) : base(id)
  {
    this.pd = data;
  }
}

public class PlayerMoving : PlayerState
{
  public PlayerMoving(StateReference id, ref PlayerData data) : base(id, ref data)
  {

  }

  public override void Act()
  {
    pd.MotionDirection = Vector2.Zero;
    if (Input.IsActionPressed("Down"))
    {
      pd.MotionDirection += Vector2.Down;
      pd.CurrentAnimation = "WalkDown";
    }

    if (Input.IsActionPressed("Up"))
    {
      pd.MotionDirection += Vector2.Up;
      pd.CurrentAnimation = "WalkUp";
    }

    if (Input.IsActionPressed("Right"))
    {
      pd.MotionDirection += Vector2.Right;
      pd.CurrentAnimation = "WalkRight";
    }

    if (Input.IsActionPressed("Left"))
    {
      pd.MotionDirection += Vector2.Left;
      pd.CurrentAnimation = "WalkLeft";
    }
    pd.MotionDirection = pd.MotionDirection.Normalized();
  }

  public override void Enter()
  {
    base.Enter();
  }

  public override void Exit()
  {
    base.Exit();
  }

  public override StateReference Transition(StateAction action)
  {
    switch (action)
    {
      case StateAction.STOP:
        return StateReference.IDLE;
      case StateAction.START_HIDING:
        return StateReference.HIDING;
      case StateAction.ENTER:
        return StateReference.SHOPPING;
      default:
        return ID;
    }
  }
}

public class PlayerIdle : PlayerState
{
  public PlayerIdle(StateReference id, ref PlayerData data) : base(id, ref data)
  {

  }

  public override void Act()
  {
    pd.MotionDirection = Vector2.Zero;
  }

  public override void Enter()
  {
    pd.CurrentAnimation = "Idle";
    base.Enter();
  }

  public override void Exit()
  {
    base.Exit();
  }

  public override StateReference Transition(StateAction action)
  {
    switch (action)
    {
      case StateAction.MOVE:
        return StateReference.MOVING;
      case StateAction.START_HIDING:
        return StateReference.HIDING;
      case StateAction.ENTER:
        return StateReference.SHOPPING;
      default:
        return ID;
    }
  }
}

public class PlayerHiding : PlayerState
{
  public PlayerHiding(StateReference id, ref PlayerData data) : base(id, ref data)
  {

  }

  public override void Act()
  {
    pd.MotionDirection = Vector2.Zero;
    pd.HidingTransitionTime += pd.delta;
  }

  public override void Enter()
  {
    pd.HidingTransitionTime = 0;
    pd.CurrentAnimation = "Hide";
    base.Enter();
  }

  public override void Exit()
  {
    pd.HidingTransitionTime = 0;
    base.Exit();
  }

  public override StateReference Transition(StateAction action)
  {
    switch (action)
    {
      case StateAction.HIDING_TRANSITION_END:
        return StateReference.HIDDEN;
      default:
        return ID;
    }
  }
}

public class PlayerHidden : PlayerState
{
  public PlayerHidden(StateReference id, ref PlayerData data) : base(id, ref data)
  {

  }

  public override void Act()
  {
    pd.MotionDirection = Vector2.Zero;
  }

  public override void Enter()
  {
    pd.CurrentAnimation = "Hidden";
    base.Enter();
  }

  public override void Exit()
  {
    base.Exit();
  }

  public override StateReference Transition(StateAction action)
  {
    switch (action)
    {
      case StateAction.STOP_HIDING:
        return StateReference.UNHIDING;
      default:
        return ID;
    }
  }
}

public class PlayerUnhiding : PlayerState
{
  public PlayerUnhiding(StateReference id, ref PlayerData data) : base(id, ref data)
  {

  }

  public override void Act()
  {
    pd.MotionDirection = Vector2.Zero;
    pd.HidingTransitionTime += pd.delta;
  }

  public override void Enter()
  {
    pd.CurrentAnimation = "Unhide";
    pd.HidingTransitionTime = 0;
    base.Enter();
  }

  public override void Exit()
  {
    pd.HidingTransitionTime = 0;
    base.Exit();
  }

  public override StateReference Transition(StateAction action)
  {
    switch (action)
    {
      case StateAction.HIDING_TRANSITION_END:
        return StateReference.IDLE;
      default:
        return ID;
    }
  }
}

public class PlayerShopping : PlayerState
{
  public PlayerShopping(StateReference id, ref PlayerData data) : base(id, ref data)
  {

  }

  public override void Act()
  {
    pd.MotionDirection = Vector2.Zero;
  }

  public override void Enter()
  {
    pd.CurrentAnimation = "Shopping";
    base.Enter();
  }

  public override void Exit()
  {
    base.Exit();
  }

  public override StateReference Transition(StateAction action)
  {
    switch (action)
    {
      case StateAction.ENTER:
        return StateReference.IDLE;
      default:
        return ID;
    }
  }
}
