using Godot;

public partial class EnemyAgent : CharacterBody2D
{
  [Export] private float ChaseVelocity;
  [Export] private AnimationPlayer AnimPlayer;
  [Export] private ShapeCast2D DetectPlayerCast;
  [Export] private float MaxVisionDistance;

  private EnemyData data;
  private StateMachine stateMachine;

  public override void _Ready()
  {
    base._Ready();

    // Load Test Instructions
    AgentInstruction[] PatrolInstructions =
    [
      new AgentInstruction(
        new Vector2(32, 120),
        Vector2.Left,
        40,
        1
      ),
      new AgentInstruction(
        new Vector2(144, 120),
        Vector2.Right,
        40,
        1
      ),
    ];

    data = new EnemyData(PatrolInstructions, ChaseVelocity);
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
  }

  private void ProcessUpdate()
  {
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
        return PlayerCharacter.PlayerState != StateReference.HIDDEN;
      }
    }
    return false;
  }

  private void DebugInfo()
  {
    ScoreDisplay.WriteString(data.ReturnPositions.Count.ToString());
  }
}
