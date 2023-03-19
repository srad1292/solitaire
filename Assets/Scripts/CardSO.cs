using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]
public class CardSO : ScriptableObject
{
    public int value;
    public string display;
    public Suit suit;
    public Sprite suitSprite;
}
