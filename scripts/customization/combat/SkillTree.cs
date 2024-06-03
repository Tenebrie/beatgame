using Godot;

namespace Project;

/// Notes:
/// - 5 skill trees: Tank, Heal, Physical Damage, Magical Damage, Summons

/// - Tank:
/// -- Basic damage mitigation (upgrades into having multiple charges)
/// -- CC reduction
/// -- CC ignore, full invuln

/// - Physical:
/// -- Quick attacks, little casting (warrior + rogue together)

/// - Magical:
/// -- Longer casts, but more powerful

/// - Heal:
/// -- Left branch is about proper healing
/// -- Right branch is about damage through healing (whenever you restore HP, deal damage to targeted enemy)

/// - Summons:
/// -- Friends that have simple AI and cast their spells on a loop
/// -- Rescue to pull them to safety
public partial class SkillTree : Node
{

}