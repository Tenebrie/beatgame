namespace Project;

public class SkillLink<T1, T2> : ISkillConnectionData<T1, T2> where T1 : BaseSkill, new() where T2 : BaseSkill, new()
{
	public int _pointsRequired = 1;
	public int PointsRequired { get => _pointsRequired; set => _pointsRequired = value; }
	public BaseBuff _passivePerPoint;
	public BaseBuff PassivePerPoint { get => _passivePerPoint; set => _passivePerPoint = value; }

	public BaseSkill Source;
	public BaseSkill Target;

	public SkillLink()
	{

	}

	public SkillLink(int pointsRequired, BaseBuff passivePerPoint)
	{
		PointsRequired = pointsRequired;
		PassivePerPoint = passivePerPoint;
	}
}

public interface ISkillConnectionData<out T1, out T2> where T1 : BaseSkill where T2 : BaseSkill
{
	public int PointsRequired { get; set; }
	public BaseBuff PassivePerPoint { get; set; }
}

public class InternalSkillLink
{
	public int PointsRequired = 1;
	public BaseBuff PassivePerPoint;

	public BaseSkill Source;
	public BaseSkill Target;

	public InternalSkillLink()
	{

	}
}