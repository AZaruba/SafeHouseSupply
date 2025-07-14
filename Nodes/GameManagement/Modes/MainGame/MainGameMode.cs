using Godot;
using System;

public partial class MainGameMode : ModeManager
{
  [Signal]
  public delegate void PlayerHitEventHandler();

  public static MainGameMode instance;

  public static void CallSignal(string SignalName)
  {
    instance.EmitSignal(SignalName);
  }

  public override void _Ready()
  {
    instance = this;
    base._Ready();

    foreach (Node child in GetChildren())
    {
      if (child is IGameEntity)
      {
        child.SetPhysicsProcess(false);
        child.SetProcess(false);
      }
    }
  }

  public override void _Process(double delta)
  {

  }

  public override void OnModeStart()
  {
    foreach (Node child in GetChildren())
    {
      if (child is IGameEntity)
      {
        child.SetPhysicsProcess(true);
        child.SetProcess(true);
      }
    }
  }

  public void OnPlayerHit()
  {
    EmitSignal(ModeManager.SignalName.PopGameMode);
  }
}
