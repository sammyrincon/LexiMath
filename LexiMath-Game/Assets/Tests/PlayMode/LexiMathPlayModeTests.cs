using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;


public class LexiMathPlayModeTests
{
    // Referencias a los componentes principales de la escena
    private GameManager       gameManager;
    private KnightController  knightController;
    private KnightAttack      knightAttack;
    private PlayerHealth      playerHealth;
    private HUDController     hudController;
    private EnemyBasic        enemyBasic;
    private NPCDialog         npcDialog;

    
    //  SETUP 
    
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Cargamos la escena del primer nivel de matemáticas
        SceneManager.LoadScene("Nivel 1 Mate - BB");

        // Esperamos dos frames para que la escena cargue completamente
        yield return null;
        yield return null;

        // Buscamos los componentes principales en la escena
        gameManager      = Object.FindAnyObjectByType<GameManager>();
        knightController = Object.FindAnyObjectByType<KnightController>();
        knightAttack     = Object.FindAnyObjectByType<KnightAttack>();
        playerHealth     = Object.FindAnyObjectByType<PlayerHealth>();
        hudController    = Object.FindAnyObjectByType<HUDController>();
        enemyBasic       = Object.FindAnyObjectByType<EnemyBasic>();
        npcDialog        = Object.FindAnyObjectByType<NPCDialog>();
    }

    
    //  TEST 1 — El Knight se mueve a la derecha
    [UnityTest]
    public IEnumerator Knight_SeMueveHaciaDerecha()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(knightController,
            "No se encontró KnightController en la escena.");

        Vector3 posInicial = knightController.transform.position;

        // ── ACT ──
        // Simulamos input positivo (tecla D / flecha derecha)
        SetPrivateField(knightController, "_inputH", 1f);

        yield return new WaitForSeconds(0.5f);

        // ── ASSERT ──
        Vector3 posFinal = knightController.transform.position;

        Assert.AreNotEqual(posInicial.x, posFinal.x,
            "El Knight no se movió al simular input horizontal.");

        Assert.Greater(posFinal.x, posInicial.x,
            "El Knight debería haberse movido hacia la derecha (X mayor).");
    }

    
    //  TEST 2 — El Knight se mueve a la izquierda
    [UnityTest]
    public IEnumerator Knight_SeMueveHaciaIzquierda()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(knightController,
            "No se encontró KnightController en la escena.");

        Vector3 posInicial = knightController.transform.position;

        // ── ACT ──
        SetPrivateField(knightController, "_inputH", -1f);

        yield return new WaitForSeconds(0.5f);

        // ── ASSERT ──
        Vector3 posFinal = knightController.transform.position;

        Assert.Less(posFinal.x, posInicial.x,
            "El Knight debería haberse movido hacia la izquierda (X menor).");
    }

    
    //  TEST 3 — El Knight salta al aplicar fuerza vertical
    [UnityTest]
    public IEnumerator Knight_Salta_CuandoAplicaFuerzaVertical()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(knightController,
            "No se encontró KnightController en la escena.");

        Rigidbody2D rb = knightController.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb, "El Knight no tiene Rigidbody2D.");

        float yInicial = knightController.transform.position.y;

        // ── ACT ──
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, knightController.fuerzaSalto);

        yield return new WaitForSeconds(0.15f);

        // ── ASSERT ──
        float yDespues = knightController.transform.position.y;

        Assert.Greater(yDespues, yInicial,
            "El Knight debería haber subido al aplicar fuerzaSalto.");
    }

    
    //  TEST 4 — El KnightAttack está configurado correctamente
    [UnityTest]
    public IEnumerator KnightAttack_TieneConfiguracionValida()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(knightAttack,
            "No se encontró KnightAttack en la escena.");

        // ── ACT ──
        yield return null;

        // ── ASSERT ──
        Assert.Greater(knightAttack.attackDamage, 0,
            "El daño del ataque debería ser mayor que 0.");

        Assert.Greater(knightAttack.attackRange, 0f,
            "El rango del ataque debería ser mayor que 0.");

        Assert.GreaterOrEqual(knightAttack.attackCooldown, 0f,
            "El cooldown del ataque no puede ser negativo.");
    }

    
    //  TEST 5 — El jugador baja vida al recibir daño
    [UnityTest]
    public IEnumerator Player_RecibeDano_BajaVida()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(playerHealth,
            "No se encontró PlayerHealth en la escena.");

        float vidaInicial = playerHealth.VidaActual;

        Assert.AreEqual(playerHealth.maxHealth, vidaInicial,
            "La vida inicial debería ser maxHealth.");

        // ── ACT ──
        playerHealth.RecibirDano();

        yield return null;

        // ── ASSERT ──
        float vidaFinal = playerHealth.VidaActual;

        Assert.Less(vidaFinal, vidaInicial,
            "La vida debería haber bajado después de recibir daño.");

        Assert.AreEqual(vidaInicial - playerHealth.danoPorGolpe, vidaFinal,
            "La vida debería haber bajado exactamente en danoPorGolpe.");
    }

    
    //  TEST 6 — El marco rojo se activa con vida baja
    [UnityTest]
    public IEnumerator Player_VidaBaja_ActivaMarcoRojo()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(playerHealth,
            "No se encontró PlayerHealth en la escena.");

        UIDocument uiDoc = playerHealth.GetComponent<UIDocument>();
        Assert.IsNotNull(uiDoc, "PlayerHealth no tiene UIDocument.");

        VisualElement dangerFrame = uiDoc.rootVisualElement.Q<VisualElement>("danger-frame");
        Assert.IsNotNull(dangerFrame, "No se encontró el elemento 'danger-frame'.");

        Assert.IsTrue(dangerFrame.ClassListContains("danger-hidden"),
            "El marco rojo debería estar oculto al inicio.");

        // ── ACT ──
        SetPrivateField(playerHealth, "_currentHealth", 15f);

        MethodInfo metodoRevisar = typeof(PlayerHealth).GetMethod("RevisarVidaBaja",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(metodoRevisar, "No se encontró el método RevisarVidaBaja.");
        metodoRevisar.Invoke(playerHealth, null);

        yield return null;

        // ── ASSERT ──
        Assert.IsFalse(dangerFrame.ClassListContains("danger-hidden"),
            "El marco rojo debería estar visible cuando la vida es baja.");
    }

    
    //  TEST 7 — El panel de Game Over aparece al morir
    [UnityTest]
    public IEnumerator Player_SinVida_MuestraGameOver()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(playerHealth,
            "No se encontró PlayerHealth en la escena.");

        UIDocument uiDoc = playerHealth.GetComponent<UIDocument>();
        VisualElement gameOverPanel = uiDoc.rootVisualElement.Q<VisualElement>("gameover-panel");
        Assert.IsNotNull(gameOverPanel, "No se encontró el panel 'gameover-panel'.");

        Assert.IsTrue(gameOverPanel.ClassListContains("gameover-hidden"),
            "El panel de Game Over debería estar oculto al inicio.");

        Assert.IsFalse(playerHealth.EstaMuerto,
            "El jugador no debería estar muerto al inicio.");

        // ── ACT ──
        playerHealth.RecibirDano(9999f);

        yield return new WaitForSecondsRealtime(1.7f);

        // ── ASSERT ──
        Assert.IsTrue(playerHealth.EstaMuerto,
            "El jugador debería estar muerto después del daño letal.");

        Assert.IsFalse(gameOverPanel.ClassListContains("gameover-hidden"),
            "El panel de Game Over debería estar visible cuando el jugador muere.");

        // Limpieza: restauramos Time.timeScale porque Morir() lo pone en 0
        Time.timeScale = 1f;
    }

    //  TEST 8 — GameManager suma puntos correctamente
    [UnityTest]
    public IEnumerator GameManager_SumarPuntos_AumentaPuntaje()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(gameManager,
            "No se encontró GameManager en la escena. " +
            "Agrega un GameObject vacío con el script GameManager.");

        int puntosIniciales = GetPrivateField<int>(gameManager, "puntosTotales");

        // ── ACT ──
        SetPrivateField(gameManager, "puntosTotales", puntosIniciales + 100);

        yield return null;

        // ── ASSERT ──
        int puntosFinales = GetPrivateField<int>(gameManager, "puntosTotales");

        Assert.AreEqual(puntosIniciales + 100, puntosFinales,
            "El puntaje debería haber aumentado en 100.");

        Assert.Greater(puntosFinales, puntosIniciales,
            "El puntaje final debería ser mayor que el inicial.");
    }

    
    //  TEST 9 — El enemigo recibe daño y muere al llegar a 0 de vida
    [UnityTest]
    public IEnumerator EnemyBasic_RecibeDanoYMuere()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(enemyBasic,
            "No se encontró EnemyBasic en la escena.");

        int vidaMaxima = enemyBasic.maxHealth;

        // Verificamos el estado inicial: no está muerto
        bool muertoAlInicio = GetPrivateField<bool>(enemyBasic, "isDead");
        Assert.IsFalse(muertoAlInicio,
            "El enemigo no debería estar muerto al inicio.");

        // ── ACT ──
        // Golpeamos al enemigo tantas veces como su vida máxima
        // (cada golpe hace 1 de daño, así que maxHealth golpes lo matan)
        for (int i = 0; i < vidaMaxima; i++)
        {
            enemyBasic.TakeDamage(1);
            yield return null;
        }

        // Esperamos un poco para que se complete el flash de daño
        yield return new WaitForSeconds(0.2f);

        // ── ASSERT ──
        // Leemos el campo privado 'isDead' para verificar la muerte
        // (no podemos usar == null porque Destroy tarda 1 segundo)
        bool estaMuerto = GetPrivateField<bool>(enemyBasic, "isDead");
        Assert.IsTrue(estaMuerto,
            "El enemigo debería estar muerto después de recibir maxHealth de daño.");
    }

    
    //  TEST 10 — El diálogo del NPC se muestra y se oculta
    [UnityTest]
    public IEnumerator NPCDialog_MuestraYOcultaDialogo()
    {
        // ── ARRANGE ──
        Assert.IsNotNull(npcDialog,
            "No se encontró NPCDialog en la escena.");

        Assert.IsNotNull(npcDialog.dialogBox,
            "El NPCDialog no tiene dialogBox asignado en el Inspector.");

        string textoDePrueba = "Hola aventurero!";

        // ── ACT: MOSTRAR ──
        npcDialog.MostrarDialogo(textoDePrueba);
        yield return null;

        // ── ASSERT: está visible ──
        Assert.IsTrue(npcDialog.dialogBox.activeSelf,
            "El dialogBox debería estar activo (visible) después de MostrarDialogo.");

        // Si tiene textoDialogo asignado, verificamos que el texto se haya actualizado
        if (npcDialog.textoDialogo != null)
        {
            Assert.AreEqual(textoDePrueba, npcDialog.textoDialogo.text,
                "El texto del diálogo debería coincidir con el que pasamos.");
        }

        // ── ACT: OCULTAR ──
        npcDialog.OcultarDialogo();
        yield return null;

        // ── ASSERT: está oculto ──
        Assert.IsFalse(npcDialog.dialogBox.activeSelf,
            "El dialogBox debería estar inactivo (oculto) después de OcultarDialogo.");
    }

    //  HELPERS — Métodos auxiliares 

    private static T GetPrivateField<T>(object instance, string fieldName)
    {
        FieldInfo field = instance.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field,
            $"No se encontró el campo privado '{fieldName}' en {instance.GetType().Name}.");
        return (T)field.GetValue(instance);
    }

    private static void SetPrivateField<T>(object instance, string fieldName, T value)
    {
        FieldInfo field = instance.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field,
            $"No se encontró el campo privado '{fieldName}' en {instance.GetType().Name}.");
        field.SetValue(instance, value);
    }
}
