using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// QuestionData — LexiMath
/// 
/// ScriptableObject con la lista de preguntas del nivel.
/// Crea uno con: Clic derecho en Project → Create → LexiMath → Question Data
/// </summary>
[CreateAssetMenu(fileName = "QuestionData", menuName = "LexiMath/Question Data")]
public class QuestionData : ScriptableObject
{
    public string nombreNivel = "Nivel 1";
    public List<Question> preguntas = new();
}

[System.Serializable]
public class Question
{
    [TextArea]
    public string enunciado;

    [Tooltip("Respuesta correcta")]
    public string respuestaCorrecta;

    [Tooltip("Dos respuestas incorrectas (distractores)")]
    public string[] distractores = new string[2];
}
