using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;

namespace WARBEN
{
    public class GameManager : MonoBehaviourPun
    {
        public static GameManager singleton;
        private PhotonView pv;
        public static ResourcesManager resourcesManager
        {
            get
            {
                if (_resourcesManager == null)
                {
                    _resourcesManager = Resources.Load("ResourcesManager") as ResourcesManager;
                    _resourcesManager.Init();
                }
                
            return _resourcesManager;
            }
        }
        
        static ResourcesManager _resourcesManager;   
        public GameObject cardPrefab;
        public Transform deckZone1;
        public Transform deckZone2;
        private Transform deckZone;
        public Transform sideDeckZone1;
        public Transform sideDeckZone2;
        private Transform sideDeckZone;
        public Transform deckIntZone1;
        public Transform deckIntZone2;
        private Transform deckIntZone;
        private Transform deck = null;
        private int deckIndex = 0;
        public Transform deckVisor;
        public Transform area;
        public Transform area2;
        public Transform sepultura1;
        public Transform sepultura2;
        private Transform sepultura;
        public Transform hand1;
        public Transform hand2;
        private Transform hand;
        public Transform backGround;
        public Transform actionButtons;
        public Sprite backCard;
        public Text empieza;
        public Text codeField;
        public float speedCard;
        public int nameID = 0;
        public SaveDuel fileSave;
        SaveDuel saveDuel;
        public bool saveGameLoaded;
        public Text textSaveLoaded;
        public string prevID;
        public Transform confirmResetButton;
        public Transform cancelResetButton;

        private void Awake()
        {
            singleton = this;
            pv = PhotonView.Get(this);            
            saveDuel = Resources.Load("SaveManager") as SaveDuel;
            if(GameObject.Find("DontDestroy").GetComponent<DontDestroy>().loadSave)
                pv.RPC( "RPC_Awake",RpcTarget.AllViaServer);
        }   
        [PunRPC]
        void RPC_Awake()
        {
            saveGameLoaded = true;
        }

        public void GenerarSeedRandom()
        {
            UnityEngine.Random.InitState ((int)System.Environment.TickCount + PhotonNetwork.GetPing());
        }

        private void Start()
        {
            DeckManager deckManager = Resources.Load("DeckManager") as DeckManager;
            
            if (PhotonNetwork.IsMasterClient)
            {
                GenerarSeedRandom();
                int rand = UnityEngine.Random.Range(0,2);
                
                if (rand == 0)
                    pv.RPC( "RPC_startPlayer",RpcTarget.AllViaServer,"Elige el Jugador 1 (Host)");
                else
                    pv.RPC( "RPC_startPlayer",RpcTarget.AllViaServer,"Elige el Jugador 2 (Invitado)");
                hand = hand1;
                sepultura = sepultura1;
                deckZone = deckZone1.GetChild(0);
                sideDeckZone = sideDeckZone1.GetChild(0);
                deckIntZone = deckIntZone1.GetChild(0);
                area2.gameObject.SetActive(false);
                //print(SaveDuel.card_ids[0]);
                if (!GameObject.Find("DontDestroy").GetComponent<DontDestroy>().loadSave)
                    pv.RPC("RPC_LoadDeck", RpcTarget.AllViaServer, deckManager.decks.Find((x) => x.identifier == deckManager.activeDeck).cards,"SideDeck1", "Deck1","DeckInt1",hand.name,sepultura.name); 
                else
                {
                    string[] _recursos = saveDuel.recursos;
                    for (int i = 0; i < 6; i++)
                    {
                        RecursosManager.singleton.buttonsA.GetChild(i).GetComponentInChildren<Text>().text = _recursos[i];
                        RecursosManager.singleton.pv.RPC("RPC_UpdateButton", RpcTarget.Others,_recursos[i], i);
                        RecursosManager.singleton.buttonsB.GetChild(i).GetComponentInChildren<Text>().text = _recursos[i+6];
                        RecursosManager.singleton.pv.RPC("RPC_UpdateButtonA", RpcTarget.Others,_recursos[i+6], i);
                        
                    }
                    
                    pv.RPC("RPC_LoadSave",RpcTarget.AllViaServer,saveDuel.card_ids, saveDuel.positions, saveDuel.decks, saveDuel.hands,saveDuel.sepulturas);
                    GameObject.Find("DontDestroy").GetComponent<DontDestroy>().loadSave = false;
                }
                    
            }
            else
            {
                deckZone = deckZone2.GetChild(0);
                sideDeckZone = sideDeckZone2.GetChild(0);
                Vector3 pos = deckZone1.position;
                deckZone1.position = deckZone2.position;
                deckZone2.position = pos;
                pos = sideDeckZone1.position;
                sideDeckZone1.position = sideDeckZone2.position;
                sideDeckZone2.position = pos;
                deckIntZone = deckIntZone2.GetChild(0);
                pos = deckIntZone1.position;
                deckIntZone1.position = deckIntZone2.position;
                deckIntZone2.position = pos;
                sepultura = sepultura2;
                pos = sepultura1.position;
                sepultura1.position = sepultura.position;
                sepultura.position = pos;
                area.gameObject.SetActive(false);
                area = area2;
                hand = hand2;
                pos = hand.localPosition;
                
                hand.localPosition = hand1.localPosition;
                hand1.localPosition = pos;
                hand1.localRotation = hand.localRotation;
                hand.localRotation = Quaternion.identity;
                pos = hand.localScale;
                hand.localScale = hand1.localScale;
                hand1.localScale = pos;
                if (!saveGameLoaded)
                    pv.RPC("RPC_LoadDeck", RpcTarget.AllViaServer, deckManager.decks.Find((x) => x.identifier == deckManager.activeDeck).cards,"SideDeck2", "Deck2","DeckInt2",hand.name,sepultura.name);
                
                
            }
        }

#region Operaciones

