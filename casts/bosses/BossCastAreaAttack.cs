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
			var circle = this.CreateGroundCircularArea(point);
			circle.Radius = AreaRadius;
			circle.GrowTime = Settings.HoldTime;
			circle.Alliance = UnitAlliance.Hostile;
			circle.TargetValidator = (target) => target.HostileTo(Parent);
			circle.OnFinishedPerTargetCallback = (target) =>
			{
				target.Health.Damage(40, this);
			};
		}
	}
}