namespace Project;
public partial class BossCastAreaAttack : BaseCast
{
	public float AreaRadius = 8;

	public BossCastAreaAttack(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Ground Slap",
			TargetType = CastTargetType.Point,
			InputType = CastInputType.AutoRelease,
			HoldTime = 8,
			RecastTime = 0,
		};
	}

	protected override void OnCastStarted(CastTargetData targetData)
	{
		foreach (var point in targetData.MultitargetPoints)
		{
			var circle = this.CreateCircularTelegraph(point);
			circle.Settings.Radius = AreaRadius;
			circle.Settings.GrowTime = Settings.HoldTime;
			circle.Settings.Alliance = UnitAlliance.Hostile;
			circle.Settings.TargetValidator = (target) => target.HostileTo(Parent);
			circle.Settings.OnFinishedPerTargetCallback = (target) =>
			{
				target.Health.Damage(40, this);
			};
		}
	}
}