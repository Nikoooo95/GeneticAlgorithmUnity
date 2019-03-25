using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    Region ownRegion; //Region actual en la que habita
    private int agesOnRegion; //Años viviendo en la region

    private short age; //Edad de la persona
    private int procreationAgeMin = 16; //Edad minima de procreacion
    private int procreationAgeMax = 65; //Edad máxima de procreacion
    private bool canProcreate; //Indica si puede procrear
    private bool hasProcreate; //Indica si ha procreado

    //Cromosomas
    private Genre genre;
    Hair hair;
    Eyes eyes;
    Skin skin;
    Temperature temp;

    //Colores de los cromosomas
    private Color[] hairColors = { new Color(0.9f, 0.7f, 0.3f), new Color(0.4f,0.3f,0.2f), new Color(0.85f, 0.4f, 0.17f), new Color(0.06f, 0.03f, 0.02f)};
    private Color[] eyesColors = { new Color(0f, 0.5f, 0.8f), new Color(0.2f, 0.6f, 0.2f), new Color(0.4f, 0.3f, 0.2f) };
    public Gradient[] gradientSkin;

    private short happiness; //Felicidad

    private Vector3 newPos;//Posicion a la que se mueve

    [SerializeField]
    private float speed = 1.0f; //Velocidad de movimiento
    
    
    void Start()
    {
        //Inicializacion de variables
        canProcreate = false;
        hasProcreate = false;
        //Toma una posicion aleatoria de la region
        newPos = ownRegion.randomLocation();
    }
    
    void Update()
    {
        //Movimiento
        if (!nearTo(newPos))
            moveTo();
        else
            newPos = ownRegion.randomLocation();
    }

    //Constructor
    public void constructPerson(short _age, Genre _genre, 
                                Hair _hair, Eyes _eyes, 
                                Skin _skin, Temperature _temp, 
                                short _happiness, Region region = null)
    {
        age = _age;
        genre = _genre;
        hair = _hair;
        eyes = _eyes;
        skin = _skin;
        temp = _temp;
        happiness = _happiness;
        ownRegion = region;
        agesOnRegion = 0;

        //body(skin)
        MeshRenderer renderer = transform.Find("Body").GetComponent<MeshRenderer>();
        renderer.material.color = gradientSkin[(byte)skin].Evaluate(age/100);
        //Eyes
        renderer = transform.Find("Eyes").transform.Find("Eye_R").GetComponent<MeshRenderer>();
        renderer.material.color = eyesColors[(byte)eyes];
        renderer = transform.Find("Eyes").transform.Find("Eye_L").GetComponent<MeshRenderer>();
        renderer.material.color = eyesColors[(byte)eyes];
        //Hair
        if(genre == (Genre)0)
        {
            renderer = transform.Find("Hair").transform.Find("Hair_M").GetComponent<MeshRenderer>();
            renderer.material.color = hairColors[(byte)hair];

            transform.Find("Hair").transform.Find("Hair_W").gameObject.SetActive(false);
        }
        else if(genre == (Genre)1)
        {
            renderer = transform.Find("Hair").transform.Find("Hair_W").GetComponent<MeshRenderer>();
            renderer.material.color = hairColors[(byte)hair];

            transform.Find("Hair").transform.Find("Hair_M").gameObject.SetActive(false);
        }
    }

    //Envejecimiento por 1 año
    public void getOlder()
    {
        age++;
        
        //Calculo del color de la piel segun la edad
        MeshRenderer renderer = transform.Find("Body").GetComponent<MeshRenderer>();
        renderer.material.color = gradientSkin[(byte)skin].Evaluate(age / 100);

        ++agesOnRegion;

        //Calculo anual de felicidad
        calculateHappiness();
        //Comprobacion de procreacion
        tryProcreate();
    }

    //Calculo de felicidad
    private void calculateHappiness()
    {
        //TEMPERATURA
        if (ownRegion.getTemperature() == temp)
            happiness += 2;
        else if (Mathf.Abs(ownRegion.getTemperature() - temp) == 1)
            --happiness;
        else
            happiness -= 4;

        //CAPACIDAD DE GENTE
        if (ownRegion.capacityCompleted())
            happiness -= 2;

        //SEXO OPUESTO
        if (ownRegion.getGenreTrending() != genre)
            ++happiness;

        //AÑOS EN LA REGION
        if (agesOnRegion > 30)
            happiness += 3;
        else if (agesOnRegion > 20)
            happiness += 2;
        else if (agesOnRegion > 10)
            ++happiness;

        //RACISMO
        if (ownRegion.getSkinTrending() == skin)
            ++happiness;
        else
            --happiness;

        //NIVELACION DE EXTREMOS
        if (happiness > 100)
            happiness = 100;
        else if (happiness < 0)
            happiness = 0;

        //MUDARSE EN CASO DE FELICIDAD BAJA
        if (happiness < 25 && agesOnRegion > 7)
            wantToMove();

    }

    //Comprobación de procreacion
    private void tryProcreate()
    {
        //Si no tiene la edad o la felicidad adecuadas, return
        if (age < procreationAgeMin || age > procreationAgeMax 
            || happiness < 65)
        {
            if (canProcreate) canProcreate = false;
            return;
        }
        
        if (canProcreate || hasProcreate)
            return;

        canProcreate = true;

        //Posibilidad de procreacion
        int stat = Random.Range(0, 101);
        if(stat <= GameController.instance.procreateStat)
        {
            //Se añade a la lista de solteros de la region
            if(happiness > 49)
            {
                ownRegion.addToSingleList(this);
            }
        }
    }

    //Movimiento a la siquiente posicion
    private void moveTo()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, newPos, step);
        transform.LookAt(newPos);
    }

    //Comprueba si la posicion actual es cercana a la posicion target
    private bool nearTo(Vector3 pos)
    {
        return Vector3.Distance(transform.position, newPos) < 0.5f; 
    }
    
    //MUDARSE
    private void wantToMove()
    {

        Region newRegion;
        do
        {
            newRegion = GameController.instance.getRandomRegion();

        } while (newRegion == ownRegion);

        ownRegion.removePerson(this);
        newRegion.addPerson(this);
        newPos = this.transform.position;
    }

    //GETTERS---------------------------------
    public short getAge() { return age; }
    public Genre getGenre() { return genre; }
    public Hair getHair() { return hair; }
    public Eyes getEyes() { return eyes; }
    public Skin getSkin() { return skin; }
    public Temperature getTemperature() { return temp; }
    public short getHappiness() { return happiness; }
    public Region getRegion() { return ownRegion; }


    //SETTERS---------------------------------
    public void setCanProcreate(bool b) { canProcreate = b; }
    public void setRegion(Region r)
    {
        if(r != ownRegion)
        {
            ownRegion = r;
            agesOnRegion = 0;
        }

    }
    public void setHasProcreate(bool b) { hasProcreate = b; }
}

