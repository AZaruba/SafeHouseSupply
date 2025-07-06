using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{

  private Vector2 MotionDirection;
  [Export] private float MotionVelocity;

  public override void _Ready()
  {
    base._Ready();
  }

  public override void _PhysicsProcess(double delta)
  {
    // if not deactivated
    ProcessInput();
    ProcessUpdate();
  }

  private void ProcessUpdate()
  {
    Velocity = MotionDirection * MotionVelocity;
    MoveAndSlide();
  }

  private void ProcessInput()
  {
    MotionDirection = Vector2.Zero;
    if (Input.IsActionPressed("Down"))
    {
      MotionDirection += Vector2.Down;
    }

    if (Input.IsActionPressed("Up"))
    {
      MotionDirection += Vector2.Up;
    }

    if (Input.IsActionPressed("Right"))
    {
      MotionDirection += Vector2.Right;
    }

    if (Input.IsActionPressed("Left"))
    {
      MotionDirection += Vector2.Left;
    }
    MotionDirection = MotionDirection.Normalized();
  }
}
