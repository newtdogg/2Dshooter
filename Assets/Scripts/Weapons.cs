using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Weapons {
	public Weapon pistol;
	public Weapon magnum44;
	public Weapon shotgun;
	public Weapon autopistol;
	public Weapon Pistol { get { return pistol; } }
	public Weapon Magnum44 { get { return magnum44; } }
	public Weapon Shotgun { get { return shotgun; } }
	public Weapon Autopistol { get { return autopistol; } }
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
}

[System.Serializable]
public class Weapon {
	public string title;
	public bool unlocked;
	public int cost;
	public WeaponStats stats;
	public string type;
}
