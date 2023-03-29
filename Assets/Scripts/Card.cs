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
    public bool canSelect = false;
    public PlacePoint placePoint;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 500f;
    [SerializeField] LayerMask tableLayer;
    [SerializeField] LayerMask placementLayer;
    [SerializeField] GameObject cardCanvas;
    
    BoxCollider myCollider;

    private bool isSelected = false;
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
    }

    IEnumerator WaitToEnableCollider() {
        yield return new WaitForSeconds(0.3f);
        myCollider.enabled = true;
        canSelect = true;
    }

    public void ReturnCard() {
        MoveToPoint(lastLocation, lastRotation);
        isSelected = false;
        myCollider.enabled = true;
    }

    public void SelectCard() {
        lastLocation = transform.position;
        lastRotation = transform.rotation;
        lastPlacePoint = placePoint;
        isSelected = true;
        myCollider.enabled = false;
    }

    public PlacePoint GetLastPlacePoint() {
        return lastPlacePoint;
    }
    
    public void PlaceCard(PlacePoint point, Vector3 location, Quaternion rotation) {
        MoveToPoint(location, rotation);
        placePoint = point;
        point.cards.Add(this);
        isSelected = false;
        myCollider.enabled = true;
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
