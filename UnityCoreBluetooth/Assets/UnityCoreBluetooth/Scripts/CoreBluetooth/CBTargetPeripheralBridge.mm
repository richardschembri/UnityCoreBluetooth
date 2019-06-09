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
    
    
    void _cbp_DiscoverServices(){
        [[CBTargetPeripheralBridge sharedInstance] DiscoverServices];
    }
    
    void _cbp_DiscoverServicesAndTrackCharacteristics(const char *characteristicsWithVariableValueUUIDs, const char *characteristicsWithStaticValueUUIDs){

        [[CBTargetPeripheralBridge sharedInstance] DiscoverServicesAndTrackCharacteristicsWithCharacteristicsWithVariableValueUUIDs:[NSString stringWithUTF8String:characteristicsWithVariableValueUUIDs] characteristicsWithStaticValueUUIDs:[NSString stringWithUTF8String:characteristicsWithStaticValueUUIDs]];

    }
    
    void _cbp_CMDSend(const char * characteristicUUID, unsigned char * s_buff, int len ){

       // [[CBTargetPeripheralBridge sharedInstance] characteristicUUID:characteristicUUID CMDSendWithS_buff:s_buff len:len];
     
        [[CBTargetPeripheralBridge sharedInstance] CMDSendWithCharacteristicUUID:[NSString stringWithUTF8String: characteristicUUID] s_buff:s_buff len:len];
    }
    
    void _cbp_ReadValue(const char * characteristicUUID){
        [[CBTargetPeripheralBridge sharedInstance] ReadValueWithCharacteristicUUID:[NSString stringWithUTF8String: characteristicUUID]];
    }
    
    void _cbp_UnityDebugLog(const char * message){
        [[CBTargetPeripheralBridge sharedInstance] UnityDebugLogWithMessage: [NSString stringWithUTF8String: message]];
    }


}
