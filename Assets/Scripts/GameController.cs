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
}


