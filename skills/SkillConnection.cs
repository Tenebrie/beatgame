using Godot;

namespace Project;

public partial class SkillConnection : Node
{
	public int PointsInvested = 0;
	public int PointsRequired = 0;
	public BuffFactory PassivePerPoint;
	public int LinkLength;
	public float LinkOffset;

	public BaseSkill Source;
	public BaseSkill Target;
}

/// <summary>Internal class for API definitions only</summary>
public class ApiSkillConnection<T1, T2> : IApiSkillConnection<T1, T2> where T1 : BaseSkill, new() where T2 : BaseSkill, new()
{
	public int _pointsRequired = 0;
	public int PointsRequired { get => _pointsRequired; set => _pointsRequired = value; }
	public BuffFactory _passivePerPoint;
	public BuffFactory PassivePerPoint { get => _passivePerPoint; set => _passivePerPoint = value; }
	public float _linkOffset = 0;
	public float LinkOffset { get => _linkOffset; set => _linkOffset = value; }
	public int _linkLength = 1;
	public int LinkLength { get => _linkLength; set => _linkLength = value; }

	public BaseSkill Source;
	public BaseSkill Target;

	public ApiSkillConnection() { }

	public ApiSkillConnection(int pointsRequired, BuffFactory factory, float offset = 0, int length = 1)
	{
		PointsRequired = pointsRequired;
		PassivePerPoint = factory;
		LinkOffset = offset;
		LinkLength = length;
	}
}

/// <summary>Internal interface for API definitions only</summary>
public interface IApiSkillConnection<out T1, out T2> where T1 : BaseSkill where T2 : BaseSkill
{
	public int PointsRequired { get; set; }
	public BuffFactory PassivePerPoint { get; set; }
	public int LinkLength { get; set; }
	public float LinkOffset { get; set; }
}