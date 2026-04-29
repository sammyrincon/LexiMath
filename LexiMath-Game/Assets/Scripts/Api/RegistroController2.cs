using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class RegistroController2 : MonoBehaviour
{
    private UIDocument _doc;
    private TextField  _inputUsuario;
    private TextField  _inputCorreo;
    private TextField  _inputContrasena;
    private TextField  _inputConfirmar;
    private Button     _botonSiguiente;
    private Button     _botonAtras;
    private Label      _textoError;

    [SerializeField] private Texture2D _cursorMano;

    void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        _inputUsuario    = root.Q<TextField>("input-usuario");
        _inputCorreo     = root.Q<TextField>("input-correo");
        _inputContrasena = root.Q<TextField>("input-contrasena");
        _inputConfirmar  = root.Q<TextField>("input-confirmar");
        _botonSiguiente  = root.Q<Button>("boton-siguiente");
        _botonAtras      = root.Q<Button>("boton-atras");
        _textoError      = root.Q<Label>("texto-error");

        _textoError.style.display = DisplayStyle.None;

        _botonSiguiente.clicked += OnClickSiguiente;
        _botonAtras.clicked     += OnClickAtras;

        AgregarCursor(_botonSiguiente);
        AgregarCursor(_botonAtras);
    }

    void OnDisable()
    {
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _botonSiguiente.clicked -= OnClickSiguiente;
        _botonAtras.clicked     -= OnClickAtras;
    }

    private void OnClickSiguiente()
    {
        _textoError.style.display = DisplayStyle.None;
        _inputUsuario.RemoveFromClassList("input-error");
        _inputCorreo.RemoveFromClassList("input-error");
        _inputContrasena.RemoveFromClassList("input-error");
        _inputConfirmar.RemoveFromClassList("input-error");

        if (string.IsNullOrEmpty(_inputUsuario.value.Trim()))
        {
            MostrarError("El usuario es obligatorio");
            _inputUsuario.AddToClassList("input-error");
            return;
        }

        if (_inputUsuario.value.Trim().Length < 4)
        {
            MostrarError("El usuario debe tener al menos 4 caracteres");
            _inputUsuario.AddToClassList("input-error");
            return;
        }

        if (string.IsNullOrEmpty(_inputCorreo.value.Trim()))
        {
            MostrarError("El correo del tutor es obligatorio");
            _inputCorreo.AddToClassList("input-error");
            return;
        }

        if (!Regex.IsMatch(_inputCorreo.value.Trim(),
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MostrarError("El correo no tiene un formato válido");
            _inputCorreo.AddToClassList("input-error");
            return;
        }

        if (string.IsNullOrEmpty(_inputContrasena.value))
        {
            MostrarError("La contraseña es obligatoria");
            _inputContrasena.AddToClassList("input-error");
            return;
        }

        if (_inputContrasena.value.Length < 6)
        {
            MostrarError("La contraseña debe tener al menos 6 caracteres");
            _inputContrasena.AddToClassList("input-error");
            return;
        }

        if (_inputContrasena.value != _inputConfirmar.value)
        {
            MostrarError("Las contraseñas no coinciden");
            _inputConfirmar.AddToClassList("input-error");
            return;
        }

        AuthManager.Instance.GuardarPaso2(
            _inputUsuario.value.Trim(),
            _inputCorreo.value.Trim(),
            _inputContrasena.value);

        SceneManager.LoadScene("RegistroScene3");
    }

    private void OnClickAtras() =>
        SceneManager.LoadScene("RegistroScene");

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