using System;
using System.Diagnostics;
using Godot;

namespace Project;

public partial class PlayerCastBar : Control
{
	private BaseCast ActiveCast;
	private double CastStartedAt;
	private double CastEndsAt;
	private bool LastCastTimingValid;

	private ProgressBar Bar;
	private ProgressBar GreenZone;
	private Label CastNameLabel;
	private TextureRect CastIconTexture;

	public override void _Ready()
	{
		Bar = GetNode<ProgressBar>("ProgressBar");
		GreenZone = GetNode<ProgressBar>("GreenZone");
		CastNameLabel = GetNode<Label>("ProgressBar/CastNameLabel");
		CastIconTexture = GetNode<TextureRect>("ProgressBar/AspectRatioContainer/CastIconTexture");
		SignalBus.Singleton.CastStarted += OnCastStarted;
		SignalBus.Singleton.CastCompleted += OnCastCompleted;
		SignalBus.Singleton.CastFailed += OnCastFailed;
	}

	public void OnCastStarted(BaseCast cast)
	{
		if (cast.Parent is not PlayerController || cast.Settings.InputType == CastInputType.Instant)
			return;

		ActiveCast = cast;
		Bar.MaxValue = cast.Settings.HoldTime * (1f / Music.Singleton.BeatsPerMinute * 60);
		CastStartedAt = CastUtils.GetEngineTime();
		CastEndsAt = CastStartedAt + Bar.MaxValue;

		CastNameLabel.Text = cast.Settings.FriendlyName;
		CastIconTexture.Texture = GD.Load<CompressedTexture2D>(cast.Settings.IconPath);

		Bar.SetFillColor(new Color(0.4f, 0.4f, 0.6f));
		Bar.SetBackgroundOpacity(.5f);
		GreenZone.SetFillOpacity(.3f);
		SetCastBarAlpha(1.0f);

		var timingWindow = Music.Singleton.TimingWindow;
		var totalWidth = Bar.Size.X;
		GreenZone.Size = new Vector2(totalWidth * timingWindow / (float)Bar.MaxValue, GreenZone.Size.Y);
		GreenZone.Position = new Vector2(Bar.Position.X + totalWidth - GreenZone.Size.X, Bar.Position.Y);
		UpdateBarValue();
	}

	public void OnCastCompleted(BaseCast cast)
	{
		if (cast != ActiveCast)
			return;

		UpdateBarValue(forceFull: true);
		ActiveCast = null;

		Bar.SetFillColor(new Color(0.15f, 0.5f, 0.15f));
	}

	public void OnCastFailed(BaseCast cast)
	{
		if (cast != ActiveCast)
			return;

		var time = CastUtils.GetEngineTime();
		if (Math.Abs(time - cast.CastStartedAt) <= Music.Singleton.TimingWindow)
		{
			Bar.SetFillColor(new Color(0.15f, 0.5f, 0.15f));
		}
		else
			Bar.SetFillColor(new Color(0.5f, 0.15f, 0.15f));

		UpdateBarValue();
		ActiveCast = null;
	}

	public override void _Process(double delta)
	{
		if (ActiveCast != null)
			UpdateBarValue();
		else
			SetCastBarAlpha(Modulate.A - (float)delta);
	}

	void SetCastBarAlpha(float value)
	{
		float clampedValue = Math.Clamp(value, 0, 1);
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, clampedValue);
		(CastIconTexture.Material as ShaderMaterial).SetShaderParameter("Modulate", clampedValue);
	}

	private void UpdateBarValue(bool forceFull = false)
	{
		var time = CastUtils.GetEngineTime();
		if (ActiveCast == null)
			return;

		var value = (time - CastStartedAt) / (CastEndsAt - CastStartedAt);
		if (forceFull)
			value = 1;
		if (ActiveCast.Settings.ReversedCastBar)
			Bar.Value = (1 - value) * Bar.MaxValue;
		else
			Bar.Value = value * Bar.MaxValue;
	}
}
