using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapons {
	public Weapon pistol;
	public Weapon magnum44;
	public Weapon shotgun;
	public Weapon autopistol;
	public Weapon Pistol { get { return pistol; } }
	public Weapon Magnum44 { get { return magnum44; } }
	public Weapon Shotgun { get { return shotgun; } }
	public Weapon AutoPistol { get { return autopistol; } }
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
