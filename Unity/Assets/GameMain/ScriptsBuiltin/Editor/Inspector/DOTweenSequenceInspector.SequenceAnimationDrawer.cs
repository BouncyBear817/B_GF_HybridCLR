// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/31 15:10:26
//  * Description:
//  * Modify Record:
//  *************************************************************/

using System;
using DG.Tweening;
using GameMain.Builtin;
using UnityEditor;
using UnityEngine;

namespace GameMain.Editor
{
    [CustomPropertyDrawer(typeof(SequenceAnimation))]
    public class SequenceAnimationDrawer : PropertyDrawer
    {
        private const float ItemWidth = 100f;
        private const float SetBtnWidth = 10f;
        private const float LineHeight = 20f;

        private SerializedProperty mAddType;
        private SerializedProperty mAnimationType;
        private SerializedProperty mUseFromValue;
        private SerializedProperty mTarget;
        private SerializedProperty mFromValue;
        private SerializedProperty mUseToTarget;
        private SerializedProperty mToTarget;
        private SerializedProperty mToValue;
        private SerializedProperty mSpeedBased;
        private SerializedProperty mDurationOrSpeed;
        private SerializedProperty mSnapping;
        private SerializedProperty mLoops;
        private SerializedProperty mLoopType;
        private SerializedProperty mUpdateType;
        private SerializedProperty mDelay;
        private SerializedProperty mCurveOrEase;
        private SerializedProperty mEaseCurve;
        private SerializedProperty mEase;
        private SerializedProperty mOnPlay;
        private SerializedProperty mOnUpdate;
        private SerializedProperty mOnComplete;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel++;

            mAddType = property.FindPropertyRelative("AddType");
            mTarget = property.FindPropertyRelative("Target");
            mAnimationType = property.FindPropertyRelative("AnimationType");
            
            mUseFromValue = property.FindPropertyRelative("UseFromValue");
            mFromValue = property.FindPropertyRelative("FromValue");

            mUseToTarget = property.FindPropertyRelative("UseToTarget");
            mToTarget = property.FindPropertyRelative("ToTarget");
            mToValue = property.FindPropertyRelative("ToValue");

            mSpeedBased = property.FindPropertyRelative("SpeedBased");
            mDurationOrSpeed = property.FindPropertyRelative("DurationOrSpeed");
            mSnapping = property.FindPropertyRelative("Snapping");

            mLoops = property.FindPropertyRelative("Loops");
            mLoopType = property.FindPropertyRelative("LoopType");
            mUpdateType = property.FindPropertyRelative("UpdateType");
            mDelay = property.FindPropertyRelative("Delay");
            mCurveOrEase = property.FindPropertyRelative("CurveOrEase");
            mEaseCurve = property.FindPropertyRelative("EaseCurve");
            mEase = property.FindPropertyRelative("Ease");
            mOnPlay = property.FindPropertyRelative("OnPlay");
            mOnUpdate = property.FindPropertyRelative("OnUpdate");
            mOnComplete = property.FindPropertyRelative("OnComplete");

            var lastRect = new Rect(position.x, position.y, position.width, LineHeight);
            Rect horizontalRect;

            EditorGUI.PropertyField(lastRect, mAddType);
            
            lastRect.y += LineHeight;
            mTarget.objectReferenceValue = EditorGUI.ObjectField(lastRect, "From Target", mTarget.objectReferenceValue,
                GetFixedComponentType((DOTweenType)mAnimationType.enumValueIndex), true);
            if (mTarget.objectReferenceValue == null)
            {
                lastRect.y += LineHeight;
                EditorGUI.HelpBox(lastRect, "The From Target Cannot be empty!", MessageType.Error);
            }

            lastRect.y += LineHeight;
            EditorGUI.PropertyField(lastRect, mAnimationType);
            
            

            // From Value
            lastRect.y += LineHeight;
            EditorGUI.BeginDisabledGroup(!mUseFromValue.boolValue);
            {
                horizontalRect = lastRect;
                horizontalRect.width -= SetBtnWidth + ItemWidth;
                SetVectorValue(horizontalRect, "From Value", mFromValue, (DOTweenType)mAnimationType.enumValueIndex);
            }
            EditorGUI.EndDisabledGroup();

            horizontalRect.x += SetBtnWidth + horizontalRect.width;
            horizontalRect.width = ItemWidth;
            mUseFromValue.boolValue = EditorGUI.ToggleLeft(horizontalRect, "Use Value", mUseFromValue.boolValue);

