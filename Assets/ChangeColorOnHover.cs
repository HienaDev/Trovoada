using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorOnHover : MonoBehaviour
{
    [SerializeField] private Color colorOnHover;
    private Image image;
    private Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        defaultColor = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hover()
    {
        image.color = colorOnHover;
    }

    public void Exit()
    {
        image.color = defaultColor;
    }
}
