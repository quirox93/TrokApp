using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WARBEN
{
    public class CardDamage : MonoBehaviour
    {
        private Text text;
        private CardPhysicalInstance card;

        void Start()
        {
            text = this.GetComponent<Text>();
            card = this.GetComponentInParent<CardPhysicalInstance>();

        }
        void Update()
        {
            int suma = card.damage + card.infection;
            if ( suma == 0)
            {
                text.text = "";
            }
            else
                text.text = suma.ToString();
        }
    }

}
