using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WARBEN
{
    [CreateAssetMenu]
    public class ResourcesManager : ScriptableObject
    {
        public Card[] all_cards;
        Dictionary<string,Card> cardsDict = new Dictionary<string, Card>();


        public void Init()
        {
            cardsDict.Clear();
            for (int i = 0; i < all_cards.Length; i++)
            {
                if (all_cards[i] != null)
                    cardsDict.Add(all_cards[i].name, all_cards[i]);
            }
        }

        Card GetCardOriginal(string id)
        {
            Card retVal = null;
            cardsDict.TryGetValue(id, out retVal);
            return retVal;
        }

        public Card GetCardAsInstance(string id)
        {
            Card instancedCard = Instantiate(GetCardOriginal(id));
            instancedCard.name = id;
            return instancedCard;
        }

    }
}

