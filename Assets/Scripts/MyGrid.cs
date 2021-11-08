using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase grilla, dado un InvaderBehaviour genera columnas y filas como una matriz para almacenarlo y mostrarlo, se ocupa del movimiento de la grilla en su totalidad. 
/// </summary>
public class MyGrid : MonoBehaviour
{
    public static MyGrid Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    private void OnEnable()
    {
        GameController.OnGameStartEvent += GridStart;
        GameController.OnWinGameDelegate += GridStart;
    }
    private void OnDisable()
    {
        GameController.OnGameStartEvent += GridStart;
        GameController.OnWinGameDelegate += GridStart;
    }


    [Header("Grid size settings")]
    public int Width;
    public int Height;

    [SerializeField]
    float cellSize;

    [SerializeField]
    Vector3 offsetPosition;

    [Header("Prefabs")]
    [SerializeField]
    GameObject invaderPrefab;
    [SerializeField]
    GameObject invaderBulletPrefab;

    [Header("Movement behaviour")]
    public bool ShouldJumpOneRow;

    Vector3 originPosition;
    Vector3 spawnPosition;

    public Invader[,] GridArray;
    public void GridStart()
    {
        StopAllCoroutines();
        GameController.Instance.EnemiesCount = 0;
        Invader.ActiveInvaders = new List<Invader>();
        originPosition = new Vector3(Width * -cellSize / 2, Height * -cellSize / 2) + offsetPosition;
        GridArray = new Invader[Width, Height];
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                spawnPosition = GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                GameObject invaderGameObject = Instantiate(invaderPrefab, spawnPosition, Quaternion.identity, transform);
                Invader invaderInstance = invaderGameObject.AddComponent<Invader>();
                invaderInstance.spriteRenderer = invaderGameObject.GetComponent<SpriteRenderer>();
                invaderInstance.boxCollider = invaderGameObject.GetComponent<BoxCollider2D>();
                invaderInstance.x = x;
                invaderInstance.y = y;
                invaderInstance.BulletPrefab = invaderBulletPrefab;
                invaderInstance.bulletSpawnRate = UnityEngine.Random.Range(1, 4);
                GridArray[x, y] = invaderInstance;
                GameController.Instance.EnemiesCount++;
                Invader.ActiveInvaders.Add(invaderInstance);
                //Mostrar lineas de guia al generar la grilla
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(Width, 0), GetWorldPosition(Width, Height), Color.white, 100f);
        MyGrid.Instance.FullRowsFireCheck();
        StartCoroutine(GridMovement());
    }
    /// <summary>
    /// Obtiene los valores X, Y del mundo segun la posicion en la matriz
    /// </summary>
    /// <param name="x">posicion en X</param>
    /// <param name="y">posicion en Y</param>
    /// <returns>Vector3 Valores del mundo en X e Y</returns>
    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Invader GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            return GridArray[x, y].GetComponent<Invader>();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Metodo encargado del movimiento de la grilla en su totalidad
    /// </summary>
    /// <returns>IEnumerator Corrutina</returns>
    IEnumerator GridMovement()
    {
        while (GameController.Instance.GameRunning)
        {
            yield return new WaitForSeconds(GameController.Instance.GameSpeed);

            if (ShouldJumpOneRow)
            {
                transform.position += new Vector3(0, -cellSize, 0);
                GameController.Instance.MovementDirection *= -1;
                ShouldJumpOneRow = false;
            }
            else
            {
                transform.position += GameController.Instance.MovementDirection;
            }
        }
    }

    /// <summary>
    /// Recorre la lista de invaders activos y chequea si pueden disparar
    /// </summary>
    public void FullRowsFireCheck()
    {
        foreach (var item in Invader.ActiveInvaders)
        {
            Invader.CheckInvaderFireEnabled(GridArray[item.x, item.y]);
        }
    }

    /// <summary>
    /// Devuelve la primera fila con invaders activos
    /// </summary>
    /// <returns></returns>
    public int GetLastActiveRow()
    {
        int y;
        for (y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (GridArray[x, y].gameObject.activeSelf)
                {
                    return y;
                }
            }
        }
        return y;
    }
}
