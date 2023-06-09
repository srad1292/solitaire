using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{

    public static SelectionController Instance;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    public float followXOffset = 1.5f;
    public float followYOffset = -0.5f;
    public float followZOffset = -3f;
    [SerializeField] LayerMask cardLayer;
    [SerializeField] LayerMask tableLayer;
    [SerializeField] LayerMask placementLayer;
    public List<Card> selectedCards = new List<Card>();


    void Update()
    {
        if(UIController.Instance.isPaused) { return; }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100f, tableLayer) && selectedCards.Count > 0) {
            MoveCardsWithMouse(hit);
        }

        if (Input.GetMouseButtonDown(0)) {
            HandleLeftClick(ray, hit);
        } else if(Input.GetMouseButtonDown(1)) {
            HandleRightClick(ray, hit);
        }
    }

    private void MoveCardsWithMouse(RaycastHit hit) {
        for (int idx = 0; idx < selectedCards.Count; idx++) {
            selectedCards[idx].MoveToPoint(
                hit.point + new Vector3(
                    followXOffset, 
                    followYOffset + (GameController.Instance.mediumOffset * idx), 
                    followZOffset + (GameController.Instance.smallOffset * idx)
                ),
                Quaternion.identity
            );
        }
    }

    private void HandleLeftClick(Ray ray, RaycastHit hit) {
        if (Physics.Raycast(ray, out hit, 100f, placementLayer) && selectedCards.Count > 0) {
            HandleHitPlacePointWhileHoldingCards(hit);
        }
        else if (Physics.Raycast(ray, out hit, 100f, cardLayer)) {
            Card card = hit.collider.GetComponent<Card>();
            if (selectedCards.Count == 0) {
                HandleHitCardWhileNotHoldingCards(card);
            }
            else {
                HandleHitCardWhileHoldingCards(card);
            }

        }
    }

    private void HandleHitPlacePointWhileHoldingCards(RaycastHit hit) {
        PlacePoint selectedPoint = hit.collider.GetComponent<PlacePoint>();
        // Clicked on placepoint cards already were at
        if (selectedPoint != null && selectedPoint == selectedCards[0].GetLastPlacePoint()) {
            foreach (Card card in selectedCards) {
                card.ReturnCard();
            }
            selectedCards.Clear();
        }
        // Clicked on new placepoint
        else if (selectedPoint != null) {
            bool canPlace = GameController.Instance.CheckIfCardCanBePlacedOnPlacePoint(selectedCards[0], selectedPoint);
            if (canPlace) {
                foreach (Card card in selectedCards) {
                    GameController.Instance.HandleCardPlaced(card, selectedPoint);
                }
                selectedCards.Clear();
            }
            else {
                foreach (Card card in selectedCards) {
                    card.ReturnCard();
                }
                selectedCards.Clear();
            }

        }
        else {
            foreach (Card card in selectedCards) {
                card.ReturnCard();
            }
            selectedCards.Clear();
        }
    }

    private void HandleHitCardWhileNotHoldingCards(Card card) {
        if (card.isFaceUp && card.canSelect) {
            bool cardFound = false;
            for (int cardIdx = 0; cardIdx < card.placePoint.cards.Count; cardIdx++) {
                if (card.placePoint.cards[cardIdx] == card) {
                    cardFound = true;
                }
                if (cardFound) {
                    selectedCards.Add(card.placePoint.cards[cardIdx]);
                    card.placePoint.cards[cardIdx].SelectCard();

                }
            }
        }
    }

    private void HandleHitCardWhileHoldingCards(Card card) {
        bool clickedOnHeldCard = false;
        foreach (Card curCard in selectedCards) {
            if (curCard == card) {
                clickedOnHeldCard = true;
            }
        }
        if (!clickedOnHeldCard) {
            bool canPlace = GameController.Instance.CheckIfCardCanBePlacedOnCard(selectedCards[0], card);
            if (canPlace) {
                foreach (Card curcard in selectedCards) {
                    GameController.Instance.HandleCardPlaced(curcard, card.placePoint);
                }
                selectedCards.Clear();
            }
            else {
                foreach (Card curcard in selectedCards) {
                    curcard.ReturnCard();
                }
                selectedCards.Clear();
            }

        }
    }

    private void HandleRightClick(Ray ray, RaycastHit hit) {
        if (selectedCards.Count > 0) {
            foreach (Card curcard in selectedCards) {
                curcard.ReturnCard();
            }
            selectedCards.Clear();
        }
        else if (Physics.Raycast(ray, out hit, 100f, cardLayer)) {
            Card card = hit.collider.GetComponent<Card>();
            PlacePoint destination = GameController.Instance.CheckForDestination(card);
            if (destination != null) {
                card.SelectCard();

                selectedCards.Add(card);
                GameController.Instance.HandleCardPlaced(selectedCards[0], destination);
                selectedCards.Clear();

            }
        }
    }


}
