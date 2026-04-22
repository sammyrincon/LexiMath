using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ProgresoManager — obtiene el progreso de niveles del estudiante
/// desde la API y lo almacena para que el MainMenu lo muestre.
/// </summary>
public class ProgresoManager : MonoBehaviour
{
    public static ProgresoManager Instance { get; private set; }

    // ── Lista de progreso por nivel ────────────────────────────
    public List<ProgresoNivel> Progreso { get; private set; } = new List<ProgresoNivel>();

    // ── Evento para notificar cuando el progreso está listo ────
    public event Action OnProgresoListo;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Cargar progreso desde la API ───────────────────────────
    public void CargarProgreso()
    {
        string endpoint = $"/estudiante/{GameManager.Instance.IdEstudiante}/progreso";
        StartCoroutine(ApiManager.Instance.Get(endpoint,
            (json) =>
            {
                ProgresoResponse resp = JsonUtility.FromJson<ProgresoResponse>(json);
                Progreso = resp.progreso;
                Debug.Log($"Progreso cargado: {Progreso.Count} niveles");
                OnProgresoListo?.Invoke();
            },
            (error) =>
            {
                Debug.LogError("Error al cargar progreso: " + error);
            }
        ));
    }

    // ── Helpers ───────────────────────────────────────────────
    public ProgresoNivel GetNivel(int idNivel)
    {
        return Progreso.Find(p => p.id_nivel == idNivel);
    }

    public bool EstaDesbloqueado(int idNivel)
    {
        var nivel = GetNivel(idNivel);
        return nivel != null && nivel.desbloqueado;
    }

    public int GetEstrellas(int idNivel)
    {
        var nivel = GetNivel(idNivel);
        return nivel?.estrellas_max ?? 0;
    }
}

// ── Modelos de datos ───────────────────────────────────────────

[Serializable]
public class ProgresoNivel
{
    public int    id_nivel;
    public string nombre_nivel;
    public string materia;
    public int    orden;
    public bool   es_jefe_final;
    public int    estrellas_max;
    public float  pct_max;
    public bool   desbloqueado;
}

[Serializable]
public class ProgresoResponse
{
    public List<ProgresoNivel> progreso;
}