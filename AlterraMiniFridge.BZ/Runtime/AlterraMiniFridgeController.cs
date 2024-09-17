using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AlterraMiniFridge.Runtime
{
    public class AlterraMiniFridgeController : MonoBehaviour
    {
        private PowerRelay powerRelay;
        private float _energyCost = 0.01f;
        private Transform FridgeDoor;

        public AlterraMiniFridgeContainer storageContainer;

        void Start()
        {
            FridgeDoor = gameObject.transform.Find("model/FridgeBody/FridgeDoor").transform;

            powerRelay = PowerSource.FindRelay(base.transform);
#if SN
            powerRelay.powerUpEvent.AddHandler(this, OnPowerStatus);
            powerRelay.powerDownEvent.AddHandler(this, OnPowerStatus);
#elif BZ
            powerRelay.powerStatusEvent.AddHandler(this, OnPowerStatus);
#endif

            storageContainer = gameObject.GetComponent<AlterraMiniFridgeContainer>();
            storageContainer.container.onAddItem += AddItem;
            storageContainer.container.isAllowedToAdd += new global::IsAllowedToAdd(this.IsAllowedToAdd);
            storageContainer.OnOpenEvent += () => AnimateDoor(true);
            storageContainer.OnCloseEvent += () => AnimateDoor(false);

            InvokeRepeating("RequestEnergy", 0, 5);
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            var eadible = pickupable.GetComponent<Eatable>();
            return eadible != null;
        }

        private void OnPowerStatus(PowerRelay relay)
        {
            bool isPowered = this.powerRelay.IsPowered();

            foreach (InventoryItem inventoryItem in (IEnumerable<InventoryItem>)this.storageContainer.container)
            {
                Eatable component = inventoryItem.item.GetComponent<Eatable>();
                if ((bool)(Object)component && component.decomposes)
                {
                    if (!isPowered)
                    {
#if SN
                        component.decomposes = true;
#elif BZ
                        component.UnpauseDecay();
#endif
                    }
                    else
                    {
#if SN
                        component.decomposes = false;
#elif BZ
                        component.PauseDecay();
#endif
                    }
                }
            }
        }

        private void RequestEnergy()
        {
            if (powerRelay != null)
            {
                powerRelay.ModifyPower(_energyCost * -1, out float modified);
            }
        }

        private void AddItem(InventoryItem item)
        {
            if (item == null || (UnityEngine.Object)item.item == (Object)null)
                return;
            Eatable component = item.item.GetComponent<Eatable>();
            if (!((Object)component != (Object)null) || !component.decomposes || !this.powerRelay.IsPowered())
                return;
#if SN
            component.decomposes = false;
#elif BZ
                        component.PauseDecay();
#endif
        }

        private void RemoveItem(InventoryItem item)
        {
            if (item == null || (Object)item.item == (Object)null)
                return;
            Eatable component = item.item.GetComponent<Eatable>();
            if (!((Object)component != (Object)null) || !component.decomposes)
                return;
#if SN
            component.decomposes = true;
#elif BZ
                        component.UnpauseDecay();
#endif
        }

        private void AnimateDoor(bool isOpening)
        {
            if (isOpening)
            {
                FridgeDoor.transform.localRotation = Quaternion.Euler(0, 0, -17);
            }
            else
            {
                FridgeDoor.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
