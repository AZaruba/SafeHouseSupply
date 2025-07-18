using Godot;
using System;

public partial class MainGameMode : ModeManager
{
  [Signal]
  public delegate void PlayerHitEventHandler();
  [Export] public PackedScene EnemyAgentScene;

  public static MainGameMode instance;

  public static void CallSignal(string SignalName)
  {
    instance.EmitSignal(SignalName);
  }

  public override void _Ready()
  {
    instance = this;
    base._Ready();

    AddLevelEnemies();

    foreach (Node child in GetChildren())
    {
      if (child is IGameEntity)
      {
        child.SetPhysicsProcess(false);
        child.SetProcess(false);
      }
    }
  }

  private void AddLevelEnemies()
  {
    LevelEnemyData[] initData = StatsManager.GetCurrentLevelEnemyData();
    foreach (LevelEnemyData enemyData in initData)
    {
      EnemyAgent newAgent = ResourceLoader.Load<PackedScene>("res://Nodes/EnemyAgent/EnemyAgent.tscn").Instantiate<EnemyAgent>();
      EnemyData newAgentData = new(enemyData.PatrolInstructions, newAgent.ChaseVelocity);
      newAgent.data = newAgentData;
      newAgent.Position = enemyData.StartPosition;
      AddChild(newAgent);
    }
  }

  public override void _Process(double delta)
  {
    if (StatsManager.Instance.IsComplete)
    {
      EmitSignal(ModeManager.SignalName.PopGameMode);
    }
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
