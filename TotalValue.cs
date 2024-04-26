using UnityEngine;
using UnityEngine.UI;

public class TotalValue : MonoBehaviour
{
    public SorobanColumn[] columns; // Array of all columns
    public Text totalValueText;
    public int totalValue;
    public AudioClip resetSound;
    private AudioSource audioSource;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = resetSound;
    }

    // Display current soroban value
    public void UpdateTotalValue()
    {
        totalValue = 0;
        foreach (SorobanColumn column in columns)
        {
            totalValue += column.CalculateColumnValue();
        }
        totalValueText.text = totalValue.ToString();
    }

    // Reset beads back to their starting positions
    public void ResetBeads()
    {
        foreach (SorobanColumn column in columns)
        {
            column.Reset();
            UpdateTotalValue(); // Update current value after reset
            audioSource.Play();
        }
    }

    // Move beads to represent a given number on the soroban
    public void MoveBeadsToRepresentValue(int value)
    {
        // Assuming columns are ordered from right to left (units, tens, hundreds, etc.)
        int columnIndex = 0; // Start with the units column
        while (columnIndex < columns.Length)
        {
            int digit = value % 10; // Get the digit in the current place (units, tens, etc.)
            value /= 10; // Move to the next digit to the left

            // Call the method to set the beads for the digit in the current column
            columns[columnIndex].SetBeadsForDigit(digit);

            columnIndex++; // Move to the next column to the left
        }
        // Update the total value display
        UpdateTotalValue();
    }
}
