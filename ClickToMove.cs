using Godot;
using System;

public partial class ClickToMove : CharacterBody3D
{
  public const float Speed = 5.0f;
  public const float JumpVelocity = 4.5f;
  public const int RayLength = 100;

  // Get the gravity from the project settings to be synced with RigidBody nodes.
  public float gravity = ProjectSettings
    .GetSetting("physics/3d/default_gravity")
    .AsSingle();

  public override void _PhysicsProcess(double delta)
  {
    return;
  }

  public override void _Ready()
  {
    if (Input.IsActionJustPressed("LeftMouse"))
    {
      var camera = GetTree().GetNodesInGroup("Camera")[0] as Camera3D;
      var mousePos = GetViewport().GetMousePosition();
      var from = camera.ProjectRayOrigin(mousePos);
      var to = from + camera.ProjectRayNormal(mousePos) * RayLength;
      var space = GetWorld3D().DirectSpaceState;
      var rayQuery = new PhysicsRayQueryParameters3D()
      {
        From = from,
        To = to,
        CollideWithAreas = true
      };
      var result = space.IntersectRay(rayQuery);
      GD.Print(result);
    }
  }
}
