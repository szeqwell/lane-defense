using Godot;

public partial class Projectile : Area2D
{
	[Export] public float Speed { get; set; } = 450f;
	[Export] public int Damage { get; set; } = 1;
	

	private double timeLeft = 3.0;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Vector2.Right * Speed * (float)delta;

		timeLeft -= delta;
		if (timeLeft <= 0)
		{
			QueueFree();
		}
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is not Enemy enemy ||
			IsQueuedForDeletion() || enemy.IsQueuedForDeletion())
		{
			return;
		}

		enemy.TakeDamage(Damage);
		QueueFree();
	}
}
