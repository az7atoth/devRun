using UnityEngine;
using UniRx;

namespace DevRun
{
	public class SFX_Controller : MonoBehaviour
	{
		[SerializeField] private AudioSource _badPickUp;
		[SerializeField] private AudioSource _goodPickUp;
		[SerializeField] private AudioSource _buffPickUp;
		[SerializeField] private AudioSource _versionUpgrade;
        [SerializeField] private AudioSource _merge;


        private void Awake()
        {
            Blackboard.OnGoodPickUp.Subscribe(_ => { _goodPickUp.Play(); }).AddTo(this);
            Blackboard.OnBadPickUp.Subscribe(_ => { _badPickUp.Play(); }).AddTo(this);
            Blackboard.OnBuffPickUp.Subscribe(_ => { _buffPickUp.Play(); }).AddTo(this);
            Blackboard.OnVersionUpgrade.Subscribe(_ => { _versionUpgrade.Play(); }).AddTo(this);
            Blackboard.OnMergeCorrect.Subscribe(_ => { _merge.Play(); }).AddTo(this);
        }
    } 
}
