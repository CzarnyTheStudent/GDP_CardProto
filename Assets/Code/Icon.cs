using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Icon : MonoBehaviour
{
    public Icone iconType;
    public Card myCard;
    public BoxCollider myColliderTrigger;
    private bool canTriggerEnter;

    private void Start()
    {
        myCard = gameObject.GetComponentInParent<Card>();
        myColliderTrigger = gameObject.GetComponent<BoxCollider>();
        myColliderTrigger.enabled = false;
        canTriggerEnter = false;
        StartCoroutine(EnableTriggerAfterDelay(2f)); 
    }
    

    void OnTriggerEnter(Collider other)
    {
        if (canTriggerEnter && myCard.isSet)
        {
            Card otherCard = other.GetComponentInParent<Card>();
            if (other.gameObject.CompareTag("Icon") && otherCard != null && otherCard.isSet)
            {
                Icone otherIconType = other.GetComponent<Icon>().iconType;
                if (myCard.botCardThis && otherCard.gameLogic._enemyTurn)
                {
                    if (CheckWinCondition(otherIconType, iconType))
                    {
                        ChangeMaterial(myCard, otherCard.gameLogic.playerMat);
                        Debug.Log("Zmieniłem materiał na playerMat");
                    }
                }
                else if (!myCard.botCardThis && !otherCard.gameLogic._enemyTurn)
                {
                    if (CheckWinCondition(otherIconType, iconType))
                    {
                        ChangeMaterial(myCard, otherCard.gameLogic.botMat);
                        Debug.LogWarning("Zmieniłem materiał na botMat");
                    }
                }
            }
        }
    }
    
    bool CheckWinCondition(Icone attackingIcon, Icone defendingIcon)
    {
        // Logika wygrywa z Manipulacją
        // Manipulacja wygrywa z Ironią
        // Ironia wygrywa z Emocjami
        // Emocje wygrywają z Logiką
        if ((attackingIcon == Icone.Logic && defendingIcon == Icone.Manipulation) ||
            (attackingIcon == Icone.Manipulation && defendingIcon == Icone.Irony) ||
            (attackingIcon == Icone.Irony && defendingIcon == Icone.Emotion) ||
            (attackingIcon == Icone.Emotion && defendingIcon == Icone.Logic))
        {
            return true; 
        }
        else
        {
            return false; // Atakujący przegrywa lub jest remis
        }
    }

    void ChangeMaterial(Card card, Material material)
    {
        Renderer renderer = card.GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.material = material;
            card.botCardThis = !card.botCardThis; // Odwrócenie flagi botCardThis
        }
    }
    
    IEnumerator EnableTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canTriggerEnter = true;
        myColliderTrigger.enabled = true;
    }
}