            // To Target
            lastRect.y += LineHeight;
            if (mUseToTarget.boolValue)
            {
                horizontalRect = lastRect;
                horizontalRect.width -= SetBtnWidth + ItemWidth;
                mToTarget.objectReferenceValue = EditorGUI.ObjectField(horizontalRect, "To Target", mToTarget.objectReferenceValue,
                    GetFixedComponentType((DOTweenType)mAnimationType.enumValueIndex), true);
                if (mToTarget.objectReferenceValue == null)
                {
                    lastRect.y += LineHeight;
                    EditorGUI.HelpBox(lastRect, "The To Target Cannot be empty!", MessageType.Error);
                }
            }
            else
            {
                horizontalRect = lastRect;
                horizontalRect.width -= SetBtnWidth + ItemWidth;
                SetVectorValue(horizontalRect, "To Value", mToValue, (DOTweenType)mAnimationType.enumValueIndex);
            }

            horizontalRect.x += SetBtnWidth + horizontalRect.width;
            horizontalRect.width = ItemWidth;
            mUseToTarget.boolValue = EditorGUI.ToggleLeft(horizontalRect, "Use Target", mUseToTarget.boolValue);

            lastRect.y += LineHeight;
            EditorGUI.LabelField(lastRect, "--------------------- Sequence Params --------------------");

            // DurationOrSpeed
            lastRect.y += LineHeight;
            horizontalRect = lastRect;
            horizontalRect.width -= SetBtnWidth + ItemWidth;
            EditorGUI.PropertyField(horizontalRect, mDurationOrSpeed);

            // Snapping
            horizontalRect.x += SetBtnWidth + horizontalRect.width;
            horizontalRect.width = ItemWidth;
            mSpeedBased.boolValue = EditorGUI.ToggleLeft(horizontalRect, "Speed Based", mSpeedBased.boolValue);

            //Delay
            lastRect.y += LineHeight;
            horizontalRect = lastRect;
            horizontalRect.width -= SetBtnWidth + ItemWidth;
            EditorGUI.PropertyField(horizontalRect, mDelay);

            // Snapping
            horizontalRect.x += SetBtnWidth + horizontalRect.width;
            horizontalRect.width = ItemWidth;
            mSnapping.boolValue = EditorGUI.ToggleLeft(horizontalRect, "Snapping", mSnapping.boolValue);

            //Ease
            lastRect.y += LineHeight;
            horizontalRect = lastRect;
            horizontalRect.width -= SetBtnWidth + ItemWidth;
            EditorGUI.PropertyField(horizontalRect, mCurveOrEase.boolValue ? mEaseCurve : mEase);

            horizontalRect.x += SetBtnWidth + horizontalRect.width;
            horizontalRect.width = ItemWidth;
            mCurveOrEase.boolValue = EditorGUI.ToggleLeft(horizontalRect, "Use Curve", mCurveOrEase.boolValue);

            //Loops
            lastRect.y += LineHeight;
            horizontalRect = lastRect;
            horizontalRect.width -= SetBtnWidth + ItemWidth;
            EditorGUI.PropertyField(horizontalRect, mLoops);

            horizontalRect.x += SetBtnWidth + horizontalRect.width;
            horizontalRect.width = ItemWidth;
            EditorGUI.BeginDisabledGroup(mLoops.intValue == 1);
            mLoopType.enumValueIndex = (int)(LoopType)EditorGUI.EnumPopup(horizontalRect, (LoopType)mLoopType.enumValueIndex);
            EditorGUI.EndDisabledGroup();
            //UpdateType
            lastRect.y += LineHeight;
            EditorGUI.PropertyField(lastRect, mUpdateType);

            //Events
            lastRect.y += LineHeight;
            property.isExpanded = EditorGUI.Foldout(lastRect, property.isExpanded, "Animation Events");
            if (property.isExpanded)
            {
                //OnPlay
                lastRect.y += LineHeight;
                EditorGUI.PropertyField(lastRect, mOnPlay);

                //OnUpdate
                lastRect.y += EditorGUI.GetPropertyHeight(mOnPlay);
                EditorGUI.PropertyField(lastRect, mOnUpdate);

                //OnComplete
                lastRect.y += EditorGUI.GetPropertyHeight(mOnUpdate);
                EditorGUI.PropertyField(lastRect, mOnComplete);
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var onPlay = property.FindPropertyRelative("OnPlay");
            var onUpdate = property.FindPropertyRelative("OnUpdate");
            var onComplete = property.FindPropertyRelative("OnComplete");

            var target = property.FindPropertyRelative("Target");
            var useToTarget = property.FindPropertyRelative("UseToTarget");
            var toTarget = property.FindPropertyRelative("ToTarget");
            var count = 12 + (target.objectReferenceValue == null ? 1 : 0) +
                        (useToTarget.boolValue && toTarget.objectReferenceValue == null ? 1 : 0);

            return LineHeight * count +
                   (property.isExpanded ? (EditorGUI.GetPropertyHeight(onPlay) + EditorGUI.GetPropertyHeight(onUpdate) + EditorGUI.GetPropertyHeight(onComplete)) : 0);
        }

