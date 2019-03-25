using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour
{
    int bestPeopleCount; //Mayor cantidad de personas en la historia
    int peopleCount; //Cantidad de personas actuales
    List<Person> people; //Lista de personas de la region

    int capacity; //Capacidad maxima deseada

    int emigrantsCount; //Numero de emigraciones
    int immigrantsCount; //Numero de inmigraciones

    List<Person> singlesMen; //Lista de hombres solteros
    List<Person> singlesWomen; //Lista de mujeres solteras

    Temperature temp; //Temperatura de la region

    //Base de datos para las gráficas
    #region HistoricData
    List<int> blondeHairHistoric;
    List<int> brownHairHistoric;
    List<int> redheadHairHistoric;
    List<int> darkHairHistoric;

    List<int> blackSkinHistoric;
    List<int> whiteSkinHistoric;
    List<int> asiaticSkinHistoric;

    List<int> brownEyesHistoric;
    List<int> greenEyesHistoric;
    List<int> blueEyesHistoric;

    List<int> hightTemperatureHistoric;
    List<int> mediumTemperatureHistoric;
    List<int> lowTemperatureHistoric;

    List<int> happinessHistoric;

    List<int> moveInHistoric;
    List<int> moveOutHistoric;

    List<int> totalPeopleHistoric;
    List<int> mensHistoric;
    List<int> womensHistoric;

    #endregion
    
    //Colores segun la temperatura
    private Color[] colors =
    {
        new Color(0.83f, 0.73f, 0.0f),
        new Color(0.25f, 0.83f, 0.35f),
        new Color(0.64f, 1f, 0.96f)
    };

    //Porcentaje de que la temperatura de la Region afecte a los niños que nacen en ella
    float affectChildren = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        //Inicializacion de variables
        people = new List<Person>();

        singlesMen = new List<Person>();
        singlesWomen = new List<Person>();

        bestPeopleCount = 0;
        peopleCount = 0;

        emigrantsCount = 0;
        immigrantsCount = 0;

        blondeHairHistoric = new List<int>();
        brownHairHistoric = new List<int>();
        redheadHairHistoric = new List<int>();
        darkHairHistoric = new List<int>();

        blackSkinHistoric = new List<int>();
        whiteSkinHistoric = new List<int>();
        asiaticSkinHistoric = new List<int>();

        brownEyesHistoric = new List<int>();
        greenEyesHistoric = new List<int>();
        blueEyesHistoric = new List<int>();

        hightTemperatureHistoric = new List<int>();
        mediumTemperatureHistoric = new List<int>();
        lowTemperatureHistoric = new List<int>();

        happinessHistoric = new List<int>();

        moveInHistoric = new List<int>();
        moveOutHistoric = new List<int>();

        totalPeopleHistoric = new List<int>();
        mensHistoric = new List<int>();
        womensHistoric = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {
        procreate();
    }

    //Limpia la lista de personas actuales y añade la lista recibida
    public void setPeople(List<Person> newPeople)
    {
        people.Clear();
        people = newPeople;
        foreach(Person temp in people)
        {
            temp.gameObject.transform.position = randomLocation();
            temp.setRegion(this);
            ++peopleCount;
        }

        if (peopleCount > bestPeopleCount)
            bestPeopleCount = peopleCount;

    }

    //Añade una persona a la region
    public void addPerson(Person newPerson)
    {
        if (newPerson.getRegion() != null && newPerson.getRegion() != this)
            ++immigrantsCount;

        newPerson.setRegion(this);
        newPerson.gameObject.transform.position = randomLocation();
        people.Add(newPerson);
        ++peopleCount;

        if (peopleCount > bestPeopleCount)
            bestPeopleCount = peopleCount;

    }

    //Quita una persona de la region
    public void removePerson(Person p)
    {
        people.Remove(p);
        removeFromSingleList(p);
        --peopleCount;
        ++emigrantsCount;
    }

    public void setCapacity(int _capacity)
    {
        capacity = _capacity;
    }

    //Indica si la capacidad deseada se ha superado
    public bool capacityCompleted()
    {
        return capacity < peopleCount;
    }

    //Ajusta la temperatura de la región y modifica el color de esta
    public void setTemperature(Temperature newTemp)
    {
        temp = newTemp;
        switch (temp)
        {
            case Temperature.HIGH:
                GetComponent<MeshRenderer>().material.color = colors[0];
                break;
            case Temperature.MEDIUM:
                GetComponent<MeshRenderer>().material.color = colors[1];
                break;
            case Temperature.LOW:
                GetComponent<MeshRenderer>().material.color = colors[2];
                break;
        }
    }

    public void setAffectChildren(float percentage)
    {
        affectChildren = percentage;
    }

    //Añade a una persona a la lista de solteros correspondiente
    public void addToSingleList(Person person)
    {
        if (person.getGenre() == Genre.WOMAN)
        {
            singlesWomen.Add(person);
        }
        else if(person.getGenre() == Genre.MAN)
        {
            singlesMen.Add(person);
        }
    }

    //Quita a una persona de la lista de solteros correspondiente
    public void removeFromSingleList(Person person)
    {
        if (person.getGenre() == Genre.WOMAN)
        {
            singlesWomen.Remove(person);
        }
        else if (person.getGenre() == Genre.MAN)
        {
            singlesMen.Remove(person);
        }
    }

    //Procreacion de la region
    private void procreate()
    {
        if (singlesMen.Count > 0 && singlesWomen.Count > 0)
        {
            GameController.instance.procreate(this, singlesMen[0], singlesWomen[0]);//Llama al GameController para que cree un nuevo individuo
            singlesMen[0].setHasProcreate(false);
            singlesWomen[0].setHasProcreate(false);
            
            singlesMen.RemoveAt(0);
            singlesWomen.RemoveAt(0);
        }
    }

    //Devuelve una posicion aleatoria en la region
    public Vector3 randomLocation()
    {
        return new Vector3(
           Random.Range(this.transform.position.x - 5f, this.transform.position.x + 5f),
            0.0f,
            Random.Range(this.transform.position.z - 5f, this.transform.position.z + 5f)
            );
    }

    public Region moveToNewRegion()
    {
        return GameController.instance.getRandomRegion();
    }

    //GETTERS*---------------------------------------------------------------------------

    //Envejece a toda la poblacion
    public void getOlderPeople()
    {
        for (int i = people.Count-1; i > 0; --i)
        {
            people[i].getOlder();
            //Si la persona tiene mas de 100 años, muere
            if (people[i].getAge() >= 100)
            {
                Person p = people[i];
                removePerson(p);
                Destroy(p.gameObject);
            }
        }

        saveData(); //Guardado de datos
    }

    //GETTERS HAIR
    public int getBlondCount()
    {
        int count = 0;
        foreach(Person p in people)
        {
            if (p.getHair() == Hair.BLOND)
                count++;
        }

        return count;
    }
    public int getRedheadCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getHair() == Hair.REDHEAD)
                count++;
        }

        return count;
    }
    public int getDarkCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getHair() == Hair.DARK)
                count++;
        }

        return count;
    }
    public int getBrownCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getHair() == Hair.BROWN)
                count++;
        }

        return count;
    }

    //GETTERS EYES
    public int getBlueEyesCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getEyes() == Eyes.BLUE)
                count++;
        }

        return count;
    }
    public int getGreenEyesCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getEyes() == Eyes.GREEN)
                count++;
        }

        return count;
    }
    public int getBrownEyesCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getEyes() == Eyes.BROWN)
                count++;
        }

        return count;
    }

    //GETTERS EYES TRENDING
    public Eyes getEyesTrending()
    {
        Eyes eyes = 0;
        int best = 0;

        if (getBlueEyesCount() > best)
        {
            eyes = Eyes.BLUE;
            best = getBlueEyesCount();
        }
        if (getBrownEyesCount() > best)
        {
            eyes = Eyes.BROWN;
            best = getBrownEyesCount();
        }

        if (getGreenEyesCount() > best)
        {
            eyes = Eyes.GREEN;
            best = getGreenEyesCount();
        }

        
        return eyes;
    }
    public Hair getHairTrending()
    {
        Hair hair = 0;
        int best = 0;

        if (getBlondCount() > best)
        {
            hair = Hair.BLOND;
            best = getBlondCount();
        }

        if (getBrownCount() > best)
        {
            hair = Hair.BROWN;
            best = getBrownCount();
        }

        if (getRedheadCount() > best)
        {
            hair = Hair.REDHEAD;
            best = getRedheadCount();

        }
        if (getDarkCount() > best)
        {
            hair = Hair.DARK;
            best = getDarkCount();

        }
        return hair;
    }
    public Skin getSkinTrending()
    {
        Skin skin = 0;
        int best = 0;

        if (getBlackPeopleCount() > best)
        {
            skin = Skin.BLACK;
            best = getBlackPeopleCount();
        }

        if (getWhitePeopleCount() > best)
        {
            skin = Skin.WHITE;
            best = getWhitePeopleCount();


        }
        if (getAsiaticPeopleCount() > best)
        {
            skin = Skin.ASIATIC;
            best = getAsiaticPeopleCount();


        }

        return skin;
    }
    public Genre getGenreTrending()
    {
        if (getWomenCount() > getMenCount())
            return Genre.WOMAN;
        else
            return Genre.MAN;
    }

    public int getTrendyHappiness()
    {
        if (peopleCount == 0)
            return 0;

        int count = 0;
        foreach (Person p in people)
        {
            count += p.getHappiness();
        }
        count /= peopleCount;

        return count;
    }

    public int getBlackPeopleCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getSkin() == Skin.BLACK)
                count++;
        }

        return count;
    }
    public int getWhitePeopleCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getSkin() == Skin.WHITE)
                count++;
        }

        return count;
    }
    public int getAsiaticPeopleCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getSkin() == Skin.ASIATIC)
                count++;
        }

        return count;
    }

    public Temperature getTemperature()
    {
        return temp;
    }

    public int getHighTempCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getTemperature() == Temperature.HIGH)
                count++;
        }

        return count;
    }
    public int getMediumTempCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getTemperature() == Temperature.MEDIUM)
                count++;
        }

        return count;
    }
    public int getLowTempCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getTemperature() == Temperature.LOW)
                count++;
        }

        return count;
    }

    public int getPeopleCount()
    {
        //return people.Count;
        return peopleCount;
    }
    public int getWomenCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getGenre() == (Genre)1)
            {
                ++count;
            }
        }

        return count;
    }
    public int getMenCount()
    {
        int count = 0;
        foreach (Person p in people)
        {
            if (p.getGenre() == (Genre)0)
            {
                ++count;
            }
        }

        return count;
    }
    public int getBestPeopleCount()
    {
        return bestPeopleCount;
    }

    public int getMaxPeople()
    {
        return bestPeopleCount;
    }
    public int getCapacity() { return capacity; }
    public int getEmigrantsCount() { return emigrantsCount; }
    public int getImmigrantsCount() { return immigrantsCount; }


    public float getAffectChildren()
    {
        return affectChildren;
    }


    void saveData()
    {
        CheckHistoricValues();

        blondeHairHistoric.Add(getBlondCount());
        brownHairHistoric.Add(getBrownCount());
        redheadHairHistoric.Add(getRedheadCount());
        darkHairHistoric.Add(getDarkCount());

        blackSkinHistoric.Add(getBlackPeopleCount());
        whiteSkinHistoric.Add(getWhitePeopleCount());
        asiaticSkinHistoric.Add(getAsiaticPeopleCount());

        brownEyesHistoric.Add(getBrownEyesCount());
        greenEyesHistoric.Add(getGreenEyesCount());
        blueEyesHistoric.Add(getBlueEyesCount());

        hightTemperatureHistoric.Add(getHighTempCount());
        mediumTemperatureHistoric.Add(getMediumTempCount());
        lowTemperatureHistoric.Add(getLowTempCount());

        happinessHistoric.Add(getTrendyHappiness());

        moveInHistoric.Add(getImmigrantsCount());
        moveOutHistoric.Add(getEmigrantsCount());

        totalPeopleHistoric.Add(getPeopleCount());
        mensHistoric.Add(getMenCount());
        womensHistoric.Add(getWomenCount());
    }

    void CheckHistoricValues()
    {
        if (blondeHairHistoric.Count > 100)
        {
            blondeHairHistoric.RemoveAt(0);
            brownHairHistoric.RemoveAt(0);
            redheadHairHistoric.RemoveAt(0);
            darkHairHistoric.RemoveAt(0);
            blackSkinHistoric.RemoveAt(0);
            whiteSkinHistoric.RemoveAt(0);
            asiaticSkinHistoric.RemoveAt(0);
            brownEyesHistoric.RemoveAt(0);
            greenEyesHistoric.RemoveAt(0);
            blueEyesHistoric.RemoveAt(0);
            hightTemperatureHistoric.RemoveAt(0);
            mediumTemperatureHistoric.RemoveAt(0);
            lowTemperatureHistoric.RemoveAt(0);
            happinessHistoric.RemoveAt(0);
            moveInHistoric.RemoveAt(0);
            moveOutHistoric.RemoveAt(0);
            totalPeopleHistoric.RemoveAt(0);
            mensHistoric.RemoveAt(0);
            womensHistoric.RemoveAt(0);
        }
    }


    //HISTORIC GETTERS
    public List<int> getBlondeHairHistoric() { return blondeHairHistoric; }
    public List<int> getBrownHairHistoric() { return brownHairHistoric; }
    public List<int> getReadheadHairHistoric() { return redheadHairHistoric; }
    public List<int> getDarkHairHistoric() { return darkHairHistoric; }

    public List<int> getBlackSkinHistoric() { return blackSkinHistoric; }
    public List<int> getWhiteSkinHistoric() { return whiteSkinHistoric; }
    public List<int> getAsiaticSkinHistoric() { return asiaticSkinHistoric; }

    public List<int> getBrownEyesHistoric() { return brownEyesHistoric; }
    public List<int> getGreenEyesHistoric() { return greenEyesHistoric; }
    public List<int> getBlueEyesHistoric() { return blueEyesHistoric; }

    public List<int> getHighTemperatureHistoric() { return hightTemperatureHistoric; }
    public List<int> getMediumTemperatureHistoric() { return mediumTemperatureHistoric; }
    public List<int> getLowTemperatureHistoric() { return lowTemperatureHistoric; }

    public List<int> getHappinessHistoric() { return happinessHistoric; }

    public List<int> getMoveInHistoric() { return moveInHistoric; }
    public List<int> getMoveOutHistoric() { return moveOutHistoric; }

    public List<int> getTotalPeopleHistoric() { return totalPeopleHistoric; }
    public List<int> getMensHistoric() { return mensHistoric; }
    public List<int> getWomensHistoric() { return womensHistoric; }

}
