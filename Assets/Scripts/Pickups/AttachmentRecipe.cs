using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AttachmentRecipe : Recipe {
    public AttachmentType attachment;

    void Start() {
        value = 1;
        type = "AttachmentRecipe";
    }

    // public List<Attachment> generateAvailableAttachments(Attachments attachments) {
    //     var attachmentList = new List<Attachment>();
    //     foreach (var attachment in attachments.GetType().GetProperties()) {
    //         var attachmentObject = attachment.GetValue(attachments) as Attachment;
    //         if(attachmentObject.gunType.Contains(gunType) || attachmentObject.gunType.Contains("All")) {
    //             attachmentList.Add(attachmentObject);
    //         }
    //     }
    //     return attachmentList;
    // }
}
