using System;

[Serializable]
public class PlayerData
{
    public string id;
    public CardData[] cardsOnBoard;
    public CardData[] cardsOnHand;
    public double timestamp;
}
