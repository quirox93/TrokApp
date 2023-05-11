using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace WARBEN
{
    public class DeckBuilder : MonoBehaviour
    {
        public int maxCards = 60;
        public Transform allCardsGrid;
        public Transform deckGrid;
        public Transform deckIntangibleGrid;
        public Transform sideDeckGrid;
        public Toggle toogleSide;
        public GameObject cardPrefab;
        public Transform visor;
        public float timerClick;
        public DeckManager fileDeck;
        public Text counter;
        public InputField inputDeckName;
        

        List<CardPhysicalInstance> deckCards = new List<CardPhysicalInstance>();
        public Button saveButton;
        public Dropdown dropDown;
        public Dropdown dropDownExpansion;
        DeckManager deckManager;

        private void Awake()
        {
            deckManager = Resources.Load("DeckManager") as DeckManager;
            deckManager.decks.Sort((Deck t1, Deck t2) => { return t1.identifier.CompareTo(t2.identifier); });
            dropDown.ClearOptions();
            List<string> optionsList = new List<string>();
            foreach (Deck d in deckManager.decks)
            {
                optionsList.Add(d.identifier);
            }
            dropDown.AddOptions(optionsList);
            dropDown.SetValueWithoutNotify(0);
            
        }
        private void Start()
        {
           
           dropDownExpansion.SetValueWithoutNotify(1);
            LoadCards("Mazo de iniciacion");
            

        }

        private void LoadCards(string expansion)
        {
            foreach (Transform child in allCardsGrid.transform) {
                Destroy(child.gameObject);
            }
            Card[] cards = GameManager.resourcesManager.all_cards;
            foreach (Card c in cards)
            {
                if (c == null)
                    continue;
                foreach (string exp in c.expancion)
                    if (exp == expansion)
                    {
                        GameObject go = Instantiate(cardPrefab);
                        go.transform.SetParent(allCardsGrid);
                        go.transform.localScale = new Vector3(1.7f,1.7f,1);
                        CardPhysicalInstance cp = go.GetComponentInChildren<CardPhysicalInstance>();

                        cp.LoadCard(c);
                        go.SetActive(true);
                    }
               
            }
        }

        private void OnEnable()
        {
            foreach(CardPhysicalInstance c in deckCards)
            {
                if (c != null)
                {
                    Destroy(c.gameObject);
                }
            }
            deckCards.Clear();
                loadDeck(dropDown.options[dropDown.value].text);
        }

        private void loadDeck(string deckName)
        {
            if (deckManager.decks.Find((x) => x.identifier == deckName) == null)
                return;

            
            foreach (string id in deckManager.decks.Find((x) => x.identifier == deckName).cards)
            {
                GameObject go = Instantiate(cardPrefab);
                
                go.transform.localScale = new Vector3(1.7f,1.7f,1);
                CardPhysicalInstance cp = go.GetComponentInChildren<CardPhysicalInstance>();
                
                string s =id.Substring(0,1);
                if(s == "*")
                {
                    s = id.Remove(0,1);
                    cp.LoadCard(GameManager.resourcesManager.GetCardAsInstance(s));
                    deckCards.Add(cp);
                    go.transform.SetParent(sideDeckGrid); 
                    go.name =  "*"+cp.actualCard.name;
                }
                else
                {
                    s = id;
                    cp.LoadCard(GameManager.resourcesManager.GetCardAsInstance(s));
                    deckCards.Add(cp);
                    
                    if (cp.cardType == "Intangible" ) 
                        go.transform.SetParent(deckIntangibleGrid);
                    else 
                        go.transform.SetParent(deckGrid); 
                    go.name = cp.actualCard.name;
                }
                    
                                
                go.transform.localScale = new Vector3(2.2f,2.2f,1);
                
                go.SetActive(true);  
                
            }
            inputDeckName.text = deckName;
            
            Invoke("UpdateCounter",1);
        }

        

        private void UpdateCounter()
        {
            counter.text = "Tangibles: "+deckGrid.childCount.ToString() + " | Intangibles: "+deckIntangibleGrid.childCount.ToString(); 
        }

        

        private void Update()
        {
            List<RaycastResult> l = GetUiObjects();
            bool isMouseDown = Input.GetMouseButtonDown(0);
            if (isMouseDown)
                timerClick = Time.time;
            bool isMouseUp = Input.GetMouseButtonUp(0);
            if (Input.GetMouseButton(0))
            {
                    for ( int i = 0; i < l.Count; i++)
                {
                    if (l[0].gameObject.name == "Visor")
                    {
                        visor.localScale= new Vector3(1.7f,1.7f,1);
                        visor.localPosition= new Vector3(-680,112,1);
                    }
                    else
                    {
                        visor.localScale= new Vector3(1.2f,1.2f,1);
                        visor.localPosition= new Vector3(-725,112,1);
                    }
                }
                
            }
            if(isMouseUp)
            {
               visor.localScale= new Vector3(1.2f,1.2f,1);
                visor.localPosition= new Vector3(-725,112,1);
            }

            for (int i = 0; i < l.Count; i++)
            {
               if (isMouseUp && (Time.time - timerClick) < 0.25 )
               {
                   if (l[i].gameObject.name == "Ventana Filtro")
                        return;
               }
                CardPhysicalInstance c = l[i].gameObject.GetComponentInParent<CardPhysicalInstance>();
                
                if (c != null)
                {
                    
                    visor.GetComponent<Image>().sprite = c.GetComponentInChildren<Image>().sprite;
                    if (isMouseUp && (Time.time - timerClick) < 0.25 )
                    {
                        if (c.transform.parent == allCardsGrid)
                        {
                            
                            if (FindGameObjectsWithSameName(c.cardArt.sprite.name) == c.limite ||c.cardType == "Intangible" & deckIntangibleGrid.childCount == 10 & !toogleSide.isOn ||toogleSide.isOn & sideDeckGrid.childCount == 10 || c.cardType != "Intangible" & deckGrid.childCount == maxCards & !toogleSide.isOn) 
                                return;
                            GameObject go = Instantiate(cardPrefab);

                            if (toogleSide.isOn)
                                go.transform.SetParent(sideDeckGrid);
                            else if (c.cardType == "Intangible" ) 
                                go.transform.SetParent(deckIntangibleGrid);
                            else 
                                go.transform.SetParent(deckGrid);
                                
                            go.transform.localScale = new Vector3(2.2f,2.2f,1);
                            CardPhysicalInstance cp = go.GetComponentInChildren<CardPhysicalInstance>();
                            cp.LoadCard(c.actualCard);
                            go.SetActive(true);
                            
                            deckCards.Add(cp);
                            go.name = cp.actualCard.name;
                            if (toogleSide.isOn)
                                cp.name =  "*"+cp.actualCard.name;
                            UpdateCounter();
                        }
                        else if (c.transform.parent == deckGrid || c.transform.parent == deckIntangibleGrid || c.transform.parent == sideDeckGrid)
                        {
                            DestroyImmediate(c.gameObject);
                            deckCards.Remove(c);
                            UpdateCounter();
                        }
                    }
                    break;
                }
            }
        }
      

        public int FindGameObjectsWithSameName(string name)
        {
            int c = 0;
            foreach (Transform child in deckGrid)
            {
                if (child.GetComponent<CardPhysicalInstance>().cardArt.sprite.name == name)
                {
                   c++;
                }
            }

            foreach (Transform child in deckIntangibleGrid)
            {
                if (child.GetComponent<CardPhysicalInstance>().cardArt.sprite.name == name)
                {
                   c++;
                }
            }

            foreach (Transform child in sideDeckGrid)
            {
                if (child.GetComponent<CardPhysicalInstance>().cardArt.sprite.name == name)
                {
                   c++;
                }
            }
            return c;
        }
 
        public void SaveProfile()
        {
            deckManager.SaveDeck(inputDeckName.text,deckCards,fileDeck);
            updateDeckList();
        }
        public void updateDeckList()
        {
            deckManager.decks.Sort((Deck t1, Deck t2) => { return t1.identifier.CompareTo(t2.identifier); });
            List<string> optionsList = new List<string>();
            dropDown.ClearOptions();
            foreach (Deck d in deckManager.decks)
            {
                optionsList.Add(d.identifier);
            }
            dropDown.AddOptions(optionsList);
            dropDown.SetValueWithoutNotify(dropDown.options.FindIndex(option => option.text == inputDeckName.text));
            OnDeckChange();
        }

        public void OnDeckChange()
        {
            foreach(CardPhysicalInstance c in deckCards)
            {
                if (c != null)
                {
                    Destroy(c.gameObject);
                }
            }
            deckCards.Clear();
            loadDeck(dropDown.options[dropDown.value].text);
            Invoke("UpdateCounter",1);
        }

        public void OnExpancionChange()
        {
            
            if (dropDownExpansion.options[dropDownExpansion.value].text == "Iniciación")
                LoadCards("Mazo de iniciacion");
            else
                LoadCards(dropDownExpansion.options[dropDownExpansion.value].text );
            allCardsGrid.localPosition = new Vector3(81.8f, -456.9f,0);

        }

        public void OnDeckSetActive()
        {
            deckManager.activeDeck = dropDown.options[dropDown.value].text;
            deckManager.SaveDeck(dropDown.options[dropDown.value].text,deckCards,fileDeck);
        }

        public void OnClearDeck()
        {
            foreach (Transform child in deckGrid) {
                Destroy(child.gameObject);
            }
            foreach (Transform child in deckIntangibleGrid) {
                Destroy(child.gameObject);
            }
            foreach (Transform child in sideDeckGrid) {
                Destroy(child.gameObject);
            }
            deckCards.Clear();
            Invoke("UpdateCounter",1);
        }
       public void SortChildrenByName() {

            List<Transform> children = new List<Transform>();
            for (int i = deckGrid.childCount - 1; i >= 0; i--) {
                Transform child = deckGrid.GetChild(i);
                children.Add(child);
                child.parent = null;
            }
            children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
            foreach (Transform child in children) {
				child.parent = deckGrid;
			}
            
            children = new List<Transform>();
            for (int i = deckIntangibleGrid.childCount - 1; i >= 0; i--) {
                Transform child = deckIntangibleGrid.GetChild(i);
                children.Add(child);
                child.parent = null;
            }
            children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
            foreach (Transform child in children) {
				child.parent = deckIntangibleGrid;
			}
            children = new List<Transform>();
            for (int i = sideDeckGrid.childCount - 1; i >= 0; i--) {
                Transform child = sideDeckGrid.GetChild(i);
                children.Add(child);
                child.parent = null;
            }
            children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
            foreach (Transform child in children) {
				child.parent = sideDeckGrid;
			}

            deckCards.Sort((CardPhysicalInstance t1, CardPhysicalInstance t2) => { return t1.name.CompareTo(t2.name); });

	    }

        public static List<RaycastResult> GetUiObjects()
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

