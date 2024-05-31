using System.Diagnostics;
using Godot;

namespace Project;

public partial class PlayerCastBar : Control
{
	private BaseCast ActiveCast;
	private long CastStartedAt;
	private long CastEndsAt;
	private bool LastCastTimingValid;

	private ProgressBar Bar;
	private ProgressBar GreenZone;

	public override void _Ready()
	{
		Bar = GetNode<ProgressBar>("ProgressBar");
		GreenZone = GetNode<ProgressBar>("GreenZone");
		SignalBus.Singleton.CastStarted += OnCastStarted;
		SignalBus.Singleton.CastPerformed += OnCastPerformed;
		SignalBus.Singleton.CastFailed += OnCastFailed;
	}

	public void OnCastStarted(BaseCast cast)
	{
		if (cast.Parent is not PlayerController || cast.Settings.InputType == CastInputType.Instant)
			return;

		ActiveCast = cast;
		Bar.Value = (float)Music.Singleton.GetCurrentBeatOffset(cast.Settings.CastTimings) / 1000;
		Bar.MaxValue = cast.Settings.HoldTime * (1f / Music.Singleton.BeatsPerMinute * 60);
		if (cast.Settings.InputType == CastInputType.HoldRelease)
			Bar.MaxValue *= 2;
		CastStartedAt = (long)Time.Singleton.GetTicksMsec() - Music.Singleton.GetCurrentBeatOffset(cast.Settings.CastTimings);
		CastEndsAt = CastStartedAt + (long)(Bar.MaxValue * 1000);

		Bar.SetFillColor(new Color(255, 255, 75, 1));
		Bar.SetBackgroundOpacity(.5f);
		GreenZone.SetFillOpacity(.5f);

		var timingWindow = (float)AccurateTimer.TimingWindow / 1000;
		var totalWidth = Bar.Size.X;
		if (cast.Settings.InputType == CastInputType.HoldRelease)
		{
			GreenZone.Size = new Vector2(timingWindow * 2 / (float)Bar.MaxValue * totalWidth, GreenZone.Size.Y);
			GreenZone.Position = new Vector2(Bar.Position.X + totalWidth / 2 - GreenZone.Size.X / 2, Bar.Position.Y);
		}
		else
		{
			GreenZone.Size = new Vector2(timingWindow / (float)Bar.MaxValue * totalWidth, GreenZone.Size.Y);
			GreenZone.Position = new Vector2(Bar.Position.X + totalWidth - GreenZone.Size.X, Bar.Position.Y);
		}
	}

	public void OnCastPerformed(BaseCast cast)
	{
		if (cast != ActiveCast)
			return;

		ActiveCast = null;
		UpdateBarValue();

		Bar.SetFillColor(new Color(0, 255, 0, Bar.GetFillColor().A));
	}

	public void OnCastFailed(BaseCast cast)
	{
		if (cast != ActiveCast)
			return;

		ActiveCast = null;
		UpdateBarValue();

		Bar.SetFillColor(new Color(255, 0, 0, Bar.GetFillColor().A));
	}

	public override void _Process(double delta)
	{
		if (ActiveCast != null)
		{
			UpdateBarValue();
			return;
		}

		Bar.SetFillOpacity(Bar.GetFillOpacity() - delta);
		Bar.SetBackgroundOpacity(Bar.GetBackgroundOpacity() - delta);
		GreenZone.SetFillOpacity(GreenZone.GetFillOpacity() - delta);
	}

	private void UpdateBarValue()
	{
		var time = (long)Time.Singleton.GetTicksMsec();
		if (ActiveCast == null)
			return;

		if (ActiveCast.Settings.ReversedCastBar)
			Bar.Value = (1 - (float)(time - CastStartedAt) / (CastEndsAt - CastStartedAt)) * Bar.MaxValue;
		else
			Bar.Value = (float)(time - CastStartedAt) / (CastEndsAt - CastStartedAt) * Bar.MaxValue;
	}
}
