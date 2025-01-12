using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    public GameObject presser;
    bool isPressed;
    bool initiatePrint;

    public string coordinatesFilePath;
    private Vector3[] targetCoordinates;
    private int currentTargetIndex = 0;
    private Transform target;
    public GameObject Printer;
    private GameObject Object3D;
    private int[] printCoord;
    private float scale = 0.001f;
    private int updateCycles = 0;
    public float speed = 1.0f;

    [SerializeField]
    public GameObject PrinterHead;
    
    [SerializeField]
    public GameObject PrinterRail;

    [SerializeField]
    public GameObject PrinterBed;


    void Start()
    {
        isPressed = false;
        initiatePrint = false;
        parseCoords();
    }

    private void parseCoords()
    {
        string[] lines = File.ReadAllLines(coordinatesFilePath);
        printCoord = new int[lines.Length];
        targetCoordinates = new Vector3[lines.Length];
        print(lines.Length);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] coords = lines[i].Split(' ');
            printCoord[i] = int.Parse(coords[0]);
            float x = - float.Parse(coords[1]) * scale;
            float y = float.Parse(coords[2]) * scale + 1.105f;
            float z = float.Parse(coords[3]) * scale;
            targetCoordinates[i] = new Vector3(x, y, z);
        }

        var targetObj = new GameObject();

        Object3D = new GameObject();
        Object3D.transform.SetParent(PrinterBed.transform);

        target = targetObj.transform;
        target.transform.position = targetCoordinates[currentTargetIndex++];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.02f, 0);
            presser = other.gameObject;
            //onPress.Invoke();
            isPressed = true;
            Destroy( Object3D );
            Object3D = new GameObject();
            Object3D.transform.SetParent(PrinterBed.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser) {
            button.transform.localPosition = new Vector3(0, 0.03f, 0);
            onRelease.Invoke();
            initiatePrint = !initiatePrint;
            if (!initiatePrint) { 
                currentTargetIndex = 0;
            }
            isPressed = false;
        }
    }

    public void print()
    {
        if (initiatePrint) {
            var step = (speed*0.001f) * Time.deltaTime; //Calculate distance to move
            PrinterHead.transform.position = Vector3.MoveTowards(PrinterHead.transform.position, new Vector3(PrinterHead.transform.position.x, target.transform.position.y, target.transform.position.z), step);
            PrinterBed.transform.position = Vector3.MoveTowards(PrinterBed.transform.position, new Vector3(target.transform.position.x, PrinterBed.transform.position.y, PrinterBed.transform.position.z), step);
            PrinterRail.transform.position = Vector3.MoveTowards(PrinterRail.transform.position, new Vector3(PrinterRail.transform.position.x, target.transform.position.y, PrinterRail.transform.position.z), step);

            if (printCoord[currentTargetIndex] != 0 && targetCoordinates.Length > currentTargetIndex && updateCycles % 5 == 0)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(Object3D.transform);
                sphere.transform.localScale = new Vector3(0.0032f, 0.0016f, 0.0032f);
                sphere.transform.position = new Vector3(PrinterHead.transform.position.x-0.001f, PrinterHead.transform.position.y - 0.04f, PrinterHead.transform.position.z-0.001f);
            }

            if (Math.Abs(PrinterHead.transform.position.z - target.transform.position.z) < 0.0001f && Math.Abs(PrinterHead.transform.position.y - target.transform.position.y) < 0.0001f && Math.Abs(PrinterBed.transform.position.x - target.transform.position.x) < 0.0001f && Math.Abs(PrinterRail.transform.position.y - target.transform.position.y) < 0.0001f)
            {
                target.transform.position = targetCoordinates[currentTargetIndex++];
            }

            updateCycles++;
        }
    }

    private void Update()
    {
        print();
    }
}
