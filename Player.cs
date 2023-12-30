using Godot;
using System;

public partial class Player : CharacterBody3D
{
  private const float Speed = 10.0f;
  private const int RayLength = 100;
  private NavigationAgent3D navigationAgent;

  public override void _Ready()
  {
    navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
  }

  public override void _PhysicsProcess(double delta)
  {
    if (navigationAgent.IsNavigationFinished())
    {
      return;
    }

    MoveToPoint(delta, Speed);
  }

  private void MoveToPoint(double delta, float speed)
  {
    var targetPos = navigationAgent.GetNextPathPosition();
    var direction = GlobalPosition.DirectionTo(targetPos);

    if (!GlobalTransform.Origin.IsEqualApprox(targetPos))
    {
      GetNode<Node3D>("Model").LookAt(targetPos, Vector3.Up);
    }

    Velocity = direction * speed;
    MoveAndSlide();
  }

  private void updateNavigationAgentTarget()
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

    if (result.Count > 1)
    {
      navigationAgent.TargetPosition = (Vector3)result["position"];
    }
  }

  public override void _Input(InputEvent @event)
  {
    if (Input.IsActionJustPressed("LeftMouse"))
    {
      updateNavigationAgentTarget();
    }
  }
}
