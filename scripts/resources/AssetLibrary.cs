using System.Collections.Generic;

namespace Project;

public class UnitLibrary
{
	public string PlayerCharacter = "res://units/player/PlayerCharacter/PlayerCharacter.tscn";
	public string StationarySummon = "res://units/player/StationarySummon/UnitStationarySummon.tscn";
}

public class TokenLibrary
{
	public string DeepGuardian = "res://casts/bosses/DeepGuardians/tokens/DeepGuardian.tscn";
	public string AnimatedTrident = "res://casts/bosses/Tridents/tokens/AnimatedTrident.tscn";
	public string LightningOrbsPylon = "res://casts/bosses/LightningOrbs/tokens/LightningOrbsPylon.tscn";
	public string PowerUpLightningOrb = "res://casts/bosses/LightningOrbs/tokens/PowerUpLightningOrb.tscn";
	public string EffectFlamethrowerWithHitbox = "uid://drx1hwgr4pt6e";
}

public class EffectLibrary
{
	public string TargetingCircle = "res://effects/TargetingCircle/TargetingCircle.tscn";
	public string GroundAreaRect = "uid://h3ilmtoftd77";
	public string GroundAreaCircle = "uid://i7uo3ra8dvfu";
	public string FireballProjectile = "uid://dcip2amav54xc";
	public string FireballProjectileImpact = "uid://dwcfa4po3yivl";
	public string HealImpact = "uid://dtpt851lanhr3";
	public string EnergyOrbPickupImpact = "uid://bsydded5w4p61";
	public string AerielDarknessRelease = "uid://br0yb56fj0miy";
	public string LightningZap = "res://effects/LightningZap/LightningZapEffect.tscn";
	public string LightningZapImpact = "uid://cx2fo4dymkanp";
	public string BusterTarget = "uid://bd1pneojwsjum";
	public string EtherealFocusBurst = "uid://bsdk74dv5et2a";
	public string EtherealFocusChannel = "uid://cwk748e7folsj";
}

public class UILibrary
{
	public string UnitCard = "res://ui/generic/UnitCard/UnitCard.tscn";
	public string CastBar = "res://ui/screens/combat/CastBar/CastBar.tscn";
	public string ActionButton = "res://ui/generic/ActionPanel/ActionButton.tscn";
	public string ActiveSkillButton = "res://ui/screens/skillForest/generated/ActiveSkillButton.tscn";
	public string PassiveSkillButton = "res://ui/screens/skillForest/generated/PassiveSkillButton.tscn";
	public string SkillLink = "res://ui/screens/skillForest/generated/SkillLinkVisual.tscn";
	public string SkillPopup = "res://ui/screens/skillForest/generated/SkillPopup.tscn";
	public string MessageListMessage = "res://ui/generic/MessageList/Message.tscn";
	public string SkillBindingNotification = "res://ui/screens/skillBinding/SkillBindingNotification.tscn";
	public string DpsMeterCast = "uid://dfoorpkl82w6j";
}

public class AudioLibrary
{
	public string MusicTrackTest = "res://assets/music/120bpm-test-track.ogg";
	public string MusicTrackSpaceship = "res://assets/music/t14d-spaceship.ogg";
	public string MusicTrackIntermission = "res://assets/music/70bpm-intermission.ogg";
	public string MusicTrackAeriel = "res://assets/music/120bpm-tidehawk.ogg";
}

public enum PlayableScene
{
	TrainingRoom,
	BossArenaAeriel,
}

public class SceneLibrary
{
	public readonly Dictionary<PlayableScene, string> Values = new()
	{
		{ PlayableScene.TrainingRoom,  "uid://bcrvev26qnfr0" },
		{ PlayableScene.BossArenaAeriel,  "uid://b1d3j63byd72g" },
	};

	public string GetPath(PlayableScene scene)
	{
		var hasValue = Values.TryGetValue(scene, out var result);
		if (!hasValue)
			throw new System.Exception($"No scene path provided for {scene}.");

		return result;
	}
}