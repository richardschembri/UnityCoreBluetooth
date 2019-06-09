#if UNITY_EDITOR
namespace UnityCoreBluetooth
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEditor;
    using UnityEngine;
    using RSToolkit.Helpers;
    using System;
    
    using marijnz.EditorCoroutines;

    public class DummyBLEController : EditorWindow
    {
        private const int DEFAULT_PERIPHERALS_LENGTH = 5;

        private bool m_isCentralPowerOn = false;
        private bool m_isCentralScanning = false;
        private CBPeripheral m_Connected_peripheral = null;
        private List<CBPeripheral> m_peripherals = new List<CBPeripheral>();

        public ReadOnlyCollection<CBPeripheral> Peripherals { get { return m_peripherals.AsReadOnly(); } }

        private void GeneratePeripherals(int _length = DEFAULT_PERIPHERALS_LENGTH)
        {
            for (int i = 0; i < _length; i++)
            {
                CBPeripheral peripheral = new CBPeripheral("{ name:\"BLE_00220" + i + "\",uuid:\"BlueTooth1982939329" + i + "\"}");
                m_peripherals.Add(peripheral);
            }
        }

        public DummyBLEController(){
            m_instance = this;
        }

        [MenuItem("Window/Dummy BLE Controller")]
        public static void ShowWindow() {
            
            var window = EditorWindow.GetWindow(typeof(DummyBLEController), false, "Dummy BLE Controller");
            window.maxSize = new Vector2(400,400); 
        }

        private static DummyBLEController m_instance;
        public static DummyBLEController Instance{
            get{
                
                return m_instance;
            }
        }

        public void ScanDisable() { m_isCentralScanning = false; }
        public void ScanListReset() { m_peripherals.Clear(); }

        public bool IsPoweredOn() { return m_isCentralPowerOn; }
        public bool IsScanning() { return m_isCentralScanning; }

        public void ScanEnable(int _disCoverPeripheralSize = DEFAULT_PERIPHERALS_LENGTH)
        {
            m_isCentralScanning = true;
            GeneratePeripherals(_disCoverPeripheralSize);
            TimerHelper.StartIntervalTimer(m_peripherals.Count, 1, (time) => { if (IsScanning()) { OnDidDiscover(ParsePeriphralToJson(m_peripherals[(int)time - 1])); } });
        }

        private void OnDidDiscover(string _peripheralJSON) { UnityCoreBluetoothManager.Instance.OnDidDiscover(_peripheralJSON); }
        private void OnDidConnect(CBPeripheral peripheral) { UnityCoreBluetoothManager.Instance.OnDidConnect(peripheral.ToJSON()); }
        private void OnDidDisconnectPeripheral() { UnityCoreBluetoothManager.Instance.OnDidDisconnectPeripheral(m_Connected_peripheral.ToJSON()); }

        public void DiscoverServicesAndTrackCharacteristics(string _uuid) { UnityCoreBluetoothManager.Instance.OnDiscoverCharacteristics(_uuid); }
        public void CmdSend(string _characteristicUUID, byte[] _s_buff, int _length, Action<string, byte[], int> _delegate = null) { if (_delegate != null) { _delegate(_characteristicUUID, _s_buff, _length); } }

        private void OnDidDiscoverServices(string peripheralName) { UnityCoreBluetoothManager.Instance.OnDidDiscoverServices(peripheralName); }
        private void OnDiscoverCharacteristics(string characteristicUUID) { UnityCoreBluetoothManager.Instance.OnDiscoverCharacteristics(characteristicUUID); }
        private void PeripheralDidUpdateValueForCharacteristic(string characteristicJSONString) { UnityCoreBluetoothManager.Instance.PeripheralDidUpdateValueForCharacteristic(characteristicJSONString); }


        public void ConnectToPeripheral(string peripheralName)
        {

            for (int i = 0; i < m_peripherals.Count; i++)
            {
                if (m_peripherals[i].name == peripheralName)
                {
                    //EditorCoroutines.StartCoroutine(OnDidConnectCoroutine(m_peripherals[i]), this);
                    break;
                }
            }
        }

        IEnumerator OnDidConnectCoroutine(CBPeripheral peripheral){
            yield return new WaitForSeconds(RandomHelpers.RandomFloatWithinRange(1, 3, 1));
            OnDidConnect(peripheral);
            m_Connected_peripheral = peripheral;
            if (IsScanning()) { ScanDisable(); }

            TimerHelper.StartTimer(RandomHelpers.RandomFloatWithinRange(1, 5, 1), () => { DiscoverServicesAndTrackCharacteristics(RandomHelpers.GetRandomHexNumber(5).ToString()); });
        }

        public void CancelPeripheralConnection(string _peripheralName) 
        {
            if(_peripheralName == null || _peripheralName == "" || m_Connected_peripheral == null){ return; }
            if(m_Connected_peripheral.name == _peripheralName) 
            {
                OnDidDisconnectPeripheral();
                m_Connected_peripheral = null;
            }
        }

        public static string ParsePeriphralToJson(CBPeripheral _periphral)
        {
            if (_periphral == null) { return ""; }
            return "{ name:\"" + _periphral.name + "\",uuid:\"" + _periphral.uuid + "\"}";
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("Application is not running", GUILayout.Height(300));
                UnityCoreBluetoothManager.Instance.CentralManager.ScanDisable();
                return; 
            }
            m_isCentralPowerOn = EditorGUILayout.Toggle("Powered On", UnityCoreBluetoothManager.Instance.CentralManager.IsPoweredOn());

            if (!UnityCoreBluetoothManager.Instance.CentralManager.IsPoweredOn())
            {
                UnityCoreBluetoothManager.Instance.CentralManager.ScanDisable();
            }

            EditorGUI.BeginDisabledGroup(!UnityCoreBluetoothManager.Instance.CentralManager.IsPoweredOn());
            if (UnityCoreBluetoothManager.Instance.CentralManager.IsScanning()){
                if(GUILayout.Button("Stop Scanning")){
                    UnityCoreBluetoothManager.Instance.CentralManager.ScanDisable();
                }
            }else{
                if(GUILayout.Button("Start Scanning")){
                    string serviceUUID = string.Format("{0}-{1}-{2}-{3}-{4}", RandomHelpers.GetRandomHexNumber(8),
                                                                        RandomHelpers.GetRandomHexNumber(4),
                                                                        RandomHelpers.GetRandomHexNumber(4),
                                                                        RandomHelpers.GetRandomHexNumber(4),
                                                                        RandomHelpers.GetRandomHexNumber(12));
                    UnityCoreBluetoothManager.Instance.CentralManager.ScanEnable(serviceUUID);

                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Is Scanning", IsScanning());
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif