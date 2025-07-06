using Godot;
using System;

public partial class MainMenuMode : ModeManager
{

  public override void _Process(double delta)
  {
    if (Input.IsActionJustPressed("Confirm"))
    {
      EmitSignal(ModeManager.SignalName.PushGameMode, (int)GAME_MODE.INSTRUCTIONS);
    }
  }
}
