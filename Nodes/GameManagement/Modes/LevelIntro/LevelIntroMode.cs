using Godot;
using System;

public partial class LevelIntroMode : ModeManager
{

  [Export] private RichTextLabel TextDisplay;

  public override void _Ready()
  {
    base._Ready();
  }

  public override void _Process(double delta)
  {
    if (Input.IsActionJustPressed("Confirm"))
    {
      EmitSignal(ModeManager.SignalName.PushGameMode, (int)GAME_MODE.GAMEPLAY);
    }
    if (Input.IsActionJustPressed("Cancel"))
    {
      EmitSignal(ModeManager.SignalName.PushGameMode, (int)GAME_MODE.INSTRUCTIONS);
    }
  }

  public override void OnModeStart()
  {
    // Load Current Level Data
    StatsManager.LoadLevel(StatsManager.Instance.CurrentLevel);
    TextDisplay.Text = StatsManager.WriteLevelData();
  }
}
