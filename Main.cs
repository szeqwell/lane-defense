using Godot;

public partial class Main : Node2D
{
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public PackedScene HeavyEnemyScene { get; set; }
	[Export] public PackedScene ProjectileScene { get; set; }
	[Export] public PackedScene RangedTowerScene { get; set; }
	[Export] public PackedScene MeleeTowerScene { get; set; }

	private int destroyedBarricades;
	private int waveNumber;
	private Button startWaveButton;
	private TextureButton rangedTowerButton;
	private TextureButton meleeTowerButton;
	private TextureButton selectedTowerButton;
	private int enemiesRemaining;

	public override void _Ready()
	{
		rangedTowerButton = GetNode<TextureButton>("TowerButtons/RangedTowerButton");
		meleeTowerButton = GetNode<TextureButton>("TowerButtons/MeleeTowerButton");

		rangedTowerButton.Pressed += () => ToggleTowerSelection(rangedTowerButton);
		meleeTowerButton.Pressed += () => ToggleTowerSelection(meleeTowerButton);

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

public override void _Input(InputEvent inputEvent)
{
	if (selectedTowerButton is null)
	{
		return;
	}

	bool cancel = inputEvent is InputEventMouseButton
		{ ButtonIndex: MouseButton.Right, Pressed: true }
		|| inputEvent is InputEventKey
		{ Keycode: Key.Q, Pressed: true, Echo: false };

	if (cancel)
	{
		SetTowerSelection(null);
		GetViewport().SetInputAsHandled();
	}
}


	private void ToggleTowerSelection(TextureButton button)
	{
		SetTowerSelection(selectedTowerButton is null ? button : null);
	}
	public override void _UnhandledInput(InputEvent inputEvent)
	{
		if (selectedTowerButton is null ||
			inputEvent is not InputEventMouseButton click ||
			click.ButtonIndex != MouseButton.Left ||
			!click.Pressed)
		{
			return;
		}

		const float cellWidth = 80f;
		const int columnCount = 8;
		Rect2 placementArea = new Rect2(64, -52, cellWidth * columnCount, 104);

		foreach (Node child in GetChildren())
		{
			if (child is not Node2D lane || !lane.HasNode("Track"))
			{
				continue;
			}

			Vector2 point = lane.GetGlobalTransformWithCanvas().AffineInverse() * click.Position;
			if (!placementArea.HasPoint(point))
			{
				continue;
			}

			int column = Mathf.FloorToInt((point.X - placementArea.Position.X) / cellWidth);
			float cellCenterX = placementArea.Position.X + (column + 0.5f) * cellWidth;

			foreach (Node laneChild in lane.GetChildren())
			{
				if (laneChild is Node2D existingTower &&
					(existingTower is RangedTower or MeleeTower) &&
					Mathf.IsEqualApprox(existingTower.Position.X, cellCenterX))
				{
					return;
				}
			}

			PackedScene scene = selectedTowerButton == rangedTowerButton
				? RangedTowerScene
				: MeleeTowerScene;

			Node2D tower = scene.Instantiate<Node2D>();
			tower.Position = new Vector2(cellCenterX, 0);
			lane.AddChild(tower);
			SetTowerSelection(null);
			return;
		}
	}

	private void SetTowerSelection(TextureButton button)
	{
		selectedTowerButton = button;
		Color dimmed = new Color(1f, 1f, 1f, 0.35f);

		rangedTowerButton.Modulate = button == meleeTowerButton ? dimmed : Colors.White;
		meleeTowerButton.Modulate = button == rangedTowerButton ? dimmed : Colors.White;
	}

	private void StartWave()
{
	SetTowerSelection(null);

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
