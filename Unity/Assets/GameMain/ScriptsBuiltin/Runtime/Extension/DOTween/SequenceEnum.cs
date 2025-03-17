// /************************************************************
//  * Unity Version: 2022.3.15f1c1
//  * Author:        bear
//  * CreateTime:    2024/10/31 15:10:50
//  * Description:
//  * Modify Record:
//  *************************************************************/

namespace GameMain.Builtin
{
    public enum AddType
    {
        Append,
        Join
    }

    public enum DOTweenType
    {
        DOMove,
        DOMoveX,
        DOMoveY,
        DOMoveZ,

        DOLocalMove,
        DOLocalMoveX,
        DOLocalMoveY,
        DOLocalMoveZ,

        DOScale,
        DOScaleX,
        DOScaleY,
        DOScaleZ,

        DORotate,
        DOLocalRotate,

        DOAnchorPos,
        DOAnchorPosX,
        DOAnchorPosY,
        DOAnchorPos3D,
        DOAnchorPos3DZ,

        DOSizeDelta,

        DOColor,
        DOColorAlphaFade,
        DOCanvasGroupAlphaFade,
        DOFillAmount,
        DOFlexibleSize,
        DOMinSize,
        DOPreferredSize,
        DOSliderValue
    }
}