using Godot;
using Godot.Collections;

public partial class Location : Node2D
{
  [Export] Area2D CollisionArea;
  [Export] Sprite2D Sprite;
  public override void _Ready()
  {
    base._Ready();
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
          Sprite.Frame = 1;
          PlayerCharacter pc = Body as PlayerCharacter;
          pc.SetIsOnDoor(true);
        }
      }
    }
  }
}
