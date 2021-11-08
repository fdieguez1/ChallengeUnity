using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase invader, hereda de ScriptableObject para poderse instanciar desde el editor y cargar datos que luego seran utilizados para los distintos tipos de enemigo
/// </summary>
[CreateAssetMenu(fileName = "Invader", menuName = "Invader")]
public class InvaderData : ScriptableObject
{
    public int Lifes;
    public Sprite spriteA;
    public Sprite spriteB;
    public Sprite spriteDeath;
    public Color Color;
    public int Points;
}
