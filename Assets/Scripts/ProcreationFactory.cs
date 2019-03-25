using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcreationFactory : MonoBehaviour
{
    //Scriptable Objects de genetica
    [SerializeField]
    private Genetic eyesGenetic = null;
    [SerializeField]
    private Genetic hairGenetic = null;
    [SerializeField]
    private Genetic skinGenetic = null;
    [SerializeField]
    private Genetic tempGenetic = null;

    [SerializeField][Range(0,100)]
    private float hairMutationPercent = 10; //Porcentaje de mutacion del pelo respecto a la piel

    //Typos de cromosomas
    short eyesTypes;
    short hairTypes;
    short skinTypes;
    short tempTypes;

    private void Start()
    {
        //Inicializacion de variables
        eyesTypes = (short)eyesGenetic.typeName.Length;
        hairTypes = (short)hairGenetic.typeName.Length;
        skinTypes = (short)skinGenetic.typeName.Length;
        tempTypes = (short)tempGenetic.typeName.Length;
    }

    public Eyes getChildEyes(Eyes parent1, Eyes parent2)
    {
        Eyes eyes = 0;
        //Valor aleatorio para la genetica
        short value = (short)Random.Range(0, 101);
        byte comb = 0;
        float amount = 0;

        //Comprueba en el scriptableObject, cual es la combinación
        //actual de los padres, y toma su indice
        for(byte i = 0;i<eyesGenetic.combinations.Length;++i)
        {
            if((eyesGenetic.combinations[i].parent1 == parent1.ToString() &&
                eyesGenetic.combinations[i].parent2 == parent2.ToString()) ||
                (eyesGenetic.combinations[i].parent1 == parent2.ToString() &&
                eyesGenetic.combinations[i].parent2 == parent1.ToString()))
            {
                comb = i;
                break;
            }
        }

        //Segun los porcentajes, se comprueba que resultado ha dado el aleatorio
        for(byte i = 0;i < eyesTypes;++i)
        {
            amount += eyesGenetic.combinations[comb].percents[i];
            if(value < amount)
            {
                eyes = (Eyes)i;
                break;
            }
        }

        return eyes;
    }

    public Hair getChildHair(Hair parent1, Hair parent2, Skin childSkin)
    {
        Hair hair = 0;
        //Valor aleatorio para la genetica
        short value = (short)Random.Range(0, 101);
        byte comb = 0;
        float amount = 0;

        //Comprueba en el scriptableObject, cual es la combinación
        //actual de los padres, y toma su indice
        for (byte i = 0; i < hairGenetic.combinations.Length; ++i)
        {
            if ((hairGenetic.combinations[i].parent1 == parent1.ToString() &&
                hairGenetic.combinations[i].parent2 == parent2.ToString()) ||
                (hairGenetic.combinations[i].parent1 == parent2.ToString() &&
                hairGenetic.combinations[i].parent2 == parent1.ToString()))
            {
                comb = i;
                break;
            }
        }
        //Segun los porcentajes, se comprueba que resultado ha dado el aleatorio
        for (byte i = 0; i < hairTypes; ++i)
        {
            amount += hairGenetic.combinations[comb].percents[i];
            if (value < amount)
            {
                hair = (Hair)i;
                break;
            }
        }
        //Se permite una mutación respecto al color de piel
        hair = mutateHair(hair, childSkin);

        return hair;
    }

    //Mutacion del pelo respecto a la piel
    private Hair mutateHair(Hair currentHair, Skin childSkin)
    {
        //Segun el tipo, si un porcentaje aleatorio es mejor a la probabilidad de mutacion del pelo,
        // este va a modificarse

        //Los ASIATICOS tieden a color negro
        //Los NEGROS tienden a negro o castaño
        switch(childSkin)
        {
            case Skin.ASIATIC:
                if(currentHair == Hair.BLOND && currentHair == Hair.REDHEAD && currentHair == Hair.BROWN)
                {
                    int random = Random.Range(0, 101);
                    if (random <= hairMutationPercent)
                        currentHair = Hair.DARK;
                }
                break;
            case Skin.BLACK:
                if (currentHair == Hair.BLOND && currentHair == Hair.REDHEAD)
                {
                    int random = Random.Range(0, 101);
                    if(random <= hairMutationPercent)
                    {
                        random = Random.Range(0, 101);
                        if (random < 35)
                            currentHair = Hair.DARK;
                        else
                            currentHair = Hair.BROWN;
                    }
                }
                break;
        }

        return currentHair;
    }

    public Skin getChildSkin(Skin parent1, Skin parent2, Region region)
    {
        Skin skin = 0;
        //Valor aleatorio para la genetica
        short value = (short)Random.Range(0, 101);
        byte comb = 0;
        float amount = 0;

        //Comprueba en el scriptableObject, cual es la combinación
        //actual de los padres, y toma su indice
        for (byte i = 0; i < skinGenetic.combinations.Length; ++i)
        {
            if ((skinGenetic.combinations[i].parent1 == parent1.ToString() &&
                skinGenetic.combinations[i].parent2 == parent2.ToString()) ||
                (skinGenetic.combinations[i].parent1 == parent2.ToString() &&
                skinGenetic.combinations[i].parent2 == parent1.ToString()))
            {
                comb = i;
                break;
            }
        }

        //Probable mutacion segun la temperatura de la region
        for (byte i = 0; i < skinTypes; ++i)
        {
            if (i == 0 && region.getTemperature() == Temperature.HIGH)
                amount += region.getAffectChildren();
            else if (i == 1 && region.getTemperature() == Temperature.MEDIUM)
                amount += region.getAffectChildren();
            else if (i == 2 && region.getTemperature() == Temperature.LOW)
                amount += region.getAffectChildren();

            amount += skinGenetic.combinations[comb].percents[i];
            if (value < amount)
            {
                skin = (Skin)i;
                break;
            }
        }

        return skin;
    }

    public Temperature getChildTemp(Temperature parent1, Temperature parent2)
    {
        Temperature temp = 0;
        //Valor aleatorio de genetica
        short value = (short)Random.Range(0, 101);
        byte comb = 0;
        float amount = 0;

        //Comprueba en el scriptableObject, cual es la combinación
        //actual de los padres, y toma su indice
        for (byte i = 0; i < tempGenetic.combinations.Length; ++i)
        {
            if ((tempGenetic.combinations[i].parent1 == parent1.ToString() &&
                tempGenetic.combinations[i].parent2 == parent2.ToString()) ||
                (tempGenetic.combinations[i].parent1 == parent2.ToString() &&
                tempGenetic.combinations[i].parent2 == parent1.ToString()))
            {
                comb = i;
                break;
            }
        }

        //Segun los porcentajes, se comprueba que resultado ha dado el aleatorio
        for (byte i = 0; i < tempTypes; ++i)
        {
            amount += tempGenetic.combinations[comb].percents[i];
            if (value < amount)
            {
                temp = (Temperature)i;
                break;
            }
        }

        return temp;
    }

    public Genre getChildGenre(Genre parent1, Genre parent2)
    {
        //El genero es aleatorio
        return (Genre)Random.Range(0, 2);
    }

}
