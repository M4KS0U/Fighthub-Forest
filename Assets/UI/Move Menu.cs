using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMenu : MonoBehaviour
{
    private Vector2 targetPosition;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void MoveLeft()
    {
        TranslateMenu(new Vector2(-800, 0));
    }

    public void MoveRight()
    {
        TranslateMenu(new Vector2(800, 0));
    }
    
    public void TranslateMenu(Vector2 addPosition)
    {
        targetPosition = rectTransform.anchoredPosition + addPosition;
        StartCoroutine(EaseInOutMove());
    }

    private IEnumerator EaseInOutMove()
    {
        float time = 0;
        Vector2 startPosition = rectTransform.anchoredPosition;
        while (time < 1)
        {
            time += Time.deltaTime * 2;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, time));
            yield return null;
        }
    }
}
