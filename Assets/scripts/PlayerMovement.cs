using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;               // Velocidad de movimiento
    public float gravity = -9.81f;         // Gravedad para que el personaje no flote
    public float jumpHeight = 1.5f;        // Altura de salto (opcional)
    
    [Header("Ratón")]
    public float mouseSensitivity = 100f; // Sensibilidad del mouse para mirar

    [Header("Cámara")]
    public Transform playerCamera;          // La cámara que será la vista del jugador
    public float cameraShakeAmount = 0.05f;// Magnitud del temblor de la cámara al caminar
    public float cameraShakeSpeed = 10f;   // Velocidad del temblor

    private CharacterController controller;
    private float xRotation = 0f;           // Para limitar rotación vertical
    private Vector3 velocity;
    private bool isGrounded;

    private float shakeTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Bloquear cursor para FPS
    }

    void Update()
    {
        // Movimiento del mouse para rotar la cámara
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movimiento con teclado
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * (speed * Time.deltaTime));

        // Gravedad y salto
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height / 2f + 0.1f);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // pequeño valor para que mantenga contacto con el suelo
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Temblor de cámara si se está moviendo
        if (move.magnitude > 0.1f)
        {
            shakeTimer += Time.deltaTime * cameraShakeSpeed;
            float shakeX = Mathf.Sin(shakeTimer * 2f) * cameraShakeAmount;
            float shakeY = Mathf.Sin(shakeTimer * 3f) * cameraShakeAmount;
            playerCamera.localPosition = new Vector3(shakeX, 1.8f + shakeY, 0f);
        }
        else
        {
            shakeTimer = 0f;
            playerCamera.localPosition = new Vector3(0f, 1.8f, 0f); // Posición normal de la cámara
        }
    }
}