        public void OperationConfirmReset()
        {
            confirmResetButton.gameObject.SetActive(true);
            cancelResetButton.gameObject.SetActive(true);
        }
        public void OperationReset()
        {
            
            pv.RPC("RPC_Reset", RpcTarget.AllViaServer); 
            confirmResetButton.gameObject.SetActive(false);
            cancelResetButton.gameObject.SetActive(false);
        }

        public void OperationCancelarReset()
        {
            confirmResetButton.gameObject.SetActive(false);
            cancelResetButton.gameObject.SetActive(false);
        }

        public void OperationRobar(string deckSelected)
        {
                DoesOperation("Mover", deckSelected,  hand.name );
                
        }

        public void OperationToHand()
        {
                CardPhysicalInstance c =  MouseOperations.singleton.selectedCard;
                DoesOperation("Mover", c.name,  hand.name );
                OperationCancelar();
                DoesOperation("Revelar", c.name,null );
                
        }

        public void OperationToHandNormal()
        {
                CardPhysicalInstance c =  MouseOperations.singleton.selectedCard;
                
                DoesOperation("Mover", c.name,  hand.name );
                HideActionButtons();
                HideCardsZones();
                backGround.gameObject.SetActive(false);
                if(MouseOperations.singleton.selectedCard != null)
                {
                    MouseOperations.singleton.selectedCard.DeHighlightCard();
                    MouseOperations.singleton.selectedCard = null;
                }   
        }

        public void OperationToDeck() {
                CardPhysicalInstance c =  MouseOperations.singleton.selectedCard;
                if(c.transform.parent.name.Contains("Deck")) {
                    if (c.deck != c.transform.parent){
                        if (c.cardType == "Intangible")
                             DoesOperation("Mover", c.name, deckIntZone.name );
                        else
                            DoesOperation("Mover", c.name, deckZone.name );
                    } else
                        DoesOperation("Mover", c.name, sideDeckZone.name );
                    return;
                }
                    
                if(c.cardType == "Tangible") {
                    DoesOperation("Mover", c.name, deckZone.name );
                    
                }
                else {
                    DoesOperation("Mover", c.name, deckIntZone.name );
                }
                    
                HideActionButtons();
                HideCardsZones();
                if(MouseOperations.singleton.selectedCard != null) {
                    DoesOperation("Ocultar", MouseOperations.singleton.selectedCard.name , null );
                    MouseOperations.singleton.selectedCard.DeHighlightCard();
                    MouseOperations.singleton.selectedCard = null;
                }       
        }

        public void OperationToIntangible() {
                CardPhysicalInstance c =  MouseOperations.singleton.selectedCard;
                
                DoesOperation("Mover", c.name, deckIntZone.name );
                OperationCancelar();      
        }

        public void OperationMarcar()
        {
            DoesOperation("Marcar", MouseOperations.singleton.selectedCard.name , null );
        }
        public void OperationMarcarRandom()
        {
            int childs = MouseOperations.singleton.selectedCard.transform.parent.childCount;
            GenerarSeedRandom();
            int rand = UnityEngine.Random.Range(0,childs);
            print(rand);

            DoesOperation("Marcar", MouseOperations.singleton.selectedCard.transform.parent.GetChild(rand).name , null );
        }


        public void OperationRevelar()
        {
            DoesOperation("Revelar", MouseOperations.singleton.selectedCard.name , null );
            MouseOperations.singleton.buttonRevelar.SetActive(false);
            MouseOperations.singleton.buttonOcultar.SetActive(true);
        }
        public void OperationQuit()
        {
            GameObject.Destroy(GameObject.Find("DontDestroy"));
            PhotonNetwork.LeaveRoom(true);
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Rooms");
            //SceneManager.LoadScene();
            //System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
            //Application.Quit();
        }

