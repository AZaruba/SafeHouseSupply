using Godot;
using System;

public partial class ScoreDisplay : RichTextLabel
{

  public static ScoreDisplay Instance;

  public static void WriteString(string input)
  {
    if (Instance != null)
    {
      Instance.Text = input;
    }
  }
  public override void _Ready()
  {
    base._Ready();
    Instance = this;
  }

  public override void _Process(double delta)
  {
    base._Process(delta);
  }
}
