using Godot;

public partial class Timer : Node2D, IGameEntity
{
  [Export] private RichTextLabel DisplayLabel;

  public static float LevelTime;
  public static float CurrentTime;

  public override void _Ready()
  {
    LevelTime = StatsManager.Instance.CurrentLevelData.Time;
    CurrentTime = LevelTime;
  }

  public override void _Process(double delta)
  {
    CurrentTime -= (float)delta;
    DisplayLabel.Text = Mathf.CeilToInt(CurrentTime).ToString();

    if (CurrentTime < 0)
    {
      // Send Signal
      MainGameMode.CallSignal(MainGameMode.SignalName.PlayerHit);
    }
  }

  public void OnReset()
  {
    CurrentTime = LevelTime;
  }
}
