using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatPicker : MonoBehaviour
{
    public void OnMouseOver()
    {
        Debug.Log(this.gameObject.name);
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    public void OnMouseExit()
    {
        transform.localScale = Vector3.one;
    }

    public void OnMouseDown()
    {
        int hatIndex = Hats.hats.IndexOf(this.gameObject);
        PlayerStats.instance.hatIndex = hatIndex;
    }
}
