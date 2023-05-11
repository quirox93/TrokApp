using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace  WARBEN
{
    
    public class RecursosManager : MonoBehaviour
    {
        public Transform buttonsA;
        public Transform buttonsB;
        public Transform buttonsC;

        public PhotonView pv;

        public static RecursosManager singleton;

        private void Awake()
        {
            singleton = this;
            pv = PhotonView.Get(this);
        }

        public void Start()
        {
            foreach (Transform child in buttonsA)
                child.GetComponentInChildren<Text>().text = "0";

            foreach (Transform child in buttonsB)
                child.GetComponentInChildren<Text>().text = "0";

            buttonsA.GetChild(0).GetComponentInChildren<Text>().text = "30";
            buttonsB.GetChild(0).GetComponentInChildren<Text>().text = "30";
        }

        public void OnButtonClickAdd(Text numberText)
        {
            int number;
            int.TryParse(numberText.text, out number);
            if (number < 30)
                number++;
            numberText.text = number.ToString();
            int id = numberText.transform.parent.GetSiblingIndex();
            pv.RPC("RPC_UpdateButton", RpcTarget.Others,numberText.text, id);
        }
        public void OnButtonClickSubstract(Text numberText)
        {
            int number;
            int.TryParse(numberText.text, out number);
            if (number > 0)
                number--;
            numberText.text = number.ToString();
            int id = numberText.transform.parent.GetSiblingIndex();
            pv.RPC("RPC_UpdateButton", RpcTarget.Others,numberText.text, id);
        }


        [PunRPC]
        void RPC_UpdateButton(string numberText, int id)
        {
            buttonsB.GetChild(id).GetComponentInChildren<Text>().text = numberText;
        }
        [PunRPC]
        void RPC_UpdateButtonA(string numberText, int id)
        {
            buttonsA.GetChild(id).GetComponentInChildren<Text>().text = numberText;
        }

    }


}
