using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCoreBluetooth;
using RSToolkit.Helpers;
using RSToolkit.UI.Controls;
using RSToolkit.Controls;

public class SampleBluetoothManager : UnityCoreBluetoothManager
{

    private Dictionary<string, Button> m_discoveredPeripherals = new Dictionary<string, Button>();
    private Dictionary<string, Button> m_connectedPeripherals = new Dictionary<string, Button>();
    private CBPeripheral m_selectedConnectedPeripheral = null;
    private CBPeripheral SelectedConnectedPeripheral{
        get{
            return m_selectedConnectedPeripheral;
        }set{
            m_selectedConnectedPeripheral = value;
            var cbt = DisconnectButton.GetComponentInChildren<Text>();
            if(value != null){
                cbt.text = "Disconnect From: " + value.name;
                DisconnectButton.interactable = true;
            }else{
                cbt.text = "Disconnect From: ";
                DisconnectButton.interactable = false;
            }
        }
    }
    private CBPeripheral m_selectedDiscoveredPeripheral = null;
    private CBPeripheral SelectedDiscoveredPeripheral{
        get{
            return m_selectedDiscoveredPeripheral;
        }set{
            m_selectedDiscoveredPeripheral = value;
            var cbt = ConnectButton.GetComponentInChildren<Text>();
            if(value != null){
                cbt.text = "Connect To: " + value.name;
                ConnectButton.interactable = true;
            }else{
                cbt.text = "Connect To: ";
                ConnectButton.interactable = false;
            }
        }
    }

    public string ServiceUUID = string.Format("{0}-{1}-{2}-{3}-{4}", RandomHelpers.GetRandomHexNumber(8),
                                                        RandomHelpers.GetRandomHexNumber(4),
                                                        RandomHelpers.GetRandomHexNumber(4),
                                                        RandomHelpers.GetRandomHexNumber(4),
                                                        RandomHelpers.GetRandomHexNumber(12));

    public Toggle IsScanningToggle;
    public UILogScrollView LogScrollView;

    public Button ConnectButton;
    public Button DisconnectButton;

    public Spawner DiscoveredDeviceButtonSpawner;
    public Spawner ConnectedDeviceButtonSpawner;

    protected override void Central_DidUpdateStateCallback(CBPeripheral.CBPeripheralState state)
    {
        //throw new System.NotImplementedException();
    }

    protected override void Central_DidDiscoverCallback(CBPeripheral peripheral){

       if (!m_discoveredPeripherals.ContainsKey(peripheral.uuid)){
           var db = DiscoveredDeviceButtonSpawner.SpawnAndGetGameObject().GetComponent<Button>();
           db.onClick.AddListener(() => { 
               SelectedDiscoveredPeripheral = peripheral;
            });
           db.GetComponentInChildren<Text>().text = peripheral.name;
            m_discoveredPeripherals.Add(peripheral.uuid, db);
       } 

    }
    protected override void Central_DidConnect(CBPeripheral peripheral){

       if (!m_connectedPeripherals.ContainsKey(peripheral.uuid)){
           var cb = ConnectedDeviceButtonSpawner.SpawnAndGetGameObject().GetComponent<Button>();
           cb.onClick.AddListener(() => {
               SelectedConnectedPeripheral = peripheral;
           });
           cb.GetComponentInChildren<Text>().text = peripheral.name;
           m_connectedPeripherals.Add(peripheral.uuid, cb);

       }
    }

    protected override void Central_DidDisconnectPeripheral(CBPeripheral peripheral){
        var cp = m_connectedPeripherals[peripheral.uuid];
        ConnectedDeviceButtonSpawner.DestroySpawnedGameObject(cp.gameObject);
        m_connectedPeripherals.Remove(peripheral.uuid);
        SelectedConnectedPeripheral = null;

    }

    // Start is called before the first frame update
    void Start()
    {
        SelectedDiscoveredPeripheral = null;
        InvokeRepeating("CheckIsScanning", 0f, 2f);        
    }

    void CheckIsScanning(){
        if(IsScanningToggle.isOn != CentralManager.IsScanning()){
            if(CentralManager.IsScanning()){
                LogScrollView.AppendLog("> Is Scanning");
            }else{
                LogScrollView.AppendLog("> Is NOT Scanning");
            }
        }
        IsScanningToggle.isOn = CentralManager.IsScanning();
    }

    public void ConnectToPeripheral(){
        if(SelectedDiscoveredPeripheral != null){
            CentralManager.ConnectToPeripheral(SelectedDiscoveredPeripheral.name);
        }
    }

    public void CancelConnectFromPeripheral(){
        if(SelectedConnectedPeripheral != null){
            CentralManager.CancelPeripheralConnection(SelectedConnectedPeripheral.name);
        }
    }

    public void StartScanButton_onClick(){
        CentralManager.ScanEnable(ServiceUUID);
        LogScrollView.AppendLog("Scan Enable");

    }
    public void StopScanButton_onClick(){
        CentralManager.ScanDisable();
        LogScrollView.AppendLog("Scan Disable");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
