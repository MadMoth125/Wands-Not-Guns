using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mr_sB.UnityTimer.Examples
{
    public class TestTimerBehaviour : MonoBehaviour
    {
        #region Unity Inspector Fields

        [Header("Controls")]
        public InputField DurationField;
        public InputField LoopCountField;

        public Button StartTimerButton;
        public Button CancelTimerButton;
        public Button PauseTimerButton;
        public Button ResumeTimerButton;
        public Button RestartTimerButton;

        public Toggle IsLoopedToggle;
        public Toggle UseGameTimeToggle;

        public Slider TimescaleSlider;

        public Text NeedsRestartText;

        [Header("Stats Texts")] public Text TimeElapsedText;
        public Text TimeRemainingText;
        public Text PercentageCompletedText;
        public Text PercentageRemainingText;

        public Text NumberOfLoopsText;
        public Text IsCancelledText;
        public Text IsCompletedText;
        public Text IsPausedText;
        public Text IsDoneText;
        public Text UpdateText;

        #endregion

        private int _numLoops;
        private Timer _testTimer;
        
        private void Awake()
        {
            RestartTimerButton.interactable = false;
            IsLoopedToggle.onValueChanged.AddListener(OnLoopToggleChanged);
            OnLoopToggleChanged(IsLoopedToggle.isOn);
            ResetState();
        }

        private void ResetState()
        {
            _numLoops = 0;
            CancelTestTimer();
        }

        public void StartTestTimer()
        {
            ResetState();

            // this is the important code example bit where we register a new timer
            if (IsLoopedToggle.isOn)
            {
                int loopCount = GetLoopCount();
                if (loopCount >= 0)
                    _testTimer = this.LoopCountAction(GetDurationValue(), loopCount, loopTime => _numLoops = loopTime,
                        secondsElapsed => { UpdateText.text = string.Format("Timer ran update callback: {0:F2} seconds", secondsElapsed); },
                        () => { UpdateText.text = string.Format("LoopCountTimer finished! LoopTimes:{0}", ((LoopTimer) _testTimer)?.loopTimes ?? 0); },
                        !UseGameTimeToggle.isOn);
                else
                    _testTimer = this.LoopAction(GetDurationValue(), loopTime => _numLoops = loopTime,
                        secondsElapsed => { UpdateText.text = string.Format("Timer ran update callback: {0:F2} seconds", secondsElapsed); },
                        !UseGameTimeToggle.isOn);
            }
            else
                _testTimer = this.DelayAction(GetDurationValue(), () => _numLoops++,
                    secondsElapsed =>
                    {
                        UpdateText.text = string.Format("Timer ran update callback: {0:F2} seconds", secondsElapsed);
                    }, !UseGameTimeToggle.isOn);
            CancelTimerButton.interactable = true;
            RestartTimerButton.interactable = true;
        }

        public void CancelTestTimer()
        {
            Timer.Cancel(_testTimer);
            CancelTimerButton.interactable = false;
            NeedsRestartText.gameObject.SetActive(false);
        }

        public void PauseTestTimer()
        {
            Timer.Pause(_testTimer);
        }

        public void ResumeTestTimer()
        {
            Timer.Resume(_testTimer);
        }
        
        public void RestartTimer()
        {
            Timer.Restart(_testTimer);
            CancelTimerButton.interactable = true;
        }

        private void Update()
        {
            NumberOfLoopsText.text = string.Format("# Loops: {0}", _numLoops);
            if (_testTimer == null)
            {
                return;
            }

            Time.timeScale = TimescaleSlider.value;

            TimeElapsedText.text = string.Format("Time elapsed: {0:F2} seconds", _testTimer.GetTimeElapsed());
            TimeRemainingText.text = string.Format("Time remaining: {0:F2} seconds", _testTimer.GetTimeRemaining());
            PercentageCompletedText.text = string.Format("Percentage completed: {0:F4}%",
                _testTimer.GetRatioComplete()*100);
            PercentageRemainingText.text = String.Format("Percentage remaining: {0:F4}%",
                _testTimer.GetRatioRemaining()*100);
            IsCancelledText.text = string.Format("Is Cancelled: {0}", _testTimer.isCancelled);
            IsCompletedText.text = string.Format("Is Completed: {0}", _testTimer.isCompleted);
            IsPausedText.text = String.Format("Is Paused: {0}", _testTimer.isPaused);
            IsDoneText.text = string.Format("Is Done: {0}", _testTimer.isDone);

            PauseTimerButton.interactable = !_testTimer.isPaused;
            ResumeTimerButton.interactable = _testTimer.isPaused;

            NeedsRestartText.gameObject.SetActive(ShouldShowRestartText());
        }

        private bool ShouldShowRestartText()
        {
            var timerType = _testTimer.GetType();
            return IsLoopedToggle.isOn != typeof(LoopTimer).IsAssignableFrom(timerType) || // we switched timer type or
                    UseGameTimeToggle.isOn == _testTimer.usesRealTime || // we switched usesRealTime or
                    Mathf.Abs(GetDurationValue() - _testTimer.duration) >= Mathf.Epsilon; // our duration changed
        }

        private float GetDurationValue()
        {
            float duration;
            return float.TryParse(DurationField.text, out duration) ? duration : 0;
        }

        private int GetLoopCount()
        {
            int loopCount;
            return int.TryParse(LoopCountField.text, out loopCount) ? loopCount : 0;
        }

        private void OnLoopToggleChanged(bool isOn)
        {
            LoopCountField.transform.parent.gameObject.SetActive(isOn);
        }
    }
}