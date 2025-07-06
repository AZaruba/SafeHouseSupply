using Godot;
using System;

public partial class InstructionsMode : ModeManager
{

  public override void _Process(double delta)
  {
    if (Input.IsActionJustPressed("Confirm"))
    {
      EmitSignal(ModeManager.SignalName.PushGameMode, (int)GAME_MODE.GAMEPLAY);
    }
    if (Input.IsActionJustPressed("Cancel"))
    {
      EmitSignal(ModeManager.SignalName.PushGameMode, (int)GAME_MODE.MAIN_MENU);
    }
  }
}
