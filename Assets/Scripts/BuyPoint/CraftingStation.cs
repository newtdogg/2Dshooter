using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;

public class CraftingStation : BuyPoint
{
    private GameObject attachmentButton;
    private GameObject attachmentObject;

    void Start() {
        title = "Crafting";
        player = GameObject.Find("Player");
        attachmentObject = GameObject.Find("Attachment");
        parentUI = GameObject.Find("CraftingStationUI").transform;
        attachmentButton = parentUI.GetChild(0).gameObject;
    }

    protected override void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            toggleUI(true);
            displayAttachments();
        }
    }

    public List<Attachment> getAvailableAttachments() {
        var recipesTransform = player.transform.GetChild(4);
        var attachments = new List<Attachment>();
        foreach (Transform recipe in recipesTransform) {
            attachments.AddRange(recipe.gameObject.GetComponent<AttachmentRecipe>().attachmentsAvailable);
        }
        foreach(var attachment in attachments) {
            Debug.Log(attachment.name);
        }
        return attachments;
    }

    public void displayAttachments() {
        // parentUI.GetChild(0).GetChild(2).gameObject.GetComponent<Text>().text = title;
        // var index = 0;
        // foreach(Transform button in parentUI.GetChild(0).GetChild(4)) {
        //     Destroy(button.gameObject);
        // }
        var test = getAvailableAttachments();
        // foreach (var attachment in getAvailableAttachments()) {
        //     generateAttachmentButton(attachment, index);
        //     index++;
        // }
    }

    public void generateAttachmentButton(Attachment attachment, int index) {
        var button = Instantiate(attachmentButton, new Vector2(2, 0), Quaternion.identity);
        button.transform.SetParent(transform.GetChild(1));
        button.transform.localScale = new Vector3(1, 1, 1);
        button.transform.localPosition = new Vector3(0, 164 - (index * 96));
        button.transform.GetChild(0).gameObject.GetComponent<Text>().text = attachment.name;
        button.transform.GetChild(1).gameObject.GetComponent<Text>().text = attachment.cost.ToString();
        var buttonScript = button.GetComponent<Button>();
        buttonScript.onClick.RemoveAllListeners();
        buttonScript.onClick.AddListener(() => addAttachment(attachment.stats));
    }

    public void addAttachment(WeaponStats statsToUpdate) {
        var gunObject = player.transform.GetChild(0).gameObject;
        var attachmentClone = Instantiate(attachmentObject, new Vector2(2, 0), Quaternion.identity);
        attachmentClone.transform.SetParent(gunObject.transform.GetChild(2));
        var attachmentCloneScript = attachmentClone.GetComponent<GunStatChange>();
        attachmentCloneScript.statsToUpdate = statsToUpdate;
        gunObject.GetComponent<Gun>().perkList.Add(attachmentCloneScript.applyGunPerk);
    }
}
