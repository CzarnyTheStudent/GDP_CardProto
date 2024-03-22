using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CardCreator", menuName = "CardCreator", order = 1)]
public class CardCreator : ScriptableObject
{
    public CardData[] gameCards;
    public Sprite[] cardSprite; // Materiały dla różnych obrazków na kartach
    [HideInInspector]public List<GameObject> cardsObject;

    public void OnDisable()
    {
        foreach (var card in gameCards)
        {
            for (int i = 0; i < 4; i++)
            {
                card.imagesData[i].cardImages = cardSprite[(int)card.imagesData[i].icone];
            }
        }
        cardsObject.Clear();
    }

    public void CreatePlayerCards(int cardCount, CardData[] cardData)
    {
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = Instantiate(cardData[i].cardPrefab, cardData[i].cardPrefab.transform.position + new Vector3(-10f, 2, 2f), Quaternion.Euler(180 , 0, 0));
            cardsObject.Add(card);
            
            // Dodaj komponenty, które będą zarządzały kartą (np. skrypt do zmiany obrazków)
            Card cardScript = card.GetComponent<Card>();

            // Zmiana wyglądu karty - dodaj dowolną logikę, aby dostosować wygląd
            SetCardAppearance(card);
        }
    }

    void SetCardAppearance(GameObject card)
    {

    }
}

[Serializable]
public class CardData
{
    public GameObject cardPrefab;
    public ImagesData[] imagesData;
}


[Serializable]
public class ImagesData
{
    public Sprite cardImages;
    public Icone icone;
}


public enum Icone
{
    Emotion = 0,
    Irony = 1,
    Logic = 2,
    Manipulation = 3
}