using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image Icon1;
    public Image Icon2;
    public Image Icon3;
    public Image Icon4; 
    public bool canMove;
    public GameObject indicator;
    private GameObject indicatorInstance;
    private bool isDragging = false;

    private void Start()
    {
        CreateIndicator();
        indicatorInstance.SetActive(false);
        canMove = false;
    }

    void OnMouseDown()
    {
        if (canMove)
        {
            isDragging = true;
            indicatorInstance.SetActive(true);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            UpdatePosition();
        }

        if (Input.GetMouseButtonUp(0) && canMove)
        {
            isDragging = false;
            indicator.SetActive(true);
            TryPlaceOnBoard();
        }
    }

    void UpdatePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            UpdateIndicatorPosition(hit.point);
        }
    }
    
    void CreateIndicator()
    {
        if (indicator != null)
        {
            indicatorInstance = Instantiate(indicator, transform.position, Quaternion.Euler(-90, 0, 0));
        }
    }
    
    void DestroyIndicator()
    {
        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance); // Usuwanie instancji wskaźnika
        }
    }
    
    void UpdateIndicatorPosition(Vector3 hitPoint)
    {
        // Określanie pozycji wskaźnika na planszy
        Vector3 indicatorPosition = SnapToNearestBoardCell(hitPoint);
        if (indicatorInstance != null)
        {
            indicatorInstance.transform.position = indicatorPosition;
        }
    }

    Vector3 SnapToNearestBoardCell(Vector3 position)
    {
        // Zaokrąglenie współrzędnych hitPoint do najbliższej komórki na planszy
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.z);
        // Ograniczenie współrzędnych x i z do rozmiaru tablicy board
        x = Mathf.Clamp(x, 0, GameLogic.Instance.board.GetLength(0) - 1);
        z = Mathf.Clamp(z, 0, GameLogic.Instance.board.GetLength(1) - 1);
        // Zwracanie zaokrąglonej pozycji
        return new Vector3(x, 0f, z);
    }

    void TryPlaceOnBoard()
    {
        RaycastHit hit;
        Vector3 rayDirection = transform.up; // Ustawienie kierunku raycasta na przeciwny do osi y karty
        
        if (Physics.Raycast(transform.position, rayDirection, out hit))
        {
            if (hit.collider.gameObject)
            {
                GameLogic.Instance.PlaceCardOnCell(gameObject, indicatorInstance);
                DestroyIndicator();
                canMove = false;
            }
            else
            {
                Debug.LogError("Trafiłeś gówno");
            }
        }
    }

}