using UnityEngine;

public class SorobanBead : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 pointPosition;
    public Vector3 lastSnappedPosition;
    private Vector3 lastTouchPosition;
    private Collider2D beadCollider;
    private bool isDragging = false;
    private int touchId = -1;
    private Camera mainCamera;
    private SorobanColumn columnHandler;
    public enum BeadType { Earth, Heaven }
    public BeadType beadType;
    private float dpiFactor;
    
    void Start()
    {
        startPosition = transform.position;
        if (beadType == BeadType.Heaven) // Set point position based on bead type
        {
            pointPosition = new Vector3(transform.position.x, transform.position.y - 80f, transform.position.z);
        }
        else
        {
            pointPosition = new Vector3(transform.position.x, transform.position.y + 80f, transform.position.z);
        }
        lastSnappedPosition = startPosition;
        mainCamera = Camera.main; // Cache the main camera
        beadCollider = GetComponent<Collider2D>(); // Cache the collider
        columnHandler = GetComponentInParent<SorobanColumn>(); // Get the column handler
        columnHandler.RegisterBead(this); // Register this bead with the column handler
        dpiFactor = Screen.dpi / 160f; // Adjust sensitivity based on screen dpi
        if (dpiFactor == 0)
        {
            dpiFactor = 1; // Fall back incase dpi is not set
        }
    }

    void Update()
    {
        // Handle multiple touches
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                Vector3 touchPos = mainCamera.ScreenToWorldPoint(touch.position);
                touchPos.z = startPosition.z; // Keep the original z position

                // Check if we've started dragging this bead
                if (touch.phase == TouchPhase.Began && touchId == -1 && beadCollider == Physics2D.OverlapPoint(touchPos))
                {
                    touchId = touch.fingerId;
                    isDragging = true;
                    lastTouchPosition = touchPos; // Store the last touch position
                }

                // Move the bead if this is the touch that's dragging it
                if (isDragging && touch.fingerId == touchId)
                {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Vector3 deltaTouch = (touchPos - lastTouchPosition) * dpiFactor; // Calculate the delta movement
                        Vector3 newPosition = transform.position + new Vector3(0, deltaTouch.y, 0); // Apply the delta movement to the bead's position
                        
                        // Determine the min and max Y positions based on bead type
                        float minY = beadType == BeadType.Heaven ? pointPosition.y : startPosition.y;
                        float maxY = beadType == BeadType.Heaven ? startPosition.y : pointPosition.y;
                        
                        // Clamp the bead's Y position within the allowed range
                        float clampedY = Mathf.Clamp(newPosition.y, minY, maxY);
                        float smoothSpeed = 15f; // Adjust this value to control the smoothing speed
                        transform.position = Vector3.Lerp(transform.position, new Vector3(startPosition.x, clampedY, startPosition.z), smoothSpeed * Time.deltaTime);
                        // OLD: transform.position = new Vector3(startPosition.x, clampedY, startPosition.z);

                        columnHandler.PushOtherBeads(this);

                        lastTouchPosition = touchPos; // Update the last touch position
                    }

                    // If the touch ends, stop dragging and snap the bead
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        isDragging = false;
                        touchId = -1; // Reset touch ID
                        columnHandler.SnapBeadsToRestPosition();
                    }
                }
            }
        }
    }
}