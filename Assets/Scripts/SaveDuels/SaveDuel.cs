using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WARBEN
{
    [CreateAssetMenu]
    public class SaveDuel : ScriptableObject
    {
        public string[] card_ids = new string[150];
        public string[] positions = new string[150];
        public string[] decks = new string[150];
        public string[] hands = new string[150];
        public string[] sepulturas = new string[150];
        public string[] recursos = new string[12];
       

        public void SaveCardPos(string[] _card_ids, string[] _positions, string[] _decks, string[] _hands, string[] _sepulturas,string[] _recursos, SaveDuel saveDuelFile)
        {
            
            card_ids = _card_ids;
            positions = _positions;
            decks = _decks;
            hands = _hands;
            sepulturas = _sepulturas;
            recursos = _recursos;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath+"/saveDuel.txt");
            var json = JsonUtility.ToJson(saveDuelFile);
            bf.Serialize(file,json);
            file.Close();
            
        }

        
        
    }


}

