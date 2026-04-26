using System.Collections.Generic;
using UnityEngine;

public class QuestionBlock : MonoBehaviour
{
    public List<QuestionSubject> temasPermitidos;
    public List<QuestionData> preguntasLectoescritura;

    private bool isUsed = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isUsed && collision.gameObject.CompareTag("Player"))
        {
            if (collision.contacts[0].normal.y > 0.5f) 
            {
                TriggerQuestion();
            }
        }
    }

    private void TriggerQuestion()
    {
        if (temasPermitidos.Count == 0) return;

        isUsed = true;
        if (spriteRenderer != null) spriteRenderer.color = Color.gray;

        QuestionSubject temaElegido = temasPermitidos[Random.Range(0, temasPermitidos.Count)];
        QuestionData preguntaAMostrar = null;

        if (temaElegido == QuestionSubject.Lectoescritura)
        {
            if (preguntasLectoescritura.Count > 0)
            {
                preguntaAMostrar = preguntasLectoescritura[Random.Range(0, preguntasLectoescritura.Count)];
            }
        }
        else
        {
            preguntaAMostrar = GenerarPreguntaMatematica(temaElegido);
        }

        if (preguntaAMostrar != null)
        {
            UIManager2.Instance.ShowQuestion(preguntaAMostrar);
        }
    }

    private QuestionData GenerarPreguntaMatematica(QuestionSubject operacion)
    {
        QuestionData q = ScriptableObject.CreateInstance<QuestionData>();
        q.subject = operacion;

        int a = 0, b = 0, respuestaCorrecta = 0;
        string signo = "";

        switch (operacion)
        {
            case QuestionSubject.Sumas:
                a = Random.Range(1, 21);
                b = Random.Range(1, 21);
                respuestaCorrecta = a + b;
                signo = "+";
                q.helpText = $"Imagina que tienes {a} manzanas y Mael te regala {b}. ¡Cuéntalas todas juntas!";
                break;

            case QuestionSubject.Restas:
                a = Random.Range(10, 30);
                b = Random.Range(1, a);
                respuestaCorrecta = a - b;
                signo = "-";
                q.helpText = $"Tienes {a} monedas y gastas {b} en la tienda mágica. ¿Cuántas te quedan?";
                break;

            case QuestionSubject.Multiplicaciones:
                a = Random.Range(2, 11);
                b = Random.Range(2, 11);
                respuestaCorrecta = a * b;
                signo = "x";
                q.helpText = $"Es como sumar el número {a} varias veces. Exactamente {b} veces.";
                break;

            case QuestionSubject.Divisiones:
                b = Random.Range(2, 11);
                respuestaCorrecta = Random.Range(2, 11);
                a = b * respuestaCorrecta;
                signo = "÷";
                q.helpText = $"Piensa en la tabla del {b}. ¿Qué número multiplicado por {b} te da {a}?";
                break;
        }

        q.questionText = $"¿Cuánto es {a} {signo} {b}?";

        List<int> opciones = new List<int> { respuestaCorrecta };
        while (opciones.Count < 4)
        {
            int distractor = respuestaCorrecta + Random.Range(-4, 5);
            if (distractor != respuestaCorrecta && distractor >= 0 && !opciones.Contains(distractor))
            {
                opciones.Add(distractor);
            }
        }

        for (int i = 0; i < opciones.Count; i++)
        {
            int temp = opciones[i];
            int randomIndex = Random.Range(i, opciones.Count);
            opciones[i] = opciones[randomIndex];
            opciones[randomIndex] = temp;
        }

        q.answers = new string[4];
        for (int i = 0; i < 4; i++)
        {
            q.answers[i] = opciones[i].ToString();
            if (opciones[i] == respuestaCorrecta) q.correctAnswerIndex = i;
        }

        return q;
    }
}