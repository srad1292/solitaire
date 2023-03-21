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
    private bool isSelected = false;

    private void Start() {
        UpdateDisplay();
    }

    public void SetSO(CardSO cardSO) {
        this.cardSO = cardSO;
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        if(cardSO == null) { return; }

        foreach (TMP_Text text in numberDisplays) {
            text.SetText(cardSO.display);
            text.color = cardSO.suitColor;
        }
        foreach (Image img in suitDisplays) {
            img.sprite = cardSO.suitSprite;
            img.color = cardSO.suitColor;
        }
    }

}
