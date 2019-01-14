using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Outro : MonoBehaviour
{
    GameObject player, throne, crosshair, credits;
    Image outroPanel;
    bool isOutroStarted;

    float alpha;
    void Start()
    {
        player = GameObject.Find("Player");
        throne = GameObject.Find("Throne");
        outroPanel = GameObject.Find("OutroPanel").GetComponent<Image>();
        outroPanel.gameObject.SetActive(false);
        credits = GameObject.Find("Texts");
        credits.SetActive(false);
    }
   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            outroPanel.gameObject.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            outroPanel.gameObject.SetActive(true);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        outroPanel.gameObject.SetActive(true);
        outroPanel.gameObject.GetComponent<Animator>().SetBool("Outro", true);
  
    }

    void OnTriggerExit(Collider other)
    {
        credits.SetActive(true);
        credits.GetComponent<Animator>().SetBool("Roll Credit",true);
    }
    
}
