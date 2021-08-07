using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject scrapWindow;
    public Transform scrapList;
    private Transform scrapItemUIElement;
    private PlayerController playerController;
    void Start()
    {
        scrapWindow = transform.GetChild(2).gameObject;
        scrapItemUIElement = scrapWindow.transform.GetChild(2);
        scrapList = scrapWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        openScrapWindow();
    }

    private void openScrapWindow() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Debug.Log(scrapWindow.activeSelf);
            if (!scrapWindow.activeSelf) {
                foreach(Transform node in scrapList) {
                    Destroy(node.gameObject);
                }
                Debug.Log("piss1");

                scrapWindow.SetActive(true);
                var count = 0;
                foreach (var scrapItem in playerController.scrap) {
                    var scrapItemObject = Instantiate(scrapItemUIElement, new Vector2(0, 0), Quaternion.identity);
                    scrapItemObject.transform.SetParent(scrapList.transform);
                    scrapItemObject.transform.localPosition = new Vector3(0, 100f + (-50f * count));
                    scrapItemObject.GetChild(1).gameObject.GetComponent<Text>().text = scrapItem.Key;
                    scrapItemObject.GetChild(0).gameObject.GetComponent<Text>().text = scrapItem.Value.ToString();
                    count += 1;
                }
            } else {
                scrapWindow.SetActive(false);
                Debug.Log("piss1");
            }
        }
    }

    
}
