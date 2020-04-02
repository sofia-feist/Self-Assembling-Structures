using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUI : MonoBehaviour
{
    _3DSelfAssembly SA;
    int numberOfAgents;

    public Button playButton;
    public Button pauseButton;
    public InputField numAgentsField;
    public Slider reconfigurationSpeed;
    public Button resetButton;

    public static bool Paused;
    public static bool Reset = false;



    void Start()
    {
        SA = FindObjectOfType(typeof(_3DSelfAssembly)) as _3DSelfAssembly;   // Fancy way to do the same


        // GUI ELEMENTS
        // Play Button
        playButton.GetComponent<Button>();
        playButton.onClick.AddListener(StartReconfiguration);

        // Pause Button
        pauseButton.GetComponent<Button>();
        pauseButton.onClick.AddListener(Pause);

        // Reset Button
        resetButton.GetComponent<Button>();
        resetButton.onClick.AddListener(reset);


        // Slider
        reconfigurationSpeed.GetComponents<Slider>();
        reconfigurationSpeed.minValue = _3DSelfAssembly.maxSpeed;
        reconfigurationSpeed.maxValue = _3DSelfAssembly.minSpeed;
        reconfigurationSpeed.value = _3DSelfAssembly.minSpeed - _3DSelfAssembly.defaultSpeed;


        // Number of Agents Input Field
        numAgentsField.GetComponent<InputField>();
        if (Reset != true)
            numAgentsField.text = _3DSelfAssembly.defaultNumAgents.ToString();
        else
            numAgentsField.text = _3DSelfAssembly.NumAgents.ToString();

    }

    void Update()
    {
        // Update Slider value
        SA.currentSpeed = reconfigurationSpeed.maxValue - reconfigurationSpeed.value;
    }
    

    void StartReconfiguration()
    {
        Paused = false;
        StartCoroutine(SA.SelfAssembly());
        //StartCoroutine(SA.RandomReconfiguration());

        playButton.onClick.AddListener(Play);
        playButton.onClick.RemoveListener(StartReconfiguration);
    }


    void Play()
    {
        Paused = false;
    }


    void Pause()
    {
        Paused = true;
    }


    void reset()
    {
        Reset = true;

        if (Int32.TryParse(numAgentsField.text, out numberOfAgents))
            _3DSelfAssembly.NumAgents = numberOfAgents;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
}
