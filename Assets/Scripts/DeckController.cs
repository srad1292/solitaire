using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    [SerializeField] CardSO[] deck;

    public CardSO[] remaining;

    public CardSO[] flipped;

    public CardSO[] GetDeck() {
        return deck;
    }

}
