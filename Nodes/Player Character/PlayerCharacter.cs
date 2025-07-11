using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{

  public static ulong PlayerId;
  public static StateReference PlayerState;
  public static Vector2 PlayerPosition;

  private StateMachine stateMachine;

  private PlayerData data;

  [Export] private float MotionVelocity;
  [Export] private AnimationPlayer AnimPlayer;

  public override void _Ready()
  {
    data = new PlayerData
    {
      MotionVelocity = MotionVelocity
    };

    PlayerId = this.GetInstanceId();
    stateMachine = new StateMachine();
    stateMachine.AddState(StateReference.IDLE, new PlayerIdle(StateReference.IDLE, ref data));
    stateMachine.AddState(StateReference.MOVING, new PlayerMoving(StateReference.MOVING, ref data));
    stateMachine.AddState(StateReference.HIDING, new PlayerHiding(StateReference.HIDING, ref data));
    stateMachine.AddState(StateReference.HIDDEN, new PlayerHidden(StateReference.HIDDEN, ref data));
    stateMachine.AddState(StateReference.UNHIDING, new PlayerUnhiding(StateReference.UNHIDING, ref data));

    base._Ready();
  }

  public override void _PhysicsProcess(double delta)
  {
    // if not deactivated

    data.delta = (float)delta;
    ProcessInput();
    ProcessUpdate();
    PlayerState = stateMachine.GetCurrentState();
  }

  private void ProcessUpdate()
  {
    stateMachine.Act();
    AnimPlayer.Play("PlayerAnim/" + data.CurrentAnimation);

    Velocity = data.MotionDirection * data.MotionVelocity;
    MoveAndSlide();
    PlayerPosition = Position.Round();
  }

  private void ProcessInput()
  {
    data.MotionDirection = Vector2.Zero;
    if (
      Input.IsActionPressed("Down") ||
      Input.IsActionPressed("Up") ||
      Input.IsActionPressed("Right") ||
      Input.IsActionPressed("Left"))
    {
      stateMachine.UpdateState(StateAction.MOVE);
    }
    else
    {
      stateMachine.UpdateState(StateAction.STOP);
    }
    if (Input.IsActionPressed("Hide"))
    {
      stateMachine.UpdateState(StateAction.START_HIDING);
    }
    if (Input.IsActionJustReleased("Hide"))
    {
      stateMachine.UpdateState(StateAction.STOP_HIDING);
    }
    if (data.HidingTransitionTime > 0.4f) // TODO Magic Number
    {
      stateMachine.UpdateState(StateAction.HIDING_TRANSITION_END);
    }
  }
}
