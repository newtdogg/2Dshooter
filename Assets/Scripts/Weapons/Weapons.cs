using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapons {
	public Weapon pistol;
	public Weapon revolver44;
	public Weapon autoPistol;
	public Weapon desertEagle;
	public Weapon uzi;
	public Weapon compactSmg;
	public Weapon heavySmg;
	public Weapon Pistol { get { return pistol; } }
	public Weapon Revolver44 { get { return revolver44; } }
	public Weapon AutoPistol { get { return autoPistol; } }
	public Weapon DesertEagle { get { return desertEagle; } }
	public Weapon Uzi { get { return uzi; } }
	public Weapon CompactSmg { get { return compactSmg; } }
	public Weapon HeavySmg { get { return heavySmg; } }
	// public Weapon shotgun;
	// public Weapon Shotgun { get { return shotgun; } }
}

[System.Serializable]
public class WeaponStats {
	public float ammoCapacity;
	public float reloadSpeed;
	public float damage;
	public float shotDelay;
	public float spread;
	public float bulletVelocity;
	public float lifetime;
	public float loudness;

	public WeaponStats duplicateStats() {
		var newStats = new WeaponStats();
		newStats.ammoCapacity = this.ammoCapacity;
		newStats.reloadSpeed = this.reloadSpeed;
		newStats.damage = this.damage;
		newStats.shotDelay = this.shotDelay;
		newStats.spread = this.spread;
		newStats.bulletVelocity = this.bulletVelocity;
		newStats.lifetime = this.lifetime;
		newStats.loudness = this.loudness;
		return newStats;
	}
}

[System.Serializable]
public class Weapon {
	public string title;
	public string description;
	public bool unlocked;
	public int cost;
	public WeaponStats stats;
	public string type;
	public string script;
}
