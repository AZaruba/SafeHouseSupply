
using Godot;
using System;
using System.Collections.Generic;

public class EnemyData
{
  public EnemyData(AgentInstruction[] instructions, float chaseVelocity)
  {
    Position = Vector2.Zero;
    ChaseTarget = Vector2.Zero;
    ChaseVelocity = chaseVelocity;
    MotionDirection = Vector2.Zero;
    MotionVelocity = 0;
    PatrolInstructions = instructions;
    PatrolInstructionIndex = 0;
    CurrentInstruction = PatrolInstructions[0];
    CurrentWaitTime = 0;
    ReadyToReturn = false;
    ReturnPositions = new Stack<Vector2>();
    delta = 0;
  }

  public Vector2 Position;
  public Vector2 MotionDirection;
  public Vector2 FacingDirection;
  public Vector2 ChaseTarget;
  public float MotionVelocity;
  public AgentInstruction[] PatrolInstructions;
  public int PatrolInstructionIndex;
  public AgentInstruction CurrentInstruction;
  public Stack<Vector2> ReturnPositions;
  public float delta;
  public float CurrentWaitTime;
  public float ChaseVelocity;

  public bool ReadyToReturn;
}

public class EnemyState : State
{
  protected EnemyData ed;

  public EnemyState(StateReference id, ref EnemyData data) : base(id)
  {
    this.ed = data;
  }
}

public class EnemyPatroling : EnemyState
{
  public EnemyPatroling(StateReference id, ref EnemyData data) : base(id, ref data)
  {

  }

  private bool HasReachedDestination()
  {
    Vector2 Destination = ed.CurrentInstruction.Destination;
    if (ed.Position.Round() == Destination.Round())
    {
      return true;
    }
    return false;
  }


  private void ProcessInstructions()
  {
    if (HasReachedDestination())
    {
      ed.Position = ed.CurrentInstruction.Destination;
      ed.CurrentWaitTime += ed.delta;
    }
    ed.FacingDirection = ed.CurrentInstruction.FacingDirection;

    if (ed.CurrentWaitTime >= ed.CurrentInstruction.WaitTime)
    {
      // iterate, move
      ed.PatrolInstructionIndex = (ed.PatrolInstructionIndex + 1) % ed.PatrolInstructions.Length;
      ed.CurrentWaitTime = 0;
      ed.CurrentInstruction = ed.PatrolInstructions[ed.PatrolInstructionIndex];
    }

    ed.MotionDirection = new Vector2(
      ed.CurrentInstruction.Destination.X - ed.Position.X,
      ed.CurrentInstruction.Destination.Y - ed.Position.Y
    ).Normalized();

    ed.MotionVelocity = ed.CurrentInstruction.Speed;
  }

  public override void Act()
  {
    ProcessInstructions();
    base.Act();
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
      case StateAction.CHASE:
        return StateReference.CHASING;
      default:
        return ID;
    }
  }
}

public class EnemyChasing : EnemyState
{
  public EnemyChasing(StateReference id, ref EnemyData data) : base(id, ref data)
  {

  }

  public void CalculateChaseTarget()
  {
    Vector2 direction = ed.FacingDirection.Normalized();
    Vector2 projectedEPos = ed.Position.Project(direction).Round();
    Vector2 projectedPPos = PlayerCharacter.PlayerPosition.Project(direction).Round();

    float dist = Mathf.Round((projectedPPos - projectedEPos).Length());

    ed.ChaseTarget = (ed.Position + direction * dist).Round();
  }

  public override void Act()
  {
    ed.MotionDirection = (ed.ChaseTarget.Round() - ed.Position.Round()).Normalized();
    ed.FacingDirection = ed.MotionDirection;
    ed.MotionVelocity = ed.ChaseVelocity;

    base.Act();
  }

  public override void Enter()
  {
    CalculateChaseTarget();
    ed.MotionDirection = (ed.ChaseTarget.Round() - ed.Position.Round()).Normalized();
    ed.FacingDirection = ed.MotionDirection;
    ed.MotionVelocity = ed.ChaseVelocity;

    ed.ReturnPositions.Push(ed.Position.Round());
    base.Enter();

    StatsManager.Instance.CurrentLevelData.TimesSpotted++;
  }

  public override void Exit()
  {
    ed.ReadyToReturn = false;
    base.Exit();
  }

  public override StateReference Transition(StateAction action)
  {
    switch (action)
    {
      case StateAction.LOOK:
        return StateReference.LOOKING;
      case StateAction.STOP:
        return StateReference.LOOKING;
      default:
        return ID;
    }
  }
}

public class EnemyLooking : EnemyState
{
  private float Timer = 0;
  private float LookTime = 1;
  private int LookCount = 0;
  private Vector2[] LookDirections = [
    Vector2.Right,
    Vector2.Up,
    Vector2.Left,
    Vector2.Down
  ];

  public EnemyLooking(StateReference id, ref EnemyData data) : base(id, ref data)
  {

  }

  public override void Act()
  {
    if (LookCount == 4)
    {
      LookCount = 0; // Done
      Timer = 0;
      ed.ReadyToReturn = true;
    }
    else
    {
      ed.FacingDirection = LookDirections[LookCount];
      Timer += ed.delta;
      if (Timer > LookTime)
      {
        Timer = 0;
        LookCount++;
      }
    }
    base.Act();
  }

  public override void Enter()
  {
    ed.MotionVelocity = 0;
    Timer = 0;
    LookCount = 0;
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
      case StateAction.RETURN_TO_PATROL:
        return StateReference.RETURNING;
      case StateAction.CHASE:
        return StateReference.CHASING;
      default:
        return ID;
    }
  }
}


public class EnemyReturning : EnemyState
{
  public EnemyReturning(StateReference id, ref EnemyData data) : base(id, ref data)
  {

  }
  
  private bool HasReachedDestination(Vector2 target)
  {
    Vector2 Destination = target;
    if (ed.Position.Round() == Destination.Round())
    {
      return true;
    }
    return false;
  }

  public override void Act()
  {
    if (ed.ReturnPositions.TryPeek(out Vector2 returnTarget))
    {
      ed.MotionDirection = (returnTarget.Round() - ed.Position.Round()).Normalized();
      ed.FacingDirection = ed.MotionDirection;

      if (HasReachedDestination(returnTarget))
      {
        ed.Position = returnTarget;
        ed.ReturnPositions.Pop();
      }
    }

    base.Act();
  }

  public override void Enter()
  {
    ed.MotionVelocity = 40f; // TODO: magic number
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
      case StateAction.CHASE:
        return StateReference.CHASING;
      case StateAction.PATROL:
        return StateReference.PATROLING;
      default:
        return ID;
    }
  }
}