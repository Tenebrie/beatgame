using Godot;

namespace Project;

/// Notes:
/// - 5 skill trees: Tank, Heal, Physical Damage, Magical Damage, Summons

/// - Tank:
/// -- Basic damage mitigation (upgrades into having multiple charges)
/// -- Thorns
/// -- CC reduction
/// -- CC ignore, full invuln
/// -- Save position, press again to return
/// -- Dying heals you back to full hp instead
/// -- Mana shield - toggle to now tank all damage into mana

/// - Physical:
/// -- Quick attacks, little casting (warrior + rogue together)

/// - Magical:
/// -- Longer casts, but more powerful
/// -- Ability to cast while moving for the next N beats

/// - Heal:
/// -- Left branch is about proper healing
/// -- Right branch is about damage through healing (whenever you restore HP, deal damage to targeted enemy)

/// - Summons:
/// -- Friends that have simple AI and cast their spells on a loop
/// -- Rescue to pull them to safety
/// -- Take damage from target to yourself (Alternatively, move damage from yourself to target)

public partial class SkillTree : Node
{

}