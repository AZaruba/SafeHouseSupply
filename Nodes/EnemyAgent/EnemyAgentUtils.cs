
using Godot;

public struct AgentInstruction
{
  public AgentInstruction(Vector2 Destination, Vector2 FacingDirection, float Speed, float WaitTime)
  {
    this.Destination = Destination;
    this.FacingDirection = FacingDirection;
    this.Speed = Speed;
    this.WaitTime = WaitTime;
  }
  public Vector2 Destination;
  public Vector2 FacingDirection;
  public float Speed;
  public float WaitTime; // in seconds
};
