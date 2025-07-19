using Godot;

public partial class EnemyAgent : CharacterBody2D, IGameEntity
{
  [Export] public float ChaseVelocity;
  [Export] private AnimationPlayer AnimPlayer;
  [Export] private ShapeCast2D DetectPlayerCast;
  [Export] private Area2D PlayerCollision;
  [Export] private float MaxVisionDistance;

  public EnemyData data;
  private StateMachine stateMachine;
  private Vector2 StartPosition;

  public override void _Ready()
  {
    base._Ready();

    StartPosition = Position;
    data.Position = Position;
    stateMachine = new StateMachine();
    stateMachine.AddState(StateReference.PATROLING, new EnemyPatroling(StateReference.PATROLING, ref data));
    stateMachine.AddState(StateReference.CHASING, new EnemyChasing(StateReference.CHASING, ref data));
    stateMachine.AddState(StateReference.LOOKING, new EnemyLooking(StateReference.LOOKING, ref data));
    stateMachine.AddState(StateReference.RETURNING, new EnemyReturning(StateReference.RETURNING, ref data));
    stateMachine.Start(StateReference.PATROLING);
  }
  public override void _PhysicsProcess(double delta)
  {
    // if not deactivated
    data.Position = Position;
    data.delta = (float)delta;
    UpdateStateMachine();
    ProcessUpdate();
    DebugInfo(); 
  }

  private void UpdateStateMachine()
  {
    if (IsPlayerSpotted())
    {
      stateMachine.UpdateState(StateAction.CHASE);
    }

    if (data.Position.Round() == data.ChaseTarget.Round())
    {
      stateMachine.UpdateState(StateAction.LOOK);
    }

    if (data.ReadyToReturn)
    {
      stateMachine.UpdateState(StateAction.RETURN_TO_PATROL);
    }
    if (data.ReturnPositions.Count == 0)
    {
      stateMachine.UpdateState(StateAction.PATROL);
    }

    if (GetWallNormal().Normalized() * -1 == data.MotionDirection)
    {
      stateMachine.UpdateState(StateAction.STOP);
    }
  }

  private void ProcessUpdate()
  {
    if (IsPlayerColliding())
    {
      // Send Signal
      MainGameMode.CallSignal(MainGameMode.SignalName.PlayerHit);
    }
    stateMachine.Act();
    UpdateAnimator();

    Velocity = data.MotionDirection * data.MotionVelocity;
    MoveAndSlide();
  }

  private void UpdateAnimator()
  {
    if (data.FacingDirection.X < 0)
    {
      AnimPlayer.Play("EnemyAgent/WalkLeft");
    }
    if (data.FacingDirection.X > 0)
    {
      AnimPlayer.Play("EnemyAgent/WalkRight");
    }
    if (data.FacingDirection.Y < 0)
    {
      AnimPlayer.Play("EnemyAgent/WalkUp");
    }
    if (data.FacingDirection.Y > 0)
    {
      AnimPlayer.Play("EnemyAgent/WalkDown");
    }
  }

  private bool IsPlayerSpotted()
  {
    Vector2 CastDirection = data.FacingDirection;
    DetectPlayerCast.TargetPosition = CastDirection * MaxVisionDistance;

    if (DetectPlayerCast.IsColliding())
    {
      GodotObject collisionResult = DetectPlayerCast.GetCollider(0);
      if (collisionResult != null && collisionResult.GetInstanceId() == PlayerCharacter.PlayerId)
      {
        return PlayerCharacter.PlayerState != StateReference.HIDDEN &&
         PlayerCharacter.PlayerState != StateReference.SHOPPING;
      }
    }
    return false;
  }

  private bool IsPlayerColliding()
  {
    // for (int i = 0; i < GetSlideCollisionCount(); i++)
    // {
    //   KinematicCollision2D col = GetSlideCollision(i);
    //   if (col.GetCollider().GetInstanceId() == PlayerCharacter.PlayerId)
    //   {
    //     SetPhysicsProcess(false);
    //     return true;
    //   }
    // }
    if (PlayerCollision.HasOverlappingBodies())
    {
      foreach (Node2D obj in PlayerCollision.GetOverlappingBodies())
      {
        if (obj.GetInstanceId() == PlayerCharacter.PlayerId)
        {
          SetPhysicsProcess(false);
          return true;
        }
      }
    }

    return false;
  }

  public void OnReset()
  {
    _Ready();
  }

  private void DebugInfo()
  {
    //ScoreDisplay.WriteString(stateMachine.GetCurrentState().ToString());
  }
}
