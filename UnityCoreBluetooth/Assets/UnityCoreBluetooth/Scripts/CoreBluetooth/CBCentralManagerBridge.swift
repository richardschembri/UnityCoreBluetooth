//
//  CBCentralManagerBridge.swift
//  CoreBluetoothSwift
//
//  Created by Richard on 2017/05/16.
//

import Foundation
import CoreBluetooth

class CBCentralManagerBridge: NSObject, CBCentralManagerDelegate { //UIViewController
    
    var kCallbackTarget = "" // Unity Game Object Name
    
    static let sharedInstance: CBCentralManagerBridge = CBCentralManagerBridge()
    //let serviceUUID = ""
    var centralmanager: CBCentralManager!
    //var myTargetPeripheral: CBPeripheral!
    var peripheralArray : [CBPeripheral] = []
    
    func initCentralManager( _ targetName : String ) {
        kCallbackTarget = targetName
        centralmanager = CBCentralManager(delegate: self, queue: nil, options: nil)
    }
    //CBCentralManager delegate method
    //Checks the central manager is working and if it's working (powered on) it starts looking for peripherals around
    func centralManagerDidUpdateState(_ central: CBCentralManager) {
        print("state \(central.state)");
        switch (central.state) {
        case .poweredOff:
            print("Bluetooth Powered Off") 
        case .poweredOn:
            print("Bluetooth Powered On") 
        case .resetting:
            print("Resetting") 
        case .unauthorized:
            print("Unauthenticated state")
        case .unknown:
            print("Unknown") 
        case .unsupported:
            print("Unsupported")
        }
        UnitySendMessageX( methodName: "OnDidUpdateState", messageToSend: central.state.rawValue)
    }
    // Start scanning for Bluetooth devices
    func ScanEnable( _ serviceUUID : String) {
        self.centralmanager.scanForPeripherals(withServices: [CBUUID(string: serviceUUID)], options: [CBCentralManagerScanOptionAllowDuplicatesKey:false])
    }
    // Stop scanning for Bluetooth devices
    func ScanDisable() {
        self.centralmanager.stopScan()
    }
    func ScanDataReset(){ peripheralArray.removeAll() }
    func IsScanning() -> Bool{
        if let centralManager = centralmanager {
            return centralManager.isScanning
        }
        return false
    }
    func IsPoweredOn() -> Bool{
        if let centralManager = centralmanager {
            if(centralManager.state.rawValue == CBCentralManagerState.poweredOn.rawValue){ return true }
        }
        return false
    }
    func ConnectToPeripheral(peripheralName : String){
        for p in peripheralArray.enumerated(){
            if(peripheralName == p.element.name){
                centralmanager.connect(p.element, options: nil)
            }
        }
    }
    func CancelPeripheralConnection(peripheralName : String){
        for p in peripheralArray.enumerated(){
            if(peripheralName == p.element.name){
                centralmanager.cancelPeripheralConnection(p.element)
            }
        }
    }
    func getJSONStringFromDic(dic:[String:String]) -> String?{
        do {
            let jsonData = try JSONSerialization.data(withJSONObject: dic, options: .prettyPrinted)
            return String(data: jsonData, encoding: String.Encoding.utf8)!
        } catch let error as NSError {
            print(error)
            return nil
        }
    }

    func getPeripheralAsDict(peripheral: CBPeripheral) -> [String : String]{
        return ["name":peripheral.name!, "uuid":peripheral.identifier.uuidString ] as [String : String]
    }

    func getCharacteristicAsDict(characteristic: CBCharacteristic) -> [String : String]{
        return ["uuid":characteristic.identifier.uuidString ] as [String : String]
    }

    func getPeripheralCharacteristicAsDict(peripheral: CBPeripheral, withCharacteristic characteristic: CBCharacteristic){
        let peripheralDict = getPeripheralAsDict(peripheral: CBPeripheral)
        let peripheralJson = getJSONStringFromDic(dic: peripheralAsDic)
        let characteristicDict = getCharacteristicAsDict(characteristic: CBCharacteristic)
        return String(format:"{peripheral:%@")
    }

    //CBCentralManager delegate method
    //Did discover peripheral
    func centralManager(_ central: CBCentralManager, didDiscover peripheral: CBPeripheral, advertisementData: [String : Any], rssi RSSI: NSNumber) {
        
        let peripheralFoundMessage = "New Peripheral Found [" + peripheral.name! + "]"
        print(peripheralFoundMessage)
        peripheralArray.append(peripheral)
        let peripheralAsDic = ["name":peripheral.name!, "uuid":peripheral.identifier.uuidString ] as [String : String]
        do {
            print("Serializing JSON")
            let jsonDataString = getJSONStringFromDic(dic: peripheralAsDic)
            UnitySendMessageX( methodName: "OnDidDiscover", messageToSend: jsonDataString!)
        } catch let error as NSError {
            UnitySendErrorMessage(errorMessage: error.localizedDescription)
        }
    }
    func centralManager(_ central: CBCentralManager, didConnect peripheral: CBPeripheral) {
        CBTargetPeripheralBridge.sharedInstance.initTargetPeripheral(targetPeripheral: peripheral, centralManager: centralmanager)
        let peripheralAsDic = ["name":peripheral.name!, "uuid":peripheral.identifier.uuidString ] as [String : String]
        let jsonDataString = getJSONStringFromDic(dic: peripheralAsDic)
        UnitySendMessageX(methodName: "OnDidConnect", messageToSend: jsonDataString!)
    }
    
    func centralManager(_ central: CBCentralManager, didDisconnectPeripheral peripheral: CBPeripheral, error: Error?) {
        let peripheralAsDic = ["name":peripheral.name!, "uuid":peripheral.identifier.uuidString ] as [String : String]
        let jsonDataString = getJSONStringFromDic(dic: peripheralAsDic)
        ScanDataReset()
        UnitySendMessageX(methodName: "OnDidDisconnectPeripheral", messageToSend: jsonDataString!)
    }
    
    func centralManager(_ central: CBCentralManager, didFailToConnect peripheral: CBPeripheral, error: Error?) {
        let peripheralAsDic = ["name":peripheral.name!, "uuid":peripheral.identifier.uuidString ] as [String : String]
        let jsonDataString = getJSONStringFromDic(dic: peripheralAsDic)
        UnitySendMessageX(methodName: "OnDidFailToConnect", messageToSend: jsonDataString!)
    }
    
    func peripheral(_ peripheral: CBPeripheral, didUpdateValueFor characteristic: CBCharacteristic, error: Error?) {
        UnitySendMessageX( methodName: "PeripheralDidUpdateValueForCharacteristic", messageToSend: "FOO")
        if (error != nil) {
            print("Read Error: \(String(describing: error)), Characteristic uuid: \(characteristic.uuid)")
            let characteristicErrorDic = ["uuid":characteristic.uuid.uuidString, "error":error.debugDescription ] as [String : String]
            let characteristicErrorJSONString = getJSONStringFromDic(dic: characteristicErrorDic)
            UnitySendMessageX( methodName: "PeripheralDidUpdateValueForCharacteristicError", messageToSend: characteristicErrorJSONString!)
        }
    }
    func UnitySendErrorMessage(errorMessage: String){
        print(errorMessage)
        UnitySendMessageX(methodName: "OnIOSError", messageToSend: errorMessage)
    }
    func UnitySendMessageX(methodName: String, messageToSend: String){
        print("Sending message to Unity method [" + methodName + "] : " + messageToSend)
        UnitySendMessage(kCallbackTarget, methodName, messageToSend)
    }
}

