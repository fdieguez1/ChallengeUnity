using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EnemyDestroyed();
public class Invader : MonoBehaviour
{
    public static List<Invader> ActiveInvaders;
    public InvaderData invaderData;
    [Header("Settings")]
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public int x;
    public int y;
    public int bulletSpawnRate;
    bool firstSpriteActive;
    Material disolveMaterial;
    float integrity;
    int lifes;
    public int Lifes
    {
        get
        {
            return lifes;
        }
        set
        {
            lifes = value;
            ChangeIntegrity(lifes);
        }
    }
    [Header("Invader bullets")]
    [SerializeField]
    Vector3 bulletOrigin;
    public GameObject BulletPrefab;
    public bool Shooting;

    /// <summary>
    /// Propiedad que busca el elemento a la izquierda en la matriz donde se encuentra la instancia
    /// </summary>
    public Invader Left
    {
        get
        {
            if (x > 0)
            {
                return MyGrid.Instance.GetGridObject(x - 1, y);
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// Propiedad que busca el elemento por encima en la matriz donde se encuentra la instancia
    /// </summary>
    public Invader Top
    {
        get
        {
            if (y > 0)
            {
                return MyGrid.Instance.GetGridObject(x, y - 1);
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// Propiedad que busca el elemento a la derecha en la matriz donde se encuentra la instancia
    /// </summary>
    public Invader Right
    {
        get
        {
            if (x < MyGrid.Instance.Width - 1)
            {
                return MyGrid.Instance.GetGridObject(x + 1, y);
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// Propiedad que busca el elemento inferior en la matriz donde se encuentra la instancia
    /// </summary>
    public Invader Bottom
    {
        get
        {
            if (y < MyGrid.Instance.Height - 1)
            {
                return MyGrid.Instance.GetGridObject(x, y + 1);
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// Propiedad que devuelve un arreglo de los elementos que rodean en X e Y en la matriz a esta instancia.
    /// </summary>
    public Invader[] Neighbours
    {
        get
        {
            return new Invader[] {
                Left,
                Top,
                Right,
                Bottom
            };
        }
    }
    /// <summary>
    /// Cambio el valor del material para utilizar mi shader personalizado, agregandole un ruido al alfa de la textura y asi crear un efecto "roto"
    /// </summary>
    /// <param name="value"></param>
    public void ChangeIntegrity(int value)
    {
        //Solo cambio los valores si las vidas son == 1 y el invader tenia mas de 1 vida
        if (value == 1 && invaderData.Lifes > 1)
        {
            disolveMaterial.SetVector("_Integrity", new Vector2(0.5f, 0.5f));
        }
        else
        {
            //Para el sprite de muerte, dejo los valores por defecto
            disolveMaterial.SetVector("_Integrity", new Vector2(1f, 1f));
        }
    }

    /// <summary>
    /// Obtiene de forma recursiva los invasores conectados entre si por su posicion en la matriz
    /// </summary>
    /// <param name="exclude">Listado de invasores ya verificados que seran excluidos de las proximas iteraciones del metodo para no duplicarlos infinitamente</param>
    /// <returns>List<InvaderBehaviour> Listado de los invasores interconectados por poseer la misma instancia de ScriptTableObject "Invader" en su atributo invaderData</returns>
    public List<Invader> GetConnectedInvaders(List<Invader> exclude = null)
    {
        var result = new List<Invader> { this, };
        if (exclude == null)
        {
            exclude = new List<Invader> { this, };
        }
        else
        {
            exclude.Add(this);
        }
        foreach (var neightbour in Neighbours)
        {
            if (neightbour == null || exclude.Contains(neightbour) || neightbour.invaderData != this.invaderData)
            {
                continue;
            }
            result.AddRange(neightbour.GetConnectedInvaders(exclude));
        }
        return result;
    }
    public void Start()
    {
        //Se cargan los datos una vez instanciado el objeto, cargando los datos de forma aleatoria desde el arreglo de ScriptableObjects de la clase GameController.
        invaderData = GameController.Instance.InvaderScriptableObjects[UnityEngine.Random.Range(0, GameController.Instance.InvaderScriptableObjects.Length)];
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = invaderData.Color;
        spriteRenderer.sprite = invaderData.spriteA;
        disolveMaterial = spriteRenderer.material;
        disolveMaterial.SetVector("_Integrity", new Vector2(1f, 1f));
        firstSpriteActive = true;
        Lifes = invaderData.Lifes;
        StartCoroutine(InvaderUpdate());
    }

    /// <summary>
    /// Disparo del invader, mientras sus vidas sean las necesarias realiza un disparo segun un intervalo aleatorio dado para esta instancia
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    public IEnumerator InvaderShoot(int interval)
    {
        while (gameObject.activeSelf)
        {
            Shooting = true;
            yield return new WaitForSeconds(interval);
            GameController.Instance.GridContainerAudioSource.PlayOneShot(GameController.Instance.InvaderFireSound);
            bulletOrigin = spriteRenderer.bounds.center + new Vector3(0, -transform.localScale.y / 4, 0);
            Instantiate(BulletPrefab, bulletOrigin, Quaternion.identity);
        }
    }

    /// <summary>
    /// Metodo utilizado para no utilizar el Update ya que resultaria costoso, en cambio es una corrutina que corre a la misma velocidad que transcurre el juego y en esta se anima el invader y se hacen comprobaciones.
    /// </summary>
    /// <returns>IEnumerator Corrutina</returns>
    IEnumerator InvaderUpdate()
    {
        while (GameController.Instance.GameRunning)
        {
            yield return new WaitForSeconds(GameController.Instance.GameSpeed);
            SwitchSprite();
            if (transform.position.y < -8)
            {
                GameController.Instance.TakePlayerLife();
                DisableInvader(this);
                GameController.Instance.EnemiesCount--;
            }
        }
    }

    /// <summary>
    /// Evalua si esta instancia pertenece a un elemento en la fila inferior de la matriz principal, en caso afirmativo invoca la corrutina encargada de los disparos del invader
    /// </summary>
    public static void CheckInvaderFireEnabled(Invader inv)
    {
        if (GameController.Instance.EnemiesCount > 0)
        {
            if (!inv.Shooting)
            {
                if (MyGrid.Instance.GetLastActiveRow() == inv.y)
                {
                    inv.StartCoroutine(inv.InvaderShoot(inv.bulletSpawnRate));
                }
            }
        }
    }
    ///<sumary>
    ///Mientras el invader este con suficientes vidas, es animado intercambiando sus sprites
    ///</sumary>
    public void SwitchSprite()
    {
        if (Lifes > 0)
        {
            if (firstSpriteActive)
            {
                spriteRenderer.sprite = invaderData.spriteB;
                firstSpriteActive = false;
            }
            else
            {
                spriteRenderer.sprite = invaderData.spriteA;
                firstSpriteActive = true;
            }
        }
    }

    /// <summary>
    /// Dada una lista de InvaderBehaviour, se recorre la misma y se desactivan todos sus miembros, se cargan los puntajes y se reduce el conteo de enemigos global
    /// </summary>
    /// <param name="itemsToDisable">Lista de elementos InvaderBehaviour a ser deshabilitados</param>
    /// <returns>IEnumerator Corrutina</returns>
    public static IEnumerator DisableInvaders(List<Invader> itemsToDisable)
    {
        yield return new WaitForSeconds(GameController.Instance.GameSpeed);
        foreach (var item in itemsToDisable)
        {
            DisableInvader(item);
            GameController.Instance.Score += item.invaderData.Points;
            GameController.Instance.EnemiesCount--;
            if (GameController.Instance.EnemiesCount <= 0)
            {
                GameController.OnWinGameDelegate?.Invoke();
            }
        }
        MyGrid.Instance.FullRowsFireCheck();
        GameController.Instance.GridContainerAudioSource.PlayOneShot(GameController.Instance.InvaderExplosionSound);
    }

    /// <summary>
    /// Realiza operaciones para desactivar un gameobjet invader
    /// </summary>
    /// <param name="inv"></param>
    public static void DisableInvader(Invader inv)
    {
        ActiveInvaders.Remove(inv);
        inv.StopAllCoroutines();
        inv.Shooting = false;
        inv.gameObject.SetActive(false);
    }

    /// <summary>
    /// Evento de collision del invader, verifica la colision con las balas del jugador y analiza las vidas restantes. En caso de no poseer mas vidas, se buscan los vecinos en la matriz del mismo tipo y se desactivan
    /// </summary>
    /// <param name="collision">Collision2D elemento relacionado a la colision</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet_Player"))
        {
            //Quito una vida al invader que colisiono con la bala del jugador.
            Lifes--;
            GameController.Instance.PlayerCanShoot = true;
            Destroy(collision.gameObject);
            if (Lifes < 1)
            {
                //Busco coincidentes por tipo y desactivo todos si el alcanzado ya no tiene vidas
                var invadersConnected = GetConnectedInvaders();
                foreach (var item in invadersConnected)
                {
                    item.Lifes = 0;
                    item.spriteRenderer.sprite = invaderData.spriteDeath;
                    item.boxCollider.enabled = false;
                }
                StartCoroutine(DisableInvaders(invadersConnected));
            }
        }
    }
}
