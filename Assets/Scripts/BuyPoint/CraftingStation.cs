using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

public class CraftingStation : MonoBehaviour {
    private GameObject attachmentObject;
    private Attachments allAttachments;
    private GameObject attachmentNodesParent;
    private GameObject attachmentGroupUIObject;
    public GameObject player;
    public PlayerController playerController;
    public Transform parentUI;

    void Start() {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        attachmentObject = GameObject.Find("Attachment");
        parentUI = transform;
        attachmentNodesParent = parentUI.GetChild(3).GetChild(0).GetChild(0).GetChild(0).gameObject;
        var attachmentJSON = File.ReadAllText("./Assets/Scripts/Attachments/Attachments.json");
        allAttachments = JsonConvert.DeserializeObject<Attachments>(attachmentJSON);
        attachmentGroupUIObject = parentUI.GetChild(4).gameObject;
        displayAttachments();
    }

    // protected override void OnCollisionEnter2D(Collision2D col) {
    //     if(col.gameObject.name == "Player") {
    //         toggleUI(true);
    //         displayAttachments();
    //     }
    // }

    public Dictionary<AttachmentType, int> getAvailableAttachments() {
        var recipesTransform = player.transform.GetChild(4);
        var attachments = new Dictionary<AttachmentType, int>();
        foreach (Transform recipe in recipesTransform) {
            var atType = recipe.gameObject.GetComponent<AttachmentRecipe>().attachment;
            if(attachments.ContainsKey(atType)) {
                attachments[atType] += 1;
            } else {
                attachments.Add(atType, 1);
            }
        }
        foreach(var attachment in attachments) {
            Debug.Log($"{attachment.Key}, {attachment.Value}");
        }
        return attachments;
    }

    public void displayAttachments() {
        foreach(Transform node in attachmentNodesParent.transform) {
            Destroy(node.gameObject);
        }
        var availableAttachments = getAvailableAttachments();
        var count = 0;
        foreach (var attachment in availableAttachments) {
            var attachmentGroup = allAttachments.GetType().GetProperty(attachment.Key.ToString()).GetValue(allAttachments, null) as List<Attachment>;
            var attachmentGroupObject = Instantiate(attachmentGroupUIObject, new Vector2(0, 0), Quaternion.identity);
            attachmentGroupObject.transform.SetParent(attachmentNodesParent.transform);
            attachmentGroupObject.transform.localPosition = new Vector3(-30f, 190f + (-210f * count), 0);
            for(var i = 0; i < 4; i++) {
                var nodeBackgroundObject = attachmentGroupObject.transform.GetChild(i).GetChild(1).gameObject;
                if(!attachmentGroup[i].unlocked) {
                    nodeBackgroundObject.GetComponent<Button>().interactable = false;
                } else {
                    if(playerController.checkScapAmount(attachmentGroup[i].cost)) {
                        nodeBackgroundObject.GetComponent<Image>().color = Color.green;
                        nodeBackgroundObject.GetComponent<Button>().onClick.AddListener(() => Debug.Log("pisssss"));
                    } else {
                        nodeBackgroundObject.GetComponent<Image>().color = Color.red;
                    }
                }
                attachmentGroupObject.transform.GetChild(i).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = attachmentGroup[i].name;
            }
            count += 1;
        }
    }



    public void generateAttachmentButton(Attachment attachment, int index) {
        // var button = Instantiate(attachmentButton, new Vector2(2, 0), Quaternion.identity);
        // button.transform.SetParent(transform.GetChild(1));
        // button.transform.localScale = new Vector3(1, 1, 1);
        // button.transform.localPosition = new Vector3(0, 164 - (index * 96));
        // button.transform.GetChild(0).gameObject.GetComponent<Text>().text = attachment.name;
        // button.transform.GetChild(1).gameObject.GetComponent<Text>().text = attachment.cost.ToString();
        // var buttonScript = button.GetComponent<Button>();
        // buttonScript.onClick.RemoveAllListeners();
        // buttonScript.onClick.AddListener(() => addAttachment(attachment.stats));
    }

    public void addAttachment(WeaponStats statsToUpdate) {
        var gunObject = player.transform.GetChild(0).gameObject;
        var attachmentClone = Instantiate(attachmentObject, new Vector2(2, 0), Quaternion.identity);
        attachmentClone.transform.SetParent(gunObject.transform.GetChild(2));
        var attachmentCloneScript = attachmentClone.GetComponent<GunStatChange>();
        attachmentCloneScript.statsToUpdate = statsToUpdate;
        gunObject.GetComponent<GunParent>().perkList.Add(attachmentCloneScript.applyGunPerk);
    }
}