        public void OperationGuardarPartida()
        {
           
            string[] _card_ids = new string[150];
            string[] _positions = new string[150];
            string[] _decks = new string[150];
            string[] _hands = new string[150];
            string[] _sepulturas = new string[150];
            string[] _recursos = new string[12];
            
            for (int i = 0; i < 150; i++) 
            {
                if (GameObject.Find(i.ToString()))
                {
                    CardPhysicalInstance _card = GameObject.Find(i.ToString()).GetComponent<CardPhysicalInstance>();
                    _card_ids[i] = _card.actualCard.artSprite.name;
                    if (_card.side)
                        _card_ids[i] = "*" + _card_ids[i];
                    if(_card.transform.parent.name == "Recipiente")
                    {
                        _positions[i] =  _card.transform.parent.name+","+_card.transform.parent.parent.name;
                    }
                    else if(_card.transform.parent.name == "Union")
                    {
                        _positions[i] =  _card.transform.parent.name+","+_card.transform.parent.parent.name;
                    }
                    else if(_card.transform.parent.name.Contains("zone"))
                    {
                        if (_card.transform.localRotation == Quaternion.identity)
                            _positions[i] =  _card.transform.parent.name+",v,"+_card.damage+","+_card.infection+","+_card.contador;
                        else
                            _positions[i] =  _card.transform.parent.name+",h,"+_card.damage+","+_card.infection+","+_card.contador;
                    }
                    else
                    {
                        _positions[i] =  _card.transform.parent.name;
                    }
                        
                    _decks[i] =  _card.deck.name;
                    _hands[i] =  _card.hand.name;
                    _sepulturas[i] =  _card.sepultura.name;
                }
                else
                    break;

            }
            for (int i = 0; i < 6; i++)
                _recursos[i] = RecursosManager.singleton.buttonsA.GetChild(i).GetComponentInChildren<Text>().text;
            for (int i = 0; i < 6; i++)
                _recursos[i+6] = RecursosManager.singleton.buttonsB.GetChild(i).GetComponentInChildren<Text>().text;
            
            saveDuel.SaveCardPos(_card_ids,_positions,_decks,_hands,_sepulturas,_recursos,fileSave);
        }

        public void OperationOcultar()
        {
            DoesOperation("Ocultar", MouseOperations.singleton.selectedCard.name , null );
            MouseOperations.singleton.buttonRevelar.SetActive(true);
            MouseOperations.singleton.buttonOcultar.SetActive(false);
        }
        
        public void OperationAddDamage()
        {
            DoesOperation("AddDamage", MouseOperations.singleton.currentCard.name , null );
        }

        public void OperationSubstractDamage()
        {
            DoesOperation("SubstractDamage", MouseOperations.singleton.currentCard.name , null );
        }

        public void OperationAddContador()
        {
            DoesOperation("AddContador", MouseOperations.singleton.currentCard.name , null );
        }

        public void OperationSubstractContador()
        {
            DoesOperation("SubstractContador", MouseOperations.singleton.currentCard.name , null );
        }

        public void OperationAddInfection()
        {
            DoesOperation("AddInfection", MouseOperations.singleton.currentCard.name , null );
        }
        public void OperationSubstractInfection()
        {
            DoesOperation("SubstractInfection", MouseOperations.singleton.currentCard.name , null );
        }
        public void OperationVerDeck()
        {
            if (MouseOperations.singleton.selectedCard.name == "BackCard")
                deck = MouseOperations.singleton.selectedCard.transform.parent.GetChild(0);
            else
                deck = MouseOperations.singleton.selectedCard.transform.parent;
            
            
            if (deck.GetComponent<CardLupa>() == null)
                return;
            
            CardLupa lupa = deck.GetComponent<CardLupa>();
            MouseOperations.singleton.selectedCard.DeHighlightCard();
            MouseOperations.singleton.selectedCard = null;
            DoesOperation("VerDeck",deck.name,null);
            deck.SetParent(deckVisor);
            deckVisor.GetChild(0).GetComponent<GridLayoutGroup>().spacing = new Vector2(80,100);
            deckVisor.GetChild(0).localPosition = Vector3.zero;
            deck.localScale = Vector3.one;
            deckVisor.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(750,800);
            HideActionButtons();
            MouseOperations.singleton.buttonCancelar.SetActive(true);
            
            backGround.gameObject.SetActive(true);
            
        }

        public void OperationCancelar()
        {
            if (deck != null)
            {
                CardLupa lupa = deck.GetComponent<CardLupa>();
                DoesOperation("VerDeck",deck.name,null);
                
                deckVisor.GetChild(0).SetParent(lupa.originalParent);
                deck.SetSiblingIndex(lupa.index);
                deck.GetComponent<GridLayoutGroup>().spacing = new Vector2(0,0);
                deck.localPosition = Vector3.zero;
                deck.localRotation = Quaternion.identity;
                deck.GetComponent<RectTransform>().sizeDelta = new Vector2(120,180);
                //if(deck.parent.name.Contains("Sepultura"))
                    deck.localScale = Vector3.one;
                deck = null;
            }
            
            HideActionButtons();
            HideCardsZones();
            backGround.gameObject.SetActive(false);
            if(MouseOperations.singleton.selectedCard != null)
            {
                MouseOperations.singleton.selectedCard.DeHighlightCard();
                MouseOperations.singleton.selectedCard = null;
                
            }
                
            
        }

       
#endregion

#region Operaciones RPC

