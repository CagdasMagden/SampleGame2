using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    // zamaný gelince buton üzerine gelindiðinde nasýl text gözüktürülür öðren
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
            // letter diyalogun içindeki her bir charý tek tek alýyor sonra onlarý bir öncekine ekleyerek wordspeed hýzýyla yan yana koyarak kelime yapýyor
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    IEnumerator InteractCooldown()
    {
        canInteract = false; // Etkileþim yapamayacak þekilde ayarla

        // 5 saniye bekle
        yield return new WaitForSeconds(3f);

        canInteract = true; // 5 saniye sonra tekrar etkileþime izin ver
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
