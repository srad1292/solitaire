using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    [SerializeField] Card cardToSpawn;
    [SerializeField] PlacePoint[] gamePlacePoints;
    [SerializeField] PlacePoint discardPile;
    [SerializeField] PlacePoint[] goals;


    private void Start() {
        SetupGame();
    }

    private void SetupGame() {
        ClearField();
        DeckController.Instance.ShuffleDeckIntoRemaining();
        for(int pointIndex = 0; pointIndex < gamePlacePoints.Length; pointIndex++) {
            for(int cardIndex = 0; cardIndex < pointIndex; cardIndex++) {
                PlaceCardInSpot(gamePlacePoints[pointIndex], false);
            }
            PlaceCardInSpot(gamePlacePoints[pointIndex], true);
        }
    }

    private void ClearField() {
        foreach (PlacePoint point in gamePlacePoints) {
            foreach (Card card in point.cards) {
                Destroy(card.gameObject);
            }
            point.cards.Clear();
        }

        foreach (PlacePoint point in goals) {
            foreach (Card card in point.cards) {
                Destroy(card.gameObject);
            }
            point.cards.Clear();
        }

        foreach (Card card in discardPile.cards) {
            Destroy(card.gameObject);
        }
        discardPile.cards.Clear();

    }

    public bool CheckIfCardCanBePlacedOnPlacePoint(Card selectedCard, PlacePoint selectedPoint) {
        if(selectedPoint.GetLocation() == Location.Discard) {
            return false;
        }
        else if(selectedPoint.GetLocation() == Location.Goal) {
            if(selectedPoint.cards.Count == 0) {
                return selectedCard.cardSO.value == 1;
            } else {
                return CheckIfCardCanBePlacedOnCard(selectedCard, selectedPoint.cards[selectedPoint.cards.Count-1]);
            }
        } else if(selectedPoint.GetLocation() == Location.PlayArea) {
            if(selectedPoint.cards.Count == 0) {
                return selectedCard.cardSO.value == 13;
            } else {
                return CheckIfCardCanBePlacedOnCard(selectedCard, selectedPoint.cards[selectedPoint.cards.Count-1]);
            }
        }

        return false;
    }

    public bool CheckIfCardCanBePlacedOnCard(Card selectedCard, Card cardToLandOn) {
        print("In check if card can be placed on card");
        if(cardToLandOn.placePoint == null || cardToLandOn.placePoint.GetLocation() == Location.Discard || !cardToLandOn.isFaceUp) {
            print("In if");
            return false;
        }
        else if(cardToLandOn.placePoint.GetLocation() == Location.Goal) {
            print("In else if 1");
            return selectedCard.cardSO.suit == cardToLandOn.cardSO.suit && selectedCard.cardSO.value == cardToLandOn.cardSO.value + 1;
        }
        
        else if(cardToLandOn.placePoint.GetLocation() == Location.PlayArea) {
            print("In else if 2");
            return selectedCard.cardSO.suitColor != cardToLandOn.cardSO.suitColor && selectedCard.cardSO.value == cardToLandOn.cardSO.value - 1;
        }

        print("Met no conditions");

        return false;

    }

    private void PlaceCardInSpot(PlacePoint placePoint, bool isFaceUp) {
        CardSO cardSO = DeckController.Instance.TakeCardFromRemainingDeck();
        Card card = Instantiate(
            cardToSpawn,
            placePoint.transform.position + new Vector3(0f, -0.5f * placePoint.cards.Count, -0.02f * placePoint.cards.Count),
            isFaceUp ? placePoint.transform.rotation : new Quaternion(0, 1, 0, 0)
        );
        card.SetCardDirection(isFaceUp);
        card.placePoint = placePoint;
        card.SetSO(cardSO);
        placePoint.cards.Add(card);
    }

    public void HandleCardPlaced(Card selectedCard, PlacePoint placePoint) {
        PlacePoint cardLastPoint = selectedCard.GetLastPlacePoint();
        if(cardLastPoint.cards.Contains(selectedCard)) {
            cardLastPoint.cards.Remove(selectedCard);
        }
        if (cardLastPoint.GetLocation() == Location.PlayArea && cardLastPoint.cards.Count > 0) {
            Card cardToFlip = cardLastPoint.cards[cardLastPoint.cards.Count - 1];
            Vector3 cardLandingPoint = cardToFlip.transform.position;
            cardToFlip.transform.position = cardToFlip.transform.position + new Vector3(0,0,-4f);
            cardToFlip.MoveToPoint(cardLandingPoint, cardLastPoint.transform.rotation);
            cardToFlip.SetCardDirection(true);
        }

        int multiplier = placePoint.cards.Count;
        Vector3 cardOffset = placePoint.GetLocation() == Location.Goal ? new Vector3(0f, 0f, -0.5f * multiplier) : new Vector3(0f, -0.5f * multiplier, -0.2f * multiplier);
        Vector3 landingPosition = placePoint.transform.position + cardOffset;
        print("Placing card at spot: " + landingPosition);
        selectedCard.PlaceCard(placePoint, landingPosition, placePoint.transform.rotation);
    }
}