        public void DoesOperation(string action, string selected, string target )
        {
            switch (action)
            {
                case "Mover":
                    pv.RPC("RPC_Mover", RpcTarget.AllViaServer, selected, target);
                    break;
                case "Cadaver":
                    pv.RPC("RPC_Cadaver", RpcTarget.AllViaServer, selected);
                    break;
                case "Union":
                    pv.RPC("RPC_Union", RpcTarget.AllViaServer, selected, target);
                    break;
                case "Encarnar":
                    pv.RPC("RPC_Encarnar", RpcTarget.AllViaServer, selected, target);
                    break;
                case "Recipiente":
                    pv.RPC("RPC_Recipiente", RpcTarget.AllViaServer, selected, target);
                    break;
                case "Revelar":
                    pv.RPC("RPC_Revelar", RpcTarget.AllViaServer, selected);
                    break;
                case "Ocultar":
                    pv.RPC("RPC_Ocultar", RpcTarget.AllViaServer, selected);
                    break;
                case "VerDeck":
                    pv.RPC("RPC_VerDeck", RpcTarget.Others, selected);
                    break;
                case "AddDamage":
                    pv.RPC("RPC_AddDamage", RpcTarget.All, selected);
                    break;
                case "SubstractDamage":
                    pv.RPC("RPC_SubstractDamage", RpcTarget.All, selected);
                    break;
                case "AddInfection":
                    pv.RPC("RPC_AddInfection", RpcTarget.All, selected);
                    break;
                case "SubstractInfection":
                    pv.RPC("RPC_SubstractInfection", RpcTarget.All, selected);
                    break;
                case "AddContador":
                    pv.RPC("RPC_AddContador", RpcTarget.All, selected);
                    break;
                case "SubstractContador":
                    pv.RPC("RPC_SubstractContador", RpcTarget.All, selected);
                    break;
                case "Marcar":
                pv.RPC("RPC_Marcar", RpcTarget.All, selected);
                break;
                default:
                    break;
            }
        }
 
        [PunRPC]
        void RPC_LoadDeck(string[] cards, string sideDeck, string deckTang, string deckInt, string _hand, string _sepultura)
        {
            if (saveGameLoaded)
                return;
            foreach (string id in cards )
            {
                GameObject go = Instantiate(cardPrefab);
                bool respaldo = false;
                string s =id.Substring(0,1);
                if (s == "*")
                {
                    respaldo = true;
                    s = id.Remove(0,1);
                }
                else
                {
                    s = id;
                }
                    

                go.transform.localScale = new Vector3(1.7f,1.7f,1);
                CardPhysicalInstance cp = go.GetComponentInChildren<CardPhysicalInstance>();
                cp.LoadCard(GameManager.resourcesManager.GetCardAsInstance(s));
                
                cp.hand = GameObject.Find(_hand).transform;
                cp.sepultura = GameObject.Find(_sepultura).transform.GetChild(0);
                
                if (cp.cardType == "Intangible" ) 
                {
                    cp.deck = GameObject.Find(deckInt).transform;
                    go.transform.SetParent(GameObject.Find(deckInt).transform);
                }
                    
                else
                {
                    cp.deck = GameObject.Find(deckTang).transform;
                    go.transform.SetParent(GameObject.Find(deckTang).transform);
                }
                cp.sideDeck = GameObject.Find(sideDeck).transform;
                if (respaldo)
                {
                    go.transform.SetParent(GameObject.Find(sideDeck).transform);
                    cp.side = true;
                }
                    
                    

                go.transform.localScale = new Vector3(1f,1f,1);
                go.transform.localPosition = new Vector3(go.transform.position.x,go.transform.position.y,0);
                
                go.SetActive(true);
                go.name = nameID.ToString();
                
                nameID++;
            }
            //OperationDeckShuffle( GameObject.Find(deckTang).transform);
        }
        [PunRPC]
        void RPC_Reset()
        {
            // Reiniciar recursos
            foreach (Transform child in RecursosManager.singleton.buttonsA)
                child.GetComponentInChildren<Text>().text = "0";
            foreach (Transform child in RecursosManager.singleton.buttonsB)
                child.GetComponentInChildren<Text>().text = "0";
            RecursosManager.singleton.buttonsA.GetChild(0).GetComponentInChildren<Text>().text = "30";
            RecursosManager.singleton.buttonsB.GetChild(0).GetComponentInChildren<Text>().text = "30";

            //Reiniciar Decks
            Transform selectedCard;
            CardPhysicalInstance c;
            for(int i = 0; i < nameID;i++)
            {
                selectedCard = GameObject.Find(i.ToString()).transform;
                c = selectedCard.GetComponent<CardPhysicalInstance>();
                
                if (c.side)
                {
                    selectedCard.SetParent(c.deck);
                    RPC_Mover( i.ToString(),  c.sideDeck.name);
                }  
                else
                {
                    selectedCard.SetParent(c.sideDeck);
                    RPC_Mover( i.ToString(),  c.deck.name);
                }
                    
            }

        }
        
