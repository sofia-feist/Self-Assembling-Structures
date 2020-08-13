using System;
using System.IO;
using UnityEngine;
//using UnityEditor;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;


public class GUI : MonoBehaviour
{
    CommonMethods CM = new CommonMethods();

    _3DSelfAssembly SA;
    int numberOfAgents;

    public Button playButton;
    public Button pauseButton;
    public InputField numAgentsField;
    public Slider reconfigurationSpeed;
    public Button resetButton;

    public Button minimizeButton;
    public Button quitButton;

    DefaultButton DefaultButtonClass;
    StoolButton StoolButtonClass;
    ChairButton ChairButtonClass;
    TableButton TableButtonClass;
    HouseButton HouseButtonClass;

    public static GameObject GoalShape;
    public GameObject Chair;
    public GameObject Stool;
    public GameObject Table;
    public GameObject House;
    GameObject CustomShape;

    public Button defaultShapeButton;
    public Button chairButton;
    public Button stoolButton;
    public Button tableButton;
    public Button houseButton;
    public Button customShapeButton;

    Color selectedButton = new Color(31 / 255f, 124 / 255f, 231 / 255f);

    public static bool Paused;
    public static bool Reset = false;
    public static bool SuggestedShapeSelected = false;



    // Minimize functionality
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();





    void Start()
    {
        SA = FindObjectOfType(typeof(_3DSelfAssembly)) as _3DSelfAssembly;


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


        // Minimize Button
        minimizeButton.GetComponent<Button>();
        minimizeButton.onClick.AddListener(Minimize);

        // Quit Button
        quitButton.GetComponent<Button>();
        quitButton.onClick.AddListener(Quit);

        

        // Slider
        reconfigurationSpeed.GetComponents<Slider>();
        reconfigurationSpeed.minValue = _3DSelfAssembly.maxSpeed;
        reconfigurationSpeed.maxValue = _3DSelfAssembly.minSpeed;
        reconfigurationSpeed.value = _3DSelfAssembly.minSpeed - _3DSelfAssembly.defaultSpeed;
        //reconfigurationSpeed.minValue = 1f;
        //reconfigurationSpeed.maxValue = 0.02f;
        //reconfigurationSpeed.value = 0.5f;


        // Number of Agents Input Field
        numAgentsField.GetComponent<InputField>();
        if (SuggestedShapeSelected != true && Reset != true)
            numAgentsField.text = _3DSelfAssembly.defaultNumAgents.ToString();
        else
            numAgentsField.text = _3DSelfAssembly.NumAgents.ToString();




        // GOAL SHAPE SELECTORS
        // Default Goal Shape
        if (SuggestedShapeSelected != true && Reset != true) GoalShapeButton.Shape = SelectedGoalShape.Default;


        // Goal Shape Buttons
        // Default Shape
        defaultShapeButton.GetComponent<Button>();
        DefaultButtonClass = new DefaultButton();
        if(GoalShapeButton.Shape == SelectedGoalShape.Default) defaultShapeButton.image.color = selectedButton;
        defaultShapeButton.onClick.AddListener(() => ChangeGoalShape(SA.DefaultShape, DefaultButtonClass));

        // Chair
        chairButton.GetComponent<Button>();
        ChairButtonClass = new ChairButton();
        if (GoalShapeButton.Shape == SelectedGoalShape.Chair) chairButton.image.color = selectedButton;
        chairButton.onClick.AddListener(() => ChangeGoalShape(Chair, ChairButtonClass));

        // Stool
        stoolButton.GetComponent<Button>();
        StoolButtonClass = new StoolButton();
        if (GoalShapeButton.Shape == SelectedGoalShape.Stool) stoolButton.image.color = selectedButton;
        stoolButton.onClick.AddListener(() => ChangeGoalShape(Stool, StoolButtonClass));

        // Table
        tableButton.GetComponent<Button>();
        TableButtonClass = new TableButton();
        if (GoalShapeButton.Shape == SelectedGoalShape.Table) tableButton.image.color = selectedButton;
        tableButton.onClick.AddListener(() => ChangeGoalShape(Table, TableButtonClass));

        // House
        houseButton.GetComponent<Button>();
        HouseButtonClass = new HouseButton();
        if (GoalShapeButton.Shape == SelectedGoalShape.House) houseButton.image.color = selectedButton;
        houseButton.onClick.AddListener(() => ChangeGoalShape(House, HouseButtonClass));

        //Custom
        customShapeButton.GetComponent<Button>();
        if (GoalShapeButton.Shape == SelectedGoalShape.Custom) customShapeButton.image.color = selectedButton;
        //customShapeButton.onClick.AddListener(CustomisedGoalShape);
    }

