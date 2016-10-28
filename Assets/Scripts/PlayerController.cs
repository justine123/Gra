using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public const string FinalMessage = "Game over. Your score: ";

    public float Speed { get; set; }
    public Text PlayerStatusText { get; set; }
    public Text FinalText { get; set; }

    private Rigidbody rb;
    private int points;
    private int lives;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        points = 0;
        lives = 3;
        SetCountText();
        FinalText.enabled = false;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * Speed);
    }

    void SetFinalText(int points)
    {
        FinalText.text = FinalMessage + points.ToString();
        FinalText.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            ++points;
            SetCountText();
        }

        else if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
            if (--lives == 0)
            {
                SetFinalText(points);
            }

            SetCountText();
        }
    }

    void SetCountText()
    {
        PlayerStatusText.text = string.Format("Points: {0} {1} Lives: {2}", points.ToString(), Environment.NewLine,
            lives.ToString());
    }
}
