using System.Collections;
using DG.Tweening;
using SeizedPixels.DotCollect.Utils;
using UnityEngine;
using UnityEngine.UI;
using static SeizedPixels.DotCollect.Utils.GameObjectUtils;

namespace SeizedPixels.DotCollect.GameCore
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        [SerializeField] private Text _startText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _highScoreText;
        [SerializeField] private Text _tutorialText;

        [SerializeField] private Button _shareButton;
        [SerializeField] private Button _restartButton;

        [SerializeField] private DotSpawner[] _spawners;
        [SerializeField] private SpriteRenderer _dotCollector;
        [SerializeField] private Transform _dotParent;

        private float _screenMiddle;
        private int _highScore;
        private float _startSpeed;
        private int _score;

        private readonly string _gameUrl = "https://play.google.com/store/apps/details?id=de.seizedpixels.dotcapture";

        [HideInInspector] public GameState State = GameState.Pregame;
        public float Speed = 110;
        public AudioClip CollectSound;

        public int Score
        {
            set
            {
                _scoreText.text = value.ToString();
                _score = value;

                if (value % 10 == 0 && value < 65)
                {
                    Speed += 10;
                }

                foreach (var dotSpawner in _spawners)
                {
                    dotSpawner.UpdateScore(value);
                }
            }
            get { return _score; }
        }

        private void Awake()
        {
            Instance = this;

            _screenMiddle = (float) Screen.width / 2;

            _highScore = PlayerPrefs.GetInt("highscore", 0);
            _highScoreText.text = "Best Score: " + _highScore;

            _scoreText.CrossFadeAlpha(0, Time.deltaTime, false);
            CrossFadeButtons(0, Time.deltaTime);

            _dotCollector.DOFade(0, Time.deltaTime);

            DisableGameObjects(_restartButton, _shareButton);

            _startSpeed = Speed;
        }

        private void Start()
        {
            StartCoroutine(PulseStartText());

            _restartButton.onClick.AddListener(() => { StartCoroutine(Restart()); });
            _shareButton.onClick.AddListener(() =>
            {
                MobileUtils.ShareText("I've reached " + _score +
                                      " points in DotCapture! Can you beat me? Download the game at " + _gameUrl);
            });
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                if (State == GameState.Ingame)
                    transform.Rotate(0, 0,
                        (Input.GetTouch(0).position.x < _screenMiddle ? Speed : -Speed) * Time.deltaTime);
                else if (State == GameState.Pregame && Input.GetTouch(0).phase == TouchPhase.Began)
                    StartGame();
            }
            else if (Input.GetMouseButton(0))
            {
                if (State == GameState.Ingame)
                    transform.Rotate(0, 0, (Input.mousePosition.x < _screenMiddle ? Speed : -Speed) * Time.deltaTime);
                else if (State == GameState.Pregame)
                    StartGame();
            }
        }

        private void StartGame()
        {
            StartCoroutine(Restart());
            _tutorialText.DOFade(0, 0.5f);
            Destroy(_tutorialText.gameObject, 0.5f);
            _dotCollector.DOFade(1, 0.5f);
            State = GameState.Ingame;
        }

        private IEnumerator Restart()
        {
            if (State == GameState.Pregame || State == GameState.Aftergame)
            {
                Score = 0;
                Speed = _startSpeed;

                foreach (var dotSpawner in _spawners)
                {
                    dotSpawner.ResetSpawner();
                }

                _startText.CrossFadeAlpha(0, 0.5f, false);
                _scoreText.CrossFadeAlpha(1, 0.5f, false);
                CrossFadeButtons(0, 0.5f);
                EnableGameObjects(_shareButton, _restartButton);

                State = GameState.Ingame;

                yield return new WaitForSeconds(0.5f);

                foreach (var dotSpawner in _spawners)
                {
                    dotSpawner.DoSpawn = true;
                }
            }
        }

        private IEnumerator Die()
        {
            foreach (var dotSpawner in _spawners)
            {
                dotSpawner.DoSpawn = false;
            }

            for (int i = 0; i < _dotParent.childCount; i++)
            {
                GameObject o = _dotParent.GetChild(i).gameObject;
                o.GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
                Destroy(o, 0.2f);
            }

            Camera.main.DOShakePosition(0.5f, 0.5f);
            if (_score > _highScore)
            {
                PlayerPrefs.SetInt("highscore", _score);
                _highScore = _score;
                _highScoreText.text = "Best Score: " + _score;
            }

            State = GameState.AftergameFade;
            EnableGameObjects(_restartButton, _shareButton);
            CrossFadeButtons(1, 0.5f);
            yield return new WaitForSeconds(0.3f);
            State = GameState.Aftergame;
        }

        private IEnumerator PulseStartText()
        {
            while (State == GameState.Pregame && _startText.color.a == 1)
            {
                _startText.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.6f);
                _tutorialText.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.6f);
                yield return new WaitForSeconds(0.6f);
                _startText.transform.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.6f);
                _tutorialText.transform.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.6f);
                yield return new WaitForSeconds(0.6f);
            }
        }

        private void CrossFadeButtons(float target, float duration)
        {
            _restartButton.GetComponent<Image>().DOFade(target, duration);
            _restartButton.GetComponentInChildren<Text>().DOFade(target, duration);
            _shareButton.GetComponent<Image>().DOFade(target, duration);
            _shareButton.GetComponentInChildren<Text>().DOFade(target, duration);
        }
    }
}