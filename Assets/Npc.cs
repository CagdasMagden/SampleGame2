using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    // zaman� gelince buton �zerine gelindi�inde nas�l text g�z�kt�r�l�r ��ren
    public HealthManager healthManager;

    public GameObject dialoguePanel;
    public GameObject continueButton;
    public GameObject marketButton;
    public GameObject marketPanel; 
    public Text dialogueText;
    public string[] dialogue;
    private int index;

    public float wordSpeed;
    public bool playerIsClose;
    private bool canInteract = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose && canInteract)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                StartCoroutine(InteractCooldown());
                ZeroText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }
        if (dialogueText.text == dialogue[index])
        {
            continueButton.SetActive(true);
        }       
    }

    public void ZeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        marketButton.SetActive(true);
        marketPanel.SetActive(false);
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue [index].ToCharArray()) 
        {
            // letter diyalogun i�indeki her bir char� tek tek al�yor sonra onlar� bir �ncekine ekleyerek wordspeed h�z�yla yan yana koyarak kelime yap�yor
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    IEnumerator InteractCooldown()
    {
        canInteract = false; // Etkile�im yapamayacak �ekilde ayarla

        // 5 saniye bekle
        yield return new WaitForSeconds(3f);

        canInteract = true; // 5 saniye sonra tekrar etkile�ime izin ver
    }

    public void NextLine()
    {

        continueButton.SetActive(false);

        if(index < dialogue.Length - 1) 
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }

        else 
            ZeroText();
    }

    public void OpenMarket()
    {
        continueButton.SetActive(false);
        marketButton.SetActive(false);
        marketPanel.SetActive(true);
        dialogueText.text = "";
    }

    public void BuyThings(Button button)
    {
        string buttonName = button.name;
        if (buttonName == "IncreaseMaxHealth")
        {
            healthManager.MaxHealth++;
            healthManager.CurrentHealth++;
        }
        else if (buttonName == "abubu")
        {

        }
        ZeroText();       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            ZeroText();
        }
    }
}
