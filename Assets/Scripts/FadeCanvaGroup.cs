using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvaGroup : MonoBehaviour
{
    CanvasGroup group;
    public float maxAlpha = 1;
    private void OnEnable()
    {
        group = GetComponent<CanvasGroup>();
    }

    public void FadeIn(float tempo)
    {
        group.LeanAlpha(maxAlpha, tempo);
        group.blocksRaycasts = true;
    }

    public void FadeOut(float tempo)
    {
        group.LeanAlpha(0, tempo);
        group.blocksRaycasts = false;
    }
}
