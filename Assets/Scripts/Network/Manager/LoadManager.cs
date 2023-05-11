using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;

namespace WARBEN
{
    public class LoadManager : MonoBehaviour
    {
        public DeckManager deckManager;
        public SaveDuel saveManager;
        public Text activeDeck;  

        public void Awake()
        {
            if (File.Exists(Application.persistentDataPath + "/Mazos.txt"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/Mazos.txt",FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file),deckManager);
                file.Close();
                activeDeck.text = deckManager.activeDeck;
            }
            if (File.Exists(Application.persistentDataPath + "/SaveDuel.txt"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/SaveDuel.txt",FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file),saveManager);
                file.Close();
            }
            
        }

        public void OnButtonVolver()
        {
           if (File.Exists(Application.persistentDataPath + "/Mazos.txt"))
            { 
                activeDeck.text = deckManager.activeDeck;
            } 
        }
    }
}

