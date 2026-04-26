using UnityEngine;

public enum TipoTema 
{ 
    Suma, 
    Resta, 
    Multiplicacion, 
    Division, 
    Vocales,
    Palabras,
    Oraciones,
    Gramatica
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class MathChest : MonoBehaviour
{
    public TipoTema temaDelCofre; 

    private Animator animator;
    private AudioSource audioSource;
    private bool isOpened = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        audioSource.playOnAwake = false; 
        audioSource.loop = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isOpened && collision.CompareTag("PlayerAttack"))
        {
            AbrirConSonido();
        }
    }

    private void AbrirConSonido()
    {
        isOpened = true;

        animator.SetTrigger("Open"); 

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        MathUIManager.Instance.GenerateAndShowQuestion(this);
    }

    public void OnAnsweredCorrectly()
    {
        Destroy(gameObject, 1f);
    }
}