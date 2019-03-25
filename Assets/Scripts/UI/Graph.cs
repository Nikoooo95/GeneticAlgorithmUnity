using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Graph : MonoBehaviour
{
    //Sprite del circulo de la gráfica
    [SerializeField]
    private Sprite circleSprite = null;

    //Contenedor donde se coloca la grafica
    [SerializeField]
    private RectTransform container = null;

    //Texto que indica que muestra la gráfica 1
    [SerializeField]
    private GameObject graphic1 = null;

    //Texto que indica que muestra la gráfica 2
    [SerializeField]
    private GameObject graphic2 = null;

    //Texto que indica que muestra la gráfica 3
    [SerializeField]
    private GameObject graphic3 = null;

    //Texto que indica que muestra la gráfica 4
    [SerializeField]
    private GameObject graphic4 = null;

    //Panel completo donde se muestra todo el apartado de graficas
    [SerializeField]
    private GameObject panel = null;

    //Texto del boton que muestra / oculta el panel de graficas
    [SerializeField]
    private Text buttonText = null;

    //Lista con los objetos que componen la gráfica amarilla
    private List<GameObject> yellowGraph;

    //Lista con los objetos que componen la gráfica verde
    private List<GameObject> greenGraph;

    //Lista con los objetos que componen la gráfica roja
    private List<GameObject> redGraph;

    //Lista con los objetos que componen la gráfica cyan
    private List<GameObject> cyanGraph;

    //Region que se está mostrando actualmente en la gráfica
    private Region region;

    //Instancia del Graph
    public static Graph instance = null;

    //False: panel de gráficas cerrado. True: panel de gráficas visible.
    private bool open;

    private enum State
    {
        HAIR,
        SKIN,
        EYES,
        TEMPERATURE,
        HAPPINESS,
        MOVING,
        PEOPLE
    };

    //Estado de qué graficas se está mostrando
    State state;

    //Valor máximo en Y de la gráfica
    float yMaximun = 100.0f;

    //Singleton de Graph
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    //Inicialización del Graph.
    //Inicialmente se muestra en falso y comenzará mostrando los valores de Hair de la Región 1 por defecto.
    private void Start()
    {
        yellowGraph = new List<GameObject>();
        greenGraph = new List<GameObject>();
        redGraph = new List<GameObject>();
        cyanGraph = new List<GameObject>();

        Region1Button();
        
        open = true;
        ShowHide();
        state = State.HAIR;
        InvokeRepeating("ShowGraphics", 1.0f, 1.0f);
    }

    //Metodo Update para el zoom en las gráficas.
    void Update()
    {
        // Check if the left mouse button was clicked
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && open)
        {
            // Check if the mouse was clicked over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                yMaximun += Input.GetAxis("Mouse ScrollWheel") * 50;

            }
        }
    }

    //Método Switch para seleccionar que graficas mostrar 
    //o parar borrar los elementos que contiene si está cerrada.
    private void ShowGraphics()
    {
        if (open)
        {
            switch (state)
            {
                case State.HAIR:
                    HairButton();
                    break;
                case State.EYES:
                    EyesButton();
                    break;
                case State.SKIN:
                    SkinButton();
                    break;
                case State.TEMPERATURE:
                    TemperatureButton();
                    break;
                case State.HAPPINESS:
                    HappinessButton();
                    break;
                case State.MOVING:
                    MoveOnButton();
                    break;
                case State.PEOPLE:
                    PeopleButton();
                    break;
            }
        }
        else
        {
            foreach (GameObject sprite in yellowGraph){ Destroy(sprite); }
            foreach (GameObject sprite in greenGraph) { Destroy(sprite); }
            foreach (GameObject sprite in redGraph) { Destroy(sprite); }
            foreach (GameObject sprite in cyanGraph) { Destroy(sprite); }
            yellowGraph.Clear();
            greenGraph.Clear();
            redGraph.Clear();
            cyanGraph.Clear();
        }
    }

    //Método que genera una grafica a partir de una lista de valores.
    //Almacena los sprites que genera en una lista dada y lo hace con un color dado.
    private void ShowGraph(List<int> values, List<GameObject> sprites, Color color)
    {
        CleanList(sprites);
        
        float graphHeight = container.sizeDelta.y;
        
        float xSize = 5.0f;
        GameObject lastPoint = null;
        for (int i = 0; i < values.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (values[i] / yMaximun) * graphHeight;
            GameObject newCircle = CreateCircle(new Vector2(xPosition, yPosition), color);
            sprites.Add(newCircle);
            if (lastPoint != null)
            {
                sprites.Add(CreateConnection(lastPoint.GetComponent<RectTransform>().anchoredPosition, newCircle.GetComponent<RectTransform>().anchoredPosition, color));
            }
            lastPoint = newCircle;
        }

    }

    //Destruye y limpia los Gameobjects que hay en una lista
    private void CleanList(List<GameObject> sprites)
    {
        foreach (GameObject sprite in sprites)
        {
            Destroy(sprite);
        }
        sprites.Clear();
    }

    //Crea un circulo para la gráfica
    private GameObject CreateCircle(Vector2 position, Color color)
    {
       GameObject littleCircle = new GameObject("Circle", typeof(Image));
        littleCircle.transform.SetParent(container, false);
        littleCircle.GetComponent<Image>().color = color;
        littleCircle.GetComponent<Image>().sprite = circleSprite;
        RectTransform transformCircle = littleCircle.GetComponent<RectTransform>();
        transformCircle.anchoredPosition = position;
        transformCircle.sizeDelta = new Vector2(2, 2);
        transformCircle.anchorMin = Vector2.zero;
        transformCircle.anchorMax = Vector2.zero;
      
        return littleCircle;
    }

    //Crea una conexión entre un circulo y el siguiente para la gráfica
    private GameObject CreateConnection(Vector2 positionA, Vector2 positionB, Color color)
    {
        GameObject connection = new GameObject("connection", typeof(Image));
        connection.transform.SetParent(container, false);
        connection.GetComponent<Image>().color = color;
        RectTransform transformConnection = connection.GetComponent<RectTransform>();
        Vector2 dir = (positionB - positionA).normalized;
        float distance = Vector2.Distance(positionA, positionB);
        transformConnection.anchorMin = Vector2.zero;
        transformConnection.anchorMax = Vector2.zero;
        transformConnection.sizeDelta = new Vector2(distance, 1f);
        transformConnection.anchoredPosition = positionA + dir * distance * 0.5f;
        transformConnection.localEulerAngles = new Vector3(0, 0, GetAngleFromVector(dir));
        return connection;
    }

    //Determina el angulo que tendrá la conexión entre circulos de la gráfica.
    private float GetAngleFromVector(Vector2 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    //Selección de la Región 1
    public void Region1Button()
    {
        region = GameController.instance.GetRegions()[0];
    }

    //Selección de la Región 2
    public void Region2Button()
    {
        region = GameController.instance.GetRegions()[1];
    }

    //Selección de la Región 3
    public void Region3Button()
    {
        region = GameController.instance.GetRegions()[2];
    }

    //Muestra los valores de pelo históricos de una región
    public void HairButton()
    {
        state = State.HAIR;
        GraphicChange(graphic1, "Blonde", Color.yellow);
        GraphicChange(graphic2, "Brown", Color.green);
        GraphicChange(graphic3, "Redhead", Color.red);
        GraphicChange(graphic4, "Dark", Color.cyan);
        ShowGraph(region.getBlondeHairHistoric(), yellowGraph, Color.yellow);
        ShowGraph(region.getBrownHairHistoric(), greenGraph, Color.green);
        ShowGraph(region.getReadheadHairHistoric(), redGraph, Color.red);
        ShowGraph(region.getDarkHairHistoric(), cyanGraph, Color.cyan);

    }

    //Muestra los valores de piel históricos de una región
    public void SkinButton()
    {
        state = State.SKIN;
        GraphicChange(graphic1, "Black", Color.yellow);
        GraphicChange(graphic2, "White", Color.green);
        GraphicChange(graphic3, "Asiatic", Color.red);
        graphic4.SetActive(false);
        CleanList(cyanGraph);
        ShowGraph(region.getBlackSkinHistoric(), yellowGraph, Color.yellow);
        ShowGraph(region.getWhiteSkinHistoric(), greenGraph, Color.green);
        ShowGraph(region.getAsiaticSkinHistoric(), redGraph, Color.red);
    }

    //Muestra los valores de ojos históricos de una región.
    public void EyesButton()
    {
        state = State.EYES;
        GraphicChange(graphic1, "Brown", Color.yellow);
        GraphicChange(graphic2, "Green", Color.green);
        GraphicChange(graphic3, "Blue", Color.red);
        graphic4.SetActive(false);
        CleanList(cyanGraph);
        ShowGraph(region.getBrownEyesHistoric(), yellowGraph, Color.yellow);
        ShowGraph(region.getGreenEyesHistoric(), greenGraph, Color.green);
        ShowGraph(region.getBlueEyesHistoric(), redGraph, Color.red);
    }

    //Muestra los valores de temeperatura históricos de una región.
    public void TemperatureButton()
    {
        state = State.TEMPERATURE;
        GraphicChange(graphic1, "Temperature High", Color.yellow);
        GraphicChange(graphic2, "Temperature Medium", Color.green);
        GraphicChange(graphic3, "Temperature Low", Color.red);
        graphic4.SetActive(false);
        CleanList(cyanGraph);
        ShowGraph(region.getHighTemperatureHistoric(), yellowGraph, Color.yellow);
        ShowGraph(region.getMediumTemperatureHistoric(), greenGraph, Color.green);
        ShowGraph(region.getLowTemperatureHistoric(), redGraph, Color.red);
    }

    //Muestra los valores de felicidad históricos de una región.
    public void HappinessButton()
    {
        state = State.HAPPINESS;
        GraphicChange(graphic1, "Happiness", Color.yellow);
        graphic2.SetActive(false);
        CleanList(greenGraph);
        graphic3.SetActive(false);
        CleanList(redGraph);
        graphic4.SetActive(false);
        CleanList(cyanGraph);
        ShowGraph(region.getHappinessHistoric(), yellowGraph, Color.yellow);
        
    }

    //Muestra los valores de migraciones históricos de una región.
    public void MoveOnButton()
    {
        state = State.MOVING;
        GraphicChange(graphic1, "Move In", Color.yellow);
        GraphicChange(graphic2, "Move Out", Color.green);
        graphic3.SetActive(false);
        CleanList(redGraph);
        graphic4.SetActive(false);
        CleanList(cyanGraph);
        ShowGraph(region.getMoveInHistoric(), yellowGraph, Color.yellow);
        ShowGraph(region.getMoveOutHistoric(), greenGraph, Color.green);
    }

    //Muestra los valores de gente históricos de una región.
    public void PeopleButton()
    {
        state = State.PEOPLE;
        GraphicChange(graphic1, "Total People", Color.yellow);
        GraphicChange(graphic2, "Mens", Color.green);
        GraphicChange(graphic3, "Womens", Color.red);
        graphic4.SetActive(false);
        CleanList(cyanGraph);
        ShowGraph(region.getTotalPeopleHistoric(), yellowGraph, Color.yellow);
        ShowGraph(region.getMensHistoric(), greenGraph, Color.green);
        ShowGraph(region.getWomensHistoric(), redGraph, Color.red);
    }

    //Cambia los textos sobre los datos de las gráficas.
    public void GraphicChange(GameObject graphicObject, string value, Color color)
    {
        graphicObject.SetActive(true);
        graphicObject.transform.Find("Text").GetComponent<Text>().text = value;
        graphicObject.transform.Find("Image").GetComponent<Image>().color = color;
    }

    //Oculta y muestra el panel de las gráficas.
    public void ShowHide()
    {
        if (open)
        {
            panel.SetActive(false);
            buttonText.text = "▼";
        }
        else
        {
            panel.SetActive(true);
            buttonText.text = "X";
        }
        open = !open;
    }
}
