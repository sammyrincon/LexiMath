using UnityEngine;

public enum QuestionSubject
{
    Sumas,
    Restas,
    Multiplicaciones,
    Divisiones,
    Lectoescritura
}

[CreateAssetMenu(fileName = "NuevaPregunta", menuName = "Juego Educativo/Nueva Pregunta")]
public class QuestionData : ScriptableObject
{
    public QuestionSubject subject;
    [TextArea(2, 4)] public string questionText;
    public string[] answers = new string[4];
    [Range(0, 3)] public int correctAnswerIndex;
    [TextArea(2, 4)] public string helpText;
}


