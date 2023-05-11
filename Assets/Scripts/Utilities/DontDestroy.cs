using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

   public bool loadSave = false;

   void Start()
   {
       DontDestroyOnLoad (this);
   }
}
