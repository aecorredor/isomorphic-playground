using Godot;
using System;

public partial class Camera3D : Godot.Camera3D
{
  private CharacterBody3D player;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    player = GetNode<CharacterBody3D>("../Player");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    GlobalTransform = player.GetNode<Node3D>("CameraPoint").GlobalTransform;
  }
}
