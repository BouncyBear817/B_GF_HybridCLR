using GameFramework;
using UnityEngine;

namespace GameMain.Example
{
    public class Example_ReferencePool : MonoBehaviour
    {
        private void Start()
        {
            GameFramework.ReferencePool.Add<Example_Reference>(5);
            var exampleReference = GameFramework.ReferencePool.Acquire<Example_Reference>();
            exampleReference.Show();

            var referenceInfos = GameFramework.ReferencePool.GetAllReferencePoolInfos();
            foreach (var info in referenceInfos)
            {
                Debug.Log($"Type ({info.Type.FullName}) has ({info.UnusedReferenceCount}) unusedReferenceCount, ({info.UsingReferenceCount}) usingReferenceCount.");
            }
            
            GameFramework.ReferencePool.Release(exampleReference);
            referenceInfos = GameFramework.ReferencePool.GetAllReferencePoolInfos();
            foreach (var info in referenceInfos)
            {
                Debug.Log($"Type ({info.Type.FullName}) has ({info.UnusedReferenceCount}) unusedReferenceCount, ({info.UsingReferenceCount}) usingReferenceCount.");
            }
        }
    }

    public class Example_Reference : IReference
    {
        public void Show()
        {
            Debug.Log("example show.");
        }
        public void Clear()
        {
            Debug.Log("example is released.");
        }
    }
}