using UnityEngine;
using UniRx;

namespace DevRun
{
    public class BugsCounter : MonoBehaviour
    {
        [SerializeField] private BugIcon[] _bugObjects;

        private void Awake()
        {
            Blackboard.BugsCollected.Subscribe(value => SetBugsActive(value)).AddTo(this);
            Blackboard.BugLimit.Subscribe(value => SetBugsLimit(value)).AddTo(this);
        }

        private void SetBugsLimit(int value)
        {
            if (value >= _bugObjects.Length) return;

            for (int i = 0; i < _bugObjects.Length; i++)
            {
                _bugObjects[i].gameObject.SetActive(i < value);
            }
        }

        private void SetBugsActive(int value)
        {
            if (value >= _bugObjects.Length) return;

            for (int i = 0; i < _bugObjects.Length; i++)
            {
                _bugObjects[i].SetActive(i < value);
            }
        }
    } 
}
