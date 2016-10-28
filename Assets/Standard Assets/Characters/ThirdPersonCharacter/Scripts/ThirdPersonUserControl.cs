using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        public Text PlayerStatusText;
        public Text RestartText;
        public Text FinalText;
        public int coins;
        public float time;
        public int lives = 3;
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam; // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward; // The current forward direction of the camera
        private Vector3 m_Move;
        private bool gameOver;
        private bool restart;

        private bool m_Jump;
        // the world-relative desired move direction, calculated from the camForward and user input.       

        private void Start()
        {
            SetCountText();
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.",
                    gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            time += Time.deltaTime;
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (restart && Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (gameOver)
            {
                RestartText.text = "Press 'R' for Restart";
                restart = true;
                return;
            }

            SetCountText();
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Pick Up"))
            {
                other.gameObject.SetActive(false);
                ++coins;
                SetCountText();
            }

            else if (other.gameObject.CompareTag("Enemy"))
            {
                if (--lives == 0)
                {
                    GameOver(Convert.ToInt32(time), coins);
                }

                SetCountText();
            }
        }

        void SetCountText()
        {
            PlayerStatusText.text = string.Format("Coins: {0} {1}Lives: {2} {1}Time: {3}s", coins.ToString(),
                Environment.NewLine, lives.ToString(), Convert.ToInt32(time));
        }

        void GameOver(int time, int coins)
        {
            SetFinalText(time, coins);
            gameOver = true;
        }

        void SetFinalText(int time, int coins)
        {
            FinalText.text = string.Format("Game over.{0}Survived time: {1}s.{0}Collected coins: {2}{0}Summary: {3}",
                Environment.NewLine, time, coins, time + coins);
            FinalText.enabled = true;
        }
    }
}
;