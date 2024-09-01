using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

/// <summary>
/// Classe para armazenar os dados de cada bola: Dados iniciais, de instanciamento, e também Dados de retorno, para alimentar o servidor
/// </summary>
[System.Serializable]
public class Bola_Info
{
    #region Dados Iniciais
    
    [SerializeField]
    public double launch_time;
    [SerializeField]
    public LaunchCoord launch_coord;
    [SerializeField]
    public double speed;
    [SerializeField]
    public Color_heal color;
    [SerializeField]
    public double size;
    [SerializeField]
    public double direction;
    [SerializeField]
    public double mature_time;
    [SerializeField]
    public double destroy_time;
    [SerializeField]
    public List<int> hit_landmarks;
    #endregion


    #region Dados do jogo
    //hit info
    [SerializeField]
    public double hit_time;
    [SerializeField]
    public HitCoord hit_coord;
    #endregion

   
    /// <summary>
    /// Construtor 1. A partir de uma bola_info
    /// </summary>
    /// <param name="bola"></param>
    public Bola_Info(Bola_Info bola)
    {
   
        this.launch_time = bola.launch_time;
        this.launch_coord = bola.launch_coord;
        this.speed = bola.speed;
        this.color = bola.color;
        this.color.a = 1;
        this.size = bola.size;
        this.direction = bola.direction;
        this.mature_time = bola.mature_time;
        this.destroy_time = bola.destroy_time;
    }

    /// <summary>
    /// Construtor 2. Setando cada parametro individualmente.
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="launch_time"></param>
    /// <param name="launch_coord"></param>
    /// <param name="speed"></param>
    /// <param name="color"></param>
    /// <param name="size"></param>
    public Bola_Info(float launch_time, Vector2 launch_coord, float speed, Color color, float size, float direction, float mature, float destroy)
    {
      //  this.ID = ID;
        this.launch_time = launch_time;
        this.launch_coord = new LaunchCoord(launch_coord);
        this.speed = speed;
        this.color = new Color_heal(color);
        this.color.a = 1;
        this.size = size;
        this.direction = direction;
        this.mature_time = mature;
        this.destroy_time = destroy;
    }


}
