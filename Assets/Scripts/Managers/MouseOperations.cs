using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WARBEN
{
    public class MouseOperations : MonoBehaviour
    {
        public CardPhysicalInstance currentCard;
        public CardPhysicalInstance selectedCard;
        AreaHook currentAreaHook;
        public Transform visor;
        public float timerClick;

        public GameObject buttonRecipiente;
        public GameObject buttonEncarnar;
        public GameObject buttonRevelar;
        public GameObject buttonOcultar;
        public GameObject buttonVer;
        public GameObject buttonMover;
        public GameObject buttonMano;
        public GameObject buttonMazo;
        public GameObject buttonMarcar;
        public GameObject buttonAtacar;
        public GameObject buttonCancelar;
        public GameObject buttonRandom;
        public GameObject buttonIntangible;
        public Text textDamage;
        public Text textInfection;

        public static MouseOperations singleton;

        private void Awake()
        {
            singleton = this;
        }

        private void Update()
        {
           //HandleCardDetection();
           HandleMouse();
        }

        void HandleMouse()
        {
            bool isMouseDown = Input.GetMouseButton(0);
            if (isMouseDown)
                timerClick = Time.time;
            bool isMouseUp = Input.GetMouseButtonUp(0);

            if (isMouseUp && (Time.time - timerClick) < 0.25)
            {
                HandleAreaDetection();
                
            }
            else 
            {
                HandleCardDetection(false);
            }

            if (Input.GetMouseButton(0))
            {
                List<RaycastResult> l = getUIObjs();
                 for ( int i = 0; i < l.Count; i++)
                {
                    if (l[0].gameObject.name == "VisorImage")
                    {
                        visor.localScale= new Vector3(1.4f,1.4f,1);
                        //visor.localPosition= visor.localPosition +new Vector3(50,0,0);
                    }
                    else
                    {
                        visor.localScale= new Vector3(1,1,1);
                        //visor.localPosition= visor.localPosition -new Vector3(50,0,0);
                    }
                }
                
            }

            if(isMouseUp)
            {
               visor.localScale= new Vector3(1,1,1);
                //visor.localPosition= new Vector3(-500,95,1); 
            }
            
            
        }

        void HandleAreaDetection()
        {
            List<RaycastResult> l = getUIObjs();

            for ( int i = 0; i < l.Count; i++)
            {
                

                currentAreaHook = l[i].gameObject.GetComponentInParent<AreaHook>();

                if (currentAreaHook != null && currentAreaHook.gameObject.GetComponent<Image>().enabled == true && selectedCard.transform.parent.name != "DeckZone")
                {
                    GameManager.singleton.HideActionButtons();
                     GameManager.singleton.HideCardsZones();   
                    GameManager.singleton.DoesOperation(currentAreaHook.GetComponentInParent<AreaHook>().currentOperation.ToString(),selectedCard.name,currentAreaHook.gameObject.name);
                    selectedCard.DeHighlightCard();
                    selectedCard = null;
                   
                    return;
                }

            }
            
            HandleCardDetection(true);
        }

        void HandleCardDetection(bool click)
        {
            List<RaycastResult> l = getUIObjs();
            
            CardPhysicalInstance cardInstance = null;
            string objName = "";

            for ( int i = 0; i < l.Count; i++)
            {
                cardInstance = l[i].gameObject.GetComponentInParent<CardPhysicalInstance>();
                objName = l[i].gameObject.name;
                 
                if (cardInstance != null |  objName == "Acciones")
                {                    
                    break;
                }
            }
            // CARGAR IMAGEN EN EL VISOR
            if (cardInstance != null)
            {
                if ((cardInstance.name != "BackCard" | click))
                    {
                        visor.GetComponent<Image>().sprite = cardInstance.cardArt.sprite;
                        currentCard = cardInstance;
                        textDamage.text = cardInstance.damage.ToString();
                        textInfection.text = cardInstance.infection.ToString();
                        
                    }
            }
            else if (selectedCard != null)
            {
                currentCard = selectedCard;
                visor.GetComponent<Image>().sprite = selectedCard.cardArt.sprite;
                textDamage.text = selectedCard.damage.ToString();
                textInfection.text = selectedCard.infection.ToString();
            }
            // RESTRINGIR ACCIONES EN VER MAZO
            if (objName == "Acciones" | buttonCancelar.activeSelf )
            {
                if (click & cardInstance != null  )
                {
                    if (cardInstance.transform.parent.parent.name != "DeckVisor")
                        return;
                    if (selectedCard != null)
                        selectedCard.DeHighlightCard();
                    selectedCard = cardInstance;
                    selectedCard.HighlightCard();
                    if (cardInstance.transform.parent.name.Contains("Deck"))
                        buttonMazo.SetActive(true);
                    if (!cardInstance.transform.parent.name.Contains("SideDeck"))
                        buttonMover.SetActive(true);
                    if (cardInstance.transform.parent.name.Contains("Recipiente"))
                        buttonIntangible.SetActive(true);
                }
                return;
            }

            // RECONOCER CLICK EN CARTA
            if (click & cardInstance != null)
            {
                bool dobleClick = false;                
                
                if (selectedCard == cardInstance)
                    dobleClick = true;
                else 
                {
                    if (selectedCard != null)
                        selectedCard.DeHighlightCard();
                    selectedCard = cardInstance;
                    selectedCard.HighlightCard();
                }

                if (selectedCard != null)
                {
                    GameManager.singleton.ShowActionButtons(selectedCard,dobleClick);
                }
                else
                {
                    GameManager.singleton.HideCardsZones();
                    GameManager.singleton.HideActionButtons();  
                }
                    
            }                
            else
            {
                if (selectedCard != null & click)
                {   
                    GameManager.singleton.HideCardsZones(); 
                    GameManager.singleton.HideActionButtons();
                    selectedCard.DeHighlightCard();
                    selectedCard = null;
                }
            }
        }

        

        public static List<RaycastResult> getUIObjs()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            return results;


        }

    }
}
    
