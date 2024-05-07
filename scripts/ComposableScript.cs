using Godot;

namespace Project;

public abstract class ComposableScript
{
	public BaseUnit Parent;

	public ComposableScript(BaseUnit parent)
	{
		this.Parent = parent;
	}
	public virtual void _Ready() { }

	public virtual void _Process(double delta) { }

	public virtual void _Input(InputEvent @event) { }

	public virtual void _ExitTree() { }
}