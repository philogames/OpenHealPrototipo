using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeColorTextOnHover : MonoBehaviour
{
    public Text text;
    public Color[] colors;
   public void SetColor(int colorIndex)
   {
       text.color = colors[colorIndex];
   }
}
