using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCannon : MonoBehaviour
{
    [Header("Player settings")]
    float horizontal;
    public float playerSpeed;
    [Header("References")]
    public Renderer thisRenderer;
    public AudioSource thisAudioSource;
    [Header("Bullets")]
    public GameObject bulletPrefab;
    Vector3 bulletOrigin;
    private void Start()
    {
        //Reset de la capacidad de disparo del jugador y de sus vidas
        GameController.Instance.PlayerCanShoot = true;
        GameController.Instance.PlayerLifes = 3;

        //En caso de que las referencias no sean cargadas desde el editor, se muestra un mensaje y se buscan
        if (thisRenderer == null)
        {
            Debug.LogWarning("Player renderer not set, performing search");
            thisRenderer = GetComponent<Renderer>();
        }
        if (thisAudioSource == null)
        {
            Debug.LogWarning("Player audio source not set, performing search");
            thisAudioSource = GetComponent<AudioSource>();
        }
    }
    /// <summary>
    /// Utiliza el nuevo Input sistem de Unity para cargar en una variable el valor ingresado para el movimiento horizontal
    /// </summary>
    /// <param name="context"></param>
    public void MovePlayer(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    /// <summary>
    /// Metodo de disparo, instancia una bala segun la ubicacion del jugador
    /// </summary>
    public void Shoot()
    {
        if (GameController.Instance.PlayerCanShoot)
        {
            thisAudioSource.Play();
            bulletOrigin = thisRenderer.bounds.center;
            Instantiate(bulletPrefab, bulletOrigin, Quaternion.identity);
            GameController.Instance.PlayerCanShoot = false;
        }
    }

    /// <summary>
    /// Deteccion de colisiones, contra enemigos o balas de los enemigos
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Invader"))
        {
            Invader.DisableInvader(collision.gameObject.GetComponent<Invader>());
            GameController.Instance.TakePlayerLife();
            GameController.Instance.EnemiesCount--;
        }
        if (collision.gameObject.CompareTag("Bullet_Invader"))
        {
            Destroy(collision.gameObject);
            GameController.Instance.TakePlayerLife();
        }
    }
    /// <summary>
    /// Realizacion de movimiento segun el input del usuario
    /// </summary>
    private void FixedUpdate()
    {
        transform.position += playerSpeed * Time.fixedDeltaTime * new Vector3(horizontal, 0, 0);
    }

}
