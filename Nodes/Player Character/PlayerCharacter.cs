using Godot;
using System;
using System.Linq.Expressions;

public partial class PlayerCharacter : CharacterBody2D, IGameEntity
{

  public static ulong PlayerId;
  public static StateReference PlayerState;
  public static Vector2 PlayerPosition;

  private StateMachine stateMachine;

  private PlayerData data;

  private Vector2 StartPosition;

  [Export] private float MotionVelocity;
  [Export] private AnimationPlayer AnimPlayer;
  [Export] private CollisionShape2D Collider;
  [Export] private Sprite2D Sprite;

  [Signal] public delegate void PlayerEnterShopEventHandler();
  [Signal] public delegate void PlayerExitShopEventHandler();

  public override void _Ready()
  {
    data = new PlayerData
    {
      MotionVelocity = MotionVelocity
    };

    StartPosition = Position;

    PlayerId = this.GetInstanceId();
    stateMachine = new StateMachine();
    stateMachine.AddState(StateReference.IDLE, new PlayerIdle(StateReference.IDLE, ref data));
    stateMachine.AddState(StateReference.MOVING, new PlayerMoving(StateReference.MOVING, ref data));
    stateMachine.AddState(StateReference.HIDING, new PlayerHiding(StateReference.HIDING, ref data));
    stateMachine.AddState(StateReference.HIDDEN, new PlayerHidden(StateReference.HIDDEN, ref data));
    stateMachine.AddState(StateReference.UNHIDING, new PlayerUnhiding(StateReference.UNHIDING, ref data));
    stateMachine.AddState(StateReference.SHOPPING, new PlayerShopping(StateReference.SHOPPING, ref data));

    base._Ready();
  }

  public override void _PhysicsProcess(double delta)
  {
    // if not deactivated

    DebugInfo();
    data.delta = (float)delta;
    ProcessInput();
    ProcessUpdate();
    PlayerState = stateMachine.GetCurrentState();
    data.IsOnDoor = false;
  }

  private void ProcessUpdate()
  {
    stateMachine.Act();
    if (stateMachine.GetCurrentState() == StateReference.IDLE)
    {
      AnimPlayer.Pause();
    }
    else
    {
      AnimPlayer.Play("PlayerAnim/" + data.CurrentAnimation);
    }

    Sprite.Visible = stateMachine.GetCurrentState() != StateReference.SHOPPING;

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

    if (Input.IsActionJustPressed("Open") && data.IsOnDoor)
    {
      stateMachine.UpdateState(StateAction.ENTER);
      if (stateMachine.GetCurrentState() != StateReference.SHOPPING)
      {
        EmitSignal(SignalName.PlayerExitShop);
        SetCollisionLayerValue(6, true);
      }
      else
      {
        EmitSignal(SignalName.PlayerEnterShop);
        SetCollisionLayerValue(6, false);
      }
    }
  }

  public void SetIsOnDoor(bool value)
  {
    data.IsOnDoor = value;

  }

  private void CheckForDoorAndHide()
  {

  }

  public void OnReset()
  {
    Position = StartPosition;
  }

  private void DebugInfo()
  {
    ScoreDisplay.WriteString(GetCollisionLayerValue(5).ToString());
  }
}
