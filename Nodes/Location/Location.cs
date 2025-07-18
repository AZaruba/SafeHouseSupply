using System;
using Godot;
using Godot.Collections;

public enum LocationName
{
  NONE,
  SAFE_HOUSE,
  BANK,
  TAILOR,
  GROCERY,
  TOOLS,
  GIFTS,
  PHARMACY
}

public partial class Location : Node2D
{
  [Export] Area2D CollisionArea;
  [Export] Sprite2D Sprite;
  [Export] LocationName LocationName;

  [Signal] public delegate void ThisShopEnteredEventHandler(int LocationID);

  public static LocationName CurrentLocation;

  public bool IsOverlappedDoor = false;
  public bool IsPlayerInShop = false;
  public override void _Ready()
  {
    base._Ready();
    PlayerCharacter pc = (PlayerCharacter)InstanceFromId(PlayerCharacter.PlayerId);
    pc.PlayerEnterShop += OnPlayerEnterShop;
    pc.PlayerExitShop += OnPlayerExitShop;
    ThisShopEntered += StatsManager.OnShopEntered;
  }
  

  public override void _PhysicsProcess(double delta)
  {
    Sprite.Frame = 0;
    IsOverlappedDoor = false;
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
      CurrentLocation = this.LocationName;
      GD.Print("Entering " + LocationName);
      EmitSignal(SignalName.ThisShopEntered, (int)LocationName);
      Sprite.Frame = 0;
    }
  }

  public void OnPlayerExitShop()
  {
    if (IsOverlappedDoor)
    {
      IsPlayerInShop = false;
      CurrentLocation = LocationName.NONE;
      Sprite.Frame = 1;
    }
  }
}
