using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipePanel : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector2 panelPos;
    #region 判断滑动方向
    private bool isLeft = false;
    private bool isRight = false;
    //滑动结束
    private bool isOver = false;
    private bool noOver = false;
    #endregion
    private Canvas canvas;
    #region 记录鼠标的位置
    private Vector2 startVec;
    private Vector3 currentVec;
    #endregion

    private float swipeDistance = 0;

    private float childWidth;
    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        childWidth = this.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
    }

    private void Start()
    {
        swipeDistance = Screen.width * 0.4f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startVec = eventData.position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(), eventData.position,
            eventData.enterEventCamera, out panelPos);
        currentVec = this.GetComponent<RectTransform>().anchoredPosition;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        float offset = Vector2.Distance(eventData.position, startVec);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.enterEventCamera,
            out pos);
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2((pos - panelPos).x, 0);
        //判断当前手势的滑动方向
        float direction = eventData.position.x - startVec.x;
        if (direction < 0)
        {
            isLeft = true;
            isRight = false;
            if (offset > swipeDistance)
            {
                isOver = true;
                Debug.Log("哥们，我滑动的长度过了");
            }
            else
            {
                noOver = true;
            }
        }
        if (direction > 0)
        {
            isRight = true;
            isLeft = false;
            if (offset > swipeDistance)
            {
                isOver = true;
            }
            else
            {
                noOver = true;
            }
        }
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLeft && isOver)
        {
            this.GetComponent<RectTransform>().DOAnchorPosX(currentVec.x - childWidth, 0.8f).OnComplete(() =>
            {
                this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            });
            isLeft = false;
            isOver = false;
            isRight = false;
            noOver = false;
        }
        if (isRight && isOver)
        {
            this.GetComponent<RectTransform>().DOAnchorPosX(currentVec.x + childWidth, 0.8f).OnComplete(() =>
             {
                 this.GetComponent<CanvasGroup>().blocksRaycasts = true;
             });
            isLeft = false;
            isOver = false;
            isRight = false;
            noOver = false;
        }
        //  isLeft || isRight &&
        if (isLeft || isRight && noOver == true)
        {
            this.GetComponent<RectTransform>().DOAnchorPosX(currentVec.x, 0.3f).OnComplete(() =>
            {
                this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            });
            isLeft = false;
            isOver = false;
            isRight = false;
            noOver = false;
        }
    }

    private void LateUpdate()
    {
        Debug.Log((this.GetComponent<RectTransform>().childCount * childWidth));
        Debug.Log(this.GetComponent<RectTransform>().anchoredPosition);
        Vector3 pos = this.GetComponent<RectTransform>().anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, -((this.GetComponent<RectTransform>().childCount - 1) * childWidth), 0);
        this.GetComponent<RectTransform>().anchoredPosition = pos;
    }
}
