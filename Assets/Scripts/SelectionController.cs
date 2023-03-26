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

    [SerializeField] LayerMask cardLayer;
    [SerializeField] LayerMask tableLayer;
    [SerializeField] LayerMask placementLayer;
    public Card selectedCard = null;



    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, tableLayer) && selectedCard != null) {
            selectedCard.MoveToPoint(hit.point + new Vector3(0, 0.5f, -1.5f), Quaternion.identity);
        }
        if (Input.GetMouseButtonDown(0)) {
            
            if (Physics.Raycast(ray, out hit, 100f, placementLayer) && selectedCard != null) {
                print("I hit placement layer");
                PlacePoint selectedPoint = hit.collider.GetComponent<PlacePoint>();
                if (selectedPoint != null && selectedPoint.cards.Count == 0) {
                    selectedCard.PlaceCard(selectedPoint, selectedPoint.transform.position, selectedPoint.transform.rotation);
                    selectedCard = null;
                }
                else {
                    selectedCard.ReturnCard();
                    selectedCard = null;
                }
            }
            else if (Physics.Raycast(ray, out hit, 100f, cardLayer)) {
                Card card = hit.collider.GetComponent<Card>();
                print("SC HIT: " + card.cardSO.value + " of " + card.cardSO.suit);
                if (selectedCard == null) {
                    if(card.isFaceUp && card.canSelect) {
                        card.SelectCard();
                        selectedCard = card;

                    }
                } else {
                    if(selectedCard != card) {
                        selectedCard.ReturnCard();
                        selectedCard = null;
                    }
                }

            }
        } else if(Input.GetMouseButtonDown(1)) {
            if(selectedCard != null) {
                selectedCard.ReturnCard();
                selectedCard = null;
            }
        }
    }
}
