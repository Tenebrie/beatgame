using Godot;
namespace Project;

public partial class ObjectForcefulMovement : ComposableScript
{
	public Vector3 Inertia = new();

	public ObjectForcefulMovement(BaseUnit parent) : base(parent) { }	

	public override void _Ready()
	{
	}

	public void Push(Vector3 distanceToMove)
	{
		
	}

	public override void _ExitTree()
	{
	}

	public override void _Process(double delta)
	{

	}
}
