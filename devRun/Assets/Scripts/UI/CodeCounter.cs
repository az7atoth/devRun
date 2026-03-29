using TMPro;
using UnityEngine;
using UniRx;
using Zenject;

namespace DevRun
{
    public class CodeCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _collectedText;
        [SerializeField] private TMP_Text _storedText;
        [SerializeField] private TMP_Text _requestedText;

        private IScoreProvider _scoreProvider;

        [Inject]
        public void Construct(IScoreProvider scoreProvider)
        {
            _scoreProvider = scoreProvider;
        }

        private void Awake()
        {
            _scoreProvider.ScoreCollected.Subscribe(value => _collectedText.text = value.ToString());
            _scoreProvider.ScoreStored.Subscribe(value => _storedText.text = value.ToString());
            _scoreProvider.ScoreRequested.Subscribe(value => _requestedText.text = value.ToString());
        }
    }
}
