using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class MathProblem : MonoBehaviour {
    public TotalValue totalValue;
    public HintScript hintScript;
    public Text number1Text;
    public Text number2Text;
    public Text number3Text;
    public GameObject EndScreen;
    public Text motivationaltitle;
    public Text practiceTimeTodayText;
    public Text answeredTodayText;
    public Text experienceTodayText;
    public Numby Numby;
    public Image progressBar;
    private float fillSpeed = 2f; // Speed at which progress bar is filled
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip completeSound;
    private AudioSource audioSource1;
    private AudioSource audioSource2;
    private AudioSource audioSource3;
    private Coroutine progressCoroutine;
    private float levelDuration = 15;
    private int num1, num2, num3, num4, num5, num6; // Individual digits
    private int number1, number2, number3; // Whole numbers made up of digits
    public int answer; // Result of number1 + number2 + number3
    public int correctCount; // Times a question is answered correctly
    public int attemptCount; // Times a question is answered, regardless of correctness
    private int currentQuestionAttempts; // Times the current question is answered
    private int answeredToday; // Times a unique question was answered
    private float practiceTimeToday; // Time spent practicing today
    private int experienceToday; // Experience gained today
    private DateTime lastResetDate; // Used to check if user already praciced today
    public int questionCount; // Current question number
    private int sectionIndex; // Current section
// Start
    void Start()
    {
        LoadPracticeStats();
        EndScreen.SetActive(false);
        progressBar = progressBar.GetComponent<Image>();

        //Initialize sectionIndex
        sectionIndex = PlayerPrefs.GetInt("AdditionSectionIndex", 0);

        // Reset counters, and generate a problem
        questionCount = 0;
        correctCount = 0;
        attemptCount = 0;
        currentQuestionAttempts = 0;
        totalValue.ResetBeads(); // Reset beads in advance
        GenerateProblem();

        // Handle sound & submissions
        if (audioSource1 == null)
        {
            audioSource1 = gameObject.AddComponent<AudioSource>();
        }
        audioSource1.clip = correctSound;
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource2.clip = incorrectSound;
        audioSource3 = gameObject.AddComponent<AudioSource>();
        audioSource3.clip = completeSound;
    }
// Update
    void Update()
    {
        practiceTimeToday += Time.deltaTime;
    }
// Generate a problem
    public void GenerateProblem() 
    {
        // Count the number of questions answered, and save it
        questionCount++;

        // Generate problem based on level number
        switch(sectionIndex)
        {
            case 0:
            GenerateProblem1();
            break;
            case 1:
            GenerateProblem2();
            break;
            case 2:
            GenerateProblem3();
            break;
            case 3:
            GenerateProblem4();
            break;
            case 4:
            GenerateProblem5();
            break;
            case 5: 
            GenerateProblem6();
            break;
            case 6:
            GenerateProblem7();
            break;
            case 7:
            GenerateProblem8();
            break;
            case 8:
            GenerateProblem9();
            break;
            case 9:
            GenerateProblem10();
            break;
            case 10:
            GenerateProblem11();
            break;
            case 11:
            GenerateProblem12();
            break;
        }
    }
// Level 1 - Without rules
    void GenerateProblem1()
    {
        // Generate a number based on another number, and based on if isAddition == true
        int GenerateNumberBasedOn(int number, bool isAddition)
        {
            int[] specificList;
            if (isAddition)
            {
                // Logic for num2 & num3
                specificList = number switch
                {
                    1 => new[] {+1, +2, +3, +5, +6, +7, +8},
                    2 => new[] {-1, +1, +2, +5, +6, +7},
                    3 => new[] {-2, -1, +1, +5, +6},
                    4 => new[] {-3, -2, -1, +5},
                    5 => new[] {+1, +2, +3, +4},
                    6 => new[] {-5, -1, +1, +2, +3},
                    7 => new[] {-6, -5, -2, -1, +1, +2},
                    8 => new[] {-7, -6, -5, -3, -2, -1, +1},
                    9 => new[] {-8, -7, -6, -5, -4, -3, -2, -1},
                    _ => new int[0]
                };
            }
            else
            {
                // Logic added for scalability
                specificList = number switch
                {
                    _ => new int[0]
                };
            }

            // Return a random element from the specificList
            if (specificList.Length > 0)
            {
                return specificList[UnityEngine.Random.Range(0, specificList.Length)];
            }
            else
            {
                // Handle the case where specificList is empty
                // This should not happen with the current logic, but it's good practice to handle this case.
                Debug.LogError("specificList is empty. Check the input number and logic.");
                return 0; // Or some other default value or error code
            }
        }

        // Generate numbers
        num1 = UnityEngine.Random.Range(1, 10);
        num2 = GenerateNumberBasedOn(num1, true);
        int sum = num1 + num2;
        num3 = GenerateNumberBasedOn(sum, true);
        int maxAttempts = 100; // Limit the number of while loop attempts to prevent an infinite loop
        int attempts = 0;        
        while (num2 + num3 == 0 && attempts < maxAttempts)
        {
            num3 = GenerateNumberBasedOn(sum, true);
            attempts++;
        }
        attempts = 0;
        number1 = num1;
        number2 = num2;
        number3 = num3;
        DisplayQuestion();
    }
// Level 2 - 5's Rule
    void GenerateProblem2()
    {
        // Generate a number based on another number, and based on if isAddition == true
        int GenerateNumberBasedOn(int number, bool isAddition)
        {
            int[] specificList;
            if (isAddition)
            {
                // Logic for num2 & num3
                specificList = number switch
                {
                    1 => new[] {+1, +2, +3, +4, +4, +4, +4, +4, +4, +4, +5, +6, +7},
                    2 => new[] {+1, +2, +3, +4, +3, +4, +3, +4, +3, +4, +3, +4, +5, +6},
                    3 => new[] {+1, +2, +3, +4, +2, +3, +4, +2, +3, +4, +2, +3, +4, +5},
                    4 => new[] {+1, +2, +3, +4},
                    5 => new[] {-4, -3, -2, -1, -4, -3, -2, -1, -4, -3, -2, -1, +1, +2, +3},
                    6 => new[] {-4, -3, -2, -1, -4, -3, -2, -1, +1, +2},
                    7 => new[] {-4, -3, -4, -3, -4, -3, -2, -1, +1},
                    8 => new[] {-4, -4, -4, -4, -3, -2, -1},
                    _ => new int[0]
                };
            }
            else
            {
                // Logic added for scalability
                specificList = number switch
                {
                    _ => new int[0]
                };
            }

            // Return a random element from the specificList
            if (specificList.Length > 0)
            {
                return specificList[UnityEngine.Random.Range(0, specificList.Length)];
            }
            else
            {
                // Handle the case where specificList is empty
                // This should not happen with the current logic, but it's good practice to handle this case.
                Debug.LogError("specificList is empty. Check the input number and logic.");
                return 0; // Or some other default value or error code
            }
        }
        // Generate num1
        number1 = UnityEngine.Random.Range(1, 9);
        number2 = GenerateNumberBasedOn(number1, true);
        int sum = number1 + number2;
        number3 = GenerateNumberBasedOn(sum, true);
        int maxAttempts = 100; // Limit the number of while loop attempts to prevent an infinite loop
        int attempts = 0;        
        while (number2 + number3 == 0 && attempts < maxAttempts)
        {
            number3 = GenerateNumberBasedOn(sum, true);
            attempts++;
        }
        attempts = 0;
        DisplayQuestion();
    }
// Level 3 - 10's Rule
    void GenerateProblem3()
    {
        // Generate a number based on another number and whether it's for num4/num6 (isAddition) or num3/num5 (isSubtraction)
        int GenerateNumberBasedOn(int number, bool isAddition)
        {
            int[] specificList;
            if (isAddition)
            {
                // Logic for num2
                specificList = number switch
                {
                    1 => new[] {9},
                    2 => new[] {8, 9},
                    3 => new[] {7, 8, 9},
                    4 => new[] {6, 7, 8, 9},
                    5 => new[] {5},
                    6 => new[] {5, 9},
                    7 => new[] {5, 8, 9},
                    8 => new[] {5, 7, 8, 9},
                    9 => new[] {5, 6, 7, 8, 9},
                    _ => new int[0]
                };
            }
            else
            {
                // Logic for num3
                specificList = number switch
                {
                    10 => new[] {-1, -2, -3, -4, -5, -6, -7, -8, -9},
                    11 => new[] {-2, -3, -4, -5, -7, -8, -9},
                    12 => new[] {-3, -4, -5, -8, -9},
                    13 => new[] {-4, -5, -9},
                    14 => new[] {-5},
                    15 => new[] {-6, -7, -8, -9},
                    16 => new[] {-7, -8, -9},
                    17 => new[] {-8, -9},
                    18 => new[] {-1, -2, -3, -5, -6, -7, -8}, // Prevent +9 -9
                    _ => new int[0]
                };
            }

            // Return a random element from the specificList
            if (specificList.Length > 0)
            {
                return specificList[UnityEngine.Random.Range(0, specificList.Length)];
            }
            else
            {
                // Handle the case where specificList is empty
                // This should not happen with the current logic, but it's good practice to handle this case.
                Debug.LogError("specificList is empty. Check the input number and logic.");
                return 0; // Or some other default value or error code
            }
        }
        // Generate num1
        number1 = UnityEngine.Random.Range(1, 10);
        number2 = GenerateNumberBasedOn(number1, true);
        int sum = number1 + number2;
        number3 = GenerateNumberBasedOn(sum, false);
        if (sum == 14 && num2 + num3 == 0)
        {
            num3 = UnityEngine.Random.Range(-4, 0);
        }
        else
        {
            int maxAttempts = 100; // Limit the number of while loop attempts to prevent an infinite loop
            int attempts = 0;        
            while (number2 + number3 == 0 && attempts < maxAttempts)
            {
                number3 = GenerateNumberBasedOn(sum, false);
                attempts++;
            }
            attempts = 0;
        }
        DisplayQuestion();
    }
// Level 4 - Randomize 5's & 10's rule
    void GenerateProblem4()
    {
        if (UnityEngine.Random.Range(0, 2) == 0) // UnityEngine.Randomly returns 0 or 1
        {
            GenerateProblem2();
        }
        else
        {
            GenerateProblem3();
        }
    }
// Level 5 - Double digits, no rules with 5's rule mixed
    void GenerateProblem5()
    {
        // Generate a number based on another number and whether it's for num4/num6 (isAddition) or num3/num5 (isSubtraction)
        int GenerateNumberBasedOn(int number, bool isAddition)
        {
            int[] specificList;
            if (isAddition)
            {
                // Logic for addition
                specificList = number switch
                {
                    1 => new[] {+1, +2, +3, +4, +5, +6, +7},
                    2 => new[] {+1, +2, +3, +4, +5, +6},
                    3 => new[] {+1, +2, +3, +4, +5},
                    4 => new[] {+1, +2, +3, +4},
                    5 => new[] {+1, +2, +3},
                    6 => new[] {+1, +2},
                    7 => new[] {+1},
                    8 => new[] {0},
                    _ => new int[0]
                };
            }
            else
            {
                // Logic for subtraction
                specificList = number switch
                {
                    1 => new[] {0},
                    2 => new[] {-1},
                    3 => new[] {-1, -2},
                    4 => new[] {-1, -2, -3},
                    5 => new[] {-1, -2, -3, -4},
                    6 => new[] {-1, -2, -3, -4, -5},
                    7 => new[] {-1, -2, -3, -4, -5, -6},
                    8 => new[] {-1, -2, -3, -4, -5, -6, -7},
                    _ => new int[0]
                };
            }

            // Return a random element from the specificList
            if (specificList.Length > 0)
            {
                return specificList[UnityEngine.Random.Range(0, specificList.Length)];
            }
            else
            {
                // Handle the case where specificList is empty
                // This should not happen with the current logic, but it's good practice to handle this case.
                Debug.LogError("specificList is empty. Check the input number and logic.");
                return 0; // Or some other default value or error code
            }
        }
        // Generate nums
        num1 = UnityEngine.Random.Range(1, 9);
        num2 = UnityEngine.Random.Range(1, 9);
        if (UnityEngine.Random.Range(0, 2) == 0) // UnityEngine.Randomly decide if number2 should be + or -
        {
            num3 = num1 + num2 == 16 ? GenerateNumberBasedOn(num1, false) : GenerateNumberBasedOn(num1, true); // Check if num1 and num2 don't result in 88, to prevent number3 displaying 00
            num4 = num1 + num2 == 16 ? GenerateNumberBasedOn(num2, false) : GenerateNumberBasedOn(num2, true); // Check if num1 and num2 don't result in 88, to prevent number3 displaying 00
        }
        else // Start with subtraction, unless num1&num2 == 11
        {
            num3 = num1 + num2 == 2 ? GenerateNumberBasedOn(num1, true) : GenerateNumberBasedOn(num1, false); // Check if num1 and num2 don't result in 11, to prevent number3 displaying 00
            num4 = num1 + num2 == 2 ? GenerateNumberBasedOn(num2, true) : GenerateNumberBasedOn(num2, false); // Check if num1 and num2 don't result in 11, to prevent number3 displaying 00
        }
        int sum = num1 + num3;
        int sum2 = num2 + num4;
        if (UnityEngine.Random.Range(0, 2) == 0) // UnityEngine.Randomly decide if number3 should be + or -
        {
            num5 = sum + sum2 == 16 ? GenerateNumberBasedOn(sum, false) : GenerateNumberBasedOn(sum, true); // Check if sum1 and sum2 don't result in 88, to prevent number3 displaying 00
            num6 = sum + sum2 == 16 ? GenerateNumberBasedOn(sum2, false) : GenerateNumberBasedOn(sum2, true); // Check if sum1 and sum2 don't result in 88, to prevent number3 displaying 00
        }
        else // Continue with subtraction, unless sum1&sum2 == 11
        {
            num5 = sum + sum2 == 2 ? GenerateNumberBasedOn(sum, true) : GenerateNumberBasedOn(sum, false); // Check if sum1 and sum2 don't result in 11, to prevent number3 displaying 00
            num6 = sum + sum2 == 2 ? GenerateNumberBasedOn(sum2, true) : GenerateNumberBasedOn(sum2, false); // Check if sum1 and sum2 don't result in 11, to prevent number3 displaying 00
        }

        number1 = num1 * 10 + num2;
        number2 = num3 * 10 + num4;
        number3 = num5 * 10 + num6;
        DisplayQuestion();
    }
// Level 6 - Double digits, no rules and 10's rule mixed
    void GenerateProblem6()
        {
            // Generate a number based on another number and whether it's for num4/num6 (isAddition) or num3/num5 (isSubtraction)
            int GenerateNumberBasedOn(int number, bool isAddition)
            {
                int[] specificList;
                if (isAddition)
                {
                    // Logic for num3 & num4
                    specificList = number switch
                    {
                        1 => new[] {1, 2, 5, 6, 7, 9},
                        2 => new[] {1, 5, 6, 8, 9},
                        3 => new[] {5, 7, 8, 9},
                        4 => new[] {6, 7, 8, 9},
                        5 => new[] {1, 2, 3, 5},
                        6 => new[] {1, 2, 5, 9},
                        7 => new[] {1, 5, 8, 9},
                        8 => new[] {5, 7, 8, 9},
                        9 => new[] {6, 7, 8, 9}, // 5 may have to be added/removed
                        _ => new int[0]
                    };
                }
                else
                {
                    // Logic for num 5 & num6
                    specificList = number switch
                    {
                        1 => new[] {0}, // Shouldn't occur
                        2 => new[] {-1},
                        3 => new[] {-1, -2},
                        4 => new[] {-1, -2, -3},
                        5 => new[] {0}, // Shouldn't occur
                        6 => new[] {-5},
                        7 => new[] {-1, -5, -6},
                        8 => new[] {-1, -2, -5, -6, -7},
                        9 => new[] {-1, -2, -3, -5, -6, -7, -8},
                        10 => new[] {-1, -2, -3, -4, -5, -6, -7, -8, -9},
                        11 => new[] {-2, -3, -4, -5, -7, -8, -9},
                        12 => new[] {-3, -4, -5, -8, -9},
                        13 => new[] {-4, -5, -9},
                        14 => new[] {-5},
                        15 => new[] {-6, -7, -8, -9},
                        16 => new[] {-7, -8, -9},
                        17 => new[] {-8, -9},
                        18 => new[] {-9},
                        19 => new[] {-1, -2, -3, -5, -6, -7, -8},
                        _ => new int[0]
                    };
                }

                // Return a random element from the specificList
                if (specificList.Length > 0)
                {
                    return specificList[UnityEngine.Random.Range(0, specificList.Length)];
                }
                else
                {
                    // Handle the case where specificList is empty
                    // This should not happen with the current logic, but it's good practice to handle this case.
                    Debug.LogError("specificList is empty. Check the input number and logic.");
                    return 0; // Or some other default value or error code
                }
            }
            // Generate nums
            num1 = UnityEngine.Random.Range(1, 10);
            num2 = UnityEngine.Random.Range(1, 10);
            num3 = GenerateNumberBasedOn(num1, true);
            num4 = GenerateNumberBasedOn(num2, true);
            int sum = num1 + num3;
            int sum2 = num2 + num4;
            num5 = sum2 > 9 ? GenerateNumberBasedOn(sum +1, false) : GenerateNumberBasedOn(sum, false); // Take the added +10 into account generated by num2 + num4
            num6 = GenerateNumberBasedOn(sum2, false);

            number1 = num1 * 10 + num2;
            number2 = num3 * 10 + num4;
            number3 = num5 * 10 + num6;
            DisplayQuestion();
        }
// Level 7 - Double digits, no rules with 5's rule && 10's rule mixed
    void GenerateProblem7()
        {
            // Generate a number based on another number and whether it's for num4/num6 (isAddition) or num3/num5 (isSubtraction)
            int GenerateNumberBasedOn(int number, bool isAddition)
            {
                int[] specificList;
                if (isAddition)
                {
                    // Logic for num3 & num4
                    specificList = number switch
                    {
                        1 => new[] {1, 2, 5, 6, 7, 9},
                        2 => new[] {1, 4, 5, 6, 8, 9},
                        3 => new[] {3, 4, 5, 7, 8, 9},
                        4 => new[] {2, 3, 4, 6, 7, 8, 9},
                        5 => new[] {1, 2, 3, 5},
                        6 => new[] {1, 2, 5, 9},
                        7 => new[] {1, 5, 8, 9},
                        8 => new[] {5, 7, 8, 9},
                        9 => new[] {6, 7, 8, 9},
                        _ => new int[0]
                    };
                }
                else
                {
                    // Logic for num 5 & num6
                    specificList = number switch
                    {
                        1 => new[] {0}, // Shouldn't occur
                        2 => new[] {-1},
                        3 => new[] {-1, -2},
                        4 => new[] {-1, -2, -3},
                        5 => new[] {0}, // Shouldn't occur
                        6 => new[] {-2, -3, -4, -5},
                        7 => new[] {-1, -3, -4, -5, -6},
                        8 => new[] {-1, -2, -4, -5, -6, -7},
                        9 => new[] {-1, -2, -3, -5, -6, -7, -8},
                        10 => new[] {-1, -2, -3, -4, -5, -6, -7, -8, -9},
                        11 => new[] {-2, -3, -4, -5, -7, -8, -9},
                        12 => new[] {-3, -4, -5, -8, -9},
                        13 => new[] {-4, -5, -9},
                        14 => new[] {-5},
                        15 => new[] {-6, -7, -8, -9},
                        16 => new[] {-7, -8, -9},
                        17 => new[] {-8, -9},
                        18 => new[] {-9},
                        19 => new[] {-1, -2, -3, -5, -6, -7, -8},
                        _ => new int[0]
                    };
                }

                // Return a random element from the specificList
                if (specificList.Length > 0)
                {
                    return specificList[UnityEngine.Random.Range(0, specificList.Length)];
                }
                else
                {
                    // Handle the case where specificList is empty
                    // This should not happen with the current logic, but it's good practice to handle this case.
                    Debug.LogError("specificList is empty. Check the input number and logic.");
                    return 0; // Or some other default value or error code
                }
            }
            // Generate nums
            num1 = UnityEngine.Random.Range(1, 10);
            num2 = UnityEngine.Random.Range(1, 10);
            num3 = GenerateNumberBasedOn(num1, true);
            num4 = GenerateNumberBasedOn(num2, true);
            int sum = num1 + num3;
            int sum2 = num2 + num4;
            num5 = sum2 > 9 ? GenerateNumberBasedOn(sum +1, false) : GenerateNumberBasedOn(sum, false); // Take the added +10 into account generated by num2 + num4
            num6 = GenerateNumberBasedOn(sum2, false);

            number1 = num1 * 10 + num2;
            number2 = num3 * 10 + num4;
            number3 = num5 * 10 + num6;
            DisplayQuestion();
        }
// Level 8 - Combining rules (10's finish with 5's)
    void GenerateProblem8()
    {
        // Generate a number based on another number and whether it's for num4/num6 (isAddition) or num3/num5 (isSubtraction)
        int GenerateNumberBasedOn(int number, bool isAddition)
        {
            int[] specificList;
            if (isAddition)
            {
                // Logic for num4 and num6
                specificList = number switch
                {
                    1 => new[] {9},
                    2 => new[] {8, 9},
                    3 => new[] {7, 8, 9},
                    4 => new[] {6, 7, 8, 9},
                    5 => new[] {5},
                    6 => new[] {4, 5, 9},
                    7 => new[] {3, 4, 5, 8, 9},
                    8 => new[] {2, 3, 4, 5, 7, 8, 9},
                    9 => new[] {1, 2, 3, 4, 5, 6, 7, 8, 9},
                    _ => new int[0]
                };
            }
            else
            {
                // Logic for num3
                specificList = number switch
                {
                    5 => new[] {-1, -2, -3, -4},
                    6 => new[] {-2, -3, -4, -5},
                    7 => new[] {-3, -4, -5, -6},
                    8 => new[] {-4, -5, -6, -7},
                    9 => new[] {-5, -6, -7, -8},
                    _ => new int[0]
                };
            }

            // Return a random element from the specificList
            if (specificList.Length > 0)
            {
                return specificList[UnityEngine.Random.Range(0, specificList.Length)];
            }
            else
            {
                // Handle the case where specificList is empty
                // This should not happen with the current logic, but it's good practice to handle this case.
                Debug.LogError("specificList is empty. Check the input number and logic.");
                return 0; // Or some other default value or error code
            }
        }
        // Generate num1
        num1 = UnityEngine.Random.Range(1, 10);

        if (num1 < 5) // Sum starts with addition
        {
            num2 = UnityEngine.Random.Range(1, 10);
            num3 = 4 - num1;
            num4 = GenerateNumberBasedOn(num2, true);
            int sum = num2 + num4;
            num5 = UnityEngine.Random.Range(-9, 10 - sum); // -10 < num5 < (10 - sum), always between -9 and -1 // Previously set by GenerateNumberBasedOn(sum, false)
            num6 = 0; // Don't display num6

            number1 = num1 * 10 + num2;
            number2 = num3 * 10 + num4;
            number3 = num5;
        }
        else // Sum starts with subtraction
        {
            num2 = UnityEngine.Random.Range(1, 5);
            num3 = GenerateNumberBasedOn(num1, false);
            num4 = UnityEngine.Random.Range(-num2 - 5, -5); // 0 < num2 < 5 && 5 < num4 < (num2 + 6)
            num5 = 4 - (num1 + num3);
            int sum = 10 + num2 + num4;
            num6 = GenerateNumberBasedOn(sum, true);

            number1 = num1 * 10 + num2;
            number2 = num3 * 10 + num4;
            number3 = num5 * 10 + num6;
        }
        DisplayQuestion();
    }
// Level 9 - Combining rules (10's finish with double 5's)
    void GenerateProblem9()
    {
        // Generate num1
        num1 = UnityEngine.Random.Range(1, 10);

        if (num1 < 5) // Sum starts with addition
        {
            num2 = UnityEngine.Random.Range(5, 9); // positive number
            num3 = 4 - num1; // positive number
            num4 = UnityEngine.Random.Range(6, 15 - num2); // 0 < num2 < 5 && 5 < num4 < (15 - num2) // positive number
            int sum = num2 + num4 - 10; // positive number
            num5 = UnityEngine.Random.Range(-5 - sum, -5); // 0 < num2 < 5 && (5 - num2) <= num4 < 5 // negative number
            //if (num4 > 6)
            //{
            //    while (num4 + num5 == 0) // Make sure num4 and num5 don't cancel out
            //    {
            //        num5 = UnityEngine.Random.Range(-5 - sum, -5);
            //    }
            //}
        }
        else // Sum starts with subtraction
        {
            num2 = UnityEngine.Random.Range(1, 5); // positive number
            num3 = 5 - num1; // negative number
            num4 = UnityEngine.Random.Range(-5 - num2, -5); // 0 < num2 < 5 && (5 - num2) <= num4 < 5 // negative number
            int sum = 10 + (num2 + num4); // sum is a positive number
            num5 = UnityEngine.Random.Range(6, 15 - sum); // 0 < num2 < 5 && 5 < num4 < (15 - sum) // positive number
            //if (num4 < -6)
            //{
            //    while (num4 + num5 == 0) // Make sure num4 and num5 don't cancel out
            //    {
            //        num5 = UnityEngine.Random.Range(6, 15 - sum);
            //    }
            //}
        }
        number1 = num1 * 10 + num2;
        number2 = num3 * 10 + num4;
        number3 = num5;
        DisplayQuestion();
    }
// Level 10 - Combining rules (Double 10's)
    void GenerateProblem10()
    {
        int GenerateNumberBasedOn(int number, bool isAddition)
        {
            int[] specificList;
            if (isAddition)
            {
                // Logic for num4 and num5
                specificList = number switch
                {
                    1 => new[] {9},
                    2 => new[] {8, 9},
                    3 => new[] {7, 8, 9},
                    4 => new[] {6, 7, 8, 9},
                    5 => new[] {5},
                    6 => new[] {4, 5, 9},
                    7 => new[] {3, 4, 5, 8, 9},
                    8 => new[] {2, 3, 4, 5, 7, 8, 9},
                    9 => new[] {1, 2, 3, 4, 5, 6, 7, 8, 9},
                    _ => new int[0]
                };
            }
            else
            {
                // Logic for scalability
                specificList = number switch
                {
                    _ => new int[0]
                };
            }

            // Return a random element from the specificList
            if (specificList.Length > 0)
            {
                return specificList[UnityEngine.Random.Range(0, specificList.Length)];
            }
            else
            {
                // Handle the case where specificList is empty
                // This should not happen with the current logic, but it's good practice to handle this case.
                Debug.LogError("specificList is empty. Check the input number and logic.");
                return 0; // Or some other default value or error code
            }
        }
        // Generate num1
        num1 = UnityEngine.Random.Range(1, 19);
        
        if (num1 < 10) // Sum starts with addition
        {
            num2 = UnityEngine.Random.Range(1, 10);
            num3 = 9 - num1; // positive number
            num4 = GenerateNumberBasedOn(num2, true); // positive number
            int sum = num2 + num4; // sum is positive
            num5 = UnityEngine.Random.Range(-9, 10 - sum); // negative number // -10 < num5 < (10 - sum), always between -9 and -1 // Previously set by GenerateNumberBasedOn(sum, false)       
            //if (num4 < 9)
            //{
            //    while (num4 + num5 == 0) // make sure num4 and num5 don't cancel out
            //   {
            //        num5 = UnityEngine.Random.Range(-9, 10 - sum);
            //    }
            //}         
        }
        else
        {
            num2 = UnityEngine.Random.Range(1, 9);
            num3 = -(num1 - 10); // negative number
            num4 = UnityEngine.Random.Range(-9, -num2); // negative number // -10 < num4 < -(num2), always between -9 and -1 // Previously set by GenerateNumberBasedOn(num2, false)
            int sum = 10 + num2 + num4; // sum is positive
            num5 = GenerateNumberBasedOn(sum, true); // positive number
            //if (num4 > -9)
            //{
            //    while (num4 + num5 == 0) // Make sure num4 and num5 don't cancel out
            //    {
            //        num5 = GenerateNumberBasedOn(sum, true);
            //    }
            //}  
        }
        number1 = num1 * 10 + num2;
        number2 = num3 * 10 + num4;
        number3 = num5;
        DisplayQuestion();
    }
// Level 11 - Random 2 digit problems
    void GenerateProblem11()
    {
        number1 = UnityEngine.Random.Range(10, 100);
        number2 = UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(-number1 + 2, 0) : UnityEngine.Random.Range(1, 100); // UnityEngine.Randomize addition and subtraction, whilst preventing a number2 == 0
        number3 = UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(-(number1 + number2), 0) : UnityEngine.Random.Range(1, 100); // UnityEngine.Randomize addition and subtraction, whilst preventing a number3 == 0

        int maxAttempts = 1000; // Limit the number of while loop attempts to prevent an infinite loop
        int attempts = 0;        
        while ((number1 + number2 + number3 == 0 || number2 + number3 == 0) && attempts < maxAttempts)
        {
            number3 = UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(-(number1 + number2), 0) : UnityEngine.Random.Range(1, 100);
            attempts++;
        }
        attempts = 0;
        DisplayQuestion();
    }
// Level 12 - Random 3 digit problems
    void GenerateProblem12()
    {
        number1 = UnityEngine.Random.Range(100, 1000);
        number2 = UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(-number1 + 20, 0) : UnityEngine.Random.Range(100, 1000); // UnityEngine.Randomize addition and subtraction, whilst preventing a number2 == 0
        number3 = UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(-(number1 + number2), 0) : UnityEngine.Random.Range(100, 1000); // UnityEngine.Randomize addition and subtraction, whilst preventing a number3 == 0

        int maxAttempts = 1000; // Limit the number of while loop attempts to prevent an infinite loop
        int attempts = 0;        
        while ((number1 + number2 + number3 == 0 || number2 + number3 == 0) && attempts < maxAttempts)
        {
            number3 = UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(-(number1 + number2), 0) : UnityEngine.Random.Range(100, 1000);
            attempts++;
        }
        attempts = 0;
        DisplayQuestion();
    }
// Display question
    void DisplayQuestion()
    {
        string sign1 = number2 >= 0 ? " +" : " ";
        string sign2 = number3 >= 0 ? " +" : " ";
        number1Text.text = $"{number1}";
        number2Text.text = sign1 + number2;
        number3Text.text = sign2 + number3;
        answer = number1 + number2 + number3;
    }
// Show hint
    public void GenerateHint()
    {
        hintScript.ShowHint(number1, number2, number3);
    }
// Today's practice stats
    private void LoadPracticeStats()
    {
        // Load saved practice stats and the date when it was last reset
        practiceTimeToday = PlayerPrefs.GetFloat("practiceTimeToday", 0);
        answeredToday = PlayerPrefs.GetInt("answeredToday", 0);
        experienceToday = PlayerPrefs.GetInt("experienceToday", 0);
        lastResetDate = DateTime.Parse(PlayerPrefs.GetString("lastResetDate", DateTime.UtcNow.ToString()));

        // Check if the last reset date is not today
        if (lastResetDate.Date != DateTime.UtcNow.Date)
        {
            // Reset the practice time if it's a new day
            practiceTimeToday = 0;
            answeredToday = 0;
            experienceToday = 0;
            lastResetDate = DateTime.UtcNow;
            SavePracticeStats();
        }
    }
    
    private void SavePracticeStats()
    {
        PlayerPrefs.SetFloat("practiceTimeToday", practiceTimeToday);
        PlayerPrefs.SetInt("answeredToday", answeredToday);
        PlayerPrefs.SetInt("experienceToday", experienceToday);
        PlayerPrefs.SetString("lastResetDate", lastResetDate.ToString());
        PlayerPrefs.Save();
    }
    
    private void DisplayEndStats()
    {
        float fromTime = PlayerPrefs.GetFloat("practiceTimeToday", 0);
        float toTime = practiceTimeToday;
        int fromAnswered = PlayerPrefs.GetInt("answeredToday", 0);
        answeredToday += correctCount; // Count today's total unique problems
        int fromExperience = PlayerPrefs.GetInt("experienceToday", 0);
        experienceToday += (((int)toTime - (int)fromTime)/2) + (attemptCount/2); // Skillpoints based on how long the player took to complete the level and how many attempts were made to answer
        int toExperience = experienceToday;
        StartCoroutine(AnimateStats(fromTime, toTime, fromAnswered, answeredToday, fromExperience, toExperience));
    }

    private IEnumerator AnimateStats(float fromTime, float toTime, int fromAnswered, int toAnswered, int fromExperience, int toExperience)
    {
        // Start with fromStats
        TimeSpan fromTimeSpan = TimeSpan.FromSeconds(fromTime);
        practiceTimeTodayText.text = fromTime >= 3600 ? fromTimeSpan.ToString(@"h\:mm") + "u" : fromTimeSpan.ToString(@"m\:ss");
        answeredTodayText.text = $"{(int)fromAnswered}";
        experienceTodayText.text = $"{(int)fromExperience}";
        yield return new WaitForSeconds(0.2f);

        float duration = 1.5f;
        float counter = 0f;
        while (counter < duration) // Animate timer
        {
            counter += Time.deltaTime;
            float timeIncrements = Mathf.Lerp(fromTime, toTime, counter / duration);
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeIncrements);
            practiceTimeTodayText.text = toTime >= 3600 ? timeSpan.ToString(@"h\:mm") + "u" : timeSpan.ToString(@"m\:ss");
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        counter = 0f; // Reset counter
        while (counter < duration) // Animate answered count
        {
            counter += Time.deltaTime;
            float answeredIncrements = Mathf.Lerp(fromAnswered, toAnswered, counter / duration);
            answeredTodayText.text = $"{(int)answeredIncrements}";
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        counter = 0f; // Reset counter
        while (counter < duration) // Animate skill points
        {
            counter += Time.deltaTime;
            float experienceIncrements = Mathf.Lerp(fromExperience, toExperience, counter / duration);
            experienceTodayText.text = $"{(int)experienceIncrements}";
            yield return null;
        }

        // Ensure the final value is set
        TimeSpan finalTimeSpan = TimeSpan.FromSeconds(toTime);
        practiceTimeTodayText.text = toTime >= 3600 ? finalTimeSpan.ToString(@"h\:mm") + "u" : finalTimeSpan.ToString(@"m\:ss");
        answeredTodayText.text = $"{toAnswered}";
        experienceTodayText.text = $"{toExperience}";
        yield break;
    }
// Submit answer
    public void SubmitAnswer()
    {
        // If answer is correct
        if(totalValue.totalValue == answer)
        {
            correctCount++; // Add a point
            attemptCount++; // Count total attempts
            if (questionCount < levelDuration)
            {
                audioSource1.Play(); // Play correctSound
                totalValue.ResetBeads();
            }
            currentQuestionAttempts = 0; // Reset currenQuestionAttempt
            Numby.numbyup.enabled = false;
            Numby.numbyside.enabled = false;
            Numby.numbydown.enabled = false;
            Numby.numbysmile.enabled = true;            

            float progress = (float)questionCount / levelDuration; // Calculate progression
            StartCoroutine(AnimateFill(progress)); // Update progress bar

            GenerateProblem(); // Create a new problem
        }
        else
        {
            audioSource2.Play(); // Play incorrectSound
            attemptCount++; // Count total attempts
            currentQuestionAttempts++; // Count attempts of this specific problem
            if (currentQuestionAttempts == 3) // Automatically provide a hint
            {
                GenerateHint();
            }
        }

        // Lesson completed: Display the stats and save it
        if (questionCount > levelDuration)
        { 
            audioSource3.Play(); // Play completeSound
            EndScreen.SetActive(true); // Display endscreen containing stats
            
            int score = correctCount / attemptCount * 100; // Calculate score before adjusting attemptCount in DisplayEndStats()
            DisplayEndStats();
            SavePracticeStats();
            
            // Display a motivational title based on performance
            switch ((int)score)
            {
                case int n when (n > 80):
                    motivationaltitle.text = "Ga zo door!";
                    break;
                case int n when (n > 50):
                    motivationaltitle.text = "Weer een stapje verder!";
                    break;
                default:
                    motivationaltitle.text = "Wat een doorzettingsvermogen!";
                    break;
            }
            // Other options:
            // Je bent weer een stap verder!
            // Blijf zo doorgaan!
            // Je maakt (geweldige) vooruitgang!
            // Klaar voor de volgende uitdaging?
            // Op naar de volgende!
            // Op naar meesterschap!
            // Je bent op de goede weg!
            // Blijf oefenen!

            // Update and save level progress
            int globalLevelIndex = PlayerPrefs.GetInt("AdditionGlobalLevelIndex", 0);
            PlayerPrefs.SetInt("AdditionLevel" + globalLevelIndex + "_completed", 1); // Set the level as completed
            PlayerPrefs.SetInt("AdditionLevel" + (globalLevelIndex + 1) + "_unlocked", 1); // Unlock the next level
            PlayerPrefs.Save();
        }
    }
    private IEnumerator AnimateFill(float progress) // Called in SubmitAnswer() to animate the progress bar
    {
        int maxAttempts = 150; // Limit the number of while loop attempts to prevent an infinite loop
        int attempts = 0;
        float threshold = 0.01f; // Stop moving the progress bar when within this threshold
        // Continue to loop until the fill amount is approximately equal to the target
        while (Mathf.Abs(progressBar.fillAmount - progress) > threshold && attempts < maxAttempts)
        {
            attempts++;
            // Lerp the fill amount to create a smooth transition
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, progress, fillSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }
        attempts = 0;
        progressBar.fillAmount = progress;
    }
// Complete Lesson
    public void CompleteLesson()
    {
        SceneManager.LoadScene("MainMenu 1");
    }
// Exit lesson
    public void ExitLesson() // Lesson ended manually
    {
        SavePracticeStats();
        SceneManager.LoadScene("MainMenu 1");
    }
}
