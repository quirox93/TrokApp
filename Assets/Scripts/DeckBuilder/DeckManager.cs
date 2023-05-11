using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WARBEN
{
    [CreateAssetMenu]
    public class DeckManager : ScriptableObject
    {
        public List<Deck> decks = new List<Deck>();
        public string activeDeck = "Mazo 1";
        
       

        public void SaveDeck(string id , List<CardPhysicalInstance> l, DeckManager deckFile)
        {
            Deck _d = decks.Find((x) => x.identifier == id);
            if (_d != null)
            {
                decks.Remove(_d);
            }

            Deck d = new Deck();
            d.identifier = id;
            List<string> cardsIds = new List<string>();
            
            foreach(CardPhysicalInstance c in l)
            {
                cardsIds.Add(c.name);
            }

            if (cardsIds.Count != 0)
            {
                d.cards = cardsIds.ToArray();
                decks.Add(d);
            }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath+"/Mazos.txt");
            var json = JsonUtility.ToJson(deckFile);
            bf.Serialize(file,json);
            file.Close();
            
        }
        
    }

    [System.Serializable]
    public class Deck
    {
        public string identifier;
        public string[] cards;
    }
}

