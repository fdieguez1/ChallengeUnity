using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Utils
    {
        /// <summary>
        /// Dada una camara devuelve la posicion del mouse traducidas a coodernadas X, Y del mundo en unity para 2 dimensiones, ignorando la Z
        /// </summary>
        /// <param name="camera">Camara a evaluar</param>
        /// <returns>Vector3 posicion en X, Y del puntero del mouse, Z es siempre 0</returns>
        public static Vector3 GetMouseWorldPosition2d(Camera camera)
        {
            Vector3 vec = camera.ScreenToWorldPoint(Input.mousePosition);
            vec.z = 0f;
            return vec;
        }
    }
}
