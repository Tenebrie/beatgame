using Godot;

namespace Project;

public partial class SkillConnection : Node
{
	public int PointsInvested = 0;
	public int PointsRequired = 0;
	public BuffFactory PassivePerPoint;

	public BaseSkill Source;
	public BaseSkill Target;

	public SkillConnection()
	{

	}
}

/// <summary>Internal class for API definitions only</summary>
public class ApiSkillConnection<T1, T2> : IApiSkillConnection<T1, T2> where T1 : BaseSkill, new() where T2 : BaseSkill, new()
{
	public int _pointsRequired = 0;
	public int PointsRequired { get => _pointsRequired; set => _pointsRequired = value; }
	public BuffFactory _passivePerPoint;
	public BuffFactory PassivePerPoint { get => _passivePerPoint; set => _passivePerPoint = value; }

	public BaseSkill Source;
	public BaseSkill Target;

	public ApiSkillConnection() { }

	public ApiSkillConnection(int pointsRequired, BuffFactory factory)
	{
		PointsRequired = pointsRequired;
		PassivePerPoint = factory;
	}
}

/// <summary>Internal interface for API definitions only</summary>
public interface IApiSkillConnection<out T1, out T2> where T1 : BaseSkill where T2 : BaseSkill
{
	public int PointsRequired { get; set; }
	public BuffFactory PassivePerPoint { get; set; }
}