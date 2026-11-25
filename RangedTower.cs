using Godot;

public partial class RangedTower : Node2D
{
	[Export] public PackedScene ProjectileScene { get; set; }

	private Node2D lane;
	private Marker2D muzzle;

	public override void _Ready()
	{
		lane = GetParent<Node2D>();
		muzzle = GetNode<Marker2D>("Muzzle");

		Timer fireTimer = GetNode<Timer>("FireTimer");
		fireTimer.Timeout += TryFire;
		fireTimer.Start();
	}

	private void TryFire()
	{
		foreach (Node child in lane.GetChildren())
		{
			if (child is Enemy enemy &&
				!enemy.IsQueuedForDeletion() &&
				enemy.Position.X > Position.X)
			{
				Projectile shot = ProjectileScene.Instantiate<Projectile>();
				shot.Position = lane.ToLocal(muzzle.GlobalPosition);
				lane.AddChild(shot);
				return;
			}
		}
	}
}
