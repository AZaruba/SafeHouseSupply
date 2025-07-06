using Godot;
using System;

public partial class ModeManager : Node2D
{
  [Export] GAME_MODE GameMode;

  [Signal]
  public delegate void PushGameModeEventHandler(GAME_MODE nextMode);

  [Signal]
  public delegate void PopGameModeEventHandler();

  public override void _Ready()
  {
    GD.Print("INIT MODE MANAGER");
    SetProcess(false);
    Visible = false;
  }

  public override void _Process(double delta)
  {
    // no processing
  }
}
