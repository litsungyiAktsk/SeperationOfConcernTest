using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Akatsuki
{
    public interface IGameProcess
    {
        bool CanEnterGame();
        bool CanQuitGame();

        bool CanJoinRoom();
        bool CanLeaveRoom();

        IEnumerator PerformEnter(Action callback);
        IEnumerator PerformQuit(Action callback);
        IEnumerator PerformJoin(Action callback);
        IEnumerator PerformLeave(Action callback);
    }

    public enum GameState
    {
        Quitted,
        Entering,
        Entered,
        Joining,
        Joined,
        Leaving,
        Quitting,
    }

    public class MainProcess : MonoBehaviour
    {
        private IGameProcess gameProcess;
        internal GameState gameState = GameState.Quitted;

        [SerializeField] private Text stateText;
        [SerializeField] private Button enterButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button resetButton;

        private void Awake()
        {
            enterButton.onClick.AddListener(OnEnterGameClicked);
            quitButton.onClick.AddListener(OnQuitGameClicked);
            joinButton.onClick.AddListener(OnJoinGameClicked);
            leaveButton.onClick.AddListener(OnLeaveGameClicked);
            resetButton.onClick.AddListener(OnResetClicked);

            DoReset();
        }

        private void UpdateState(GameState state)
        {
            gameState = state;
            if (PlayerPrefs.HasKey("TUTORIAL_FINISHED"))
            {
                stateText.text = state.ToString();
            }
            else
            {
                stateText.text = $"[TUTORIAL] {state.ToString()}";
            }
        }

        private void OnEnterGameClicked()
        {
            if (!gameProcess.CanEnterGame())
            {
                return;
            }

            UpdateState(GameState.Entering);
            StartCoroutine(gameProcess.PerformEnter(() => UpdateState(GameState.Entered)));
        }

        private void OnQuitGameClicked()
        {
            if (!gameProcess.CanQuitGame())
            {
                return;
            }

            UpdateState(GameState.Quitting);
            StartCoroutine(gameProcess.PerformQuit(() => UpdateState(GameState.Quitted)));
        }

        private void OnJoinGameClicked()
        {
            if (!gameProcess.CanJoinRoom())
            {
                return;
            }

            UpdateState(GameState.Joining);
            StartCoroutine(gameProcess.PerformJoin(() => UpdateState(GameState.Joined)));
        }

        private void OnLeaveGameClicked()
        {
            if (!gameProcess.CanLeaveRoom())
            {
                return;
            }

            UpdateState(GameState.Leaving);
            StartCoroutine(gameProcess.PerformLeave(() => UpdateState(GameState.Entered)));
        }

        private void OnResetClicked()
        {
            PlayerPrefs.DeleteKey("TUTORIAL_FINISHED");
            DoReset();
        }

        private void DoReset()
        {
            gameProcess = PlayerPrefs.HasKey("TUTORIAL_FINISHED")
                ? new GameProcess(this) : new TutorialProcess(this, FinishTutorial);

            UpdateState(GameState.Quitted);
        }

        private void FinishTutorial()
        {
            PlayerPrefs.SetString("TUTORIAL_FINISHED", "DONE");
            gameProcess = new GameProcess(this);
        }
    }

    public class GameProcess : IGameProcess
    {
        protected MainProcess mainProcess;

        public GameProcess(MainProcess mainProcess)
        {
            this.mainProcess = mainProcess;
        }

        public virtual bool CanEnterGame()
        {
            return mainProcess.gameState == GameState.Quitted;
        }

        public virtual bool CanQuitGame()
        {
            return mainProcess.gameState == GameState.Entered;
        }

        public virtual bool CanJoinRoom()
        {
            return mainProcess.gameState == GameState.Entered;
        }

        public virtual bool CanLeaveRoom()
        {
            return mainProcess.gameState == GameState.Joined;
        }

        public virtual IEnumerator PerformEnter(Action callback)
        {
            Debug.Log("Entering");
            yield return new WaitForSeconds(2f);
            Debug.Log("Entered");
            callback?.Invoke();
        }

        public virtual IEnumerator PerformQuit(Action callback)
        {
            Debug.Log("Quitting");
            yield return new WaitForSeconds(2f);
            Debug.Log("Quitted");
            callback?.Invoke();
        }

        public virtual IEnumerator PerformJoin(Action callback)
        {
            Debug.Log("Joining");
            yield return new WaitForSeconds(2f);
            Debug.Log("Joined");
            callback?.Invoke();
        }

        public virtual IEnumerator PerformLeave(Action callback)
        {
            Debug.Log("Leaving");
            yield return new WaitForSeconds(2f);
            Debug.Log("Left");
            callback?.Invoke();
        }
    }

    public class TutorialProcess : GameProcess
    {
        private Action onFinished;

        public TutorialProcess(MainProcess mainProcess, Action onFinished)
            : base(mainProcess)
        {
            this.onFinished = onFinished;
        }

        public override IEnumerator PerformJoin(Action callback)
        {
            Debug.Log("Joining");
            yield return new WaitForSeconds(2f);
            Debug.Log("This");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("is");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Turtorial");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Joined");
            callback?.Invoke();
        }

        public override IEnumerator PerformLeave(Action callback)
        {
            Debug.Log("Leaving");
            yield return new WaitForSeconds(2f);
            Debug.Log("This");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("is");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Turtorial");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Left");
            onFinished?.Invoke();
            callback?.Invoke();
        }
    }
}
