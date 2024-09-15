using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterraMiniFridge.BZ.Runtime
{
    public class AlterraMiniFridgeContainer : StorageContainer
    {
        public Action OnOpenEvent;
        public Action OnCloseEvent;

        public override void OnOpen()
        {
            base.OnOpen();
            OnOpenEvent?.Invoke();
        }
        public override void OnClose()
        {
            base.OnClose();
            OnCloseEvent?.Invoke();
        }
    }
}
