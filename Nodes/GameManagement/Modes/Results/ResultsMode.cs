
using Godot;
using Godot.Collections;
using System;

public partial class ResultsMode : ModeManager
{

  [Export] private RichTextLabel StatsDisplay;
  [Export] private RichTextLabel RankDisplay;

  public override void _Ready()
  {
    base._Ready();
  }

  public override void _Process(double delta)
  {
    // Final Results screen

    if (Input.IsActionJustPressed("Confirm"))
    {
      if (StatsManager.Instance.CurrentLevel.Equals("FRS"))
      {
        EmitSignal(ModeManager.SignalName.PushGameMode, (int)GAME_MODE.FINAL_RESULTS_SCREEN);
      }
      else
      {
        // go all the way back to level instructions
        EmitSignal(ModeManager.SignalName.PopGameMode);
        EmitSignal(ModeManager.SignalName.PopGameMode);
      }
    }
  }

  public override void OnModeStart()
  {
    StatsDisplay.Text =
      "RESULTS\n\n\tWANTS COLLECTED: " + ScoringUtils.FormatMultiDigitString(StatsManager.Instance.CurrentLevelData.WantsCollected) +
      "\n\tTIMES SPOTTED:   " + ScoringUtils.FormatMultiDigitString(StatsManager.Instance.CurrentLevelData.TimesSpotted) +
      "\n\tTIME REMAINING:  " + ScoringUtils.FormatMultiDigitString(Mathf.CeilToInt(Timer.CurrentTime));

    RankDisplay.Text = ScoringUtils.CalculateRank();
  }
}
