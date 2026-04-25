using UnityEngine;

[CreateAssetMenu(fileName = "PreguntaDataRM", menuName = "Scriptable Objects/PreguntaDataRM")]
public class PreguntaDataRM : ScriptableObject
{
    [Header("Enunciado de la pregunta")]
    public string enunciado;        // Ej: "15 - 7 = ?"

    [Header("Opciones de respuesta")]
    public int opcionA;             // Ej: 8  ← correcta
    public int opcionB;             // Ej: 6
    public int opcionC;             // Ej: 9

    [Header("Índice de la respuesta correcta")]
    [Tooltip("0 = opción A  |  1 = opción B  |  2 = opción C")]
    public int indiceCorrecta;
}
