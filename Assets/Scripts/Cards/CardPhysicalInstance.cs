using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WARBEN
{
    public class CardPhysicalInstance : MonoBehaviour
    {
        public Card actualCard
        {
            get {
                return _actualCard;
            }
        }
        
        Card _actualCard;

        public Image cardArt;
        public string cardType;
        public string cardSubType;
        public string cardSubType2;
        public int damage = 0;
        public int contador = 0;
        public int infection = 0;
        public int limite = 3;
        public string[] expancion;
        public Transform card;
        public Transform recipientes;
        public Transform uniones;
        public Transform revelada;
        public Transform BackCard;
        public Transform Target;
        public Text Damage;
        public Text Contador;
        public Transform sepultura;
        public Transform deck;
        public Transform sideDeck;
        public Transform hand;
        public Transform fondoDamage;
        public bool side;


        public void LoadCard(Card targetCard)
        {
            _actualCard = targetCard;
            cardArt.sprite = targetCard.artSprite;
            cardType = targetCard.cardType;
            cardSubType = targetCard.cardSubType;
            cardSubType2 = targetCard.cardSubType2;
            expancion = targetCard.expancion;
            limite = targetCard.limite;
        }

        public void HighlightCard()
        {
            if(transform.name == "BackCard")
            {
                transform.GetComponentInChildren<Image>().color = Color.yellow;
            }
            else
                transform.GetChild(2).GetComponent<Image>().color = Color.yellow;
                        
        }

        public void addDamage()
        {
            damage++;
            updateDamage();
        }

        public void substractDamage()
        {
            if (damage != 0)
                damage--;

            updateDamage();
        }

        public void addInfection()
        {
            infection++;
            updateDamage();
        } 

        public void substractInfection()
        {
            if (infection != 0)
                infection--;

            updateDamage();
        } 

        public void addContador()
        {
            contador++;
            updateContador();
        } 

        public void substractContador()
        {
            if (contador != 0)
                contador--;
            updateContador();
        }

        public void updateContador()
        {
            if (contador == 0)
            {
                Contador.text = "";
            }
            else
                Contador.text = contador.ToString();
        }

        public void updateDamage()
        {
            int suma = damage + infection;
            if ( suma == 0)
            {
                Damage.text = "";
            }
            else
            {
                Damage.text = suma.ToString();
                if (infection > 0)
                    Damage.color = Color.green;
                else
                    Damage.color = Color.red;
            }
                
        }

        public void DeHighlightCard()
        {   
            if(transform.name == "BackCard")
            {
                transform.GetComponentInChildren<Image>().color = Color.white;
            }
            else
                transform.GetChild(2).GetComponent<Image>().color = Color.white;
            
        }


    }
}
