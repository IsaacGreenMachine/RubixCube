using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenButton : MonoBehaviour, IPointerClickHandler
{
    public int layer;
    public string axis;
    public int direction;
    public GameObject manager;
    public UnityEngine.UI.Button button;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            manager.GetComponent<Manager>().Rotate(axis, layer, 1);
        if (eventData.button == PointerEventData.InputButton.Right)
            manager.GetComponent<Manager>().Rotate(axis, layer, -1);
    }
}
