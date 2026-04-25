using UnityEngine;

[CreateAssetMenu(fileName = "BancoPreguntasRM", menuName = "Scriptable Objects/BancoPreguntasRM")]
public class BancoPreguntasRM : ScriptableObject
{
    [Header("Lista de preguntas (mínimo 25 recomendado)")]
    public PreguntaDataRM[] preguntas;

    /// <summary>
    /// Regresa 15 preguntas en orden aleatorio sin repetir.
    /// Cada llamada produce un set diferente.
    /// </summary>
    public PreguntaDataRM[] ObtenerPreguntasAleatorias(int cantidad = 15)
    {
        // Validar que haya suficientes preguntas
        if (preguntas == null || preguntas.Length < cantidad)
        {
            Debug.LogError($"[BancoPreguntasRM] Se necesitan al menos {cantidad} preguntas. " +
                           $"Solo hay {preguntas?.Length ?? 0}.");
            return preguntas;
        }

        // Copia el arreglo para no modificar el original
        PreguntaDataRM[] copia = (PreguntaDataRM[])preguntas.Clone();

        // Fisher-Yates shuffle — mezcla aleatoria
        for (int i = copia.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            PreguntaDataRM temp = copia[i];
            copia[i] = copia[j];
            copia[j] = temp;
        }

        // Regresa solo la cantidad pedida
        PreguntaDataRM[] resultado = new PreguntaDataRM[cantidad];
        System.Array.Copy(copia, resultado, cantidad);
        return resultado;
    }
}
