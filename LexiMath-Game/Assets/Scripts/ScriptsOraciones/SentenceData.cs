// Assets/Scripts/Questions/SentenceData.cs
using System.Collections.Generic;

[System.Serializable]
public class SentenceData
{
    public string sentenceWithBlank;  // ej: "El gato come ___"
    public string correctWord;         // ej: "pescado"
    public List<string> options;       // 4 opciones, una correcta
}