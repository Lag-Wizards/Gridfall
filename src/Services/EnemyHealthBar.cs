using Godot;
using System;
using Gridfall.Domain;

public partial class EnemyHealthBar : Node2D
{
	private CombatUnit _unit;
	private ProgressBar _progressBar;
	private Label _hpLabel;

	private const float Width = 32f;
	private const float Height = 6f;
	private float _yOffset = -18f;

	private StyleBoxFlat _fillStyle;

	public EnemyHealthBar(CombatUnit unit)
	{
		_unit = unit;
		ZIndex = 5; 
	}

	public override void _Ready()
	{
		if (_unit != null)
		{
			_unit.OnHealthChanged += UpdateHealthVisuals;
		}

		var sprite = GetParent()?.GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite != null && sprite.Texture != null)
		{
			float textureHeight = sprite.Texture.GetHeight();
			float top = -textureHeight / 2f;
			_yOffset = top - 8f; 
		}

		CreateProgressBar();
		CreateHpLabel();
		UpdateHealthVisuals();
	}

	public override void _ExitTree()
	{
		if (_unit != null)
		{
			_unit.OnHealthChanged -= UpdateHealthVisuals;
		}
	}

	private void CreateProgressBar()
	{
		_progressBar = new ProgressBar();
		_progressBar.Name = "HpBar";
		_progressBar.ShowPercentage = false;
		
		_progressBar.Size = new Vector2(Width, Height);
		_progressBar.Position = new Vector2(-Width / 2f, _yOffset);

		var bgStyle = new StyleBoxFlat();
		bgStyle.BgColor = new Color(0.12f, 0.12f, 0.12f, 0.9f);
		bgStyle.CornerRadiusTopLeft = 2;
		bgStyle.CornerRadiusTopRight = 2;
		bgStyle.CornerRadiusBottomLeft = 2;
		bgStyle.CornerRadiusBottomRight = 2;
		bgStyle.BorderWidthLeft = 1;
		bgStyle.BorderWidthTop = 1;
		bgStyle.BorderWidthRight = 1;
		bgStyle.BorderWidthBottom = 1;
		bgStyle.BorderColor = new Color(0.35f, 0.35f, 0.35f, 1f);
		bgStyle.AntiAliasing = true;
		_progressBar.AddThemeStyleboxOverride("background", bgStyle);

		_fillStyle = new StyleBoxFlat();
		_fillStyle.CornerRadiusTopLeft = 2;
		_fillStyle.CornerRadiusTopRight = 2;
		_fillStyle.CornerRadiusBottomLeft = 2;
		_fillStyle.CornerRadiusBottomRight = 2;
		_fillStyle.AntiAliasing = true;
		_progressBar.AddThemeStyleboxOverride("fill", _fillStyle);

		AddChild(_progressBar);
	}

	private void CreateHpLabel()
	{
		_hpLabel = new Label();
		_hpLabel.Name = "HpLabel";
		_hpLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_hpLabel.VerticalAlignment = VerticalAlignment.Center;

		var labelSettings = new LabelSettings();
		labelSettings.FontSize = 18;
		labelSettings.OutlineSize = 3;
		labelSettings.OutlineColor = Colors.Black;
		labelSettings.FontColor = Colors.White;
		_hpLabel.LabelSettings = labelSettings;

		_hpLabel.CustomMinimumSize = new Vector2(100, 24);
		_hpLabel.Position = new Vector2(-50, _yOffset - 16f);

		_hpLabel.Scale = new Vector2(0.38f, 0.38f);
		_hpLabel.PivotOffset = new Vector2(50, 12); 

		AddChild(_hpLabel);
	}

	private void UpdateHealthVisuals()
	{
		if (_unit == null) return;

		float healthPercent = Mathf.Clamp((float)_unit.Health / _unit.MaxHealth, 0f, 1f);

		if (_progressBar != null)
		{
			_progressBar.Value = healthPercent * 100f;
			if (_fillStyle != null)
			{
				_fillStyle.BgColor = GetHealthColor(healthPercent);
			}
		}

		if (_hpLabel != null)
		{
			_hpLabel.Text = $"{_unit.Health}/{_unit.MaxHealth}";
		}
	}

	private Color GetHealthColor(float percent)
	{
		if (percent > 0.5f)
			return new Color(0.2f, 0.85f, 0.2f);
		if (percent > 0.2f)
			return new Color(0.95f, 0.75f, 0.15f);
		return new Color(0.85f, 0.15f, 0.15f);
	}
}
