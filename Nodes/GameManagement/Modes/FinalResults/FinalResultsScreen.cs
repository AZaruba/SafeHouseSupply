using Godot;
using System;

public partial class FinalResultsScreen : ModeManager
{
  [Export] private RichTextLabel L1Rank;
  [Export] private RichTextLabel L2Rank;
  [Export] private RichTextLabel L3Rank;
  [Export] private RichTextLabel FinalRank;

  public override void _Ready()
  {
    base._Ready();
  }

  public override void _Process(double delta)
  {

  }
  
  public override void OnModeStart()
  {
    
    L1Rank.Text = ScoringUtils.ScoreToRank(StatsManager.Scores[0]);
    L2Rank.Text = ScoringUtils.ScoreToRank(StatsManager.Scores[1]);
    L3Rank.Text = ScoringUtils.ScoreToRank(StatsManager.Scores[2]);

    FinalRank.Text = ScoringUtils.ScoreToRank(
      StatsManager.Scores[0] +
      StatsManager.Scores[1] +
      StatsManager.Scores[2],
      3
    );
  }
}
