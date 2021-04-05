using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapons {
	public Weapon pistol9mm;
	public Weapon tec9;
	public Weapon pistol45;
	public Weapon revolver357;
	public Weapon autoPistol;
	public Weapon revolver44;
	public Weapon akimboAutoPistols;
	public Weapon desertEagle;
	public Weapon smg22;
	public Weapon uzi;
	public Weapon compactSmg;
	public Weapon akimboUzis;
	public Weapon heavySmg;
	
	public Weapon Pistol9mm { get { return pistol9mm; } }
	public Weapon Tec9 { get { return tec9; } }
	public Weapon Pistol45 { get { return pistol45; } }
	public Weapon Revolver357 { get { return revolver357; } }
	public Weapon AutoPistol { get { return autoPistol; } }
	public Weapon Revolver44 { get { return revolver44; } }
	public Weapon AkimboAutoPistols { get { return akimboAutoPistols; } }
	public Weapon DesertEagle { get { return desertEagle; } }
	public Weapon Smg22 { get { return smg22; } }
	public Weapon Uzi { get { return uzi; } }
	public Weapon CompactSmg { get { return compactSmg; } }
	public Weapon AkimboUzis { get { return akimboUzis; } }
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

	public float getVelocity() {
		return Mathf.Round((bulletVelocity/300f) * 100f);
	}

	public float getAccuracy() {
		return Mathf.Round(100f - ((spread - 60f)/100f));
	}
}

[System.Serializable]
public class Weapon {
	public string title;
	public string description;
	public bool unlocked;
	public Dictionary<string, int> cost;
	public WeaponStats stats;
	public string group;
	public string script;
	public int xpCost;
	public string requiredUnlocks;
	public int tier;
	private List<Weapon> requiredUnlocksList;

	public void createUnlocksList() {
		requiredUnlocksList = new List<Weapon>();
	}

	public void addWeaponToUnlocks(Weapon weapon) {
		requiredUnlocksList.Add(weapon);
	}

	public bool isUnlockable() {
		foreach (var child in requiredUnlocksList) {
			if(!child.unlocked) {
				return false;
			}
		}
		return true;
	}
}
