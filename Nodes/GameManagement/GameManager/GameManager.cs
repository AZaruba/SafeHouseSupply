using System.Collections.Generic;
using Godot;

public partial class GameManager : Node
{
  private Stack<GAME_MODE> GameModeStack;

  Godot.Collections.Dictionary<GAME_MODE, ModeManager> GameModeLookup = [];

  [Export] public PackedScene PackedMainGameMode;
  [Export] public PackedScene PackedIntroMode;
  [Export] public PackedScene PackedMainMenuMode;
  [Export] public PackedScene PackedInstructionsMode;
  [Export] public PackedScene PackedResultsMode;
  [Export] public PackedScene PackedFinalResultsScreen;

  Node ActiveGameModeNode;

  // on game start
  public override void _Ready()
  {
    MainMenuMode mmm = ResourceLoader.Load<PackedScene>("res://Nodes/GameManagement/Modes/MainMenu/MainMenuMode.tscn").Instantiate<MainMenuMode>();
    AddChild(mmm);
    GameModeLookup[GAME_MODE.MAIN_MENU] = mmm;
    mmm.PushGameMode += OnPushGameMode;

    GameModeStack = new Stack<GAME_MODE>();
    GameModeStack.Push(GAME_MODE.MAIN_MENU);
    GameModeLookup[GAME_MODE.MAIN_MENU].SetProcess(true);
    GameModeLookup[GAME_MODE.MAIN_MENU].Visible = true;
  }

  public override void _Process(double delta)
  {
    if (Input.IsActionJustPressed("Quit"))
    {
      GetTree().Quit();
    }
    if (Input.IsActionJustPressed("Pause"))
    {
      GetTree().Paused = !GetTree().Paused;
    }
  }

// TODO Programmatically Handle the adding and removing of scenes (I think the game mode is the only one that requires us do this)
  public void OnPushGameMode(GAME_MODE nextMode)
  {
    GameModeLookup[GameModeStack.Peek()].SetProcess(false);
    GameModeLookup[GameModeStack.Peek()].SetPhysicsProcess(false);
    if (nextMode == GAME_MODE.INSTRUCTIONS)
    {
      InstructionsMode mode = ResourceLoader.Load<PackedScene>("res://Nodes/GameManagement/Modes/Instructions/InstructionsMode.tscn").Instantiate<InstructionsMode>();
      AddChild(mode);
      GameModeLookup[GAME_MODE.INSTRUCTIONS] = mode;
      mode.PopGameMode += OnPopGameMode;
      mode.PushGameMode += OnPushGameMode;
    }
    else if (nextMode == GAME_MODE.LEVEL_INTRO_SCREEN)
    {
      LevelIntroMode mode = ResourceLoader.Load<PackedScene>("res://Nodes/GameManagement/Modes/LevelIntro/LevelIntroMode.tscn").Instantiate<LevelIntroMode>();
      AddChild(mode);
      GameModeLookup[GAME_MODE.LEVEL_INTRO_SCREEN] = mode;
      mode.PopGameMode += OnPopGameMode;
      mode.PushGameMode += OnPushGameMode;
    }
    else if (nextMode == GAME_MODE.GAMEPLAY)
    {
      StatsManager.OnReset();
      MainGameMode mgm = ResourceLoader.Load<PackedScene>("res://Nodes/GameManagement/Modes/MainGame/MainGameMode.tscn").Instantiate<MainGameMode>();
      AddChild(mgm);
      GameModeLookup[GAME_MODE.GAMEPLAY] = mgm;
      mgm.PopGameMode += OnPopGameMode;
      mgm.PushGameMode += OnPushGameMode;
    }
    else if (nextMode == GAME_MODE.RESULTS_SCREEN)
    {
      ResultsMode rm = ResourceLoader.Load<PackedScene>("res://Nodes/GameManagement/Modes/Results/ResultsMode.tscn").Instantiate<ResultsMode>();
      AddChild(rm);
      GameModeLookup[GAME_MODE.RESULTS_SCREEN] = rm;
      rm.PopGameMode += OnPopGameMode;
      rm.PushGameMode += OnPushGameMode;
    }
    else if (nextMode == GAME_MODE.FINAL_RESULTS_SCREEN)
    {
      FinalResultsScreen frs = ResourceLoader.Load<PackedScene>("res://Nodes/GameManagement/Modes/FinalResults/FinalResultsScreen.tscn").Instantiate<FinalResultsScreen>();
      AddChild(frs);
      GameModeLookup[GAME_MODE.FINAL_RESULTS_SCREEN] = frs;
      // how to reset the whole game
    }
    GameModeStack.Push(nextMode);
    GameModeLookup[nextMode].SetProcess(true);
    GameModeLookup[nextMode].SetPhysicsProcess(true);
    GameModeLookup[nextMode].OnModeStart();
    GameModeLookup[GameModeStack.Peek()].Visible = true;
  }

// TODO programmatically DELETE the nodes when we pop them (queue_free is our friend here)
  public void OnPopGameMode()
  {
    if (GameModeStack.TryPop(out GAME_MODE currentMode))
    {
      // handle successful pop
      GameModeLookup[currentMode].QueueFree();
      GameModeLookup[GameModeStack.Peek()].SetProcess(true);
      GameModeLookup[GameModeStack.Peek()].SetPhysicsProcess(true);
      GameModeLookup[GameModeStack.Peek()].OnModeStart();
      GameModeLookup[GameModeStack.Peek()].Visible = true;
    }
  }
}