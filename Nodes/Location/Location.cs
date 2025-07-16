using Godot;
using Godot.Collections;

public partial class Location : Node2D
{
  [Export] Area2D CollisionArea;
  [Export] Sprite2D Sprite;

  public bool IsOverlappedDoor = false;
  public bool IsPlayerInShop = false;
  public override void _Ready()
  {
    base._Ready();
    PlayerCharacter pc = (PlayerCharacter)InstanceFromId(PlayerCharacter.PlayerId);
    pc.PlayerEnterShop += OnPlayerEnterShop;
    pc.PlayerExitShop += OnPlayerExitShop;
  }

  public override void _PhysicsProcess(double delta)
  {
    Sprite.Frame = 0;
    if (CollisionArea.HasOverlappingBodies())
    {
      Array<Node2D> Bodies = CollisionArea.GetOverlappingBodies();
      foreach (Node2D Body in Bodies)
      {
        if (Body.GetInstanceId() == PlayerCharacter.PlayerId)
        {
          if (PlayerCharacter.PlayerState != StateReference.SHOPPING)
          {
            Sprite.Frame = 1;
          }
          PlayerCharacter pc = Body as PlayerCharacter;
          pc.SetIsOnDoor(true);
          IsOverlappedDoor = true;
        }
      }
    }
  }

  public void OnPlayerEnterShop()
  {
    if (IsOverlappedDoor)
    {
      IsPlayerInShop = true;
      Sprite.Frame = 0;
    }
  }

  public void OnPlayerExitShop()
  {
    if (IsOverlappedDoor)
    {
      IsPlayerInShop = false;
      Sprite.Frame = 1;
    }
  }
}
