using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D thisRb;
    [Header("Settings")]
    public float speed;
    void Start()
    {
        //En caso de que las referencias no sean cargadas desde el editor, se muestra un mensaje y se buscan
        if (thisRb == null)
        {
            Debug.LogWarning("bullet rigidbody not set, performing search");
            thisRb = GetComponent<Rigidbody2D>();
        }
    }
    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * new Vector3(0, 1, 0);
    }
    public void DestroyBullet()
    {

        if (gameObject.CompareTag("Bullet_Player"))
        {
            //Reestablesco la capacidad de disparo del jugador
            GameController.Instance.PlayerCanShoot = true;
        }
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Barrier"))
        {
            collision.gameObject.SetActive(false);
            DestroyBullet();
        }
        if (collision.gameObject.CompareTag("Bullet_Player") || collision.gameObject.CompareTag("Bullet_Invader"))
        {
            Destroy(collision.gameObject);
            DestroyBullet();
        }
        if (collision.gameObject.CompareTag("LvlBound"))
        {
            DestroyBullet();
        }
    }
}
