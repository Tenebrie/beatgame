using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;
namespace Project;

public static class CastUtils
{

	public static GroundAreaCircle CreateGroundCircularArea(this Node node, Vector3 point)
	{
		var circle = Lib.LoadScene(Lib.Effect.GroundAreaCircle).Instantiate<GroundAreaCircle>();
		circle.Position = point;
		node.GetTree().CurrentScene.AddChild(circle);
		circle.EnableCulling();
		return circle;
	}

	public static GroundAreaRect CreateGroundRectangularArea(this Node node, Vector3 point)
	{
		var rect = Lib.LoadScene(Lib.Effect.GroundAreaRect).Instantiate<GroundAreaRect>();
		rect.Position = point;
		node.GetTree().CurrentScene.AddChild(rect);
		rect.EnableCulling();
		return rect;
	}

	public static LightningZapEffect CreateZapEffect(this Node node, Vector3 from, Vector3 to)
	{
		var zap = Lib.LoadScene(Lib.Effect.LightningZap).Instantiate<LightningZapEffect>();
		zap.Position = from;
		node.GetTree().CurrentScene.AddChild(zap);
		zap.SetTarget(to);
		zap.FadeDuration = 0.50f;
		zap.AnimationFinished += () => node.CreateSimpleParticleEffect(Lib.Effect.LightningZapImpact, to).SetLifetime(0.05f);
		return zap;
	}

	public static SimpleParticleEffect CreateSimpleParticleEffect(this Node node, string resourcePath, Vector3 target)
	{
		var impact = Lib.LoadScene(resourcePath).Instantiate<SimpleParticleEffect>();
		impact.Position = target;
		node.GetTree().CurrentScene.AddChild(impact);
		return impact;
	}

	public static float GetArenaSize(this Node _)
	{
		return 16;
	}

	/// <summary>
	/// Generates a position located at the edge of the arena with a given offset.
	/// <br />
	/// Use X coordinate for offset along the edge, and Z coordinate for distance from the edge.
	/// <br /><br />
	/// Units are relative, and should be in range (0; 1).
	/// <br /><br />
	/// With an offset of (0; 0; 0), the returned position is the middle of the corresponding edge.
	/// </summary>
	public static Vector3 GetArenaEdgePosition(this Node _, Vector3 offset, ArenaFacing facing)
	{
		var arenaSize = 16;
		offset *= arenaSize;
		if (facing == ArenaFacing.East)
			return new Vector3(-offset.Z + arenaSize, offset.Y, offset.X);
		if (facing == ArenaFacing.North)
			return new Vector3(offset.X, offset.Y, offset.Z - arenaSize);
		if (facing == ArenaFacing.West)
			return new Vector3(offset.Z - arenaSize, offset.Y, -offset.X);
		if (facing == ArenaFacing.South)
			return new Vector3(-offset.X, offset.Y, -offset.Z + arenaSize);
		return offset;
	}

	/// <summary>
	/// Returns an angle in radians corresponding to facing inside the arena.
	/// <br />
	/// Assumes that the target is facing towards negative Z axis (Y rotation is set to 0)
	/// <br />
	/// Usage: <code>Node3D.Rotate(Vector3.Up, this.GetArenaFacingAngle(...))</code>
	/// </summary>
	public static float GetArenaFacingAngle(this Node _, ArenaFacing facing)
	{
		if (facing == ArenaFacing.East)
			return (float)Math.PI / 2;
		if (facing == ArenaFacing.North)
			return (float)Math.PI;
		if (facing == ArenaFacing.West)
			return (float)-Math.PI / 2;
		if (facing == ArenaFacing.South)
			return 0;
		return 0;
	}

	public static List<ArenaFacing> AllArenaFacings()
	{
		return new() { ArenaFacing.East, ArenaFacing.North, ArenaFacing.West, ArenaFacing.South };
	}

	public static Vector3 SnapToGround(this Node3D node, Vector3 fromPos)
	{
		var spaceState = node.GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(fromPos + Vector3.Up * 5, fromPos + Vector3.Down * 10, 1 << 4);
		var result = spaceState.IntersectRay(query);
		if (result.Count == 0)
			return Vector3.Zero;

		return (Vector3)result["position"];
	}

	public static Vector3 GetGroundedPosition(this Node3D node, float verticalOffset = 0.05f)
	{
		var pos = node is BaseUnit unit ? unit.GlobalCastAimPosition : node.GlobalPosition;
		var spaceState = node.GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(pos + Vector3.Up * 5, pos + Vector3.Down * 10, 1 << 4);
		var result = spaceState.IntersectRay(query);
		if (result.Count == 0)
			return Vector3.Zero;

		var res = (Vector3)result["position"];
		return new Vector3(res.X, res.Y + verticalOffset, res.Z);
	}

	public static bool HasSkill<T>(this BaseCast _) where T : BaseSkill
	{
		var skill = SkillTreeManager.Singleton.Skills.Find(skill => skill is T);
		if (skill == null)
			return false;

		return skill.IsLearned;
	}

	public static string GetReadableCastTimings(this BaseCast.CastSettings settings)
	{
		var timings = settings.CastTimings;
		if (timings == BeatTime.Free)
			return "[color=orange]Free cast[/color]";

		var builder = new StringBuilder();

		List<BeatTime> timesToCheck = new()
		{
			BeatTime.Whole,
			BeatTime.Half,
			BeatTime.Quarter,
			BeatTime.Eighth,
			BeatTime.Sixteenth,
		};

		foreach (var time in timesToCheck)
		{
			var color = "gray";
			if (timings.Has(time))
				color = "orange";
			builder.Append($"[color={color}]|[/color]");
		}
		return builder.ToString();
	}

	public static string MakeDescription(params string[] strings)
	{
		var valueRegex = new Regex("{([^}]+)}");
		var passiveRegex = new Regex("\\(\\(([^}]+)\\)\\)");
		return strings.Select(str =>
		{
			var s1 = valueRegex.Replace(str, (q) => $"{Colors.Tag(q.Groups[1])}");
			var s2 = passiveRegex.Replace(s1, (q) => $"{Colors.Lore(q.Groups[1])}");
			return s2;
		}).ToArray().Join(" ");
	}

	public static Color GetAllianceColor(UnitAlliance alliance)
	{
		var color = new Color(0, 0.7f, 0.7f);

		if (alliance == UnitAlliance.Player)
			color = new Color(0, 0.7f, 0);
		else if (alliance == UnitAlliance.Hostile)
			color = new Color(0.7f, 0.0f, 1.0f);

		return color;
	}

	public static async void NextFrame(this Node node, Action action = null)
	{
		await node.ToSignal(node.GetTree().CreateTimer(0), "timeout");
		action?.Invoke();
	}

	public static float GetTicksSec()
	{
		return ((float)Time.GetTicksMsec()) / 1000;
	}
}

public enum ArenaFacing : int
{
	East = 0,
	North = 1,
	West = 2,
	South = 3,
}