namespace UnityCoreBluetooth
{
	using UnityEngine;
	using UnityEngine.Events;
	using System;
	using System.Collections;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	public class CBCentralManagerBridge
	{
		#region Declare external C interface
		#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern void _cbcm_initCentralManager(string objectName);

		[DllImport("__Internal")]
		private static extern void _cbcm_ScanEnable(string serviceUUID);

		[DllImport("__Internal")]
		private static extern void _cbcm_ScanDisable();

		[DllImport("__Internal")]
		private static extern void _cbcm_ScanDataReset();

		[DllImport("__Internal")]
		private static extern bool _cbcm_IsScanning();

		[DllImport("__Internal")]
		private static extern bool _cbcm_ConnectToPeripheral(string peripheralName);

		[DllImport("__Internal")]
		private static extern bool _cbcm_CancelPeripheralConnection(string peripheralName);

		[DllImport("__Internal")]
		private static extern bool _cbcm_IsPoweredOn();
		#endif
		#endregion 

		#region Wrapped methods and properties
		public void InitCentralManager(string objectName){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbcm_initCentralManager(objectName);
		#endif
		}

		public void ScanEnable(string serviceUUID){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbcm_ScanEnable(serviceUUID);
		#endif
		#if UNITY_EDITOR
			if(DummyBLEController.Instance != null){
				DummyBLEController.Instance.ScanEnable();
			}
		#endif
		}

		public void ScanDisable(){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbcm_ScanDisable();
		#endif
		#if UNITY_EDITOR
			if(DummyBLEController.Instance != null){
				DummyBLEController.Instance.ScanDisable();
			}
		#endif
		}

		//public void ScanDataReset(){
		//#if UNITY_IOS && !UNITY_EDITOR
		//	_cbcm_ScanDataReset();
		//#endif
		//}

		public bool IsPoweredOn(){
		#if UNITY_IOS && !UNITY_EDITOR
			return _cbcm_IsPoweredOn();
		#endif
		#if UNITY_EDITOR
			if(DummyBLEController.Instance != null){
				return DummyBLEController.Instance.IsPoweredOn();
			}
		#endif
			return false;
		}

		public bool IsScanning(){
		#if UNITY_IOS && !UNITY_EDITOR
			return _cbcm_IsScanning();
		#endif
		#if UNITY_EDITOR
			if(DummyBLEController.Instance != null){
				return DummyBLEController.Instance.IsScanning();
			}
		#endif
			return false;
		}

		public void ConnectToPeripheral(string peripheralName){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbcm_ConnectToPeripheral(peripheralName);
		#endif
		#if UNITY_EDITOR
			if(DummyBLEController.Instance != null){
				DummyBLEController.Instance.ConnectToPeripheral(peripheralName);
			}
		#endif
		}

		public void CancelPeripheralConnection(string peripheralName){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbcm_CancelPeripheralConnection(peripheralName);
		#endif
		#if UNITY_EDITOR
			if(DummyBLEController.Instance != null){
				DummyBLEController.Instance.CancelPeripheralConnection(peripheralName);
			}
		#endif
		}
			
		#endregion

		#region Singleton implementation

		private CBCentralManagerBridge() { }
		private static CBCentralManagerBridge _instance;
		public static CBCentralManagerBridge Instance{ get{ if(_instance == null){ _instance = new CBCentralManagerBridge(); } return _instance; } }




		#endregion

	//   #region Delegates
		///*
		//public void PeripheralDidUpdateValueForCharacteristic(string characteristicJSONString){
		//	if(peripheralDidUpdateValueForCharacteristic != null){
		//		peripheralDidUpdateValueForCharacteristic.Invoke(characteristicJSONString);
		//	}
		//}

		//public void PeripheralDidUpdateValueForCharacteristicError(string characteristicErrorJSONString){
		//	if(peripheralDidUpdateValueForCharacteristic != null){
		//		peripheralDidUpdateValueForCharacteristic.Invoke(characteristicErrorJSONString);
		//	}
		//}
		//*/

		//#endregion
	}
}