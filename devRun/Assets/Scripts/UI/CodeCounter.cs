using TMPro;
using UnityEngine;
using UniRx;

namespace Branches
{
    public class CodeCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _collectedText;
        [SerializeField] private TMP_Text _storedText;
        [SerializeField] private TMP_Text _requestedText;

        private void Awake()
        {
            Blackboard.CodeCollected.Subscribe(value => _collectedText.text = value.ToString());
            Blackboard.CodeRequested.Subscribe(value => _requestedText.text = value.ToString());
            Blackboard.CodeStored.Subscribe(value => _storedText.text = value.ToString());
        }
    }
}
