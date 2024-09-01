using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    Camera mainCam;
    GerentePartidaBubbles gerente;
    private void OnEnable()
    {
        gerente = GameObject.FindObjectOfType<GerentePartidaBubbles>();
        mainCam = Camera.main;
    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
            CheckClick();
    }


    void CheckClick()
    {
        RaycastHit2D hit;
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (hit = Physics2D.Raycast(mousePos, mainCam.transform.forward))
        {
            if(hit.transform.CompareTag("Bola"))
            {
                Bola bolaClicada = hit.transform.GetComponent<Bola>();
                if (bolaClicada.PlayerAcertou())
                {

                    gerente.AddScore();
                }
            }

            
        }


    }
}
