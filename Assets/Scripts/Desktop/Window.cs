using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static SoundUtils;

public class Window : MonoBehaviour, IDragHandler, IPointerDownHandler{

    [Header("Main")]
    public RectTransform dragRectTransform;
    [SerializeField] Canvas canvas;
    [SerializeField] float offset = 60;

    Vector2 pos;

    void Start(){
        pos = dragRectTransform.anchoredPosition;
        GotCanvas();
    }

    bool GotCanvas(){
        if(canvas != null)
            return true;
        Transform search = this.transform;
        while(search.parent != null && canvas == null){
            search = search.parent;
            if(search.TryGetComponent<Canvas>(out Canvas canny))
                canvas = canny;
        }
        return canvas != null;
    }

    public void OnDrag(PointerEventData eventData){
        if(!GotCanvas())
            return;
        dragRectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData){
        dragRectTransform.SetAsLastSibling();
    }

    public void RandomisePosition(){
        dragRectTransform.anchoredPosition = pos + new Vector2(Random.Range(-offset, offset), Random.Range(-offset, offset));
    }
}