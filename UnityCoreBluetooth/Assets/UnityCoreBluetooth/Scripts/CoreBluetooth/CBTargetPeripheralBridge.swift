//
//  CBPeripheralBridge.swift
//  Unity-iPhone
//
//  Created by Richard on 2017/05/19.
//

import Foundation
import CoreBluetooth

class CBTargetPeripheralBridge: NSObject, CBPeripheralDelegate{
    let kCallbackTarget = "Swift BLE Manager" // Unity Game Object Name
    
    static let sharedInstance: CBTargetPeripheralBridge = CBTargetPeripheralBridge()
    
    var TargetPeripheral: CBPeripheral!
    var CentralManager: CBCentralManager!
    var serviceArray : [CBService] = []
    
    //Convert these to CBUUID
    var characteristicsWithVariableValueUUIDs : [String] = []
    var characteristicsWithStaticValueUUIDs : [String] = []

    var characteristicsWithVariableValue : [CBCharacteristic] = []
    var characteristicsWithStaticValue : [CBCharacteristic] = []
    
    /*
    mySecondViewController.setPeripheral(peripheral)
    mySecondViewController.setCentralManager(central)
    mySecondViewController.searchService()
    */
    
    func initTargetPeripheral(targetPeripheral : CBPeripheral, centralManager : CBCentralManager){
        self.TargetPeripheral = targetPeripheral
        self.CentralManager = centralManager
        self.TargetPeripheral.delegate = self
    }
    
    func DiscoverServices(){
        self.TargetPeripheral.discoverServices(nil)
        print("Start Discover Services")
    }
    
    //func DiscoverServicesAndTrackCharacteristics(characteristicsWithVariableValueUUIDs : [String], characteristicsWithStaticValueUUIDs : [String])
    func DiscoverServicesAndTrackCharacteristics(characteristicsWithVariableValueUUIDs : String, characteristicsWithStaticValueUUIDs : String)
    {
        self.characteristicsWithStaticValue.removeAll()
        self.characteristicsWithVariableValue.removeAll()
        
        self.characteristicsWithStaticValueUUIDs = characteristicsWithStaticValueUUIDs.components(separatedBy: ",")
        self.characteristicsWithVariableValueUUIDs = characteristicsWithVariableValueUUIDs.components(separatedBy: ",")
        
        DiscoverServices()
    }
    
    // サービス発見時に呼ばれる
    func peripheral(_ peripheral: CBPeripheral, didDiscoverServices error: Error?) {
        
        print("didDiscoverServices")
        
        if (error != nil) {
            print("エラー: \(error)")
            UnitySendErrorMessage(errorMessage: error.debugDescription)
            return
        }
        
        for service in peripheral.services! {
            
            if(!serviceArray.contains(service)){
                print("P: \(peripheral.name) - Discovered service S:'\(service.uuid)'")
                serviceArray.append(service)
                print("Start Discover Characteristics")
                peripheral.discoverCharacteristics(nil, for: service)
                UnitySendMessageX(methodName: "OnDidDiscoverServices", messageToSend: service.uuid.uuidString)
            }
            
        }
    }
    
    //  Did Discover Characteristics For Service
    func peripheral(_ peripheral: CBPeripheral,
                    didDiscoverCharacteristicsFor service: CBService,
                    error: Error?)
    {
        if (error != nil) {
            print("エラー: \(error)")
        
            UnitySendErrorMessage(errorMessage: error.debugDescription)
            
            return
        }
        
        if !((service.characteristics?.count)! > 0) {
            print("no characteristics")
            return
        }
        
        let characteristics = service.characteristics!
        print(String(characteristics.count) + " characteristics found！ ")

        
        for characteristic in characteristics{
    
            // Set Notify Value for this characteristic?
            if (characteristicsWithVariableValueUUIDs.contains(characteristic.uuid.uuidString)){
                print("setNotifyValue for " + characteristic.uuid.uuidString)
                print("")
                peripheral.setNotifyValue(true, for: characteristic)
            }
            
            if(characteristicsWithStaticValueUUIDs.contains(characteristic.uuid.uuidString) && !characteristicsWithStaticValue.contains(characteristic)){
                print("Saving characteristicsWithStaticValue [" + characteristic.uuid.uuidString + "] to memory")
                characteristicsWithStaticValue.append(characteristic)
            }
            
            if(characteristicsWithVariableValueUUIDs.contains(characteristic.uuid.uuidString) && !characteristicsWithVariableValue.contains(characteristic)){
                print("Saving characteristicsWithVariableValue [" + characteristic.uuid.uuidString + "] to memory")
                characteristicsWithVariableValue.append(characteristic)
            }
            UnitySendMessageX(methodName: "OnDiscoverCharacteristics", messageToSend: characteristic.uuid.uuidString)
        }
        
    }
    
