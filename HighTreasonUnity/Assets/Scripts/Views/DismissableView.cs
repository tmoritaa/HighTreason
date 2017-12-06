using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DismissableView : MonoBehaviour
{
    public bool Dismissable
    {
        private get; set;
    }

    public void HideTopViewIfDismissable()
    {
        if (Dismissable)
        {
            ViewManager.Instance.HideTopView();
        }
    }
}
