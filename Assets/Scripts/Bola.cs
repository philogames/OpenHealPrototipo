using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Classe para gerenciar as bolas instanciadas no jogo.
/// </summary>
public class Bola : MonoBehaviour
{

    //dados da bola
    Bola_Info info;

    //spriterenderer da bola
    SpriteRenderer rend;

    //collider da bola
    Collider2D colisor;

    //tamanho do sprite pela metade. deve ser armazenado para aplicar um offset ao efeito de rebater na borda da tela.
    private Vector2 halfSpriteSize;

    // limites da tela, para calcular o efeito de rebater.
    private Vector2 screenBounds;

    //amazena o tempo que a bola foi iniciada. Essa variável será comparada com o tempo atual para definir a propriedade "hit_time"
    float timeOnEnable;

    //vetor da direcao de movimento da bola
    Vector3 vectorDirection;

    //flag para sinalizar que o jogador conseguir acertar essa bola
    bool playerAcertou = false;

    //valor de score adicionado ao acertar a bola
    public float baseScoreToAdd = 10;


    private Material mat;
    private int fadeVibrate, fadeDissolve;

    [Header("Visual")]
    public float rotatioOffset = 0;//valor para alterar a rotacao da bola, note que essa propriedade é puramente visual, não tendo qualquer relação com a direção da bola
    // Variáveis para controle da suavização
    public float rotationSmoothTime = 0.1f;
    private Quaternion targetRotation;
    private Quaternion currentVelocity;
    public MMF_Player OnPop, OnLoseBall, OnBallBorn;

 
    /// <summary>
    /// Preparar a bola para ser iniciada.
    /// </summary>
    /// <param name="bola">Dados da bola vindos do servidor</param>
    /// <param name="cam">Referencia da camera principal</param>
    public void Inicializar(Bola_Info bola, Camera cam)
    {
      
        mat = GetComponent<SpriteRenderer>().material;
        fadeVibrate = Shader.PropertyToID("_VibrateFade");
        fadeDissolve = Shader.PropertyToID("_FullAlphaDissolveFade");

        //desabilita o sprite
        rend = gameObject.GetComponent<SpriteRenderer>();
        rend.enabled = false;

        //desabilita o collider (para evitar que o jogador clique sem querer)
        colisor = gameObject.GetComponent<CircleCollider2D>();
        colisor.enabled = false;

        //seta a bola com as informações vindas do servidor
        info = new Bola_Info(bola);

        //seta a cor (color)
        rend.color = info.color.GetColor();

        //seta o tamanho (size)
        transform.localScale = Vector3.one * (float) info.size;

        //armazena o tamanho do sprite
       // halfSpriteSize = rend.sprite.bounds.size / 2;


        //armazena a resolucao da tela
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        //aplica o offset com o tamanho do sprite
      //  screenBounds -= halfSpriteSize;

        //calcula o vetor de direção de acordo com o angulo de movimento da bola
        vectorDirection = AngleToDirection((float)info.direction);

        //transforma a posicao para coordenadas do mundo
        Vector2 screenPos = Camera.main.ViewportToWorldPoint(info.launch_coord.GetLaunchCoord());
        Debug.Log("Viewport Coord: " + info.launch_coord.GetLaunchCoord());
        Debug.Log("World Coord: " + screenPos);

     
        //seta a posição inicial (launch_coord)
        //o valor da coordenada Z é 0.5 para que a posicao das bolas estejam numa zona favorável para colisão com raycasts
        transform.position = new Vector3(screenPos.x, screenPos.y, 0.5f);

        //seta a posicao de lançamento com a coordenada do mund, para evitar conflitos de coordenadas
        //dati vai analisar esses dados para verificar se de fato as coordenadas recebidas da api sao equivalentes as coordenadas do mundo
        info.launch_coord = new LaunchCoord((Vector2)Camera.main.WorldToScreenPoint(transform.position));
        Debug.Log("Screen Coord: " + info.launch_coord.GetLaunchCoord());
        //inicia a coroutine para aplicar os parametros na bola
        StartCoroutine(IniciarMovimento());
       
    }

