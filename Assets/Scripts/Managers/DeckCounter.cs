using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WARBEN
{
    public class DeckCounter : MonoBehaviour
    {
        public Transform obj;
        public Transform image;
        private int c = 0;
        
        void Update()
        {
            if (obj.childCount != c)
            {
                this.GetComponent<Text>().text = obj.childCount.ToString();
                c= obj.childCount;
                if(c == 0)
                    image.GetComponent<Image>().enabled = false;
                else
                    image.GetComponent<Image>().enabled = true;
                
            }
        }
    }
}

