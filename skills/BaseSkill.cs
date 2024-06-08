using System.Collections.Generic;
using Godot;

namespace Project;

public partial class BaseSkill : Node
{
	public string FriendlyName;
	public string IconPath = null;
	public int Depth;
	public float PosX;

	public bool IsLearned;
	public SkillGroup Group;
	public CastFactory ActiveCast;
	public BuffFactory PassiveBuff;
	public SkillConnection ParentLink;
	public List<SkillConnection> ChildrenLinks = new();
}