    /// <summary>
    /// Inicia o movimento da bola.
    /// </summary>
    IEnumerator IniciarMovimento()
    {
        float t = Time.realtimeSinceStartup;
        //aguardar X segundos, respeitando o launch_time da bola
        yield return new WaitForSecondsRealtime((float)info.launch_time);
        float t2 = Time.realtimeSinceStartup - t;
       // Debug.Log("Tempo de espera game: " + t2 + "____ Tempo de espera API: " + info.launch_time);

        //sebrescreve o launchtime com o tempo que a bola foi instanciada para comparação futura
        info.launch_time = t2;

        //ativa o sprite
        rend.enabled = true;

        //ativa o collider
        colisor.enabled = true;

        //armazenar o tempo do jogo que a bola foi instanciada, para mais tarde calcular o tempo em que o jogador acertou
        timeOnEnable = Time.time;

        //Iniciar, de fato, a movimentação
        StartCoroutine(Movimentar());

        //aumentar tamanho da bolha
        this.gameObject.transform.localScale = Vector3.zero;
        this.gameObject.transform.LeanScale(new Vector3(1, 1, 1) * (float)info.size, (float)(info.mature_time - info.launch_time)).setOnComplete(PrepararParaExplodir);
       
        OnBallBorn?.PlayFeedbacks();
    }

    void PrepararParaExplodir()
    {
        LeanTween.value(mat.GetFloat(fadeVibrate), 1, (float)(info.destroy_time - info.mature_time)).setOnUpdate(FadeVibrate).setOnComplete(IniciarExplosaoBola);
       // this.gameObject.LeanScale(this.gameObject.transform.localScale/1.05f, info.destroy_time - info.mature_time).setOnComplete(ExplodirBola);
    }

    void FadeVibrate(float v)
    {
        //mat.SetFloat(fadeVibrate, v);
    }

    void IniciarExplosaoBola()
    {
        OnLoseBall?.PlayFeedbacks();
        transform.LeanScale(Vector3.zero, 0.1f).setOnComplete(DestruirBola);
    }

    void IniciarExplosaoBolaAcertou()
    {
        LeanTween.value(mat.GetFloat(fadeDissolve), 0, .2f).setOnUpdate(FadeDissolve).setOnComplete(DestruirBola);
       

    }
    void FadeDissolve(float v)
    {
        mat.SetFloat(fadeDissolve, v);
    }

