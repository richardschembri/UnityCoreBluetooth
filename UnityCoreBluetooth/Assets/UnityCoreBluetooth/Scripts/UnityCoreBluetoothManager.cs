namespace UnityCoreBluetooth
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Serialization;

    public abstract class UnityCoreBluetoothManager : MonoBehaviour 
    {
        private const string GAMEOBJECT_DEFAULTNAME = "Swift BLE Manager";

        [FormerlySerializedAs("Service UUID ")]
        [SerializeField]
        private string m_serviceUUID;
        public string ServiceUUID { get; protected set; }
        public CBCentralManagerBridge CentralManager { get { return CBCentralManagerBridge.Instance; } }
        public CBTargetPeripheralBridge TargetManager { get { return CBTargetPeripheralBridge.Instance; } }
        public bool isAutoInit = false;

        private List<CBPeripheral> discover_peripherals = new List<CBPeripheral>();
        public ReadOnlyCollection<CBPeripheral> DiscoverPeripherals { get { return discover_peripherals.AsReadOnly(); } }

        private List<CBPeripheral> connect_peripherals = new List<CBPeripheral>();
        public ReadOnlyCollection<CBPeripheral> ConnectedPeripherals { get { return connect_peripherals.AsReadOnly(); } }


        private static UnityCoreBluetoothManager m_instance;
        public static UnityCoreBluetoothManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        private void Awake()
        {

            m_instance = this;
        }
        protected virtual void Central_DidUpdateStateCallback(CBPeripheral.CBPeripheralState state) { }
        protected virtual void Central_DidDiscoverCallback(CBPeripheral peripheral) { }
        protected virtual void Central_DidConnect(CBPeripheral peripheral) { }
        protected virtual void Central_DidDisconnectPeripheral(CBPeripheral peripheral) { }
        protected virtual void Central_DidFailToConnect(CBPeripheral peripheral) { }

        protected virtual void Target_DidDiscoverServices(string serviceUUID) { }
        protected virtual void Target_DiscoverCharacteristics(string characteristicUUID) { }
        protected virtual void Target_PeripheralDidUpdateValueForCharacteristic(string characteristicJSONString) { }
        protected virtual void Target_PeripheralDidUpdateNotificationStateForCharacteristic(string characteristicUUID) { }
        protected virtual void IOSError(string errorMessage) { }

        public void OnDidUpdateState(string state) 
        {
            var peripheralState = CBPeripheral.CBPeripheralState.noState;//

            try{
                peripheralState = (CBPeripheral.CBPeripheralState) Enum.Parse(typeof(CBPeripheral.CBPeripheralState), state, true);
            }catch(Exception ex){
                Debug.LogError("Error occured whilst getting state!");
            }
            Central_DidUpdateStateCallback(peripheralState);
        }
        public void OnDidDiscover(string jsonDataString) { 
            CBPeripheral peripheral = new CBPeripheral(jsonDataString);
            if(!discover_peripherals.Any(n => n.name.Equals(peripheral.name)) ) { discover_peripherals.Add(peripheral); }
            Central_DidDiscoverCallback(peripheral); 
        }
        public void OnDidConnect(string jsonDataString) {
            CentralManager.ScanDisable();
            CBPeripheral peripheral = new CBPeripheral(jsonDataString);
            if (!connect_peripherals.Any(n => n.name.Equals(peripheral.name))) { connect_peripherals.Add(peripheral); }
            Central_DidConnect(peripheral); 
        }
        public void OnDidDisconnectPeripheral(string jsonDataString) {
            CBPeripheral peripheral = new CBPeripheral(jsonDataString);
            Central_DidDisconnectPeripheral(peripheral);
            if (connect_peripherals.Any(n => n.name.Equals(peripheral.name))) { connect_peripherals.Remove(peripheral); }
            discover_peripherals.Clear();
        }
        public void OnDidFailToConnect(string jsonDataString) { Central_DidFailToConnect(new CBPeripheral(jsonDataString)); }

        public void OnDidDiscoverServices(string serviceUUID) { Target_DidDiscoverServices(serviceUUID); }
        public void OnDiscoverCharacteristics(string characteristicUUID) { Target_DiscoverCharacteristics(characteristicUUID); }
        public void PeripheralDidUpdateValueForCharacteristic(string characteristicJSONString) { Target_PeripheralDidUpdateValueForCharacteristic(characteristicJSONString); }
        public void PeripheralDidUpdateNotificationStateForCharacteristic(string characteristicUUID) { Target_PeripheralDidUpdateNotificationStateForCharacteristic(characteristicUUID); }
        public void OnIOSError(string errorMessage) { IOSError(errorMessage); }
    }

}