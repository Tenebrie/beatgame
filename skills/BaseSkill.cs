using System.Collections.Generic;
using Godot;

namespace Project;

public partial class BaseSkill : Node
{
	public string FriendlyName;
	public int Depth;
	public float PosX;

	public BaseCast ActiveCast;
	public BaseBuff PassiveBuff;
	public List<InternalSkillLink> Parents = new();
	public List<InternalSkillLink> Children = new();
}
