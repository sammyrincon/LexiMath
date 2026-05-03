using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestionManager : MonoBehaviour
{
    [Header("Datos")]
    public QuestionData questionData;

    [Header("Bloques de respuesta en la escena")]
    public AnswerBlock[] bloques = new AnswerBlock[3];

    [Header("NPC y Diálogo")]
    [Tooltip("Componente NPCDialog del NPC que hace las preguntas")]
    public NPCDialog npcDialog;

    [Header("Recompensas")]
    [Tooltip("Daño al jugador cuando responde mal (HP)")]
    public float danoPorFallar = 20f;

    [Tooltip("Estrellas ganadas por pregunta correcta")]
    public int estrellasPorAcierto = 1;

    [Header("Delays")]
    public float delayFeedback = 1.5f;

    private int _preguntaActual = 0;
    private int _estrellasGanadas = 0;
    private bool _esperandoRespuesta = false;
    private bool _yaIniciado = false;

    public int EstrellasGanadas => _estrellasGanadas;
    public bool YaIniciado => _yaIniciado;

    void Start()
    {
        if (questionData == null || questionData.preguntas.Count == 0)
        {
            Debug.LogError("[QuestionManager] No hay preguntas asignadas!");
            return;
        }

        OcultarTodosLosBloques();
    }

    public void IniciarPreguntas()
    {
        if (_yaIniciado) return;
        _yaIniciado = true;

        MostrarPregunta(_preguntaActual);
    }

    private void MostrarPregunta(int indice)
    {
        if (indice >= questionData.preguntas.Count)
        {
            CompletarNivel();
            return;
        }

        Question p = questionData.preguntas[indice];

        if (npcDialog != null)
            npcDialog.MostrarDialogo(p.enunciado);

        List<string> opciones = new() { p.respuestaCorrecta };
        opciones.AddRange(p.distractores);
        Barajar(opciones);

        for (int i = 0; i < bloques.Length && i < opciones.Count; i++)
        {
            bool esCorrecta = (opciones[i] == p.respuestaCorrecta);
            bloques[i].Configurar(opciones[i], esCorrecta, this);
            bloques[i].gameObject.SetActive(true);
        }

        _esperandoRespuesta = true;
    }

    private void OcultarTodosLosBloques()
    {
        foreach (var b in bloques)
        {
            if (b != null)
                b.gameObject.SetActive(false);
        }
    }

    public void OnBloqueGolpeado(AnswerBlock bloque)
    {
        if (!_esperandoRespuesta) return;
        _esperandoRespuesta = false;

        if (bloque.esCorrecto)
            StartCoroutine(ProcesarAcierto());
        else
            StartCoroutine(ProcesarFallo());
    }

    private IEnumerator ProcesarAcierto()
    {
        if (npcDialog != null)
            npcDialog.MostrarDialogo("¡Correcto!");

        _estrellasGanadas += estrellasPorAcierto;

        yield return new WaitForSeconds(delayFeedback);

        OcultarTodosLosBloques();

        _preguntaActual++;
        MostrarPregunta(_preguntaActual);
    }

    private IEnumerator ProcesarFallo()
    {
        if (npcDialog != null)
            npcDialog.MostrarDialogo("¡Incorrecto! Inténtalo de nuevo.");

        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.RecibirDano(danoPorFallar);

        yield return new WaitForSeconds(delayFeedback);

        OcultarTodosLosBloques();
        MostrarPregunta(_preguntaActual);
    }

    private void CompletarNivel()
    {
        Debug.Log($"[QuestionManager] ¡Nivel completado! Estrellas: {_estrellasGanadas}");

        if (npcDialog != null)
            npcDialog.MostrarDialogo($"¡Completaste el nivel! Estrellas: {_estrellasGanadas}");

        OcultarTodosLosBloques();
    }

    private void Barajar<T>(List<T> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
    }
}
