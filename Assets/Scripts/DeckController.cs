using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    [SerializeField] CardSO[] deck;

    public List<CardSO> remaining;

    public List<CardSO> flipped;

    public PlacePoint discardPile;

    public CardSO[] GetDeck() {
        return deck;
    }

    private void DrawCard() {
        if(remaining.Count <= 0 && flipped.Count >= 0) {
            remaining.Clear();
            remaining.AddRange(flipped);
            foreach(Card card in discardPile.cards) {
                Destroy(card.gameObject);
            }
        } 
        if(remaining.Count > 0) {
            // instanstiate card
            // move card to discard pile
        }
    }

}
