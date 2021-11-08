using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlBoundController : MonoBehaviour
{
 
    /// <summary>
    /// Deteccion de colisiones laterales con los enemigos, para realizar el salto de linea llamando al GameController, cambiando el estado del booleano ShouldJumOneRow para que en la proxima actualizacion de movimiento, realize un movimiento vertical
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Invader"))
        {
            MyGrid.Instance.ShouldJumpOneRow = true;
        }
    }
}
