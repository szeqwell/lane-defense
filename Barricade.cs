using Godot;

public partial class Barricade : Area2D
{
	[Signal]
public delegate void DestroyedEventHandler();
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is not Enemy enemy || IsQueuedForDeletion() || enemy.IsQueuedForDeletion())
		{
			return;
		}
GD.Print("Enemy hit the barricade");
		enemy.QueueFree();
		enemy.QueueFree();
EmitSignal(SignalName.Destroyed);
QueueFree();
		QueueFree();
	}
}
