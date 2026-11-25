using Godot;

public partial class MeleeTower : Area2D
{
	private Timer holdTimer;

	public override void _Ready()
	{
		holdTimer = GetNode<Timer>("HoldTimer");
		AreaEntered += OnEnemyContact;
		holdTimer.Timeout += BreakTower;
	}

	private void OnEnemyContact(Area2D area)
	{
		if (area is not Enemy enemy || enemy.IsQueuedForDeletion())
		{
			return;
		}

		enemy.SetPhysicsProcess(false);

		if (holdTimer.IsStopped())
		{
			holdTimer.Start();
		}
	}

	private void BreakTower()
	{
		foreach (Area2D area in GetOverlappingAreas())
		{
			if (area is Enemy enemy && !enemy.IsQueuedForDeletion())
			{
				enemy.SetPhysicsProcess(true);
			}
		}

		QueueFree();
	}
}
