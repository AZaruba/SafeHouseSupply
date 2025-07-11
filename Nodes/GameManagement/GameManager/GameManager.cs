using System.Collections.Generic;
using Godot;

public partial class GameManager : Node
{
  private Stack<GAME_MODE> GameModeStack;

  [Export] Godot.Collections.Dictionary<GAME_MODE, ModeManager> GameModeLookup = [];

  Node ActiveGameModeNode;

  // on game start
  public override void _Ready()
  {
    GameModeStack = new Stack<GAME_MODE>();
    GameModeStack.Push(GAME_MODE.MAIN_MENU);
    GameModeLookup[GAME_MODE.MAIN_MENU].SetProcess(true);
    GameModeLookup[GAME_MODE.MAIN_MENU].Visible = true;
  }

  public void OnPushGameMode(GAME_MODE nextMode)
  {
    GameModeLookup[GameModeStack.Peek()].SetProcess(false);
    GameModeLookup[nextMode].SetPhysicsProcess(false);
    GameModeLookup[GameModeStack.Peek()].Visible = false;
    GameModeStack.Push(nextMode);
    GameModeLookup[nextMode].SetProcess(true);
    GameModeLookup[nextMode].SetPhysicsProcess(true);
    GameModeLookup[GameModeStack.Peek()].Visible = true;
  }

  public void OnPopGameMode()
  {
    if (GameModeStack.TryPop(out GAME_MODE currentMode))
    {
      // handle successful pop
      GameModeLookup[currentMode].SetProcess(false);
      GameModeLookup[currentMode].SetPhysicsProcess(false);
      GameModeLookup[currentMode].Visible = false;
      GameModeLookup[GameModeStack.Peek()].SetProcess(true);
      GameModeLookup[GameModeStack.Peek()].SetPhysicsProcess(true);
      GameModeLookup[GameModeStack.Peek()].Visible = true;
    }
  }
}