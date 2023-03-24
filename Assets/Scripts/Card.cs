using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    public TMP_Text[] numberDisplays;
    public Image[] suitDisplays;
    public CardSO cardSO;
    public bool isFaceUp = false;
    public PlacePoint placePoint;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 500f;
    [SerializeField] LayerMask tableLayer;
    [SerializeField] LayerMask placementLayer;
    [SerializeField] GameObject cardCanvas;
    
    BoxCollider myCollider;


    private bool isSelected = false;
    private bool justPressed = false;

    private Vector3 targetPoint;
    private Quaternion targetRotation;
    private Vector3 lastLocation;
    private Quaternion lastRotation;
    private PlacePoint lastPlacePoint;



    private void Start() {
        myCollider = GetComponent<BoxCollider>();
        myCollider.enabled = false;
        StartCoroutine(WaitToEnableCollider());
        if (targetPoint == Vector3.zero) {
            targetPoint = transform.position;
            targetRotation = transform.rotation;
        }
        UpdateDisplay();
    }

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        if (isSelected) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, tableLayer)) {
                MoveToPoint(hit.point + new Vector3(0, 0.5f, -1.5f), Quaternion.identity);
            }
            if (Input.GetMouseButtonDown(0) && !justPressed) {
                if(Physics.Raycast(ray, out hit, 100f, placementLayer)) {
                    PlacePoint selectedPoint = hit.collider.GetComponent<PlacePoint>();
                    print("Selected Point is null? " + selectedPoint == null);
                    if(selectedPoint != null)
                        print("Selected Point Card Count = " + selectedPoint.cards.Count);
                    if(selectedPoint != null && selectedPoint.cards.Count == 0) {
                        PlaceCard(selectedPoint, selectedPoint.transform.position, selectedPoint.transform.rotation);
                    } else {
                        PlaceCard(lastPlacePoint, lastLocation, lastRotation);
                    }
                } else {
                    PlaceCard(lastPlacePoint, lastLocation, lastRotation);
                }
                
            }

            if (Input.GetMouseButtonDown(1)) {
                PlaceCard(lastPlacePoint, lastLocation, lastRotation);
            }
        }

        justPressed = false;
    }

    IEnumerator WaitToEnableCollider() {
        yield return new WaitForSeconds(0.3f);
        myCollider.enabled = true;
    }

    private void PlaceCard(PlacePoint point, Vector3 location, Quaternion rotation) {
        MoveToPoint(location, rotation);
        placePoint = point;
        if(lastPlacePoint.cards.Contains(this)) {
            lastPlacePoint.cards.Remove(this);
        }
        point.cards.Add(this);
        isSelected = false;
        GameController.Instance.isHoldingCard = false;
        myCollider.enabled = true;
    }

    private void OnMouseDown() {
        if(!isSelected && isFaceUp && !GameController.Instance.isHoldingCard) {
            print("I am a selected card");
            justPressed = true;
            lastLocation = transform.position;
            lastRotation = transform.rotation;
            lastPlacePoint = placePoint;
            isSelected = true;
            GameController.Instance.isHoldingCard = true;
            myCollider.enabled = false;
        } 
    }

    public void MoveToPoint(Vector3 destination, Quaternion rotation) {
        targetPoint = destination;
        targetRotation = rotation;
    }

    public void SetSO(CardSO cardSO) {
        this.cardSO = cardSO;
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        if(cardSO == null) { print("Updating display with null so"); return; }

        foreach (TMP_Text text in numberDisplays) {
            text.SetText(cardSO.display);
            text.color = cardSO.suitColor;
        }
        foreach (Image img in suitDisplays) {
            img.sprite = cardSO.suitSprite;
            img.color = cardSO.suitColor;
        }
    }

    public void SetCardDirection(bool isFaceUp) {
        this.isFaceUp = isFaceUp;
        cardCanvas.SetActive(isFaceUp);
    }



}
