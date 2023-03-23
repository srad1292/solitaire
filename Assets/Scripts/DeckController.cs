using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{

    public Card cardToSpawn;

    [SerializeField] CardSO[] deck;

    [SerializeField] float timeBetweenDraws;

    public List<CardSO> remaining;

    public List<CardSO> flipped;

    public PlacePoint discardPile;

    private bool canDraw = true;

    private void Start() {

        ShuffleDeckIntoRemaining();
    }

    private void ShuffleDeckIntoRemaining() {
        remaining.Clear();
        List<CardSO> deckCopy = new List<CardSO>();
        deckCopy.AddRange(deck);
        int idx = 0;
        while(remaining.Count < deck.Length) {
            idx = Random.Range(0, deckCopy.Count);
            remaining.Add(deckCopy[idx]);
            deckCopy.RemoveAt(idx);
        }
    }

    public CardSO[] GetDeck() {
        return deck;
    }

    private void OnMouseDown() {
        if(canDraw) {
            canDraw = false;
            DrawCard();
            StartCoroutine(WaitBetweenDraws());
        }
    }

    IEnumerator WaitBetweenDraws() {
        yield return new WaitForSeconds(timeBetweenDraws);
        canDraw = true;
    }

    private void DrawCard() {
        print("In Draw Card");
        if(remaining.Count <= 0 && flipped.Count >= 0) {
            print("Moving flipped back to deck");
            remaining.Clear();
            remaining.AddRange(flipped);
            foreach(Card card in discardPile.cards) {
                Destroy(card.gameObject);
            }
            flipped.Clear();
        } 
        if(remaining.Count > 0) {
            print("Drawing a card!");
            Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
            newCard.SetSO(remaining[0]);
            flipped.Add(remaining[0]);
            remaining.RemoveAt(0);
            newCard.placePoint = discardPile;
            discardPile.cards.Add(newCard);
            newCard.MoveToPoint(discardPile.transform.position+new Vector3(0f, 0f, -0.05f*discardPile.cards.Count), discardPile.transform.rotation);
            newCard.isFaceUp = true;
        }
    }

}