    void Update()
    {
        // Update Slider value
        SA.currentSpeed = reconfigurationSpeed.maxValue - reconfigurationSpeed.value;
        //Time.fixedDeltaTime = reconfigurationSpeed.maxValue - reconfigurationSpeed.value;
        //Debug.Log(Time.timeScale.ToString());


        // QUIT APP
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        
    }



    // StartReconfiguration: Function triggered by the Play button when pressed for the first time
    void StartReconfiguration()
    {
        Paused = false;
        StartCoroutine(SA.SelfAssembly());
        //StartCoroutine(SA.RandomReconfiguration());

        playButton.onClick.AddListener(Play);
        playButton.onClick.RemoveListener(StartReconfiguration);
    }


    // Play: Function triggered by the Play button
    void Play()
    {
        Paused = false;
    }


    // Pause: Function triggered by the Pause button
    void Pause()
    {
        Paused = true;
    }


    // reset: Function triggered by the Reset button
    void reset()
    {
        Reset = true;
        SuggestedShapeSelected = false;

        if (Int32.TryParse(numAgentsField.text, out numberOfAgents))
            _3DSelfAssembly.NumAgents = numberOfAgents;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // Minimize: Function triggered by the Minimize button
    void Minimize()
    {
        ShowWindow(GetActiveWindow(), 2);
    }


    // Quit: Function triggered by the Quit button
    void Quit()
    {
        Application.Quit();
    }


    // ChangeGoalShape: Function triggered by the different Goal Shape Buttons
    void ChangeGoalShape(GameObject ChosenShape, GoalShapeButton PressedButton)
    {
        Reset = false;
        GoalShape = ChosenShape;
        SuggestedShapeSelected = true;

        GoalShapeButton.Shape = PressedButton.SelectedShape();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // CustomisedGoalShape: Function triggered by the Custom Goal Shape button   <-  DISABLED FOR NOW BECAUSE IT'S NOT WORKING PROPERLY YET
    //void CustomisedGoalShape() 
    //{
    //    string sourceFile = EditorUtility.OpenFilePanel("Select Unity compatible Model file", "", ".fbx;*.dae;*.3ds;*.dxf;*.obj");

    //    if (sourceFile.Length != 0)
    //    {
    //        Reset = true;
    //        SuggestedShapeSelected = true;

    //        // Copy Source file to Unity Asset Folder
    //        string fileName = Path.GetFileName(sourceFile);
    //        string destinationPath = Application.dataPath + "/Resources/";
    //        string destinationFile = destinationPath + fileName;
    //        File.Copy(sourceFile, destinationFile, true);

    //        // Load and prepare new Asset 
    //        CustomShape = Resources.Load<GameObject>(Path.GetFileNameWithoutExtension(destinationFile));
    //        GameObject customShapeInstance = Instantiate(CustomShape, Vector3.zero, Quaternion.identity);
    //        CM.SetupColliders(customShapeInstance);

    //        // ReScale to Grid Area
    //        int targetSize = 100;
    //        Vector3 size = CM.GeometrySize(customShapeInstance).size;
    //        float importedSize = new float[] { size.x, size.y, size.z }.Max();
    //        Vector3 newScale = customShapeInstance.transform.localScale;
    //        newScale.y = targetSize * newScale.y / importedSize;
    //        customShapeInstance.transform.localScale = newScale;

    //        // Assign Goal Shape to imported and adjusted asset
    //        GoalShape = customShapeInstance;
    //        GoalShapeButton.Shape = SelectedGoalShape.Custom;

    //        // Reset
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //    }
    //}
}




// SELECTED GOAL SHAPE: Possible Goal Shapes selected
public enum SelectedGoalShape
{ 
    Default,
    Stool,
    Chair,
    Table,
    House,
    Custom
}



// GOAL SHAPE BUTTONS
public abstract class GoalShapeButton
{
    public static SelectedGoalShape Shape;
    public abstract SelectedGoalShape SelectedShape();
}

public class DefaultButton : GoalShapeButton
{
    public override SelectedGoalShape SelectedShape() { return SelectedGoalShape.Default; }
}

public class StoolButton : GoalShapeButton
{
    public override SelectedGoalShape SelectedShape() { return SelectedGoalShape.Stool; }
}

public class ChairButton : GoalShapeButton
{
    public override SelectedGoalShape SelectedShape() { return SelectedGoalShape.Chair; }
}

public class TableButton : GoalShapeButton
{
    public override SelectedGoalShape SelectedShape() { return SelectedGoalShape.Table; }
}

public class HouseButton : GoalShapeButton
{
    public override SelectedGoalShape SelectedShape() { return SelectedGoalShape.House; }
}

