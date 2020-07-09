using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// using System;

public class Recipe : Pickup {
    public string gunType;
    public List<Attachment> attachmentsAvailable;

    void Start()
    {
        type = "Recipe";
        var jsonString = File.ReadAllText("./Assets/Scripts/Attachments/Attachments.json");
        var attachmentJson = JsonUtility.FromJson<Attachments>(jsonString);
        gunType = GameObject.Find("Gun").GetComponent<Gun>().type;
        attachmentsAvailable = generateAvailableAttachments(attachmentJson);
    }

    public List<Attachment> generateAvailableAttachments(Attachments attachments) {
        var attachmentList = new List<Attachment>();
        foreach (var attachment in attachments.GetType().GetProperties()) {
            var attachmentObject = attachment.GetValue(attachments) as Attachment;
            if(attachmentObject.gunType.Contains(gunType) || attachmentObject.gunType.Contains("All")) {
                attachmentList.Add(attachmentObject);
            }
        }
        return attachmentList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