        [PunRPC]
        void RPC_LoadSave(string[] _cardsIds, string[] _positions, string[] _decks, string[] _hands, string[] _sepulturas)
        {
            
            bool respaldo;
            for (int i = 0; i < _cardsIds.Length; i++) 
            {
                respaldo = false;
                if (_cardsIds[i] == "")
                   continue;
                nameID++;
                string s =_cardsIds[i].Substring(0,1);
                if (s == "*")
                {
                    respaldo = true;
                    _cardsIds[i] = _cardsIds[i].Remove(0,1);
                }

                GameObject go = Instantiate(cardPrefab);
                CardPhysicalInstance cp = go.GetComponentInChildren<CardPhysicalInstance>();
                cp.LoadCard(GameManager.resourcesManager.GetCardAsInstance(_cardsIds[i]));
                cp.deck = GameObject.Find(_decks[i]).transform;
                if (_decks[i].Contains("1"))
                    cp.sideDeck = GameObject.Find("SideDeck1").transform;
                else
                    cp.sideDeck = GameObject.Find("SideDeck2").transform;

                cp.hand = GameObject.Find(_hands[i]).transform;
                cp.sepultura = GameObject.Find(_sepulturas[i]).transform;
                if (respaldo)
                {
                    cp.side = true;
                    if (_positions[i] == cp.sideDeck.name)
                        cp.transform.SetParent(cp.deck);
                }
                    
                go.SetActive(true);
                go.name = i.ToString();
                
                if (!_positions[i].Contains("Recipiente") & !_positions[i].Contains("Union"))
                {    
                    if (_positions[i].Contains("zone"))
                    {
                        string[] arr = _positions[i].Split(',');
                        go.GetComponent<SmoothMove>().destination = GameObject.Find(arr[0]).transform;
                        
                        cp.damage = int.Parse(arr[2]);
                        cp.infection = int.Parse(arr[3]);
                        cp.contador = int.Parse(arr[4]);
                        cp.updateContador();
                        cp.updateDamage();
                        RPC_Mover( i.ToString(),  arr[0]);
                        if (arr[1] == "h")
                            go.transform.localRotation = Quaternion.Euler(0, 0, 270);
                    }
                    else
                    {
                        go.GetComponent<SmoothMove>().destination = GameObject.Find(_positions[i]).transform;
                        RPC_Mover( i.ToString(),  _positions[i]);
                        
                    }
                    
                }
                
                    
            }
            for (int i = 0; i < _cardsIds.Length; i++) 
            {
                if (_cardsIds[i] == "")
                   continue;

                if (_positions[i].Contains("Recipiente"))
                {
                    string[] arr = _positions[i].Split(',');
                    GameObject.Find(i.ToString()).GetComponent<SmoothMove>().destination = GameObject.Find(arr[1]).transform;
                    RPC_Recipiente2(i.ToString(),arr[1]);
                }
                else if (_positions[i].Contains("Union"))
                {
                    string[] arr = _positions[i].Split(',');
                    GameObject.Find(i.ToString()).GetComponent<SmoothMove>().destination = GameObject.Find(arr[1]).transform;
                   
                    RPC_Union2(i.ToString(),arr[1]);
                }
                
                
            }
            textSaveLoaded.text = "Partida cargada";
        }

        [PunRPC]
        void RPC_VerDeck(string selected)
        {
            Image lupa = GameObject.Find(selected).GetComponent<CardLupa>().lupa;
            if (lupa.enabled)
                lupa.enabled = false;
            else
                lupa.enabled = true;
        }

        [PunRPC]
        void RPC_startPlayer(string startPlayer)
        {
           empieza.text = startPlayer;
        }

        [PunRPC]
        void RPC_AddDamage(string selected)
        {
            GameObject selectedCard = GameObject.Find(selected);
            CardPhysicalInstance cp = selectedCard.GetComponentInChildren<CardPhysicalInstance>();
            cp.addDamage();
            MouseOperations.singleton.textDamage.text = MouseOperations.singleton.currentCard.damage.ToString();
            MouseOperations.singleton.textInfection.text = MouseOperations.singleton.currentCard.infection.ToString();
        }

        [PunRPC]
        void RPC_SubstractDamage(string selected)
        {
            GameObject selectedCard = GameObject.Find(selected);
            CardPhysicalInstance cp = selectedCard.GetComponentInChildren<CardPhysicalInstance>();
            cp.substractDamage();
            MouseOperations.singleton.textDamage.text = MouseOperations.singleton.currentCard.damage.ToString();
            MouseOperations.singleton.textInfection.text = MouseOperations.singleton.currentCard.infection.ToString();
        }

        [PunRPC]
        void RPC_AddContador(string selected)
        {
            GameObject selectedCard = GameObject.Find(selected);
            CardPhysicalInstance cp = selectedCard.GetComponentInChildren<CardPhysicalInstance>();
            cp.addContador();
        }

        [PunRPC]
        void RPC_SubstractContador(string selected)
        {
            GameObject selectedCard = GameObject.Find(selected);
            CardPhysicalInstance cp = selectedCard.GetComponentInChildren<CardPhysicalInstance>();
            cp.substractContador();
        }

        [PunRPC]
        void RPC_AddInfection(string selected)
        {
            GameObject selectedCard = GameObject.Find(selected);
            CardPhysicalInstance cp = selectedCard.GetComponentInChildren<CardPhysicalInstance>();
            cp.addInfection();
            MouseOperations.singleton.textDamage.text = MouseOperations.singleton.currentCard.damage.ToString();
            MouseOperations.singleton.textInfection.text = MouseOperations.singleton.currentCard.infection.ToString();
        }

