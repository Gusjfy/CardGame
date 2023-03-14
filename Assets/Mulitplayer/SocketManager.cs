using System;
using System.Net;
using System.Xml.Serialization;
using WebSocketSharp;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    private WebSocket socket;
    public GameObject cardPrefab;
    public GameObject player;
    public GameObject enemy;
    public PlayerData enemyData;
    public PlayerData playerData;


    private static SocketManager _instance;

    
    public static SocketManager Instance
    {
        get
        {
            _instance = FindObjectOfType<SocketManager>();
            return _instance;
        }
    }
    

    void Start()
    {
        socket = new WebSocket("ws://localhost:8080");
        socket.Connect();

        socket.OnMessage += (sender, e) =>
        {
            if( e.Data.Contains("Closed:") )
            {
                Debug.Log("disconnected");
            }
            
            if (e.IsText)
            {
                PlayerData tempPlayerData = JsonUtility.FromJson<PlayerData>(e.Data);
                this.ReceivePlayerData(tempPlayerData);
                this.ReceiveEnemyData(tempPlayerData);
                
            }
        };
        
        socket.OnClose += (sender, e) =>
        {
            Debug.Log(e.Code);
            Debug.Log(e.Reason);
            Debug.Log("Connection Closed!");
        };
        
    }

    private void Update()
    {
        this.SendPlayerData();
        this.AttEnemy();
    }

    private void ReceivePlayerData(PlayerData playerData)
    {
        if (this.playerData.id == "")
        {
            Debug.Log("rodou apenas uma vez. Id:" + playerData.id);
            this.playerData = playerData;
        }
    }

    private void ReceiveEnemyData(PlayerData playerData)
    {
        if (playerData.id != this.playerData.id && playerData.id != "")
        {
            this.enemyData = playerData;
        }
    }

    public void AttEnemy()
    {
        if (this.enemyData.id != "")
        {
            this.UpdateCards(enemyData.cardsOnBoard, this.enemy.transform.Find("EnemysBoard").gameObject);
            this.UpdateCards(enemyData.cardsOnHand, this.enemy.transform.Find("EnemysHand").gameObject);
        }
    }

    private void UpdateCards(CardData[] placeData, GameObject place)
    {
        //destroy 
        if (placeData != null)
        {
            if (placeData.Length < place.transform.childCount)
            {
                Debug.Log("Destruindo as cartas");
                foreach (Transform child in place.transform) {
                    Destroy(child.gameObject);
                }
            }
        
            //create the cards
            foreach (var cardData in placeData)
            {
                foreach (Transform child in place.transform)
                {
                    Card placeCard = child.gameObject.GetComponent<Card>();
                    if (placeCard.id == cardData.id)
                    {
                        goto End;
                    }
                }
                Debug.Log("Criando a carta " + cardData.id);
                GameObject cardObj = Instantiate(cardPrefab,  Vector3.zero, Quaternion.identity, place.transform);
                cardObj.GetComponent<Card>().id = cardData.id;
                cardObj.GetComponent<Card>().playerId = cardData.playerId;
                End:;
            }       
        }
    }

    public void SendPlayerData()
    {
        if (playerData.id != "")
        {
            playerData.timestamp = this.GetTimestamp();
            playerData.cardsOnBoard = this.GetCardsFrom(player.transform.Find("PlayersBoard").gameObject);
            playerData.cardsOnHand = this.GetCardsFrom(player.transform.Find("PlayersHand").gameObject);
            string playerDataJson = JsonUtility.ToJson(playerData);
            socket.Send(playerDataJson);
        }
    }

    private CardData[] GetCardsFrom(GameObject playersHand)
    {
        int cardsCount = playersHand.transform.childCount;
        CardData[] cards = new CardData[cardsCount];
        for (int key = 0; key < cardsCount; key++)
        {
            Card card = playersHand.transform.GetChild(key).gameObject.GetComponent<Card>();
            CardData data = new CardData();
            data.id = card.id;
            data.playerId = card.playerId;
            cards[key] = data;
        }
        return cards;
    }

    private double GetTimestamp()
    {
        System.DateTime epochStart =  new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        return (System.DateTime.UtcNow - epochStart).TotalSeconds;
    }

    private void OnDestroy()
    {
        socket.Close();
    }

}
