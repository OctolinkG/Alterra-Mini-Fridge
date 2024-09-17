using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterraMiniFridge.Runtime
{
    public class AlterraMiniFridgeContainer : StorageContainer
    {
        public Action OnOpenEvent;
        public Action OnCloseEvent;

#if SN
        void Start()
        {
            onUse.AddListener(OnUse);
        }

        private void OnUse()
        {
            OnOpenEvent?.Invoke();
        }
#elif BZ

        public override void OnOpen()
        {
            base.OnOpen();
            OnOpenEvent?.Invoke();
        }
#endif
        public override void OnClose()
        {
            base.OnClose();
            OnCloseEvent?.Invoke();
        }
    }
}
