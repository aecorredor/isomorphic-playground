using Godot;
using System;

public partial class Player : CharacterBody3D
{
  private const float Speed = 5.0f;
  private const int RayLength = 100;
  private NavigationAgent3D navigationAgent;
  private Node3D model;

  public override void _Ready()
  {
    navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
    model = GetNode<Node3D>("Model");
  }

  public override void _PhysicsProcess(double delta)
  {
    updateAnimationState();
    if (navigationAgent.IsNavigationFinished())
    {
      return;
    }

    MoveToPoint(delta, Speed);
  }

  private void updateAnimationState()
  {
    var animationTree = model.GetNode<AnimationTree>("AnimationTree");

    animationTree.Set(
      "parameters/conditions/idle",
      navigationAgent.IsNavigationFinished()
    );
    animationTree.Set(
      "parameters/conditions/run",
      !navigationAgent.IsNavigationFinished()
    );
  }

  private void MoveToPoint(double delta, float speed)
  {
    var targetPos = navigationAgent.GetNextPathPosition();
    var direction = GlobalPosition.DirectionTo(targetPos);

    if (!GlobalTransform.Origin.IsEqualApprox(targetPos))
    {
      // Lerp look at
      // Create a target rotation basis from the direction
      var targetTransform = Transform.LookingAt(targetPos, Vector3.Up);
      var targetRotation = new Quaternion(targetTransform.Basis);
      // Slerp from current rotation to target rotation
      var currentRotation = new Quaternion(model.Transform.Basis);
      var newRotation = currentRotation.Slerp(
        targetRotation,
        speed * (float)delta
      );
      model.Transform = new Transform3D(
        new Basis(newRotation),
        model.Transform.Origin
      );
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
