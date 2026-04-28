// Assets/Scripts/Questions/SentencePool.cs
using System.Collections.Generic;
using UnityEngine;

public static class SentencePool
{
    private static List<SentenceData> sentences = new List<SentenceData>
    {
        Make("El sol sale por la ___", "mañana", "noche", "tarde", "luna"),
        Make("Los peces viven en el ___", "agua", "cielo", "árbol", "fuego"),
        Make("La vaca da ___", "leche", "huevos", "miel", "lana"),
        Make("El perro mueve la ___", "cola", "pata", "oreja", "lengua"),
        Make("En invierno hace mucho ___", "frío", "calor", "viento", "sol"),
        Make("Las abejas producen ___", "miel", "leche", "cera", "agua"),
        Make("La gallina pone ___", "huevos", "pollos", "plumas", "nidos"),
        Make("Las plantas necesitan ___ para crecer", "agua", "fuego", "ruido", "frío"),
        Make("Los pájaros pueden ___", "volar", "nadar", "correr", "saltar"),
        Make("La luna se ve de ___", "noche", "día", "tarde", "siempre"),
        Make("El león es el rey de la ___", "selva", "casa", "ciudad", "playa"),
        Make("Usamos los ojos para ___", "ver", "oír", "oler", "tocar"),
        Make("Usamos los oídos para ___", "escuchar", "ver", "comer", "hablar"),
        Make("Los árboles tienen ___ verdes", "hojas", "piedras", "nubes", "alas"),
        Make("El fuego es muy ___", "caliente", "frío", "suave", "azul"),
        Make("La nieve es de color ___", "blanco", "rojo", "verde", "negro"),
        Make("Los niños van a la ___ a aprender", "escuela", "tienda", "playa", "cocina"),
        Make("El elefante tiene una ___ larga", "trompa", "cola", "pata", "oreja"),
        Make("Las estrellas brillan en el ___", "cielo", "mar", "suelo", "bosque"),
        Make("Para escribir usamos un ___", "lápiz", "tenedor", "zapato", "vaso")
    };
    
    private static SentenceData Make(string sentence, string correct, string w1, string w2, string w3)
    {
        SentenceData s = new SentenceData();
        s.sentenceWithBlank = sentence;
        s.correctWord = correct;
        s.options = new List<string> { correct, w1, w2, w3 };
        return s;
    }
    
    public static SentenceData GetRandomSentence()
    {
        SentenceData original = sentences[Random.Range(0, sentences.Count)];
        
        // Devolver copia con opciones barajadas
        SentenceData copy = new SentenceData();
        copy.sentenceWithBlank = original.sentenceWithBlank;
        copy.correctWord = original.correctWord;
        copy.options = new List<string>(original.options);
        Shuffle(copy.options);
        
        return copy;
    }
    
    private static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}