        [PunRPC]
        void RPC_SubstractInfection(string selected)
        {
            GameObject selectedCard = GameObject.Find(selected);
            CardPhysicalInstance cp = selectedCard.GetComponentInChildren<CardPhysicalInstance>();
            cp.substractInfection();
            MouseOperations.singleton.textDamage.text = MouseOperations.singleton.currentCard.damage.ToString();
            MouseOperations.singleton.textInfection.text = MouseOperations.singleton.currentCard.infection.ToString();
        }
        

        [PunRPC]
        void RPC_Mover(string selected, string target)
        {
            Transform selectedCard = GameObject.Find(selected).transform;
            Transform zoneTarget = GameObject.Find(target).transform;
            CardPhysicalInstance c = selectedCard.GetComponent<CardPhysicalInstance>();
            c.cardArt.sprite = 
            c.actualCard.artSprite;
            c.revelada.GetComponent<Image>().enabled = false;
            c.fondoDamage.GetComponent<Image>().enabled = true;
            selectedCard.localPosition = Vector3.zero;
            if (target.Contains("Hands"))
            {
                
                cleanCard(selectedCard);
                if(target != hand.name)
                {
                    c.cardArt.sprite = backCard;
                    c.fondoDamage.GetComponent<Image>().enabled = false;
                }
                selectedCard.GetComponent<SmoothMove>().SetDestination(c.hand);
                
            }
            else if (target.Contains("Sepultura"))
            {
                cleanCard(selectedCard);
                selectedCard.GetComponent<SmoothMove>().SetDestination(c.sepultura);
                
            }
            else if (target.Contains("Deck"))
            {
                cleanCard(selectedCard);
                if (selectedCard.parent == c.deck){
                    selectedCard.SetParent(c.sideDeck);
                    selectedCard.position = selectedCard.parent.position;
                    selectedCard.localScale = new Vector3(1f,1,1);
                    selectedCard.localRotation = Quaternion.identity;
                }
                else if (c.sideDeck == selectedCard.parent)
                {
                    selectedCard.SetParent(zoneTarget);
                    selectedCard.position = selectedCard.parent.position;
                    selectedCard.localScale = new Vector3(1f,1,1);
                    selectedCard.localRotation = Quaternion.identity;
                }
                    
                else
                    selectedCard.GetComponent<SmoothMove>().SetDestination(zoneTarget);
            }
            else
            {
                
                selectedCard.GetComponent<SmoothMove>().SetDestination(zoneTarget);
                
            }
            
                
        }
        
        [PunRPC]
        void RPC_Encarnar(string selected, string target)
        {
            Transform selectedCard = GameObject.Find(selected).transform;
            Transform zoneTarget = GameObject.Find(target).transform;
            Transform recipiente = zoneTarget.GetChild(0);
            CardPhysicalInstance cp = selectedCard.GetComponent<CardPhysicalInstance>();
            cp.revelada.GetComponent<Image>().enabled = false;
            cp.fondoDamage.GetComponent<Image>().enabled = true;
            cleanCard(recipiente);
            selectedCard.SetParent(zoneTarget);
            selectedCard.SetAsFirstSibling();
            selectedCard.localRotation = Quaternion.identity;
            selectedCard.localPosition = new Vector3(0,0,0);
            cp.cardArt.sprite = 
            cp.actualCard.artSprite;
            selectedCard.localScale = new Vector3(0.5f,0.5f,1);
            RPC_Recipiente(recipiente.name,target);
            
        }

        void cleanCard(Transform card)
        {
            CardPhysicalInstance cp = card.GetComponent<CardPhysicalInstance>();
            cp.damage = 0;
            cp.infection = 0;
            cp.contador = 0;
            cp.updateDamage();
            cp.updateContador();
            foreach (Transform child in cp.recipientes)
            {
                RPC_Mover(child.name,"Sepultura");
            }
            foreach (Transform child in cp.uniones)
            {
                RPC_Mover(child.name,"Sepultura");
            }
        }

        [PunRPC]
        void RPC_Revelar(string selected)
        {
            Transform selectedCard = GameObject.Find(selected).transform;
            CardPhysicalInstance cp = selectedCard.GetComponent<CardPhysicalInstance>();
            cp.revelada.GetComponent<Image>().enabled = true;
            if(cp.hand == hand)
                return;
            cp.cardArt.sprite = 
            cp.actualCard.artSprite;
        }

        [PunRPC]
        void RPC_Ocultar(string selected)
        {
            Transform selectedCard = GameObject.Find(selected).transform;
            CardPhysicalInstance cp = selectedCard.GetComponent<CardPhysicalInstance>();
            cp.revelada.GetComponent<Image>().enabled = false;
            if(hand == cp.hand | deck == cp.deck) 
                return;
            cp.cardArt.sprite = backCard;
            cp.fondoDamage.GetComponent<Image>().enabled = false;
        }

        [PunRPC]
        void RPC_Marcar(string selected)
        {
            CardPhysicalInstance cp = GameObject.Find(selected).transform.GetComponent<CardPhysicalInstance>();
            cp.Target.GetComponent<Image>().enabled = true;
            StartCoroutine(Disable(cp));  
        }
        IEnumerator Disable(CardPhysicalInstance cp)
        {
            yield return new WaitForSeconds(3);
            cp.Target.GetComponent<Image>().enabled = false;
        }

        

