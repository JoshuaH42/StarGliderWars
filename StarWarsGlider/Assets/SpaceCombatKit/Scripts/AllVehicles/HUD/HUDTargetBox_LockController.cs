using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Manages a single lock on a target box on the HUD.
    /// </summary>
    public class HUDTargetBox_LockController : MonoBehaviour
    {
        [SerializeField]
        protected Image lockImage;
        public RectTransform RectTransform
        {
            get { return lockImage.rectTransform; }
        }


        // Activate the lock box
        public virtual void Activate()
        {
            lockImage.gameObject.SetActive(true);
        }

        // Deactivate the lock box
        public virtual void Deactivate()
        {
            lockImage.gameObject.SetActive(false);
        }
    }
}