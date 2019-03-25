using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{

    //Panel donde se muestran los datos
    [SerializeField]
    private GameObject panel = null;

    //Texto del botón que muestra y oculta el panel
    [SerializeField]
    private Text buttonText = null;

    //Texto para el nombre de la región seleccionada
    [SerializeField]
    private Text nameRegion = null;

    //Texto para la polación de la región seleccionada
    [SerializeField]
    private Text poblation = null;

    //Texto para los hombres de la región seleccionada
    [SerializeField]
    private Text mens = null;

    //Texto para las mujeres de la región seleccionada
    [SerializeField]
    private Text womans = null;

    //Texto para la temperatura de la región seleccionada
    [SerializeField]
    private Text temperature = null;

    //Texto para la población máxima que ha tenido la región seleccionada
    [SerializeField]
    private Text maxRegion = null;

    //Texto para la población máxima que ha tenido el mundo.
    [SerializeField]
    private Text maxWorld = null;

    //Texto para la capacidad de la región seleccionada
    [SerializeField]
    private Text capacity = null;

    //Región seleccionada
    private Region region;

    //False si el panel está oculto. True si el panel está visible.
    private bool open = true;

    //Variable para cantidad máxima de gente que ha habido en el mundo.
    private float maxPeopleWorld = 0.0f;

    //Método para ocultar y mostrar el panel
    public void showHide(){
        if (open)
        {
            panel.SetActive(false);
            buttonText.text = "<";
        }
        else
        {
            panel.SetActive(true);
            buttonText.text = ">";
        }

        open = !open;
    }

    //Método Start para actualizar el panel cada segundo
    private void Start()
    {
        InvokeRepeating("UpdatePanel", 0.5f, 1.0f);
    }

    //Método para sleccionar la región de la que se quieren conocer los datos.
    void Update(){
        if (Input.GetMouseButtonDown(0))
        {
            region = null;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.name.Contains("Region_"))
                {
                    region = hit.transform.gameObject.GetComponent<Region>(); ;
                }
                UpdatePanel();
            }
        }
    }

    //Método que actualiza los datos del panel 
    //Si hay una región seleccionada, muestra los datos de la misma.
    //En caso contrario, muestra los datos del mundo.
    void UpdatePanel()
    {
        if (region)
        {
            //DATOS DE LA REGIÓN
            int peopleAmount = 0;
            nameRegion.text = region.gameObject.name;
            poblation.text = "Poblation: " + region.getPeopleCount().ToString();
            mens.text = "Mens: " + region.getMenCount().ToString();
            womans.text = "Womens: " + region.getWomenCount().ToString();
            temperature.text = "Temperature: " + region.getTemperature().ToString();
            maxRegion.text = "Max. Region: " + region.getMaxPeople().ToString();
            foreach (Region r in GameController.instance.GetRegions())
            {
                peopleAmount += r.getPeopleCount();
            }
            if (peopleAmount > maxPeopleWorld)
                maxPeopleWorld = peopleAmount;
            maxWorld.text = "Max. World: " + maxPeopleWorld.ToString();
            capacity.text = "Capactiy: " + region.getCapacity().ToString();

        }
        else
        {
            //DATOS DEL MUNDO
            int peopleAmount = 0;
            int mensAmount = 0;
            int womansAmount = 0;
            int capacityTotal = 0;
          
            foreach (Region r in GameController.instance.GetRegions())
            {
                peopleAmount += r.getPeopleCount();
                mensAmount += r.getMenCount();
                womansAmount += r.getWomenCount();
                capacityTotal += r.getCapacity();
            }

            if (peopleAmount > maxPeopleWorld)
                maxPeopleWorld = peopleAmount;

            nameRegion.text = "World";
            poblation.text = "Poblation: " + peopleAmount.ToString();
            mens.text = "Mens: " + mensAmount.ToString();
            womans.text = "Womens: " + womansAmount.ToString();
            temperature.text = "";
            maxRegion.text = "";
            maxWorld.text = "Max. World: " + maxPeopleWorld.ToString();
            capacity.text = "T. Capacity: " + capacityTotal.ToString();
        }
        
    }

}
