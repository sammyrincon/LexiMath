using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// LoginController — controla la pantalla de Login
/// Conecta los campos de UI con AuthManager para
/// llamar a la API y manejar la respuesta.
/// </summary>
public class LoginController : MonoBehaviour
{
    [Header("Campos de entrada")]
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContrasena;

    [Header("Botones")]
    public GameObject botonLogin;
    public GameObject botonRegistrarse;
    public GameObject botonAdmin;

    [Header("Mensajes")]
    public TextMeshProUGUI textError;

    // ───────────────────────────────────────────────────────────
    void Start()
    {
        // Asegurarse que el texto de error esté oculto al inicio
        textError.gameObject.SetActive(false);
    }

    // ───────────────────────────────────────────────────────────
    // BOTÓN LOGIN — llamado desde el Inspector
    // ───────────────────────────────────────────────────────────
    public void OnClickLogin()
    {
        // Limpiar error anterior
        textError.gameObject.SetActive(false);

        // Validar que los campos no estén vacíos
        if (string.IsNullOrEmpty(inputUsuario.text) ||
            string.IsNullOrEmpty(inputContrasena.text))
        {
            MostrarError("Por favor llena todos los campos");
            return;
        }

        // Llamar al AuthManager
        AuthManager.Instance.Login(
            inputUsuario.text.Trim(),
            inputContrasena.text,
            onSuccess: () =>
            {
                // Login exitoso — ir a pantalla de progreso
                // o mostrar tutorial si es primera vez
                if (!GameManager.Instance.TutorialMecanicas)
                    SceneManager.LoadScene("TutorialScene");
                else
                    SceneManager.LoadScene("MapaScene");
            },
            onError: (error) =>
            {
                MostrarError("Usuario o contraseña incorrectos");
                Debug.LogError("Login error: " + error);
            }
        );
    }

    // ───────────────────────────────────────────────────────────
    // BOTÓN REGISTRARSE — ir a pantalla de registro
    // ───────────────────────────────────────────────────────────
    public void OnClickRegistrarse()
    {
        SceneManager.LoadScene("RegistroScene");
    }

    // ───────────────────────────────────────────────────────────
    // BOTÓN ADMIN — abre panel web en nueva pestaña
    // ───────────────────────────────────────────────────────────
    public void OnClickAdmin()
    {
        Application.OpenURL("https://leximath.app/admin");
    }

    // ───────────────────────────────────────────────────────────
    // Mostrar mensaje de error
    // ───────────────────────────────────────────────────────────
    private void MostrarError(string mensaje)
    {
        textError.text = mensaje;
        textError.gameObject.SetActive(true);
    }
}