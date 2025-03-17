using GameFramework;
using GameFramework.Event;
using GameMain.Builtin;
using UnityEngine;

namespace GameMain.Example
{
    public class Example_EventPool : MonoBehaviour
    {
        private void Start()
        {
            var eventArgs = new ExampleEventArgs
            {
                Name = "example"
            };

            MainEntry.Event.Subscribe(eventArgs.Id, Handler);

            MainEntry.Event.FireNow(this, eventArgs);
        }

        private void Handler(object sender, BaseEventArgs e)
        {
            var args = e as ExampleEventArgs;
            if (args != null)
                Debug.Log(args.Name + "1");
        }
    }

    public class ExampleEventArgs : GameEventArgs
    {
        public override void Clear()
        {
        }

        public override int Id => 5;

        public string Name;
    }
}