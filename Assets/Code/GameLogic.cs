using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLogic : MonoBehaviour
{
    public CardCreator cardCreator;
    public GameObject indicator;
    [FormerlySerializedAs("player")] public Material playerMat;
    [FormerlySerializedAs("bot")] public Material botMat;
    [HideInInspector]public List<GameObject> addedCards;
    public GameObject[,] board = new GameObject[5,5];
    private bool[,] isCellOccupied;
    private bool getStop = true;
    private bool _enemyTurn;
    private Vector3 _cardStartPos;

    public static GameLogic Instance { get; private set; }

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        cardCreator.CreatePlayerCards(cardCreator.gameCards.Length, cardCreator.gameCards); // Dla gracza 1
        InitializeGame();
        _enemyTurn = false;
    }
    
    void Update()
    {
        HandleInput();

        if (Input.GetKey(KeyCode.D) && getStop)
        {
            GameObject playerCard = addedCards[Random.Range(0, addedCards.Count)];
            playerCard.transform.position += new Vector3(1.5f, 0, 0);
            playerCard.GetComponent<Renderer>().material = playerMat;
            playerCard.GetComponent<Card>().canMove = true;
            _cardStartPos = playerCard.transform.position;
            getStop = false;
        }
    }

    void InitializeGame()
    {
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board.Length; j++)
            {
                isCellOccupied = new bool[i , j]; // Inicjalizacja tablicy informującej o zajętości komórek
            }
        }
        CreateBoard();
        addedCards = cardCreator.cardsObject;

        for (int i = 0; i < addedCards.Count; i++)
        {
            Card cardScript = addedCards[i].GetComponent<Card>();

            cardScript.indicator = indicator;
            cardScript.Icon1.sprite = cardCreator.gameCards[i].imagesData[0].cardImages;
            cardScript.Icon2.sprite = cardCreator.gameCards[i].imagesData[1].cardImages;
            cardScript.Icon3.sprite = cardCreator.gameCards[i].imagesData[2].cardImages;
            cardScript.Icon4.sprite = cardCreator.gameCards[i].imagesData[3].cardImages;
        }
    }
    
    public void SetPositionEmpty(int x, int z) // konkretne pozycje ustawia puste
    {
        board[x, z] = null;
    }
    
    void CreateBoard()
    {
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                isCellOccupied[row, col] = false; 
            }
        }
    }

    IEnumerator EnemyTurn()
    {
        GameObject playerCard = addedCards[Random.Range(0, addedCards.Count)];
        playerCard.transform.position += new Vector3(1.5f, 0, 0);
        playerCard.GetComponent<Renderer>().material = botMat;
        yield return null;
    }
    
    public void PlaceCardOnCell(GameObject card, GameObject cell)
    {
        int row = Mathf.RoundToInt(cell.transform.position.x);
        int col = Mathf.RoundToInt(cell.transform.position.z);

        if (!isCellOccupied[row, col])
        {
            // Umieść kartę na planszy i oznacz komórkę jako zajętą
            card.transform.position = cell.transform.position + new Vector3(0f, 0.5f, 0f);
            isCellOccupied[row, col] = true;
            _enemyTurn = true;
            if (_enemyTurn)
            {
                StartCoroutine(EnemyTurn());
                _enemyTurn = false;
            }
        }
        else
        {
            // Jeśli komórka jest już zajęta, wróć do poprzedniej pozycji karty
            Debug.Log("Nie udało się umieścić");
            card.transform.position = _cardStartPos;
            //cardScript.ReturnToStartPosition();
        }
    }
    

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        }
    }

    void SelectCard(GameObject card)
    {
        // Deselect the currently selected card (if any)
        if (cardCreator != null)
        {
           //PlaceCardOnCell(card);
        }
        
    }

   
}
