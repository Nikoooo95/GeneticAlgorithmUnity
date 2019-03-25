using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptableObject que contiene los valores de genetica de cualquier cromosoma
[CreateAssetMenu(fileName = "Cromosome", menuName = "Scriptable/Genetic", order = 1)]
public class Genetic : ScriptableObject, ISerializationCallbackReceiver
{
    public int types = 3; //Cantidad de tipos del mismo cromosoma
    private int oldSide = 3; //Ultima cantidad

    public bool saved = true; //Guardado del scriptableObject (Se debe mantener a true para que no se pierdan datos)
    public bool stringsAdded = false; //Poner a true cuando se modifiquen los nombres


    public string[] typeName; //Nombres de los tipos 

    public MyArray[] combinations; //Array de combinaciones

    public void OnAfterDeserialize()
    {
        //Cuando se ha modificado algun valor, se vuelve a crear el array de combinaciones
        if (oldSide != types && !saved)
        {
            oldSide = types;
            combinations = new MyArray[types * (types + 1) / 2];
            for (int i = 0; i < combinations.Length; ++i)
            {
                combinations[i] = new MyArray(types);
            }
            typeName = new string[types];
        }

        //Cuando se añaden los nombres de los tipos, automaticamente se crean
        //las combinaciones automaticamente
        if (stringsAdded)
        {
            int index = 0;
            for (int i = 0; i < typeName.Length; ++i)
            {
                for (int j = i; j < typeName.Length; ++j)
                {

                    combinations[index].parent1 = typeName[i];
                    combinations[index].parent2 = typeName[j];
                    combinations[index].name = combinations[index].parent1 + " y " + combinations[index].parent2;
                    index++;
                }
            }
        }

    }

    //Heredado de la interfaz
    public void OnBeforeSerialize()
    {
       // nothing
    }

}

//Clase de guardado de combinaciones
[System.Serializable]
public class MyArray
{
    public string name; //Nombre de la combinacion
    public string parent1, parent2; //Nombre de los padres
    public float[] percents; //Porcentajes de probabilidad de cada tipo


    public MyArray(int size)
    {
        percents = new float[size];
    }
}

