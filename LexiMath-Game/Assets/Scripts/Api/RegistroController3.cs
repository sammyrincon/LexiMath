using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class RegistroController3 : MonoBehaviour
{
    private UIDocument _doc;
    private TextField  _inputNide;
    private Button     _botonRegistrarse;
    private Button     _botonAtras;
    private Label      _textoError;

    [SerializeField] private Texture2D _cursorMano;

    void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        _inputNide        = root.Q<TextField>("input-nide");
        _botonRegistrarse = root.Q<Button>("boton-registrarse");
        _botonAtras       = root.Q<Button>("boton-atras");
        _textoError       = root.Q<Label>("texto-error");

        _textoError.style.display = DisplayStyle.None;

        // Forzar formato NIDE-XXXX
        _inputNide.RegisterValueChangedCallback(evt =>
        {
            string valor = evt.newValue.ToUpper();
            if (!valor.StartsWith("NIDE-"))
                valor = "NIDE-";

            string numeros = "";
            foreach (char c in valor.Substring(5))
                if (char.IsDigit(c)) numeros += c;
            if (numeros.Length > 4)
                numeros = numeros.Substring(0, 4);

            string resultado = "NIDE-" + numeros;
            if (_inputNide.value != resultado)
                _inputNide.SetValueWithoutNotify(resultado);
        });

        _botonRegistrarse.clicked += OnClickRegistrarse;
        _botonAtras.clicked       += OnClickAtras;

        AgregarCursor(_botonRegistrarse);
        AgregarCursor(_botonAtras);
    }

    void OnDisable()
    {
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _botonRegistrarse.clicked -= OnClickRegistrarse;
        _botonAtras.clicked       -= OnClickAtras;
    }

    private void OnClickRegistrarse()
    {
        _textoError.style.display = DisplayStyle.None;
        _inputNide.RemoveFromClassList("input-error");

        if (_inputNide.value.Trim() == "NIDE-" ||
            string.IsNullOrEmpty(_inputNide.value.Trim()))
        {
            MostrarError("El código NIDE es obligatorio");
            _inputNide.AddToClassList("input-error");
            return;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(
            _inputNide.value.Trim(), @"^NIDE-[0-9]{4}$"))
        {
            MostrarError("Formato inválido. Debe ser NIDE-XXXX");
            _inputNide.AddToClassList("input-error");
            return;
        }

        _botonRegistrarse.SetEnabled(false);
        _botonRegistrarse.text = "REGISTRANDO...";

        AuthManager.Instance.RegistrarPaso3(
            _inputNide.value.Trim(),
            onSuccess: () =>
            {
                SceneManager.LoadScene("LoginScene");
            },
            onError: (error) =>
            {
                _botonRegistrarse.SetEnabled(true);
                _botonRegistrarse.text = "¡CREAR CUENTA! ▶";

                if (error.Contains("404"))
                    MostrarError("Código NIDE no encontrado");
                else if (error.Contains("409"))
                    MostrarError("Ese usuario ya existe");
                else
                    MostrarError("Error al registrar. Intenta de nuevo");

                _inputNide.AddToClassList("input-error");
                Debug.LogError("Registro error: " + error);
            }
        );
    }

    private void OnClickAtras() =>
        SceneManager.LoadScene("RegistroScene2");

    private void AgregarCursor(Button boton)
    {
        boton.RegisterCallback<MouseEnterEvent>(_ =>
            UnityEngine.Cursor.SetCursor(_cursorMano, Vector2.zero, CursorMode.Auto));
        boton.RegisterCallback<MouseLeaveEvent>(_ =>
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto));
    }

    private void MostrarError(string mensaje)
    {
        _textoError.text = mensaje;
        _textoError.style.display = DisplayStyle.Flex;
    }
}