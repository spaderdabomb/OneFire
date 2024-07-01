using UnityEngine.UIElements;
using UnityEngine;
using static ItemData;
using System.Collections.Generic;

namespace OneFireUi
{
    public partial class EquipmentInventory : BaseInventory
    {
        public List<EquipmentInventorySlot> equipmentSlots = new();
        public List<EquipmentSlotData> equipmentSlotData = new();
        public List<ItemType> equipmentSlotKeys = new();
        public EquipmentInventory(VisualElement root, int inventoryRows, int inventoryCols) : base(root, inventoryRows, inventoryCols)
        {
            AssignQueryResults(root);
            InitEquipmentInventory();
            InitEquipmentSlots();
        }

        private void InitEquipmentInventory()
        {
            // Clear current layout
            root.Clear();

            // Make lists
            foreach (var kvp in InventoryManager.Instance.equipmentSlotDataDict)
            {
                equipmentSlotData.Add(kvp.Value);
                equipmentSlotKeys.Add(kvp.Key);
            }
        }

        private void InitEquipmentSlots()
        {
            // Init slots
            Debug.Log("equipment");

            inventorySlots = new List<BaseInventorySlot>();
            for (int i = 0; i < inventoryRows; i++)
            {
                Debug.Log("slot");

                VisualElement gearSlotClone = InventoryManager.Instance.equipmentSlotAsset.CloneTree();
                EquipmentSlotData tempEquipementSlotData = i >= equipmentSlotData.Count ? null : equipmentSlotData[i];
                ItemType tempItemType = i >= equipmentSlotKeys.Count ? ItemType.None : equipmentSlotKeys[i];
                EquipmentInventorySlot newEquipmentSlot = new EquipmentInventorySlot(gearSlotClone, i, this, tempEquipementSlotData, tempItemType);
                newEquipmentSlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, newEquipmentSlot));
                equipmentSlots.Add(newEquipmentSlot);
                inventorySlots.Add(newEquipmentSlot);
                root.Add(gearSlotClone);
            }
        }
    }
}
