//
//  CBCentralManagerBridge.m
//  CoreBluetoothSwift
//
//  Created by tongull1 on 2017/05/16.
//  Copyright © 2017 tongullman. All rights reserved.
//

#import <CoreBluetooth/CoreBluetooth.h>
#include "unityswift-Swift.h"

#pragma mark -C interface

extern "C" {
    
    void _cbcm_initCentralManager(const char *targetName) {
        [[CBCentralManagerBridge sharedInstance] initCentralManager:[NSString stringWithUTF8String:targetName]];
    }
    
    void _cbcm_ScanEnable(const char *peripheralName){
        [[CBCentralManagerBridge sharedInstance] ScanEnable:[NSString stringWithUTF8String:peripheralName]];
    }
    
    void _cbcm_ScanDisable(){
        [[CBCentralManagerBridge sharedInstance] ScanDisable];
    }
    
    void _cbcm_ScanDataReset(){
        [[CBCentralManagerBridge sharedInstance] ScanDataReset];
    }
    
    bool _cbcm_IsScanning(){
        return [[CBCentralManagerBridge sharedInstance] IsScanning];
    }
    
    
    bool _cbcm_IsPoweredOn(){
        return [[CBCentralManagerBridge sharedInstance] IsPoweredOn];
    }
     
    
    void _cbcm_ConnectToPeripheral(const char *peripheralName){
        [[CBCentralManagerBridge sharedInstance] ConnectToPeripheralWithPeripheralName:[NSString stringWithUTF8String:peripheralName]];

        //[[CBCentralManagerBridge sharedInstance] ConnectToPeripheral:[NSString stringWithUTF8String:peripheralName]];
    }
    
    void _cbcm_CancelPeripheralConnection(const char *peripheralName){
        [[CBCentralManagerBridge sharedInstance] CancelPeripheralConnectionWithPeripheralName:[NSString stringWithUTF8String:peripheralName]];
    }
    
}
