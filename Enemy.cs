using Godot;

public partial class Enemy : Area2D
{
	[Export] public float Speed { get; set; } = 120f;
	[Export] public int MaxHealth { get; set; } = 1;
	

private int health;

public override void _Ready()
{
	health = MaxHealth;
}

public void TakeDamage(int amount)
{
	health -= amount;

	if (health <= 0)
	{
		QueueFree();
	}
}

	public override void _PhysicsProcess(double delta)
	{
		Position += Vector2.Left * Speed * (float)delta;
	}
}