    // Called when Notify starts/stops / Notify開始／停止時に呼ばれる
    func peripheral(_ peripheral: CBPeripheral,
                    didUpdateNotificationStateFor characteristic: CBCharacteristic,
                    error: Error?)
    {
        print("didUpdateNotificationStateFor :\(characteristic.uuid)")
        if error != nil {
            let errorMsg = "didUpdateNotificationStateFor error with Characteristic [" + characteristic.uuid.uuidString + "]; Error Message: " + error.debugDescription
            
            UnitySendErrorMessage(errorMessage: errorMsg)
        }else{
            print("Notify status update success！characteristic UUID:\(characteristic.uuid), isNotifying: \(characteristic.isNotifying)")
            
            UnitySendMessageX(methodName: "PeripheralDidUpdateNotificationStateForCharacteristic", messageToSend: characteristic.uuid.uuidString)
            
        }
    }
    

    // On Characteristic Data Read (End)
    func peripheral(_ peripheral: CBPeripheral,
                    didUpdateValueFor characteristic: CBCharacteristic,
                    error: Error?){
        if (error != nil){
            let errorMsg = "didUpdateValueFor error with Characteristic [" + characteristic.uuid.uuidString + "]; Error Message: " + error.debugDescription
            
            UnitySendErrorMessage(errorMessage: errorMsg)
            return
        }
        
        let nsDataValue: NSData? = (characteristic.value as NSData?)
        
        if (nsDataValue == nil){
            return
        }
        
        let dataLength: Int = (nsDataValue?.length)!
        
        
        var  dat = [CUnsignedChar](repeating: 0, count: dataLength)
        
        nsDataValue?.getBytes(&dat, length: dataLength)
        
        let characteristicAsDic = ["uuid": characteristic.uuid.uuidString, "value":dat] as [String: Any]
        
        let characteristicAsJSONString = getJSONStringFromDic(dic: characteristicAsDic)
    
        
        UnitySendMessageX(methodName: "PeripheralDidUpdateValueForCharacteristic", messageToSend: characteristicAsJSONString!)
    }
    
    func getJSONStringFromDic(dic:[String:Any]) -> String?{
        do {
            let jsonData = try JSONSerialization.data(withJSONObject: dic, options: .prettyPrinted)
            return String(data: jsonData, encoding: String.Encoding.utf8)!
        } catch let error as NSError {
            print(error)
            return nil
        }
    }
    
    func CMDSend(characteristicUUID: String, s_buff:UnsafeMutablePointer<CUnsignedChar>, len:Int){
        
        let data: Data = Data(bytes: UnsafePointer<UInt8>(s_buff), count:len)
        //var chr: CBCharacteristic
      print("CMDSend characteristic [" + characteristicUUID + "]")
        
        for chr in characteristicsWithStaticValue{
            if(chr.uuid.uuidString.contains(characteristicUUID)){
                print("Sending command to characteristic [" + characteristicUUID + "] of BLE device")
                self.TargetPeripheral.writeValue(data, for: chr, type: CBCharacteristicWriteType.withResponse)
                return
            }
        }
        
    }
    
    func ReadValue(characteristicUUID: String){
        for chr in characteristicsWithVariableValue{ //characteristicsWithStaticValue{
            if(chr.uuid.uuidString.contains(characteristicUUID)){
                print("Performing Read Value for characteristic [" + characteristicUUID + "] of BLE device")
                self.TargetPeripheral.readValue(for: chr)
                return
            }
        }
    }
    
    /*
    func ptrToArray(src: UnsafeMutablePointer<CUnsignedChar>, length: Int) -> Array<CUnsignedChar> {
        var dst = [CUnsignedChar?](repeating: nil, count: length)
        
        dst.withUnsafeMutableBufferPointer({
            ptr -> UnsafeMutablePointer<CUnsignedChar> in
            return ptr.baseAddress
        }).initializeFrom(src, count: length)
        
        return dst
    }
    */
    
    func peripheral(_ peripheral: CBPeripheral, didReadRSSI RSSI: NSNumber, error: Error?) {
        print("RSSI: " + String(describing: RSSI))
        
    }
    
    func UnitySendErrorMessage(errorMessage: String){
        print(errorMessage)
        print("")
        UnitySendMessageX(methodName: "OnIOSError", messageToSend: errorMessage)
    }
    
    
    func UnitySendMessageX(methodName: String, messageToSend: String){
        print("Sending message to Unity method [" + methodName + "]") // : " + messageToSend)
        print("")
        UnitySendMessage(kCallbackTarget, methodName, messageToSend)
    }
    
    func UnityDebugLog(message: String){
        print(message)
    }
    
}
