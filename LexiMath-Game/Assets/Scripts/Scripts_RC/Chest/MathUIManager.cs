using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MathUIManager : MonoBehaviour
{
    public static MathUIManager Instance;

    private VisualElement overlay;
    private VisualElement guideOverlay;
    private Label questionLabel;
    private Button[] answerButtons = new Button[4];
    private Button hintButton;

    private int currentCorrectIndex;
    private MathChest currentChest;
    // Este es un struct privado para organizar las preguntas de texto, con su pregunta, respuesta correcta e incorrectas.
    private struct PreguntaTexto
    {
        public string pregunta;
        public string correcta;
        public string[] incorrectas;
    }
    // Son los bancos de las preguntas de texto para cada tema, con 5 preguntas cada uno, con su pregunta, respuesta correcta e incorrectas.

    private PreguntaTexto[] bancoVocales = new PreguntaTexto[]
    {
        new PreguntaTexto { pregunta = "¿Cuál es una vocal fuerte?", correcta = "A", incorrectas = new string[] { "I", "U", "M" } },
        new PreguntaTexto { pregunta = "¿Cuál es una vocal débil?", correcta = "U", incorrectas = new string[] { "O", "E", "A" } },
        new PreguntaTexto { pregunta = "¿Qué letra es una vocal?", correcta = "E", incorrectas = new string[] { "P", "S", "L" } },
        new PreguntaTexto { pregunta = "¿Con qué vocal empieza 'Oso'?", correcta = "O", incorrectas = new string[] { "A", "U", "E" } },
        new PreguntaTexto { pregunta = "¿Con qué vocal termina 'Elefante'?", correcta = "E", incorrectas = new string[] { "A", "I", "O" } }
    };

    private PreguntaTexto[] bancoPalabras = new PreguntaTexto[]
    {
        new PreguntaTexto { pregunta = "Sinónimo de Feliz", correcta = "Contento", incorrectas = new string[] { "Triste", "Enojado", "Lento" } },
        new PreguntaTexto { pregunta = "Antónimo de Rápido", correcta = "Lento", incorrectas = new string[] { "Veloz", "Fuerte", "Alto" } },
        new PreguntaTexto { pregunta = "¿Cuál es un color?", correcta = "Verde", incorrectas = new string[] { "Saltar", "Ayer", "Mesa" } },
        new PreguntaTexto { pregunta = "¿Cuál es un animal?", correcta = "Perro", incorrectas = new string[] { "Silla", "Correr", "Mucho" } },
        new PreguntaTexto { pregunta = "Sinónimo de Grande", correcta = "Enorme", incorrectas = new string[] { "Pequeño", "Bajo", "Poco" } }
    };

    private PreguntaTexto[] bancoOraciones = new PreguntaTexto[]
    {
        new PreguntaTexto { pregunta = "El perro ___ fuerte.", correcta = "ladra", incorrectas = new string[] { "maúlla", "vuela", "nada" } },
        new PreguntaTexto { pregunta = "Mañana ___ al parque.", correcta = "iré", incorrectas = new string[] { "fui", "yendo", "ido" } },
        new PreguntaTexto { pregunta = "La niña ___ una manzana.", correcta = "come", incorrectas = new string[] { "bebe", "lee", "salta" } },
        new PreguntaTexto { pregunta = "Los pájaros ___ en el cielo.", correcta = "vuelan", incorrectas = new string[] { "nadan", "caminan", "corren" } },
        new PreguntaTexto { pregunta = "Mi mamá ___ la cena.", correcta = "cocina", incorrectas = new string[] { "pinta", "duerme", "juega" } }
    };

    private PreguntaTexto[] bancoGramatica = new PreguntaTexto[]
    {
        new PreguntaTexto { pregunta = "¿Qué palabra es aguda?", correcta = "Camión", incorrectas = new string[] { "Árbol", "Pájaro", "Mesa" } },
        new PreguntaTexto { pregunta = "¿Qué palabra es grave?", correcta = "Árbol", incorrectas = new string[] { "Ratón", "Cantar", "Allí" } },
        new PreguntaTexto { pregunta = "¿Qué palabra es esdrújula?", correcta = "Pájaro", incorrectas = new string[] { "Reloj", "Cama", "Papel" } },
        new PreguntaTexto { pregunta = "¿Cuál es un verbo?", correcta = "Correr", incorrectas = new string[] { "Mesa", "Lindo", "Ayer" } },
        new PreguntaTexto { pregunta = "¿Cuál es un sustantivo?", correcta = "Casa", incorrectas = new string[] { "Saltar", "Ayer", "Muy" } }
    };
    // En el Awake se asigna la instancia para que sea accesible desde otros scripts, como el MathChest, y en el OnEnable se buscan los elementos de la UI y se configuran los eventos de los botones.

    private void Awake()
    {
        Instance = this;
    }
    // Aqui se asignan los elementos de la UI a las variables, se configuran los eventos de los botones y se ocultan los paneles al inicio. Es importante que el nombre de los elementos en el UI Builder coincida con los nombres usados aquí para que se asignen correctamente.

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        overlay = root.Q<VisualElement>("math-overlay");
        guideOverlay = root.Q<VisualElement>("guide-overlay");
        questionLabel = root.Q<Label>("question-label");
    // Se asignan los botones de respuesta y se configuran sus eventos para que llamen a la función OnAnswerClicked con el índice del botón cuando se haga clic en ellos.
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            answerButtons[i] = root.Q<Button>($"btn-ans-{i + 1}");
            answerButtons[i].clicked += () => OnAnswerClicked(index);
        }

        hintButton = root.Q<Button>("btn-hint");
        hintButton.clicked += OpenMathGuide;

        overlay.style.display = DisplayStyle.None;
        if (guideOverlay != null) guideOverlay.style.display = DisplayStyle.None;
    }
    // En este método se genera la pregunta y las opciones de respuesta según el tema del cofre que se abrió. Para los temas de matemáticas se generan números aleatorios y se calcula la respuesta correcta, mientras que para los temas de texto se selecciona una pregunta aleatoria del banco correspondiente. Luego se mezclan las opciones de respuesta, se asignan a los botones y se muestra el panel de la pregunta. También se pausa el juego para que el jugador pueda responder sin presión de tiempo.

    public void GenerateAndShowQuestion(MathChest chestReference)
    {
        currentChest = chestReference;
        List<string> options = new List<string>();
        string correctAnswer = "";

        switch (chestReference.temaDelCofre)
        {
            case TipoTema.Suma:
                int s1 = Random.Range(1, 51);
                int s2 = Random.Range(1, 51);
                correctAnswer = (s1 + s2).ToString();
                questionLabel.text = $"{s1} + {s2} = ?";
                options = GenerateMathOptions(int.Parse(correctAnswer));
                break;

            case TipoTema.Resta:
                int r1 = Random.Range(10, 100);
                int r2 = Random.Range(1, r1); 
                correctAnswer = (r1 - r2).ToString();
                questionLabel.text = $"{r1} - {r2} = ?";
                options = GenerateMathOptions(int.Parse(correctAnswer));
                break;

            case TipoTema.Multiplicacion:
                int m1 = Random.Range(2, 13);
                int m2 = Random.Range(2, 13);
                correctAnswer = (m1 * m2).ToString();
                questionLabel.text = $"{m1} × {m2} = ?";
                options = GenerateMathOptions(int.Parse(correctAnswer));
                break;

            case TipoTema.Division:
                int d2 = Random.Range(2, 13);
                int ans = Random.Range(2, 13);
                int d1 = d2 * ans;
                correctAnswer = ans.ToString();
                questionLabel.text = $"{d1} ÷ {d2} = ?";
                options = GenerateMathOptions(int.Parse(correctAnswer));
                break;

            case TipoTema.Vocales:
                ConfigurarPreguntaTexto(bancoVocales, out correctAnswer, options);
                break;

            case TipoTema.Palabras:
                ConfigurarPreguntaTexto(bancoPalabras, out correctAnswer, options);
                break;

            case TipoTema.Oraciones:
                ConfigurarPreguntaTexto(bancoOraciones, out correctAnswer, options);
                break;

            case TipoTema.Gramatica:
                ConfigurarPreguntaTexto(bancoGramatica, out correctAnswer, options);
                break;
        }

        for (int i = 0; i < options.Count; i++)
        {
            string temp = options[i];
            int r = Random.Range(i, options.Count);
            options[i] = options[r];
            options[r] = temp;
        }

        for (int i = 0; i < 4; i++)
        {
            answerButtons[i].text = options[i];
            if (options[i] == correctAnswer)
            {
                currentCorrectIndex = i;
            }
        }

        overlay.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }
    // En esta parte se configura la pregunta de texto seleccionando una aleatoria del banco correspondiente al tema, asignando su texto a la etiqueta de la pregunta, guardando la respuesta correcta y llenando la lista de opciones con la respuesta correcta e incorrectas para luego mezclarlas y asignarlas a los botones. Este método se llama desde el GenerateAndShowQuestion cuando el tema es de texto.

    private void ConfigurarPreguntaTexto(PreguntaTexto[] banco, out string correcta, List<string> opciones)
    {
        int randomIndex = Random.Range(0, banco.Length);
        PreguntaTexto q = banco[randomIndex];
        
        questionLabel.text = q.pregunta;
        correcta = q.correcta;
        
        opciones.Add(correcta);
        foreach (string wrong in q.incorrectas)
        {
            opciones.Add(wrong);
        }
    }
    // En esta otra parte se generan opciones de respuesta para las preguntas de matemáticas creando números cercanos a la respuesta correcta para que el jugador tenga opciones plausibles pero no triviales. Se asegura de no repetir opciones y de no incluir números negativos.

    private List<string> GenerateMathOptions(int correctAns)
    {
        List<string> mathOptions = new List<string> { correctAns.ToString() };
        int offsetRange = correctAns < 20 ? 5 : 15;

        while (mathOptions.Count < 4)
        {
            int wrong = correctAns + Random.Range(-offsetRange, offsetRange + 1);
            if (wrong != correctAns && wrong >= 0 && !mathOptions.Contains(wrong.ToString()))
            {
                mathOptions.Add(wrong.ToString());
            }
        }
        return mathOptions;
    }
    // Este método se llama cuando el jugador hace clic en una opción de respuesta. Verifica si el índice del botón clickeado coincide con el índice de la respuesta correcta. Si es correcto, llama al método OnAnsweredCorrectly del cofre para que se destruya y luego cierra los paneles. Si es incorrecto, simplemente cierra los paneles para que el jugador pueda intentarlo de nuevo o seguir jugando.

    private void OnAnswerClicked(int clickedIndex)
    {
        if (clickedIndex == currentCorrectIndex)
        {
            currentChest.OnAnsweredCorrectly();
            ClosePanels();
        }
        else
        {
            ClosePanels();
        }
    }
    // Este método se llama cuando el jugador hace clic en el botón de pista. Simplemente muestra el panel de la guía con información útil para resolver la pregunta. El contenido de la guía se puede configurar en el UI Builder y puede incluir texto, imágenes o cualquier elemento visual que ayude al jugador a entender el concepto relacionado con la pregunta.

    private void OpenMathGuide()
    {
        if (guideOverlay != null) guideOverlay.style.display = DisplayStyle.Flex;
    }

    private void ClosePanels()
    {
        overlay.style.display = DisplayStyle.None;
        if (guideOverlay != null) guideOverlay.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }
}