        [PunRPC]
        void RPC_Union(string selected, string target)
        {
            Transform selectedCard = GameObject.Find(selected).transform;
            CardPhysicalInstance cp = selectedCard.GetComponent<CardPhysicalInstance>();
            Transform zoneTarget = GameObject.Find(target).transform.GetChild(0).GetComponent<CardPhysicalInstance>().uniones;
            cp.revelada.GetComponent<Image>().enabled = false;
            cp.fondoDamage.GetComponent<Image>().enabled = true;
            selectedCard.GetComponent<SmoothMove>().SetDestination(zoneTarget);
            cp.cardArt.sprite = 
            cp.actualCard.artSprite;
        }

        void RPC_Union2(string selected, string target)
        {
            print(selected + " " + target);
            Transform selectedCard = GameObject.Find(selected).transform;
            CardPhysicalInstance cp = selectedCard.GetComponent<CardPhysicalInstance>();
            Transform zoneTarget = GameObject.Find(target).GetComponent<CardPhysicalInstance>().uniones;
            cp.revelada.GetComponent<Image>().enabled = false;
            cp.fondoDamage.GetComponent<Image>().enabled = true;
            selectedCard.parent = zoneTarget;
            cp.cardArt.sprite = 
            cp.actualCard.artSprite;
        }
        
        [PunRPC]
        void RPC_Recipiente(string selected, string target)
        {
            if (target.Contains("b"))
                target = target.Substring(0, target.Length - 1);
            Transform selectedCard = GameObject.Find(selected).transform;            
            Transform zoneTarget = GameObject.Find(target).transform.GetChild(0).GetComponent<CardPhysicalInstance>().recipientes;
            CardPhysicalInstance cp = selectedCard.GetComponent<CardPhysicalInstance>();
            cleanCard(selectedCard);
            cp.revelada.GetComponent<Image>().enabled = false;
            cp.fondoDamage.GetComponent<Image>().enabled = true;
            selectedCard.GetComponent<SmoothMove>().SetDestination(zoneTarget);
            cp.cardArt.sprite = 
            cp.actualCard.artSprite;
        }

        void RPC_Recipiente2(string selected, string target)
        {
            if (target.Contains("b"))
                target = target.Substring(0, target.Length - 1);
            Transform selectedCard = GameObject.Find(selected).transform;
            Transform zoneTarget = GameObject.Find(target).GetComponent<CardPhysicalInstance>().recipientes;
            CardPhysicalInstance cp = selectedCard.GetComponent<CardPhysicalInstance>();
            cp.revelada.GetComponent<Image>().enabled = false;
            cp.fondoDamage.GetComponent<Image>().enabled = true;
            selectedCard.parent = zoneTarget;
            selectedCard.localPosition = Vector3.zero;
            cp.cardArt.sprite = 
            cp.actualCard.artSprite;
        }

        [PunRPC]
        void RPC_Cadaver(string selected)
        {
            Transform selectedCard = GameObject.Find(selected).transform;
            if (selectedCard.localRotation.eulerAngles.z == 270)
                selectedCard.localRotation = Quaternion.Euler(0, 0, 0);                          
            else
                selectedCard.localRotation = Quaternion.Euler(0, 0, 270);
            selectedCard.GetComponent<CardPhysicalInstance>().damage = 0;
            selectedCard.GetComponent<CardPhysicalInstance>().updateDamage();
        }
#endregion

#region Handle Buttons and Zones
        public void ShowActionButtons(CardPhysicalInstance selected, bool dobleClick)
        {
           
            Transform zone = selected.transform.parent;
            string type = "BackCard";
            string subType = "BackCard";
            string subType2 = "BackCard";
            if (selected.name != "BackCard")
            {
                type = selected.actualCard.cardType;
                subType = selected.actualCard.cardSubType;
                subType2 = selected.actualCard.cardSubType2;
            }
            HideActionButtons();
            HideCardsZones();
            if (zone.name.Contains("DeckZone"))
            {
                if(zone != deckZone.parent)
                    return;
                if(dobleClick)
                {
                    
                    int childs =selected.transform.parent.GetChild(0).childCount;
                    GenerarSeedRandom();
                    int rand = UnityEngine.Random.Range(0,childs);
                    
                    CardPhysicalInstance cp = selected.transform.parent.GetChild(0).GetChild(rand).GetComponent<CardPhysicalInstance>();
                    if (cp.actualCard.name == prevID)
                    {
                        GenerarSeedRandom();
                        if (UnityEngine.Random.Range(0,2) == 1)
                        {
                            rand = UnityEngine.Random.Range(0,childs);
                            cp = selected.transform.parent.GetChild(0).GetChild(rand).GetComponent<CardPhysicalInstance>();                        
                             
                            print("Evito");
                        }
                        else
                        {
                        print("repitio");
                        }
                           
                    }
                    prevID = cp.actualCard.name;
                    OperationRobar(selected.transform.parent.GetChild(0).GetChild(rand).name);
                    selected.DeHighlightCard();
                    MouseOperations.singleton.selectedCard = null;
                }
                else
                {
                    MouseOperations.singleton.buttonVer.SetActive(true);
                }
                return;
            }
            else if (zone.name.Contains("Zone") | zone.name.Contains("Union") | zone.name.Contains("Recipiente") | zone.name.Contains("Sepultura") | zone.name.Contains("SideDeckZ"))
            {
                
                if(zone.name.Contains("Int") & zone != deckIntZone.parent)
                    return;
                 if(zone.name.Contains("SideDeckZ") & zone != sideDeckZone.parent)
                    return;

                MouseOperations.singleton.buttonVer.SetActive(true);
                return;
            }
            else if (zone.parent.name.Contains("Area"))
            {
                if(dobleClick)
                {
                    DoesOperation("Cadaver",selected.name,null);
                    selected.DeHighlightCard();
                    MouseOperations.singleton.selectedCard = null;
                    return;
                }
                if(type != "Intangible" & hand == selected.hand)
                    MouseOperations.singleton.buttonMano.SetActive(true);

                MouseOperations.singleton.buttonMarcar.SetActive(true);
                
            }
            
            
            if(zone.name.Contains("Hands Grid") & type != "Intangible" )
            {
               
                if (hand == selected.hand)
                {
                     if(selected.revelada.GetComponent<Image>().enabled) 
                    {
                        MouseOperations.singleton.buttonOcultar.SetActive(true);
                        MouseOperations.singleton.buttonRevelar.SetActive(false);
                    }
                        
                    else
                    {
                        MouseOperations.singleton.buttonOcultar.SetActive(false);
                        MouseOperations.singleton.buttonRevelar.SetActive(true);
                    }
                    
                }
                else
                {
                    MouseOperations.singleton.buttonRandom.SetActive(true);
                }
                 
            }
            if( deckZone == selected.deck & zone.name.Contains("Hands Grid")) {
                MouseOperations.singleton.buttonMazo.SetActive(true);
                MouseOperations.singleton.buttonIntangible.SetActive(true);
            } else if (deckIntZone == selected.deck ) {
                MouseOperations.singleton.buttonIntangible.SetActive(true);
            }
                

            if(zone.name.Contains("Hands Grid") & hand != selected.hand)
                return;
            ShowCardsZones(type,subType,subType2,zone);
        }

