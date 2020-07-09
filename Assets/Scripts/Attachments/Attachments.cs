using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attachment {
	public string name;
	public bool unlocked;
    public List<string> gunType;
	public WeaponStats stats;
	public int cost;
}

[System.Serializable]
public class Attachments {
	public Attachment silencer;
	public Attachment heavyRounds;
	public Attachment grip;
	public Attachment longBarrel;
	public Attachment Silencer { get { return silencer; } }
	public Attachment HeavyRounds { get { return heavyRounds; } }
	public Attachment Grip { get { return grip; } }
	public Attachment LongBarrel { get { return longBarrel; } }
}
