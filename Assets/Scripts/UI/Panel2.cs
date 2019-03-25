using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel2 : MonoBehaviour
{
    
    enum State
    {
        HAIR,
        SKIN,
        EYES,
        TEMPERATURE,
        HAPPINESS
    };

    //Valores que se están mostrando en cada momento
    State state = State.HAIR;

    //Panel donde se muestran los datos
    [SerializeField]
    private GameObject panel = null;

    //Texto del botón que muestra y oculta el panel
    [SerializeField]
    private Text buttonText = null;

    //Texto del nombre de la región seleccionada
    [SerializeField]
    private Text nameRegion = null;

    //Texto del titulo de los Datos que se están mostrando.
    [SerializeField]
    private Text title = null;

    //Texto del valor 1 a mostrar
    [SerializeField]
    private Text option_1 = null;

    //Texto del valor 2 a mostrar
    [SerializeField]
    private Text option_2 = null;

    //Texto del valor 3 a mostrar
    [SerializeField]
    private Text option_3 = null;

    //Texto del valor 4 a mostrar
    [SerializeField]
    private Text option_4 = null;

    //Texto del valor 5 a mostrar
    [SerializeField]
    private Text option_5 = null;

    //Región seleccionada
    private Region region;

    //False: Panel oculto. True: Panel visible.
    private bool open = true;


    //Start del Panel. Actualiza los datos del panel cada segundo.
    private void Start()
    {
        InvokeRepeating("UpdatePanel", 0.5f, 1.0f);
        region = GameController.instance.GetRegions()[0];
        nameRegion.text = region.gameObject.name;
        UpdatePanel();
    }

    //Método para mostrar y ocultar el panel
    public void showHide()
    {
        if (open)
        {
            panel.SetActive(false);
            buttonText.text = ">";
        }
        else
        {
            panel.SetActive(true);
            buttonText.text = "<";
        }

        open = !open;
    }

    //Método para determinar de qué región se quieren conocer sus datos haciendo click sobre la misma.
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.name.Contains("Region_"))
                {
                    region = hit.transform.gameObject.GetComponent<Region>(); ;
                    nameRegion.text = region.gameObject.name;
                }
                UpdatePanel();
            }
        }
    }

    //Método para conocer los datos de pelo de la región
    public void hairOption()
    {
        title.text = "Hair";
        option_1.text = "Brown: " + region.getBrownCount().ToString();
        option_2.text = "Blonde: " + region.getBlondCount().ToString();
        option_3.text = "Readhead: " + region.getRedheadCount().ToString();
        option_4.text = "Dark: " + region.getDarkCount().ToString();
        option_5.text = "Trendy: " + region.getHairTrending().ToString();
        state = State.HAIR;
    }

    //Método para conocer los datos de piel de la región
    public void skinOption()
    {
        title.text = "Skin";
        option_1.text = "White: " + region.getWhitePeopleCount().ToString();
        option_2.text = "Black: " + region.getBlackPeopleCount().ToString();
        option_3.text = "Asiatic: " + region.getAsiaticPeopleCount().ToString();
        option_4.text = "";
        option_5.text = "Trendy: " + region.getSkinTrending().ToString();
        state = State.SKIN;
    }

    //Método para conocer los datos de ojos de la región
    public void eyesOption()
    {
        title.text = "Eyes";
        option_1.text = "Brown: " + region.getBrownEyesCount().ToString();
        option_2.text = "Blue: " + region.getBlueEyesCount().ToString();
        option_3.text = "Green: " + region.getGreenEyesCount().ToString();
        option_4.text = "";
        option_5.text = "Trendy: " + region.getEyesTrending().ToString();
        state = State.EYES;
    }

    //Método para conocer los datos de temperatura de la región
    public void temperatureOption()
    {
        title.text = "Temperature";
        option_1.text = "Low: " + region.getLowTempCount().ToString();
        option_2.text = "Medium: " + region.getBlackPeopleCount().ToString();
        option_3.text = "High: " + region.getHighTempCount().ToString();
        option_4.text = "";
        option_5.text = "";
        state = State.TEMPERATURE;
    }

    //Método para conocer los datos de felicidad de la región
    public void happinessOption()
    {
        title.text = "Happiness";
        option_1.text = "";
        option_2.text = "";
        option_3.text = "";
        option_4.text = "";
        option_5.text = "Trendy: " + region.getTrendyHappiness().ToString();
        state = State.HAPPINESS;
    }

    //Método para actualizar los datos del Panel
    void UpdatePanel()
    {
        if (region)
        {
            
            switch (state)
            {
                case State.HAIR:
                    hairOption();
                    break;
                case State.SKIN:
                    skinOption();
                    break;
                case State.EYES:
                    eyesOption();
                    break;
                case State.TEMPERATURE:
                    temperatureOption();
                    break;
                case State.HAPPINESS:
                    happinessOption();
                    break;
            }
        }
    }
}
