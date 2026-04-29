using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// QuestionManager — LexiMath
/// 
/// Controla toda la mecánica de preguntas:
///   • Espera a que un NPCTrigger lo active con IniciarPreguntas()
///   • Asigna valores a los 3 AnswerBlocks de la escena
///   • Muestra la pregunta en el NPC (caja de diálogo)
///   • Al acertar: siguiente pregunta + estrella
///   • Al fallar: pierde vida + repite misma pregunta
/// 
/// SETUP EN UNITY:
///   1. Crea GameObject "QuestionManager" en la escena
///   2. Asigna este script
///   3. En el Inspector:
///        • Question Data → el ScriptableObject de preguntas
///        • Bloques → arrastra los 3 AnswerBlocks de la escena
///        • NPC Dialog → el componente NPCDialog del NPC
/// </summary>
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

    // ── Estado ────────────────────────────────────────────────
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

        // IMPORTANTE: Los bloques empiezan OCULTOS
        // Solo aparecen cuando el NPC activa las preguntas
        OcultarTodosLosBloques();
    }

    // ══════════════════════════════════════════════════════════
    //  API PÚBLICA — llamada por NPCTrigger
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// Llamado por el NPCTrigger cuando el jugador se acerca.
    /// Arranca la primera pregunta.
    /// </summary>
    public void IniciarPreguntas()
    {
        if (_yaIniciado) return; // Evitar reinicios accidentales
        _yaIniciado = true;

        MostrarPregunta(_preguntaActual);
    }

    // ══════════════════════════════════════════════════════════
    //  MOSTRAR PREGUNTA
    // ══════════════════════════════════════════════════════════

    private void MostrarPregunta(int indice)
    {
        if (indice >= questionData.preguntas.Count)
        {
            CompletarNivel();
            return;
        }

        Question p = questionData.preguntas[indice];

        // Mostrar el enunciado en la caja de diálogo del NPC
        if (npcDialog != null)
            npcDialog.MostrarDialogo(p.enunciado);

        // Preparar las 3 opciones (correcta + 2 distractores) y barajar
        List<string> opciones = new() { p.respuestaCorrecta };
        opciones.AddRange(p.distractores);
        Barajar(opciones);

        // Asignar a cada bloque y ACTIVARLOS
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

    // ══════════════════════════════════════════════════════════
    //  API PÚBLICA — llamada por AnswerBlock al ser golpeado
    // ══════════════════════════════════════════════════════════

    public void OnBloqueGolpeado(AnswerBlock bloque)
    {
        if (!_esperandoRespuesta) return;
        _esperandoRespuesta = false;

        if (bloque.esCorrecto)
            StartCoroutine(ProcesarAcierto());
        else
            StartCoroutine(ProcesarFallo());
    }

    // ══════════════════════════════════════════════════════════
    //  ACIERTO — siguiente pregunta + estrella
    // ══════════════════════════════════════════════════════════

    private IEnumerator ProcesarAcierto()
    {
        // Feedback en el NPC
        if (npcDialog != null)
            npcDialog.MostrarDialogo("¡Correcto! 🌟");

        // Sumar estrellas
        _estrellasGanadas += estrellasPorAcierto;

        // Esperar para que el jugador vea el feedback
        yield return new WaitForSeconds(delayFeedback);

        // Ocultar bloques viejos antes de mostrar los nuevos
        OcultarTodosLosBloques();

        // Siguiente pregunta
        _preguntaActual++;
        MostrarPregunta(_preguntaActual);
    }

    // ══════════════════════════════════════════════════════════
    //  FALLO — pierde vida + misma pregunta
    // ══════════════════════════════════════════════════════════

    private IEnumerator ProcesarFallo()
    {
        // Feedback
        if (npcDialog != null)
            npcDialog.MostrarDialogo("¡Incorrecto! Inténtalo de nuevo.");

        // Hacer daño al jugador (si existe PlayerHealth en la escena)
        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.RecibirDano(danoPorFallar);

        yield return new WaitForSeconds(delayFeedback);

        // Ocultar bloques y volver a mostrar la misma pregunta
        OcultarTodosLosBloques();
        MostrarPregunta(_preguntaActual);
    }

    // ══════════════════════════════════════════════════════════
    //  NIVEL COMPLETADO
    // ══════════════════════════════════════════════════════════

    private void CompletarNivel()
    {
        Debug.Log($"[QuestionManager] ¡Nivel completado! Estrellas: {_estrellasGanadas}");

        if (npcDialog != null)
            npcDialog.MostrarDialogo($"¡Completaste el nivel! Estrellas: {_estrellasGanadas}");

        OcultarTodosLosBloques();

        // TODO: Aquí puedes conectar al panel de victoria del HUD si lo quieres
    }

    // ══════════════════════════════════════════════════════════
    //  HELPER
    // ══════════════════════════════════════════════════════════

    private void Barajar<T>(List<T> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
    }
}
