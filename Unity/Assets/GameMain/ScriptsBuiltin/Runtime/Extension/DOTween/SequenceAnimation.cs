// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/31 10:10:31
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace GameMain.Builtin  
{
    
    [Serializable]
    public class SequenceAnimation
    {
        public AddType AddType = AddType.Append;
        public DOTweenType AnimationType = DOTweenType.DOMove;
        public Component Target;
        
        // From Value
        public bool UseFromValue = true;
        public Vector4 FromValue = Vector4.zero;

        // To Target
        public bool UseToTarget;
        public Component ToTarget;
        public Vector4 ToValue = Vector4.zero;

        // Sequence Animation params
        public bool SpeedBased;
        public float DurationOrSpeed = 1; // value > 0
        public bool Snapping;

        // DOTween params
        public int Loops = 1;
        public LoopType LoopType;
        public UpdateType UpdateType = UpdateType.Normal;
        public float Delay;
        public bool CurveOrEase;
        public AnimationCurve EaseCurve;
        public Ease Ease = Ease.OutQuad;
        public UnityEvent OnPlay;
        public UnityEvent OnUpdate;
        public UnityEvent OnComplete;
        
        public Tween CreateTween(bool reverse = false)
        {
            Tween result = null;

            switch (AnimationType)
            {
                case DOTweenType.DOMove:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        Vector3 start = !UseFromValue ? targetTrans.position : FromValue;
                        Vector3 end = toTrans != null && UseToTarget ? toTrans.position : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.position = start;
                        var duration = GetDuration(Vector3.Distance(start, end));

                        result = targetTrans.DOMove(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOMoveX:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.position.x : FromValue.x;
                        var end = toTrans != null && UseToTarget ? toTrans.position.x : ToValue.x;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetPositionX(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOMoveX(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOMoveY:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.position.y : FromValue.y;
                        var end = toTrans != null && UseToTarget ? toTrans.position.y : ToValue.y;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetPositionY(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOMoveY(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOMoveZ:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.position.z : FromValue.z;
                        var end = toTrans != null && UseToTarget ? toTrans.position.z : ToValue.z;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetPositionZ(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOMoveZ(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOLocalMove:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        Vector3 start = !UseFromValue ? targetTrans.localPosition : FromValue;
                        Vector3 end = toTrans != null && UseToTarget ? toTrans.localPosition : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.localPosition = start;
                        var duration = GetDuration(Vector3.Distance(start, end));

                        result = targetTrans.DOLocalMove(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOLocalMoveX:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.localPosition.x : FromValue.x;
                        var end = toTrans != null && UseToTarget ? toTrans.localPosition.x : ToValue.x;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetLocalPositionX(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOLocalMoveX(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOLocalMoveY:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.localPosition.y : FromValue.y;
                        var end = toTrans != null && UseToTarget ? toTrans.localPosition.y : ToValue.y;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetLocalPositionY(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOLocalMoveY(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOLocalMoveZ:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.localPosition.z : FromValue.z;
                        var end = toTrans != null && UseToTarget ? toTrans.localPosition.z : ToValue.z;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetLocalPositionZ(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOLocalMoveZ(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOScale:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        Vector3 start = !UseFromValue ? targetTrans.localScale : FromValue;
                        Vector3 end = toTrans != null && UseToTarget ? toTrans.localScale : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.localScale = start;
                        var duration = GetDuration(Vector3.Distance(start, end));

                        result = targetTrans.DOScale(end, duration);
                    }
                }
                    break;
                case DOTweenType.DOScaleX:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.localScale.x : FromValue.x;
                        var end = toTrans != null && UseToTarget ? toTrans.localScale.x : ToValue.x;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetLocalScaleX(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOScaleX(end, duration);
                    }
                }
                    break;
                case DOTweenType.DOScaleY:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.localScale.y : FromValue.y;
                        var end = toTrans != null && UseToTarget ? toTrans.localScale.y : ToValue.y;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetLocalScaleY(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOScaleY(end, duration);
                    }
                }
                    break;
                case DOTweenType.DOScaleZ:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        var start = !UseFromValue ? targetTrans.localScale.z : FromValue.z;
                        var end = toTrans != null && UseToTarget ? toTrans.localScale.z : ToValue.z;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetLocalScaleZ(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOScaleZ(end, duration);
                    }
                }
                    break;
                case DOTweenType.DORotate:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        Vector3 start = !UseFromValue ? targetTrans.eulerAngles : FromValue;
                        Vector3 end = toTrans != null && UseToTarget ? toTrans.eulerAngles : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.eulerAngles = start;
                        var duration = GetDuration(GetEulerAngle(end, start));

                        result = targetTrans.DORotate(end, duration, RotateMode.FastBeyond360);
                    }
                }
                    break;
                case DOTweenType.DOLocalRotate:
                {
                    var targetTrans = Target as Transform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as Transform;
                        Vector3 start = !UseFromValue ? targetTrans.localEulerAngles : FromValue;
                        Vector3 end = toTrans != null && UseToTarget ? toTrans.localEulerAngles : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.localEulerAngles = start;
                        var duration = GetDuration(GetEulerAngle(end, start));

                        result = targetTrans.DOLocalRotate(end, duration, RotateMode.FastBeyond360);
                    }
                }
                    break;
                case DOTweenType.DOAnchorPos:
                {
                    var targetTrans = Target as RectTransform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as RectTransform;
                        Vector2 start = !UseFromValue ? targetTrans.anchoredPosition : FromValue;
                        Vector2 end = toTrans != null && UseToTarget ? toTrans.anchoredPosition : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.anchoredPosition = start;
                        var duration = GetDuration(Vector2.Distance(start, end));

                        result = targetTrans.DOAnchorPos(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOAnchorPosX:
                {
                    var targetTrans = Target as RectTransform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as RectTransform;
                        var start = !UseFromValue ? targetTrans.anchoredPosition.x : FromValue.x;
                        var end = toTrans != null && UseToTarget ? toTrans.anchoredPosition.x : ToValue.x;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetAnchoredPositionX(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOAnchorPosX(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOAnchorPosY:
                {
                    var targetTrans = Target as RectTransform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as RectTransform;
                        var start = !UseFromValue ? targetTrans.anchoredPosition.y : FromValue.y;
                        var end = toTrans != null && UseToTarget ? toTrans.anchoredPosition.y : ToValue.y;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetAnchoredPositionY(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOAnchorPosY(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOAnchorPos3D:
                {
                    var targetTrans = Target as RectTransform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as RectTransform;
                        Vector3 start = !UseFromValue ? targetTrans.anchoredPosition3D : FromValue;
                        Vector3 end = toTrans != null && UseToTarget ? toTrans.anchoredPosition3D : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.anchoredPosition3D = start;
                        var duration = GetDuration(Vector3.Distance(start, end));

                        result = targetTrans.DOAnchorPos3D(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOAnchorPos3DZ:
                {
                    var targetTrans = Target as RectTransform;
                    if (targetTrans != null)
                    {
                        var toTrans = ToTarget as RectTransform;
                        var start = !UseFromValue ? targetTrans.anchoredPosition3D.z : FromValue.z;
                        var end = toTrans != null && UseToTarget ? toTrans.anchoredPosition3D.z : ToValue.z;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        targetTrans.SetAnchoredPosition3DZ(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = targetTrans.DOAnchorPos3DZ(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOSizeDelta:
                {
                    var target = Target as RectTransform;
                    if (target != null)
                    {
                        var to = ToTarget as RectTransform;
                        Vector2 start = !UseFromValue ? target.sizeDelta : FromValue;
                        Vector2 end = to != null && UseToTarget ? to.sizeDelta : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.sizeDelta = start;
                        var duration = GetDuration(Vector2.Distance(end, start));

                        result = target.DOSizeDelta(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOColor:
                {
                    var target = Target as UnityEngine.UI.Graphic;
                    if (target != null)
                    {
                        var to = ToTarget as UnityEngine.UI.Graphic;
                        var start = !UseFromValue ? target.color : (Color)FromValue;
                        var end = to != null && UseToTarget ? to.color : (Color)ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.color = start;
                        var duration = GetDuration(Vector4.Distance(end, start));

                        result = target.DOColor(end, duration);
                    }
                }
                    break;
                case DOTweenType.DOColorAlphaFade:
                {
                    var target = Target as UnityEngine.UI.Graphic;
                    if (target != null)
                    {
                        var to = ToTarget as UnityEngine.UI.Graphic;
                        var start = !UseFromValue ? target.color.a : FromValue.w;
                        var end = to != null && UseToTarget ? to.color.a : ToValue.w;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.SetColorAlpha(start);
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = target.DOFade(end, duration);
                    }
                }
                    break;
                case DOTweenType.DOCanvasGroupAlphaFade:
                {
                    var target = Target as CanvasGroup;
                    if (target != null)
                    {
                        var to = ToTarget as CanvasGroup;
                        var start = !UseFromValue ? target.alpha : FromValue.w;
                        var end = to != null && UseToTarget ? to.alpha : ToValue.w;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.alpha = start;
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = target.DOFade(end, duration);
                    }
                }
                    break;
                case DOTweenType.DOFillAmount:
                {
                    var target = Target as UnityEngine.UI.Image;
                    if (target != null)
                    {
                        var to = ToTarget as UnityEngine.UI.Image;
                        var start = !UseFromValue ? target.fillAmount : FromValue.x;
                        var end = to != null && UseToTarget ? to.fillAmount : ToValue.x;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.fillAmount = start;
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = target.DOFillAmount(end, duration);
                    }
                }
                    break;
                case DOTweenType.DOFlexibleSize:
                {
                    var target = Target as UnityEngine.UI.LayoutElement;
                    if (target != null)
                    {
                        var to = ToTarget as UnityEngine.UI.LayoutElement;
                        Vector2 start = !UseFromValue ? target.GetFlexibleSize() : FromValue;
                        Vector2 end = to != null && UseToTarget ? to.GetFlexibleSize() : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.SetFlexibleSize(start);
                        var duration = GetDuration(Vector2.Distance(end, start));

                        result = target.DOFlexibleSize(end, duration);
                    }
                }
                    break;
                case DOTweenType.DOMinSize:
                {
                    var target = Target as UnityEngine.UI.LayoutElement;
                    if (target != null)
                    {
                        var to = ToTarget as UnityEngine.UI.LayoutElement;
                        Vector2 start = !UseFromValue ? target.GetMinSize() : FromValue;
                        Vector2 end = to != null && UseToTarget ? to.GetMinSize() : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.SetMinSize(start);
                        var duration = GetDuration(Vector2.Distance(end, start));

                        result = target.DOMinSize(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOPreferredSize:
                {
                    var target = Target as UnityEngine.UI.LayoutElement;
                    if (target != null)
                    {
                        var to = ToTarget as UnityEngine.UI.LayoutElement;
                        Vector2 start = !UseFromValue ? target.GetPreferredSize() : FromValue;
                        Vector2 end = to != null && UseToTarget ? to.GetPreferredSize() : ToValue;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.SetPreferredSize(start);
                        var duration = GetDuration(Vector2.Distance(end, start));

                        result = target.DOPreferredSize(end, duration, Snapping);
                    }
                }
                    break;
                case DOTweenType.DOSliderValue:
                {
                    var target = Target as UnityEngine.UI.Slider;
                    if (target != null)
                    {
                        var to = ToTarget as UnityEngine.UI.Slider;
                        var start = !UseFromValue ? target.value : FromValue.x;
                        var end = to != null && UseToTarget ? to.value : ToValue.x;
                        if (reverse)
                        {
                            (end, start) = (start, end);
                        }

                        target.value = start;
                        var duration = GetDuration(Mathf.Abs(end - start));

                        result = target.DOValue(end, duration, Snapping);
                    }
                }
                    break;
            }

            if (result != null && Target != null)
            {
                result.SetAutoKill(true).SetTarget(Target.gameObject).SetLoops(Loops, LoopType).SetUpdate(UpdateType);
                if (Delay > 0)
                {
                    result.SetDelay(Delay);
                }

                if (CurveOrEase)
                {
                    result.SetEase(EaseCurve);
                }
                else
                {
                    result.SetEase(Ease);
                }

                if (OnPlay != null)
                {
                    result.OnPlay(OnPlay.Invoke);
                }

                if (OnUpdate != null)
                {
                    result.OnUpdate(OnUpdate.Invoke);
                }

                if (OnComplete != null)
                {
                    result.OnComplete(OnComplete.Invoke);
                }
            }

            return result;
        }

        private float GetDuration(float CalculatedValue)
        {
            if (SpeedBased)
            {
                return CalculatedValue / DurationOrSpeed;
            }

            return DurationOrSpeed;
        }

        private float GetEulerAngle(Vector3 to, Vector3 target)
        {
            var delta = target - to;
            delta.x = Mathf.DeltaAngle(to.x, target.x);
            delta.y = Mathf.DeltaAngle(to.y, target.y);
            delta.z = Mathf.DeltaAngle(to.z, target.z);

            var angle = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            return (angle + 360) % 360;
        }
    }
}