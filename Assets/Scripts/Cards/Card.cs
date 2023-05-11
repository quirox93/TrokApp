using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WARBEN
{
    public abstract class Card : ScriptableObject
    {
        public RuntimeValues  runtimeValues;
        public Sprite artSprite;
        public string nombre;
        public string cardType;
        public string cardSubType;
        public string cardSubType2;
        public string color;
        public string especie;
        public int limite = 3;
        public string[] expancion;

        public virtual void initCard()
        {
            runtimeValues = new RuntimeValues();
        }
        
        public abstract bool canBajar();
    }

    public class RuntimeValues
    {
        public int instID;

    }
}