    void DestruirBola()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// Função para converter ângulo em vetor
    /// </summary>
    /// <param name="angleInDegrees"></param>
    /// <returns></returns>
    public Vector3 AngleToDirection(float angleInDegrees)
    {
        // Convertendo ângulo de graus para radianos
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Calculando x e y usando seno e cosseno
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);

        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Função recursiva para movimentar a bola para baixo
    /// </summary>
    /// <returns></returns>
    IEnumerator Movimentar()
    {
      

        //aplica a movimentação de acordo com o vetor direção
        this.gameObject.transform.position += vectorDirection.normalized * (float)info.speed * Time.deltaTime;

        // Calcula o ângulo de rotação com base na direção do vetor de movimento
        float angle = Mathf.Atan2(vectorDirection.y, vectorDirection.x) * Mathf.Rad2Deg;

        // Define a rotação alvo
        targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + rotatioOffset));

        // Suaviza a rotação
        this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, targetRotation, rotationSmoothTime);


        //verifica se precisa rebater alguma bola que está saindo da tela
        RebaterNaTela();

        //espera até o fim do frame
        yield return new WaitForEndOfFrame();

        //recursão
        StartCoroutine(Movimentar());
    }


    public void UpdateScreenBounds()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

     float offsetTelaBaixo = 2f;
    void RebaterNaTela()
    {
        if (info.speed != 0)
        {
            // Obtém o raio da bola
            float bolaRaio = transform.localScale.y / 2;

            // Verifica e ajusta a direção horizontal
            vectorDirection.x = transform.position.x - bolaRaio < -screenBounds.x ? Mathf.Abs(vectorDirection.x) :
                                transform.position.x + bolaRaio > screenBounds.x ? -Mathf.Abs(vectorDirection.x) : vectorDirection.x;

            // Verifica e ajusta a direção vertical com offset
            vectorDirection.y = transform.position.y - bolaRaio < -screenBounds.y + offsetTelaBaixo ? Mathf.Abs(vectorDirection.y) :
                                transform.position.y + bolaRaio > screenBounds.y ? -Mathf.Abs(vectorDirection.y) : vectorDirection.y;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Bola"))
        {
            Bola outraBola = collision.gameObject.GetComponent<Bola>();

            // Calcula a direção do vetor do centro desta bola ao centro da outra bola
            Vector3 direcaoAoCentro = (collision.transform.position - transform.position).normalized;

            // Calcula a projeção do vetor de direção desta bola sobre a direção ao centro
            Vector3 minhaProjecao = Vector2.Dot(vectorDirection, direcaoAoCentro) * direcaoAoCentro;

            // Calcula a projeção do vetor de direção da outra bola sobre a direção ao centro
            Vector3 outraProjecao = Vector2.Dot(outraBola.vectorDirection, direcaoAoCentro) * direcaoAoCentro;

            // Substitui a componente da velocidade ao longo da direção ao centro pela da outra bola
            // Isso simula uma troca de momentum na direção da linha que conecta os centros das bolas
            vectorDirection += outraProjecao - minhaProjecao;
           // outraBola.vectorDirection += minhaProjecao - outraProjecao;
        }
    }





    /// <summary>
    /// Armazena os dados adicionais que serão enviados ao servidor quando o jogador acertar a bola.
    /// </summary>
    public bool PlayerAcertou()
    {
        if (!playerAcertou)
        {
            OnPop?.PlayFeedbacks();

            //armazena coordenada atual da bola
            info.hit_coord = new HitCoord(Camera.main.WorldToScreenPoint(transform.position));

            //calcula o tempo de vida da bola
            info.hit_time = Time.time - timeOnEnable;

            //flag para sinalizar que o jogador acertou a bola
            playerAcertou = true;

            //desativa a bola
            IniciarExplosaoBolaAcertou();
            return true;
        }
        return false;
    }
 

    public Bola_Info GetInfo()
    {
        return info;
    }
    void OnDrawGizmos()
    {

        if (GameDataBubbles.Instance.DEBUG_MODE)
        {
            // Obtém o raio da bola
            float bolaRaio = transform.localScale.y / 2;

            // Calcula os limites da área de detecção com offset
            Vector3 min = new Vector3(-screenBounds.x + bolaRaio, -screenBounds.y + bolaRaio + offsetTelaBaixo, transform.position.z);
            Vector3 max = new Vector3(screenBounds.x - bolaRaio, screenBounds.y - bolaRaio, transform.position.z);

            // Desenha a área de detecção
            Gizmos.color = Color.red;
            Gizmos.DrawLine(min, new Vector3(min.x, max.y, min.z));
            Gizmos.DrawLine(min, new Vector3(max.x, min.y, min.z));
            Gizmos.DrawLine(max, new Vector3(min.x, max.y, max.z));
            Gizmos.DrawLine(max, new Vector3(max.x, min.y, max.z));
        }
    }

    /*
    //O valor de lambda influencia diretamente como a pontuação é sensível à diferença de tempo.
    //Um valor pequeno torna o jogo mais permissivo, pois a pontuação diminui lentamente com o aumento da diferença de tempo.
    //Um valor maior torna o jogo mais desafiador, pois a pontuação cai mais rapidamente. 
    public float CalculateScore(float lambda)
    {
        // Calcula a diferença de tempo entre o hit e o momento ideal após o launch
        float idealHitTime = info.mature_time - info.launch_time;
        float timeDifference = Mathf.Abs(info.hit_time - idealHitTime);

        // Calcula o score baseado na diferença de tempo
        float score =  baseScoreToAdd * Mathf.Exp(-lambda * timeDifference);

        return score;
    }
    */


}
