using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{

    public static DeckController Instance;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    public Card cardToSpawn;

    [SerializeField] CardSO[] deck;

    [SerializeField] float timeBetweenDraws;

    public List<CardSO> remaining;

    public PlacePoint discardPile;

    private bool canDraw = true;

    private void Start() {
    }

    public void ShuffleDeckIntoRemaining() {
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

    private void OnMouseDown() {
        if (canDraw) {
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
        if(remaining.Count <= 0 && discardPile.cards.Count >= 0) {
            print("Moving discard back to deck");
            remaining.Clear();
            foreach(Card card in discardPile.cards) {
                remaining.Add(card.cardSO);
                Destroy(card.gameObject);
            }
            discardPile.cards.Clear();
        } 

        if(remaining.Count > 0) {
            print("Drawing a card!");
            Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
            newCard.SetSO(remaining[0]);
            remaining.RemoveAt(0);
            newCard.placePoint = discardPile;
            discardPile.cards.Add(newCard);
            float landingZ = -0.05f * discardPile.cards.Count;
            newCard.transform.position = newCard.transform.position + new Vector3(0, 0, landingZ - 2f);
            newCard.MoveToPoint(discardPile.transform.position+new Vector3(0f, 0f, landingZ), discardPile.transform.rotation);
            newCard.SetCardDirection(true);
        }
    }

    public List<CardSO> GetRemainingDeck() {
        return remaining;
    }

    public CardSO TakeCardFromRemainingDeck() {
        CardSO cardSO = remaining[0];
        remaining.RemoveAt(0);
        return cardSO;
    }

}
