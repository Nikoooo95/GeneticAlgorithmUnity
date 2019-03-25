using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Prefab que representa a una persona
    [SerializeField]
    private GameObject person = null;

    //Cantidad inciial de gente que se quiere
    [SerializeField]
    private int initialPeopleCount = 300;

    //Cantidad minima en una región
    [SerializeField]
    private int minPeopleInRegion = 0;

    //Cantidad máxima en una región.
    [SerializeField]
    private int maxPeopleInRegion = 100;

    //Velocidad del tiempo de ejecución
    [SerializeField]
    private float timeSpeed = 1;

    //Lista de las regiones de la escena.
    [SerializeField]
    private List<Region> regions = null;

    //Rango de probabilidad de procreacíón entre personas
    [SerializeField][Range(1,100)]
    public int procreateStat = 100;

    //Rango de probabilidad de que el entorno afecte a un niño nuevo
    [SerializeField]
    [Range(1, 100)]
    public float affectChildren = 20.0f;

    //Factoría de procreación
    private ProcreationFactory factory;

    //Instancia del Gamecontroller
    public static GameController instance;

    //Singleton del Gamecontroller
    private void Awake()
    {
        if (!instance)
            instance = this;

        factory = GetComponent<ProcreationFactory>();
    }

    //Inicia la Demo-
    //Establece la velocidad de ejecución.
    //Crea el mundo y lo envejece cada 1 seg.
    void Start()
    {
        Time.timeScale = timeSpeed;
        instance = this;

        createWorld();
        InvokeRepeating("getOlderRegion", 0.5f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeSpeed;
    }

    //Crea el mundo.
    //Para ello crea una determinada cantidad de regiones y distribuye
    //la cantidad inicial de gente en dichas regiones.
    private void createWorld()
    {
        for (int i = 0; i < regions.Count; ++i)
        {
            createRegion(regions[i]);
        }

        distributePeople(initialPeopleCount);
    }

    //Crea una region.
    //Establece unos determinados valores de temperatura, de cantidad de gente y 
    //el porcentaje de que dicho entorno afecte a los niños que nazcan en ella.
    private void createRegion(Region region)
    {
        Array values = Enum.GetValues(typeof(Temperature));
        Temperature temp = (Temperature)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        region.setTemperature(temp);
        region.setCapacity(UnityEngine.Random.Range(minPeopleInRegion, maxPeopleInRegion + 1));
        region.setAffectChildren(affectChildren);
    }

    //Crea a una persona de manera aleatoria (solo para las personas iniciales)
    private Person createPerson()
    {
        GameObject newPerson = Instantiate(person);

        //Eyes
        Array values = Enum.GetValues(typeof(Eyes));
        Eyes eye = (Eyes)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        //Hair
        values = Enum.GetValues(typeof(Hair));
        Hair hair = (Hair)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        //Skin
        values = Enum.GetValues(typeof(Skin));
        Skin skin = (Skin)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        //Temperature
        values = Enum.GetValues(typeof(Temperature));
        Temperature temp = (Temperature)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        //Genre
        values = Enum.GetValues(typeof(Genre));
        Genre genre = (Genre)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        newPerson.GetComponent<Person>().constructPerson(
            (short)UnityEngine.Random.Range(0, 40),
            genre,
            hair,
            eye,
            skin,
            temp,
            (short)50
            );
        return newPerson.GetComponent<Person>();
    }

    //Establece la velocidad del tiempo
    public void setTimeScale(float f)
    {
        Time.timeScale = f;
    }

    //Distribuye la población inicial por las regiones que hay en el mundo de manera aleatoria
    public void distributePeople(int count)
    {

        int amountPeople;
        for(int i = 0; i < regions.Count-1; ++i)
        {
           amountPeople = (int)UnityEngine.Random.Range(0, count);
            for (int j = 0; j < amountPeople; ++j)
            {
                regions[i].addPerson(createPerson());
            }
            count -= amountPeople; 
        }
        for (int j = 0; j < count; ++j)
        {
            regions[regions.Count-1].addPerson(createPerson());
        }

    }

    //Envejece a la población de todas las regiones
    private void getOlderRegion()
    {
        foreach(Region r in regions)
        {
            r.getOlderPeople();
        }
    }

    //Genera a un nuevo individuo a partir de su padre, su madre y la región en la que nazca.
    public void procreate(Region region, Person father, Person mother)
    {
        int randomChildren = UnityEngine.Random.Range(2, 4);
        for(int i = 0;i<randomChildren;++i)
        {
            GameObject newPerson = Instantiate(person);

            //Eyes
            Eyes eye = factory.getChildEyes(father.getEyes(), mother.getEyes());

            //Skin
            Skin skin = factory.getChildSkin(father.getSkin(), mother.getSkin(), region);

            //Hair
            Hair hair = factory.getChildHair(father.getHair(), mother.getHair(), skin);


            //Temperature
            Temperature temp = factory.getChildTemp(father.getTemperature(), mother.getTemperature());

            //Genre
            Genre genre = factory.getChildGenre(father.getGenre(), mother.getGenre());

            newPerson.GetComponent<Person>().constructPerson(
                                            0, genre, hair,
                                            eye, skin, temp,
                                            (short)40
                                            );

            region.addPerson(newPerson.GetComponent<Person>());
        }
    }

    //Devuelve una lista de las regiones existentes
    public List<Region> GetRegions() { return regions;  }

    //Devuelve el mayor numero de gente que ha habido en la historia en el mundo
    public int getBestGlobalPeopleCount()
    {
        int count = 0;
        foreach(Region r in regions)
        {
            count += r.getBestPeopleCount();
        }

        return count;
    }

    //Devuelve una región aleatoria.
    public Region getRandomRegion()
    {
        return regions[(int)UnityEngine.Random.Range(0, regions.Count)];
    }
}
