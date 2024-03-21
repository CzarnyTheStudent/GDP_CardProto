using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public float countdownDuration = 4f; // Czas odliczania w sekundach
    private float countdownTimer; 

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
        StartCountdown();
    }

    void StartCountdown()
    {
        countdownTimer = countdownDuration;
        InvokeRepeating("UpdateCountdown", 0f, 1f); // Wywołanie funkcji co 1 sekundę
    }

    void UpdateCountdown()
    {
        countdownTimer -= 1f;

        if (countdownTimer <= 0f)
        {
            CancelInvoke("UpdateCountdown");
            Debug.Log("Koniec odliczania. Gra rozpoczęta!");
        }
    }
    
    void Update()
    {
        HandleInput();

        if (countdownTimer == 0 && getStop)
        {
            GameObject playerCard = addedCards[Random.Range(0, addedCards.Count)];
            addedCards.Remove(playerCard);
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
        Vector2Int randomCell = GetRandomEmptyCellPosition();
        GameObject botCard = addedCards[Random.Range(0, addedCards.Count)];
        Card cardBotData = botCard.GetComponent<Card>();
        addedCards.Remove(botCard);
        //botCard.transform.position = new Vector3(randomCell.x, 0, randomCell.y);
        botCard.GetComponent<Renderer>().material = botMat;
        yield return new WaitForSeconds(2f);
        cardBotData.indicatorInstance.SetActive(true);
        cardBotData.indicatorInstance.transform.position = new Vector3(randomCell.x, 0, randomCell.y);
        yield return new WaitForSeconds(1f);
        GameLogic.Instance.PlaceCardOnCell(botCard, botCard.GetComponent<Card>().indicatorInstance);
        cardBotData.DestroyIndicator();
        getStop = true;
        yield return null;
        _enemyTurn = false;
        StopCoroutine(EnemyTurn());
    }

    Vector2Int GetRandomEmptyCellPosition()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                if (!isCellOccupied[row, col])
                {
                    emptyCells.Add(new Vector2Int(row, col));
                }
            }
        }

        if (emptyCells.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyCells.Count);
            return emptyCells[randomIndex];
        }
        else
        {
            // No empty cells found, return (-1, -1) as an indication of failure
            return new Vector2Int(-1, -1);
        }
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
            if (_enemyTurn && card.GetComponent<Card>().canMove)
            {
                StartCoroutine(EnemyTurn());
                card.GetComponent<Card>().DestroyIndicator();
                card.GetComponent<Card>().canMove = false;
            }
        }
        else
        {
            Debug.Log("Nie udało się umieścić");
            card.transform.position = _cardStartPos;
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
