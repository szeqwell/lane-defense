using Godot;

public partial class Main : Node2D
{
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public PackedScene HeavyEnemyScene { get; set; }

	private int destroyedBarricades;
	private int waveNumber;
	private Button startWaveButton;
	private int enemiesRemaining;

	public override void _Ready()
	{


		startWaveButton = GetNode<Button>("StartWaveButton");
		startWaveButton.Pressed += StartWave;

		foreach (Node child in GetChildren())
		{
			if (child.GetNodeOrNull<Barricade>("Barricade") is Barricade barricade)
			{
				barricade.Destroyed += OnBarricadeDestroyed;
			}
		}
	}

	private void StartWave()
{

	if (enemiesRemaining > 0 || destroyedBarricades >= 2)
	{
		return;
	}

	waveNumber++;
	PackedScene scene = waveNumber % 2 == 0 ? HeavyEnemyScene : EnemyScene;

	foreach (Node child in GetChildren())
	{
		if (child is not Node2D lane ||
			lane.GetNodeOrNull<Barricade>("Barricade") is null)
		{
			continue;
		}

		Enemy enemy = scene.Instantiate<Enemy>();
		enemy.Position = new Vector2(758, 0);
		enemy.TreeExited += OnEnemyExited;

		enemiesRemaining++;
		lane.AddChild(enemy);
	}

	startWaveButton.Disabled = enemiesRemaining > 0;
}

private void OnEnemyExited()
{
	enemiesRemaining--;

	if (enemiesRemaining == 0 && destroyedBarricades < 2)
	{
		startWaveButton.Disabled = false;
	}
}

	private void OnBarricadeDestroyed()
	{
		destroyedBarricades++;

		if (destroyedBarricades == 2)
		{
			GetTree().Paused = true;
			GetNode<AcceptDialog>("GameOverDialog").PopupCentered();
		}
	}
}