        private void SetVectorValue(Rect lastRect, string label, SerializedProperty property, DOTweenType tweenType)
        {
            switch (tweenType)
            {
                case DOTweenType.DOMove:
                case DOTweenType.DOLocalMove:
                case DOTweenType.DOAnchorPos3D:
                case DOTweenType.DOScale:
                case DOTweenType.DORotate:
                case DOTweenType.DOLocalRotate:
                {
                    property.vector4Value = EditorGUI.Vector3Field(lastRect, label, property.vector4Value);
                }
                    break;
                case DOTweenType.DOMoveX:
                case DOTweenType.DOLocalMoveX:
                case DOTweenType.DOScaleX:
                case DOTweenType.DOAnchorPosX:
                case DOTweenType.DOFillAmount:
                case DOTweenType.DOSliderValue:
                {
                    var value = property.vector4Value;
                    value.x = EditorGUI.FloatField(lastRect, label, value.x);
                    property.vector4Value = value;
                }
                    break;

                case DOTweenType.DOMoveY:
                case DOTweenType.DOLocalMoveY:
                case DOTweenType.DOScaleY:
                case DOTweenType.DOAnchorPosY:
                {
                    var value = property.vector4Value;
                    value.y = EditorGUI.FloatField(lastRect, label, value.y);
                    property.vector4Value = value;
                }
                    break;
                case DOTweenType.DOMoveZ:
                case DOTweenType.DOLocalMoveZ:
                case DOTweenType.DOScaleZ:
                case DOTweenType.DOAnchorPos3DZ:
                {
                    var value = property.vector4Value;
                    value.z = EditorGUI.FloatField(lastRect, label, value.z);
                    property.vector4Value = value;
                }
                    break;
                case DOTweenType.DOColorAlphaFade:
                case DOTweenType.DOCanvasGroupAlphaFade:
                {
                    var value = property.vector4Value;
                    value.w = EditorGUI.FloatField(lastRect, label, value.w);
                    property.vector4Value = value;
                }
                    break;
                case DOTweenType.DOAnchorPos:
                case DOTweenType.DOFlexibleSize:
                case DOTweenType.DOMinSize:
                case DOTweenType.DOPreferredSize:
                case DOTweenType.DOSizeDelta:
                {
                    property.vector4Value = EditorGUI.Vector2Field(lastRect, label, property.vector4Value);
                }
                    break;
                case DOTweenType.DOColor:
                {
                    property.vector4Value = EditorGUI.ColorField(lastRect, label, property.vector4Value);
                }
                    break;
            }
        }

        private Type GetFixedComponentType(DOTweenType tweenType)
        {
            switch (tweenType)
            {
                case DOTweenType.DOMove:
                case DOTweenType.DOMoveX:
                case DOTweenType.DOMoveY:
                case DOTweenType.DOMoveZ:
                case DOTweenType.DOLocalMove:
                case DOTweenType.DOLocalMoveX:
                case DOTweenType.DOLocalMoveY:
                case DOTweenType.DOLocalMoveZ:
                case DOTweenType.DOScale:
                case DOTweenType.DOScaleX:
                case DOTweenType.DOScaleY:
                case DOTweenType.DOScaleZ:
                case DOTweenType.DORotate:
                case DOTweenType.DOLocalRotate:
                    return typeof(Transform);
                case DOTweenType.DOAnchorPos:
                case DOTweenType.DOAnchorPosX:
                case DOTweenType.DOAnchorPosY:
                case DOTweenType.DOAnchorPos3D:
                case DOTweenType.DOAnchorPos3DZ:
                case DOTweenType.DOSizeDelta:
                    return typeof(RectTransform);
                case DOTweenType.DOColor:
                case DOTweenType.DOColorAlphaFade:
                    return typeof(UnityEngine.UI.Graphic);
                case DOTweenType.DOCanvasGroupAlphaFade:
                    return typeof(CanvasGroup);
                case DOTweenType.DOFillAmount:
                    return typeof(UnityEngine.UI.Image);
                case DOTweenType.DOFlexibleSize:
                case DOTweenType.DOMinSize:
                case DOTweenType.DOPreferredSize:
                    return typeof(UnityEngine.UI.LayoutElement);
                case DOTweenType.DOSliderValue:
                    return typeof(UnityEngine.UI.Slider);
            }

            return null;
        }
    }
}