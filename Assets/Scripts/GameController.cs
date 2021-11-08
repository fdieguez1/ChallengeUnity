using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;

public delegate void GameStart();
public delegate void GameOver();
public delegate void WinGameDelegate();
/// <summary>
/// Clase principal del juego, utiliza el patron singleton para poseer una instancia, donde se guardaran todas las referencias para tener un lugar en la memoria accesible desde cualquier otra clase en el proyecto
/// </summary>
public class GameController : MonoBehaviour
{
    public static event GameStart OnGameStartEvent;
    public static event GameOver OnGameOverEvent;
    public static WinGameDelegate OnWinGameDelegate;

    [Header("Dificulty settings")]
    public float DificultySpeedIncrease;

    [Header("Enemies")]
    int enemiesCount;

    /// <summary>
    /// Propiedad EnemiesCount, se utiliza para cada vez que se carga un valor diferente en el conteo de enemigos llamar a metodos que evaluan la capacidad de los enemigos restantes de disparar y en caso de no existir mas enemigos, cargar la escena nuevamente
    /// </summary>
    public int EnemiesCount
    {
        get
        {
            return enemiesCount;
        }
        set
        {
            if (value < enemiesCount)
            {
                AddDificulty();
            }
            enemiesCount = value;
        }
    }
    int score;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            ScoreText.text = $"Puntaje: {score}";
        }
    }

    [Header("Gameplay settings")]
    public float GameSpeed;
    public Vector3 MovementDirection;

    [Header("References")]
    public GameObject CameraObject;
    public GameObject CanvasGameObject;
    public GameObject PausePanelGameObject;
    public GameObject LifesContainerGameObject;
    public GameObject ScoreTextGameObject;
    public GameObject LifesCountTextGameObject;
    public Camera MainGameCamera;
    public GameObject GridContainer;
    public TextMeshProUGUI ScoreText;

    [Header("Audio")]
    public AudioSource PlayerAudioSource;
    public AudioSource GridContainerAudioSource;
    public AudioClip InvaderFireSound;
    public AudioClip InvaderExplosionSound;
    public AudioClip PlayerHurtSound;

    public InvaderData[] InvaderScriptableObjects;

    [Header("Player data and references")]
    public bool PlayerCanShoot;
    public int PlayerLifes;
    public GameObject LifesContainer;
    public TextMeshProUGUI LifesText;

    Transform[] playerCannonsUI;

    GameObject CameraInstance;
    GameObject CanvasInstance;
    GameObject PausePanelInstance;
    GameObject LifesContainerInstance;
    GameObject LifesCountTextInstance;
    GameObject ScoreTextInstance;
    Transform LifesPanelTranform;

    /// <summary>
    /// Verifica la escala del tiempo para ver si el juego esta corriendo
    /// </summary>
    public bool GameRunning
    {
        get
        {
            return Time.timeScale > 0;
        }
    }

    public static GameController Instance;

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
    private void Start()
    {
        GameControllerStart();
        OnGameStartEvent?.Invoke();
    }
    void GameControllerStart()
    {
        //Carga de prefabs con los gameobjects que seran requeridos durante el ciclo de vida del nivel
        if (CameraInstance == null)
            CameraInstance = Instantiate(CameraObject);
        if (CanvasInstance == null)
            CanvasInstance = Instantiate(CanvasGameObject);
        if (LifesPanelTranform == null)
            LifesPanelTranform = CanvasInstance.GetComponentsInChildren<RectTransform>()[1];
        if (PausePanelInstance == null)
            PausePanelInstance = Instantiate(PausePanelGameObject, CanvasInstance.transform);
        if (LifesContainerInstance == null)
            LifesContainerInstance = Instantiate(LifesContainerGameObject, LifesPanelTranform, false);
        if (LifesCountTextInstance == null)
            LifesCountTextInstance = Instantiate(LifesCountTextGameObject, LifesPanelTranform, false);
        if (ScoreTextInstance == null)
            ScoreTextInstance = Instantiate(ScoreTextGameObject, CanvasInstance.transform);
        MainGameCamera = CameraInstance.GetComponent<Camera>();
        LifesContainer = LifesContainerInstance;
        LifesText = LifesCountTextInstance.GetComponent<TextMeshProUGUI>();
        ScoreText = ScoreTextInstance.GetComponent<TextMeshProUGUI>();
        Button[] pauseButtons = PausePanelInstance.GetComponentsInChildren<Button>();
        pauseButtons[0].onClick.AddListener(() => { ResumeGame(); });
        pauseButtons[1].onClick.AddListener(() => { QuitApp(); });

        if (GridContainer == null)
        {
            GridContainer = MyGrid.Instance.gameObject;
            Debug.LogError("Grid container not set in game controller");
        }
        if (GridContainerAudioSource == null)
        {
            Debug.LogWarning("Grid container audio source not set in game controller, searching");
            GridContainerAudioSource = GridContainer.GetComponent<AudioSource>();
        }
        if (InvaderFireSound == null)
        {
            Debug.LogError("Invader audio clip not set in game controller");
            //ToDo mover los audios a una carpeta en resources, para cargarlo utilizando su path
        }
        InvaderScriptableObjects = Resources.LoadAll<InvaderData>("Invaders");
        playerCannonsUI = LifesContainer.GetComponentsInChildren<Transform>();
        
    }

    private void OnEnable()
    {
        OnGameStartEvent += ResumeGame;
        OnWinGameDelegate += GameControllerStart;
        OnWinGameDelegate += ResumeGame;
        OnGameOverEvent += GoMainMenu;
        OnGameOverEvent += DestroyGameplayInstances;
    }
    private void OnDisable()
    {
        OnGameStartEvent = null;
        OnWinGameDelegate = null;
        OnGameOverEvent = null;
    }

    /// <summary>
    /// Disminuye el tiempo entre iteraciones del GameSpeed utilizado por los metodos encargados del movimiento, los disparos y las animaciones
    /// </summary>
    public void AddDificulty()
    {
        GameSpeed -= DificultySpeedIncrease;
    }
    /// <summary>
    /// Le quita una vida al jugador, llamando a los metodos necesarios cuando esto sucede para mostarse en la UI
    /// </summary>
    public void TakePlayerLife()
    {
        PlayerLifes--;
        RefreshPlayerLifes(PlayerLifes);
        if (PlayerLifes == 0)
        {
            OnGameOverEvent?.Invoke();
        }
        PlayerAudioSource.PlayOneShot(PlayerHurtSound);
    }

    public void DestroyGameplayInstances()
    {
        OnGameStartEvent = null;
        OnWinGameDelegate = null;
        OnGameOverEvent = null;
        Time.timeScale = 0;
        Destroy(GameController.Instance.gameObject);
        Destroy(MyGrid.Instance.gameObject);
        OnGameStartEvent = null;
        OnWinGameDelegate = null;
        OnGameOverEvent = null;
    }

    #region UI
    /// <summary>
    /// Despliega el menu de pausa y cambia la escala de tiempo para detener el juego
    /// </summary>
    public void PauseGame()
    {
        PausePanelInstance.SetActive(true);
        Time.timeScale = 0;
    }
    /// <summary>
    /// Reestablece la escala del tiempo y oculta el menu de pausa
    /// </summary>
    public void ResumeGame()
    {
        PausePanelInstance.SetActive(false);
        Time.timeScale = 1;
    }
    /// <summary>
    /// Utiliza el metodo estatico ExitApp() para cerrar la aplicacion
    /// </summary>
    public void QuitApp()
    {
        MainMenu.ExitApp();
    }

    /// <summary>
    /// Actualiza el texto y las imagenes de las vidas del usuario en la UI dado un valor entero
    /// </summary>
    /// <param name="Value">valor actual de las vidas</param>
    public void RefreshPlayerLifes(int Value)
    {
        LifesText.text = Value.ToString();
        for (int i = 0; i < playerCannonsUI.Length; i++)
        {
            if (i < Value)
            {
                playerCannonsUI[i].gameObject.SetActive(true);
            }
            else
            {
                playerCannonsUI[i].gameObject.SetActive(false);
            }
        }
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
