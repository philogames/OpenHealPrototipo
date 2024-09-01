using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class bubble_match
{

    public int match_id;
    public string user_id;
    public string user_nickname;
    public List<Bola_Info> balls;
    public List<int> screen_resolution;
    public double hit_percentage;

    public void SetScreenResolution(int w, int h)
    {
        screen_resolution = new List<int>();
        screen_resolution.Add(w);
        screen_resolution.Add(h);
    }

}
[System.Serializable]
public class bubble_match_data
{

    public bubble_match data;
    public bool isSuccess;

}

/*
[System.Serializable]
public class Ball
{
    public LaunchCoord launch_coord;
    public double launch_time;
    public double speed;
    public double direction;
    public Color_heal color;
    public double size;
    public double mature_time;
    public double destroy_time;
    public List<int> hit_landmarks;
}
*/

[System.Serializable]
public class LaunchCoord
{
    public double x;
    public double y;

    public LaunchCoord(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public LaunchCoord(Vector2 v)
    {
        this.x = v.x;
        this.y = v.y;
    
    }

    public Vector2 GetLaunchCoord()
    {
        return new Vector2((float)x, (float)y);
    }
}

[System.Serializable]
public class HitCoord
{
    public double x;
    public double y;

    public HitCoord(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
    public HitCoord(Vector2 v)
    {
        this.x = v.x;
        this.y = v.y;
    }
}



[System.Serializable]
public class Color_heal
{
    public double r;
    public double g;
    public double b;
    public double a;

    public Color_heal(double r, double g, double b, double a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Color_heal(Color c)
    {
        this.r = c.r;
        this.g = c.g;
        this.b = c.b;
        this.a = c.a;
    }

    public Color GetColor()
    {
        return new Color((float)r, (float)g, (float)b, (float)a);
    }
}