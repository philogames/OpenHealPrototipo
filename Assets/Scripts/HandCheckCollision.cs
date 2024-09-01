using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCheckCollision : MonoBehaviour
{
    GerentePartidaBubbles gerente;

    private void Start()
    {
        gerente = GameObject.FindObjectOfType<GerentePartidaBubbles>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bola"))
        {
            Bola bolaClicadaMao = collision.transform.GetComponent<Bola>();
            if(bolaClicadaMao.PlayerAcertou())
                gerente.AddScore();
        }
    }
}
