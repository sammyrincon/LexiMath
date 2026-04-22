using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class RegistroController : MonoBehaviour
{
    private UIDocument    _doc;
    private TextField     _inputNombre;
    private TextField     _inputEdad;
    private DropdownField _dropdownGenero;
    private Button        _botonSiguiente;
    private Button        _botonCancelar;
    private Label         _textoError;

    [SerializeField] private Texture2D _cursorMano;

    void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        _inputNombre    = root.Q<TextField>("input-nombre");
        _inputEdad      = root.Q<TextField>("input-edad");
        _dropdownGenero = root.Q<DropdownField>("dropdown-genero");
        _botonSiguiente = root.Q<Button>("boton-siguiente");
        _botonCancelar  = root.Q<Button>("boton-cancelar");
        _textoError     = root.Q<Label>("texto-error");

        _textoError.style.display = DisplayStyle.None;

        // Solo letras y espacios en nombre
        _inputNombre.RegisterValueChangedCallback(evt =>
        {
            string soloLetras = "";
            foreach (char c in evt.newValue)
                if (char.IsLetter(c) || c == ' ') soloLetras += c;
            if (_inputNombre.value != soloLetras)
                _inputNombre.SetValueWithoutNotify(soloLetras);
        });

        // Solo números en edad
        _inputEdad.RegisterValueChangedCallback(evt =>
        {
            string soloNumeros = "";
            foreach (char c in evt.newValue)
                if (char.IsDigit(c)) soloNumeros += c;
            if (soloNumeros.Length > 2)
                soloNumeros = soloNumeros.Substring(0, 2);
            if (_inputEdad.value != soloNumeros)
                _inputEdad.SetValueWithoutNotify(soloNumeros);
        });

        // Colores dropdown
        _dropdownGenero.RegisterValueChangedCallback(evt =>
            ActualizarColorGenero(evt.newValue));
        ActualizarColorGenero(_dropdownGenero.value);

        // Botones
        _botonSiguiente.clicked += OnClickSiguiente;
        _botonCancelar.clicked  += OnClickCancelar;

        // Cursor
        AgregarCursor(_botonSiguiente);
        AgregarCursor(_botonCancelar);
    }

    void OnDisable()
    {
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _botonSiguiente.clicked -= OnClickSiguiente;
        _botonCancelar.clicked  -= OnClickCancelar;
    }

    private void OnClickSiguiente()
    {
        _textoError.style.display = DisplayStyle.None;
        _inputNombre.RemoveFromClassList("input-error");
        _inputEdad.RemoveFromClassList("input-error");

        if (string.IsNullOrEmpty(_inputNombre.value.Trim()))
        {
            MostrarError("El nombre es obligatorio");
            _inputNombre.AddToClassList("input-error");
            return;
        }

        if (string.IsNullOrEmpty(_inputEdad.value))
        {
            MostrarError("La edad es obligatoria");
            _inputEdad.AddToClassList("input-error");
            return;
        }

        int edad = int.Parse(_inputEdad.value);
        if (edad < 1 || edad > 99)
        {
            MostrarError("La edad debe ser entre 1 y 99");
            _inputEdad.AddToClassList("input-error");
            return;
        }

        string genero = _dropdownGenero.value switch
        {
            "Masculino" => "masculino",
            "Femenino"  => "femenino",
            _           => "otro"
        };

        AuthManager.Instance.GuardarPaso1(
            _inputNombre.value.Trim(), edad, genero);

        SceneManager.LoadScene("RegistroScene2");
    }

    private void OnClickCancelar() =>
        SceneManager.LoadScene("LoginScene");

    private void ActualizarColorGenero(string valor)
    {
        _dropdownGenero.RemoveFromClassList("genero-masculino");
        _dropdownGenero.RemoveFromClassList("genero-femenino");
        _dropdownGenero.RemoveFromClassList("genero-otro");

        switch (valor)
        {
            case "Masculino":
                _dropdownGenero.AddToClassList("genero-masculino"); break;
            case "Femenino":
                _dropdownGenero.AddToClassList("genero-femenino"); break;
            default:
                _dropdownGenero.AddToClassList("genero-otro"); break;
        }
    }

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