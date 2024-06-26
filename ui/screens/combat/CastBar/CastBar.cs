using Godot;

namespace Project;

public partial class CastBar : Control
{
	[Signal] public delegate void FinishedEventHandler();

	ProgressBar progressBar;
	Label label;

	BaseCast trackedCast;
	float castStartedAt;
	float castFinishesAt;
	float prepFinishesAt;

	bool isFinished;
	bool isInterrupted;
	float fadeOutSpeed = 1;

	public override void _Ready()
	{
		progressBar = GetNode<ProgressBar>("ProgressBar");
		label = GetNode<Label>("Label");
	}

	public override void _Process(double delta)
	{
		if (!isFinished || !isInterrupted)
			UpdateBar();

		if (isFinished)
			FadeOut((float)delta);
	}

	void UpdateBar()
	{
		var time = (float)Time.GetTicksMsec();
		var value = (time - prepFinishesAt) / (castFinishesAt - prepFinishesAt);
		if (time < prepFinishesAt)
			value = (time - castStartedAt) / (prepFinishesAt - castStartedAt);

		if (trackedCast.Settings.PrepareTime > 0 && time > prepFinishesAt)
			value = 1 - value;

		if (trackedCast.Settings.ReversedCastBar)
			value = 1 - value;

		progressBar.Value = value * 100;
	}

	void FadeOut(float delta)
	{
		var color = progressBar.Modulate;
		var newColor = new Color(color.R, color.G, color.B, color.A - delta * fadeOutSpeed);
		progressBar.Modulate = newColor;
		label.Modulate = newColor;
		if (newColor.A <= 0)
			QueueFree();
	}

	public void TrackCast(BaseCast cast)
	{
		trackedCast = cast;
		label.Text = trackedCast.Settings.FriendlyName;

		castStartedAt = Time.GetTicksMsec();
		prepFinishesAt = castStartedAt + trackedCast.Settings.PrepareTime * Music.Singleton.SecondsPerBeat * 1000;
		castFinishesAt = prepFinishesAt + trackedCast.Settings.HoldTime * Music.Singleton.SecondsPerBeat * 1000;
		UpdateBar();

		cast.Completed += OnCastCompleted;
		cast.Interrupted += OnCastInterrupted;
		cast.Failed += OnCastFailed;
	}

	void UntrackCast()
	{
		if (trackedCast == null)
			return;

		trackedCast.Completed -= OnCastCompleted;
		trackedCast.Interrupted -= OnCastInterrupted;
		trackedCast.Failed -= OnCastFailed;

		isFinished = true;
		EmitSignal(SignalName.Finished);
	}

	void OnCastCompleted()
	{
		fadeOutSpeed = 4;
		progressBar.SetFillColor(new Color(0, 0.75f, 0, progressBar.GetFillColor().A));
		UntrackCast();
	}

	void OnCastInterrupted()
	{
		isInterrupted = true;
		fadeOutSpeed = 0.25f;
		progressBar.SetFillColor(new Color(0.75f, 0, 0, progressBar.GetFillColor().A));
		UntrackCast();
	}

	void OnCastFailed()
	{
		fadeOutSpeed = 2;
		progressBar.SetFillColor(new Color(0.75f, 0, 0, progressBar.GetFillColor().A));
		UntrackCast();
	}
}
