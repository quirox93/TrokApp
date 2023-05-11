using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WARBEN
{
    public class AdjustWidth : MonoBehaviour
    {
        GridLayoutGroup grid;
        public int maxCards = 6;
        public int width = 500;
        void Start()
        {
            grid = this.GetComponent<GridLayoutGroup>();
        }

        void Update()
        {
            
        }

        void OnTransformChildrenChanged() {
            if (this.transform.childCount > maxCards)
            {
                grid.cellSize = new Vector2(width/this.transform.childCount,100);
            }
            else
            {
                grid.spacing = Vector2.zero;
            }
        }
        
    }
}
