// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/31 10:10:12
//  * Description:
//  * Modify Record:
//  *************************************************************/

using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace GameMain.Builtin
{
    public class DOTweenSequence : MonoBehaviour
    {
        [SerializeField] private SequenceAnimation[] mSequenceAnimations;

        [SerializeField] private bool mPlayOnAwake;
        [SerializeField] private float mDelay;
        [SerializeField] private Ease mEase = Ease.OutQuad;
        [SerializeField] private int mLoops = 1;
        [SerializeField] private LoopType mLoopType;
        [SerializeField] private UpdateType mUpdateType = UpdateType.Normal;
        [SerializeField] private UnityEvent mOnPlay;
        [SerializeField] private UnityEvent mOnUpdate;
        [SerializeField] private UnityEvent mOnComplete;

        private Tween mTween;

        private void Awake()
        {
            if (mPlayOnAwake)
            {
                DOPlay();
            }
        }

        private Tween CreateTween(bool reverse = false)
        {
            if (mSequenceAnimations == null || mSequenceAnimations.Length <= 0)
            {
                return null;
            }

            var sequence = DOTween.Sequence();
            if (reverse)
            {
                for (var i = mSequenceAnimations.Length - 1; i >= 0; i--)
                {
                    var sequenceAnimation = mSequenceAnimations[i];
                    var tween = sequenceAnimation.CreateTween(true);
                    if (tween == null)
                    {
                        var typeName = sequenceAnimation.Target == null ? "null" : sequenceAnimation.Target.GetType().Name;
                        Debug.LogError($"Tween is null, Index : {i}, Animation Type : {sequenceAnimation.AnimationType}, Component Type : {typeName}");
                        continue;
                    }

                    switch (sequenceAnimation.AddType)
                    {
                        case AddType.Append:
                            sequence.Append(tween);
                            break;
                        case AddType.Join:
                            sequence.Join(tween);
                            break;
                    }
                }
            }
            else
            {
                for (var i = 0; i < mSequenceAnimations.Length; i++)
                {
                    var sequenceAnimation = mSequenceAnimations[i];
                    var tween = sequenceAnimation.CreateTween();
                    if (tween == null)
                    {
                        var typeName = sequenceAnimation.Target == null ? "null" : sequenceAnimation.Target.GetType().Name;
                        Debug.LogError($"Tween is null, Index : {i}, Animation Type : {sequenceAnimation.AnimationType}, Component Type : {typeName}");
                        continue;
                    }

                    switch (sequenceAnimation.AddType)
                    {
                        case AddType.Append:
                            sequence.Append(tween);
                            break;
                        case AddType.Join:
                            sequence.Join(tween);
                            break;
                    }
                }
            }

            sequence.SetAutoKill(true).SetEase(mEase).SetLoops(mLoops, mLoopType).SetUpdate(mUpdateType);
            if (mDelay > 0)
            {
                sequence.SetDelay(mDelay);
            }

            if (mOnPlay != null)
            {
                sequence.OnPlay(mOnPlay.Invoke);
            }

            if (mOnUpdate != null)
            {
                sequence.OnUpdate(mOnUpdate.Invoke);
            }

            if (mOnComplete != null)
            {
                sequence.OnComplete(mOnComplete.Invoke);
            }

            return sequence;
        }

        public Tween DOPlay()
        {
            mTween = CreateTween();
            return mTween?.Play();
        }

        public Tween DOReversePlay()
        {
            mTween = CreateTween(true);
            return mTween?.Play();
        }

        public void DoComplete(bool withCallback = false)
        {
            mTween?.Complete(withCallback);
        }

        public void DoKill()
        {
            mTween?.Kill();
            mTween = null;
        }

        public UnityEvent OnPlay
        {
            get => mOnPlay;
            set => mOnPlay = value;
        }

        public UnityEvent OnUpdate
        {
            get => mOnUpdate;
            set => mOnUpdate = value;
        }

        public UnityEvent OnComplete
        {
            get => mOnComplete;
            set => mOnComplete = value;
        }
    }
}