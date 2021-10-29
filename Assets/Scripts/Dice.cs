using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour {
    private Color highlight = Color.green;
    private Color holdColor = new Color (0.5f, 0.0f, 0.5f);
    private Ray ray;
    private RaycastHit hit;
    public Color startColor;
    public Renderer myRenderer;
    public int dieValue;
    public bool comboCheck = false;
    public bool farkleCheck = false;
    public bool onBottom = false;
    private LayerMask mask;

    void Awake()
    {
        myRenderer = GetComponentInChildren<MeshRenderer>();
        startColor = myRenderer.material.color;
    }

    // Start is called before the first frame update
    void Start() 
    {
        transform.rotation = Random.rotation;
        mask = LayerMask.GetMask("Bottom"); // Mask is the bottom of the box - raycasts will not read true for anything else while this is in place
    }

    // Update is called once per frame
    void Update() 
    {
        DetectSide();

        ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    }

    void DetectSide() 
    {
        Ray ray1 = new Ray(transform.position, -transform.up);
        Ray ray2 = new Ray(transform.position, -transform.right);
        Ray ray3 = new Ray(transform.position, transform.forward); 
        Ray ray4 = new Ray(transform.position, -transform.forward); 
        Ray ray5 = new Ray(transform.position, transform.right);
        Ray ray6 = new Ray(transform.position, transform.up);

        bool cast1 = Physics.Raycast(ray1, 0.27f, mask);
        bool cast2 = Physics.Raycast(ray2, 0.27f, mask);
        bool cast3 = Physics.Raycast(ray3, 0.27f, mask);
        bool cast4 = Physics.Raycast(ray4, 0.27f, mask);
        bool cast5 = Physics.Raycast(ray5, 0.27f, mask);
        bool cast6 = Physics.Raycast(ray6, 0.27f, mask);

        if(onBottom)
        {

            if (cast1) // If the 1 face is down, the 6 face is up, making the dieValue 6
            {
                dieValue = 6;
            } 
            else if(cast2) 
            {
                dieValue = 5;
            } 
            else if(cast3) 
            {
                dieValue = 4;
            } 
            else if(cast4) 
            {
                dieValue = 3;
            } 
            else if(cast5) 
            {
                dieValue = 2;
            } 
            else if(cast6) 
            {
                dieValue = 1;
            } 
            else 
            {
                dieValue = 0;
            }
        }
    }

    void OnMouseDown() 
    {
        if (Physics.Raycast (ray, out hit)) 
        {
            for (int i = 0; i < GameManager.instance.dice.Length; i++) 
            {
                if (hit.collider.gameObject == GameManager.instance.dice[i].gameObject) 
                {

                    if (GameManager.instance.dice[i].gameObject.layer == 0) 
                    {
                        GameManager.instance.dice[i].gameObject.layer = 8;
                    } 
                    else 
                    {
                        GameManager.instance.dice[i].gameObject.layer = 0;
                    }

                    myRenderer.material.color = holdColor;
                }
            }
        }
    }

    void OnMouseEnter() 
    {
        if (gameObject.layer == 0) {
            myRenderer.material.color = highlight;
        }
    }

    void OnMouseExit() 
    {
        if (gameObject.layer == 0) {
            myRenderer.material.color = startColor;
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Bottom"))
        {
            onBottom = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        onBottom = false;
    }
}

