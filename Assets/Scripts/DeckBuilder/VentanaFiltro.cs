using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;

namespace WARBEN
{
    public class VentanaFiltro : MonoBehaviour
    {
        public Dropdown expanciones;
        public InputField nombre;
        public Dropdown carta;
        public Dropdown tipo;
        public Dropdown tipoInt;
        public Dropdown color;
        public Dropdown entidad;
        public Dropdown entidadInt;
        public Dropdown especie;
        public Dropdown especieInt;
        public Dropdown estructura;
        public Dropdown herramienta;
        public Transform allCardsGrid;
        public GameObject cardPrefab;

        
        public void aplicarFiltros()
        {
            foreach (Transform child in allCardsGrid.transform) {
                GameObject.Destroy(child.gameObject);
            }
            Card[] cards = GameManager.resourcesManager.all_cards;
            string cleanName;
            foreach (Card c in cards)
            {
                cleanName = Clean(c.nombre);
                print(cleanName);
                if ((nombre.text == "" | cleanName.Contains(Clean(nombre.text))) != true)
                    continue;
                if ((carta.options[carta.value].text == "Cualquiera" | c.cardType == carta.options[carta.value].text) != true)
                    continue;
                if ((!tipo.IsActive() | tipo.options[tipo.value].text == "Cualquiera" | c.cardSubType == tipo.options[tipo.value].text) != true)
                    continue;
                if ((!tipoInt.IsActive() | tipoInt.options[tipoInt.value].text == "Cualquiera" | c.cardSubType == tipoInt.options[tipoInt.value].text) != true)
                    continue;
                if ((!especie.IsActive() | especie.options[especie.value].text == "Cualquiera" | c.especie == especie.options[especie.value].text) != true)
                    continue;
                if ((!especieInt.IsActive() | especieInt.options[especieInt.value].text == "Cualquiera" | c.especie == especieInt.options[especieInt.value].text) != true)
                    continue;
                if ((!color.IsActive() | color.options[color.value].text == "Cualquiera" | c.color == color.options[color.value].text) != true)
                    continue;
                if ((!entidad.IsActive() | entidad.options[entidad.value].text == "Cualquiera" | c.cardSubType2 == entidad.options[entidad.value].text) != true)
                {
                    if ( (entidad.options[entidad.value].text == "Sin habilidad" & c.cardSubType2 == "") != true)
                        continue;
                }
                if ((!entidadInt.IsActive() | entidadInt.options[entidadInt.value].text == "Cualquiera" | c.cardSubType2 == entidadInt.options[entidadInt.value].text) != true)
                    continue;
                if ((!estructura.IsActive() | estructura.options[estructura.value].text == "Cualquiera" | c.cardSubType2 == estructura.options[estructura.value].text) != true)
                {
                    if ( (estructura.options[estructura.value].text == "Sin habilidad" & c.cardSubType2 == "") != true)
                        continue;
                }
                if ((!herramienta.IsActive() | herramienta.options[herramienta.value].text == "Cualquiera" | c.cardSubType2 == herramienta.options[herramienta.value].text) != true)
                    continue;
                    
                
                GameObject go = Instantiate(cardPrefab);
                go.transform.SetParent(allCardsGrid);
                go.transform.localScale = new Vector3(1.7f,1.7f,1);
                CardPhysicalInstance cp = go.GetComponentInChildren<CardPhysicalInstance>();

                cp.LoadCard(c);
                go.SetActive(true);
                
            
            }
            
            allCardsGrid.localPosition = new Vector3(81.8f, -456.9f,0);
            carta.SetValueWithoutNotify(0);
            tipo.SetValueWithoutNotify(0);
            tipoInt.SetValueWithoutNotify(0);
            color.SetValueWithoutNotify(0);
            entidad.SetValueWithoutNotify(0);
            entidadInt.SetValueWithoutNotify(0);
            especie.SetValueWithoutNotify(0);
            especieInt.SetValueWithoutNotify(0);
            estructura.SetValueWithoutNotify(0);
            herramienta.SetValueWithoutNotify(0);
            expanciones.SetValueWithoutNotify(0);
            LimpiarFiltros();
            Invoke("FiltroOcultar",0.5f);
        }

        public string Clean(string text)
        {
            text = text.ToLower();
            text = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
            var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(text[i]);
            if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
            sb.Append(text[i]);
            }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        public void LimpiarFiltros()
        {
            nombre.text = "";
            tipo.gameObject.SetActive(false);
            tipoInt.gameObject.SetActive(false);
            color.gameObject.SetActive(false);
            entidad.gameObject.SetActive(false);
            entidadInt.gameObject.SetActive(false);
            especie.gameObject.SetActive(false);
            especieInt.gameObject.SetActive(false);
            estructura.gameObject.SetActive(false);
            herramienta.gameObject.SetActive(false);
        }

        public void OnButtonFiltro()
        {
            if (!this.gameObject.active)
                this.gameObject.SetActive(true);
            else
                Invoke("FiltroOcultar",0.5f);
            
        }
        public void FiltroOcultar()
        {
            this.gameObject.SetActive(false);
        }

        public void OnCartaChange()
        {
            LimpiarFiltros();
            if (carta.options[carta.value].text == "Tangible")
            {
                color.gameObject.SetActive(true);
                tipo.gameObject.SetActive(true);
                
            }
            else if (carta.options[carta.value].text == "Intangible")
            {
                tipoInt.gameObject.SetActive(true);
                
            }
        }

        public void OnTangibleChange()
        {
            LimpiarFiltros();
            color.gameObject.SetActive(true);
            tipo.gameObject.SetActive(true);
            if (tipo.options[tipo.value].text == "Entidad")
            {
                entidad.gameObject.SetActive(true);
                especie.gameObject.SetActive(true);
            }
            else if (tipo.options[tipo.value].text == "Estructura")
            {
                estructura.gameObject.SetActive(true);
            }
            else if (tipo.options[tipo.value].text == "Herramienta")
            {
                herramienta.gameObject.SetActive(true);
            }
        }
        public void OnIntangibleChange()
        {
            LimpiarFiltros();
            tipoInt.gameObject.SetActive(true);
            if (tipoInt.options[tipoInt.value].text == "Entidad")
            {
                entidadInt.gameObject.SetActive(true);
                especieInt.gameObject.SetActive(true);
            }
        }


        public void loadAllCards()
        {
            foreach (Transform child in allCardsGrid.transform) {
                GameObject.Destroy(child.gameObject);
            }
            Card[] cards = GameManager.resourcesManager.all_cards;
            foreach (Card c in cards)
            {
                GameObject go = Instantiate(cardPrefab);
                go.transform.SetParent(allCardsGrid);
                go.transform.localScale = new Vector3(1.7f,1.7f,1);
                CardPhysicalInstance cp = go.GetComponentInChildren<CardPhysicalInstance>();

                cp.LoadCard(c);
                go.SetActive(true);
            }
            allCardsGrid.localPosition = new Vector3(81.8f, -456.9f,0);
        }

    }
}

