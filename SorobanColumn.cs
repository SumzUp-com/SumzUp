using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SorobanColumn : MonoBehaviour
{
    public List<SorobanBead> beadsInColumn = new List<SorobanBead>();
    public int columnIndex; // Index of this column, starting from 0 for the units column
    public Numby Numby;
    public AudioClip upSound;
    public AudioClip downSound;
    private AudioSource audioSource1;
    private AudioSource audioSource2;

    void Start()
    {
        if (audioSource1 == null)
        {
            audioSource1 = gameObject.AddComponent<AudioSource>();
        }
        audioSource1.clip = upSound;
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource2.clip = downSound;
    }

    // Add a bead to the column
    public void RegisterBead(SorobanBead bead)
    {
        if (!beadsInColumn.Contains(bead))
        {
            beadsInColumn.Add(bead);
            bead.transform.SetParent(this.transform, false); // Optionally, set the bead's parent to this column
        }
    }

    // Push beads
    public void PushOtherBeads(SorobanBead movingBead)
    {
        Queue<SorobanBead> beadsToPush = new Queue<SorobanBead>();
        HashSet<SorobanBead> pushedBeads = new HashSet<SorobanBead>(); // To keep track of pushed beads

        beadsToPush.Enqueue(movingBead); // Start with the moving bead
        pushedBeads.Add(movingBead); // Mark it as pushed

        while (beadsToPush.Count > 0)
        {
            SorobanBead beadToPush = beadsToPush.Dequeue(); // Get the next bead to push

            foreach (SorobanBead bead in beadsInColumn)
            {
                if (bead != beadToPush && !pushedBeads.Contains(bead))
                {
                    float distance = Mathf.Abs(bead.transform.position.y - beadToPush.transform.position.y);
                    BoxCollider2D beadCollider = bead.GetComponent<BoxCollider2D>();
                    float minPushDistance = 160f;

                    if (distance < minPushDistance)
                    {
                        // Determine the direction to push based on relative positions
                        float pushDirection = Mathf.Sign(bead.transform.position.y - beadToPush.transform.position.y);
                        // Calculate the new position for the bead being pushed
                        Vector3 pushPosition = bead.transform.position + new Vector3(0f, pushDirection * (minPushDistance - distance), 0f);

                        // Clamp the y position to be within the starting and point positions
                        pushPosition.y = Mathf.Clamp(pushPosition.y, bead.startPosition.y, bead.pointPosition.y);

                        // Apply the new position to the bead being pushed
                        bead.transform.position = pushPosition;

                        // Enqueue the bead to check if it needs to push other beads
                        beadsToPush.Enqueue(bead);
                        pushedBeads.Add(bead); // Mark it as pushed
                    }
                }
            }
        }
    }

    public void SnapBeadsToRestPosition()
    {
        bool shouldPlaydownSound = false;
        bool shouldPlayupSound = false;
        const float snapThreshold = 0.01f; // Define the threshold for snapping

        foreach (SorobanBead bead in beadsInColumn)
        {
            // Calculate the distances from the current position to the start and point positions
            float distanceToStart = Mathf.Abs(bead.transform.position.y - bead.startPosition.y);
            float distanceToPoint = Mathf.Abs(bead.transform.position.y - bead.pointPosition.y);

            // Determine the direction of the bead's movement based on the last snapped position
            bool movedFromStart = bead.lastSnappedPosition == bead.startPosition && distanceToStart >= snapThreshold;
            bool movedFromPoint = bead.lastSnappedPosition == bead.pointPosition && distanceToPoint >= snapThreshold;

            // Snap the bead based on its movement direction and distance
            if (movedFromStart)
            {
                // Snap to the point position if the bead has moved up from the start position
                bead.transform.position = new Vector3(bead.transform.position.x, bead.pointPosition.y, bead.transform.position.z);
                bead.lastSnappedPosition = bead.pointPosition; // Update the last snapped position
                shouldPlayupSound = true;
            }
            else if (movedFromPoint)
            {
                // Snap back to the start position if the bead has moved down from the point position
                bead.transform.position = new Vector3(bead.transform.position.x, bead.startPosition.y, bead.transform.position.z);
                bead.lastSnappedPosition = bead.startPosition; // Update the last snapped position
                shouldPlaydownSound = true;
            }
        }

        if (shouldPlaydownSound && downSound != null)
        {
            audioSource1.PlayOneShot(downSound);
        }
        else if (shouldPlayupSound && upSound != null)
        {
            audioSource2.PlayOneShot(upSound);
        }

        FindObjectOfType<TotalValue>().UpdateTotalValue();
        Numby.numbyup.enabled = false;
        Numby.numbyside.enabled = true;
        Numby.numbydown.enabled = false;
        Numby.numbysmile.enabled = false;
    }

    // Calculate the current soroban value, called by TotalValue script
    public int CalculateColumnValue()
    {
        const float snapThreshold = 0.01f;
        int columnValue = 0; // Start with a value of 0
        foreach (SorobanBead bead in beadsInColumn)
        {
            if (Mathf.Abs(bead.transform.position.y - bead.pointPosition.y) < snapThreshold) // Determine which beads are in their point positions
            {
                int beadValue = bead.beadType == SorobanBead.BeadType.Earth ? 1 : 5; // Set the base value of 1 or 5 based on the beadtype
                beadValue *= (int)Mathf.Pow(10, columnIndex); // Multiply this base value with a multiplier, based on which column the bead is on
                columnValue += beadValue; // Add the beads value to the column value
            }
        }
        return columnValue; // Output
    }

    // Reset beads back to their starting positions, called by TotalValue script
    public void Reset()
    {
        foreach (SorobanBead bead in beadsInColumn)
        {
            bead.transform.position = new Vector3(bead.transform.position.x, bead.startPosition.y, bead.transform.position.z);
            bead.lastSnappedPosition = bead.startPosition;
        }
    }

    // Move beads for hints and flashcards
    void MoveBeadBasedOnNumber(int beadIndex, bool shouldMove)
    {
        if (beadIndex >= 0 && beadIndex < beadsInColumn.Count && shouldMove)
        {
            SorobanBead bead = beadsInColumn[beadIndex];
            bead.transform.position = new Vector3(bead.transform.position.x, bead.pointPosition.y, bead.transform.position.z);
            bead.lastSnappedPosition = bead.pointPosition; // Update the last snapped position
        }
        else
        {
            Debug.LogError("Bead index out of range: " + beadIndex);
        }
    }

    // Method to move beads based on a single digit (0-9)
    public void SetBeadsForDigit(int digit)
    {
        // Reset the column first
        Reset();

        // If the digit is 5 or more, set the bead with index 0 (worth 5)
        if (digit >= 5)
        {
            MoveBeadBasedOnNumber(0, true); // Assuming this method moves the bead at the given index
            digit -= 5; // Subtract 5 from the digit
        }

        // Set the beads with index 1-4 (each worth 1)
        for (int i = 1; i <= digit; i++)
        {
            MoveBeadBasedOnNumber(i, true); // Move each bead for the remaining value
        }
    }
}