        public void HideActionButtons()
        {
            foreach(Transform child in actionButtons)
            {
                child.gameObject.SetActive(false);
            }
        }

        public void ShowCardsZones(string type, string subType, string subType2, Transform zone)
        {
            int cont = 0;
            string targetType;
            string targetSubType;
            string cardName;
            foreach(Transform child in area)
            {
                
                cont++;

                if(cont == 13)
                    break;
                if (child.childCount > 0)
                {
                    targetType = child.GetChild(0).GetComponent<CardPhysicalInstance>().actualCard.cardType;
                    targetSubType = child.GetChild(0).GetComponent<CardPhysicalInstance>().actualCard.cardSubType;
                    cardName = child.GetChild(0).GetComponent<CardPhysicalInstance>().actualCard.name;
                    
                    if(cont<7 & type == "Intangible" & subType == "Entidad" & targetType == "Tangible") 
                    {
                        child.GetComponent<Image>().enabled = true;
                        child.GetComponent<Image>().color = new Color32(175,88, 255, 60);
                        child.GetComponent<AreaHook>().currentOperation = GameOperations.Encarnar;
                    } 
                    else if( (subType == "Maldicion" | subType2 == "Unir"))
                    {
                        child.GetComponent<Image>().enabled = true;
                        child.GetComponent<Image>().color = new Color32(250,255, 88, 60);
                        child.GetComponent<AreaHook>().currentOperation = GameOperations.Union;
                    }
                    else if(  subType == "Herramienta")
                    {
                        if(  subType2 == "Unir" | cardName == "A-098" | cardName == "A-052")
                        {
                            child.GetComponent<Image>().enabled = true;
                            child.GetComponent<Image>().color = new Color32(250,255, 88, 60);
                            child.GetComponent<AreaHook>().currentOperation = GameOperations.Union;
                        }
                        
                    }
                    if(cont<7 & type == "Tangible" & targetType == "Intangible" ) 
                    {
                        
                        area.GetChild(cont+11).GetComponent<Image>().enabled = true;
                        area.GetChild(cont+11).GetComponent<Image>().color = new Color32(175,88, 255, 100);
                        area.GetChild(cont+11).GetComponent<AreaHook>().currentOperation = GameOperations.Recipiente;
                    }
                }
                else 
                {
                    if ( type == "Tangible" | subType == "Entidad")
                    {
                        child.GetComponent<Image>().enabled = true;
                        child.GetComponent<Image>().color = new Color32(88,128, 255, 60);
                        child.GetComponent<AreaHook>().currentOperation = GameOperations.Mover;
                    }
                    
                }
    
            }

            sepultura.GetChild(0).GetComponent<Image>().enabled = true;
            sepultura.GetChild(0).GetComponent<AreaHook>().currentOperation = GameOperations.Mover;
        }       

        public void HideCardsZones()
        {          
            foreach(Transform child in area)
            {
                child.GetComponent<Image>().enabled = false;
            }
            sepultura.GetChild(0).GetComponent<Image>().enabled = false;
        }

        public enum GameOperations
        {
            Mover, Union, Recipiente,Encarnar
        }

#endregion

    }
    
}

