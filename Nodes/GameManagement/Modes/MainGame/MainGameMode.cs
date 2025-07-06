using Godot;
using System;

public partial class MainGameMode : ModeManager
{
  public override void _Process(double delta)
  {
    if (Input.IsActionJustPressed("Cancel"))
    {
      EmitSignal(ModeManager.SignalName.PopGameMode);
    }
  }
}
