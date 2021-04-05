using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttachmentType {
	Sight,
	Magazine,
	Grip,
	Silencer
}

[System.Serializable]
public class Attachment {
	public string name;
	public string script;
	public int rarity;
	public bool unlocked;
	public Dictionary<string, int> cost;
}

[System.Serializable]
public class Attachments {
	public List<Attachment> sight;
	public List<Attachment> Sight { get { return sight; } }
}
