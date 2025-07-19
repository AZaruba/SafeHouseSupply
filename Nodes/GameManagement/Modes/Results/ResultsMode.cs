
using Godot;
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
    if (Input.IsActionJustPressed("Confirm"))
    {
      // go all the way back to level instructions
      EmitSignal(ModeManager.SignalName.PopGameMode);
      EmitSignal(ModeManager.SignalName.PopGameMode);
    }
  }

  public override void OnModeStart()
  {
    StatsDisplay.Text =
      "RESULTS\n\n\tWANTS COLLECTED: " + FormatMultiDigitString(StatsManager.Instance.CurrentLevelData.WantsCollected) +
      "\n\tTIMES SPOTTED:   " + FormatMultiDigitString(StatsManager.Instance.CurrentLevelData.TimesSpotted) +
      "\n\tTIME REMAINING:  " + FormatMultiDigitString(Mathf.CeilToInt(Timer.CurrentTime));

    RankDisplay.Text = CalculateRank();
  }

  private string CalculateRank()
  {
    int WantsListed = StatsManager.Instance.CurrentLevelData.Wants;
    int WantsCollected = StatsManager.Instance.CurrentLevelData.WantsCollected;
    int TimesSpotted = StatsManager.Instance.CurrentLevelData.TimesSpotted;

    int FinalScore = Mathf.CeilToInt((WantsCollected / WantsListed * 100) +
                                     (100 - Mathf.Min(TimesSpotted, 5) * 20) +
                                     (Timer.CurrentTime / Timer.LevelTime * 100));

    if (FinalScore > 200)
    {
      return "S";
    }
    else if (FinalScore > 160)
    {
      return "A";
    }
    else if (FinalScore > 130)
    {
      return "B";
    }
    else if (FinalScore > 100)
    {
      return "C";
    }
    else
    {
      return "D";
    }
  }

  private string FormatMultiDigitString(int number)
  {
    if (number < 10)
    {
      return "  " + number.ToString();
    }
    else if (number < 100)
    {
      return " " + number.ToString();
    }
    return number.ToString();
  }
}
