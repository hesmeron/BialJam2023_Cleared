using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class TutorialSequence
{
    [SerializeField]
    private AudioClip _audioClip;
    [SerializeField]
    private GameObject _panel;

    public AudioClip AudioClip => _audioClip;

    public GameObject Panel => _panel;
}
public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private List<Material> _gameplayMaterials = new List<Material>();

    [SerializeField] 
    private List<TutorialSequence> _tutorialSequences = new List<TutorialSequence>();
    public event Action OnGameEnded;
    public event Action OnGameStarted;

    [SerializeField]
    private GameObject _restartBackground;
    [SerializeField]
    private AudioSource _source;

    [SerializeField] 
    private AudioClip _victoryClip;
    
    [SerializeField] 
    private AudioClip _defeatsClip;
    
    [SerializeField]
    private PlayerInput _playerInput;
    [SerializeField] 
    private GameObject _losePanel;
    [SerializeField] 
    private GameObject _winPanel;

    [SerializeField]
    private float _timeToPass = 2.5f;

    [SerializeField] 
    private GameObject _restartButton;
    [SerializeField]
    private Image _restartProgress;
    private bool _hasEnded = false;
    [SerializeField]
    private float _timePassed = 0f;

    private void Start()
    {
       StartCoroutine(TutorialSequence());
    }

    private void Update()
    {
        if (_hasEnded)
        {
            if (_playerInput.actions["BButton"].IsPressed())
            {
                _timePassed += Time.deltaTime;
                if (_timePassed > _timeToPass)
                {
                    Restart();
                }
            }
            else
            {
                _timePassed -= Time.deltaTime / 4f;
            }

            _timePassed = Mathf.Max(_timePassed, 0);

            _restartProgress.fillAmount = _timePassed / _timeToPass;
        }
    }

    public void Win()
    {
        if (!_hasEnded)
        {
            _source.PlayOneShot(_victoryClip);
            EndGame();
            _winPanel.SetActive(true);
        }
    }

    public void Lose()
    {
        if (!_hasEnded)
        {
            _source.PlayOneShot(_defeatsClip);
            EndGame();
            _losePanel.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndGame()
    {
        if (!_hasEnded)
        {
            _hasEnded = true;
        }
        _restartButton.gameObject.SetActive(true);
        _restartBackground.gameObject.SetActive(true);
        OnGameEnded?.Invoke();
    }

    private void StartGame()
    {
        foreach (Material material in _gameplayMaterials)
        {
            material.SetFloat("_GrayOut", 0f);
        }
        OnGameStarted?.Invoke();
    }
    
    IEnumerator TutorialSequence()
    {
        foreach (Material material in _gameplayMaterials)
        {
            material.SetFloat("_GrayOut", 1f);
        }
        _restartButton.SetActive(true);
        foreach (TutorialSequence tutorialSequence in _tutorialSequences)
        {
            _source.clip = tutorialSequence.AudioClip;
            _timePassed = 0f;
            tutorialSequence.Panel.SetActive(true);
            _source.Play();
            do
            {
                if (_playerInput.actions["BButton"].IsPressed())
                {
                    _timePassed += Time.deltaTime;
                    if (_timePassed > _timeToPass)
                    {
                        _source.Stop();
                        break;
                    }
                }
                else
                {
                    _timePassed -= Time.deltaTime / 4f;
                }

                _timePassed = Mathf.Max(_timePassed, 0);

                _restartProgress.fillAmount = _timePassed / _timeToPass;
                yield return null;
            } while (_source.isPlaying);
            tutorialSequence.Panel.SetActive(false);
        }
        _restartButton.SetActive(false);
        StartGame();
    }
}
