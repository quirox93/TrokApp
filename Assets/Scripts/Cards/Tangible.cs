using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WARBEN
{
    [CreateAssetMenu(menuName="Cards/Tangible")]
    public class Tangible : Card
    {
        public override void initCard()
        {
            
        }
        public override bool canBajar()
        {
            //Comprueba si puedes BAJAR una carta
            return true;
        }
    }
}
